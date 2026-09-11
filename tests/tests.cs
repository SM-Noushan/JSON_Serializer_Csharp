using JSONSerializer.libs;

namespace JSONSerializer.tests;

public static class TestCase
{
    public static void PrimitiveSerialization()
    {
        Equal("\"hello\"", Json.Serialize("hello"));
        Equal("42", Json.Serialize(42));
        Equal("42", Json.Serialize(42L));
        Equal("1.5", Json.Serialize(1.5m));
        Equal("true", Json.Serialize(true));
        Equal("null", Json.Serialize<string?>(null));
    }

    public static void ObjectSerialization()
    {
        var user = new User { Id = 1, Name = "John", IsActive = true };
        var json = Json.Serialize(user);
        Equal("{\"Id\":1,\"Name\":\"John\",\"IsActive\":true}", json);
    }

    public static void NestedObjects()
    {
        var order = new Order { Id = 7, Customer = new User { Id = 1, Name = "John", IsActive = true } };
        var json = Json.Serialize(order);
        Contains(json, "\"Customer\":{\"Id\":1");
    }

    public static void Collections()
    {
        var model = new CollectionModel { Values = [1, 2, 3], Users = [new User { Id = 1, Name = "A", IsActive = true }] };
        var json = Json.Serialize(model);
        Contains(json, "\"Values\":[1,2,3]");
        Contains(json, "\"Users\":[{\"Id\":1");
    }

    public static void EnumerableDeserialization()
    {
        var values = Json.Deserialize<IEnumerable<int>>("[1,2,3]")!.ToList();
        Equal(3, values.Count);
        Equal(2, values.ElementAt(1));
    }

    public static void DictionarySerialization()
    {
        var data = new Dictionary<string, object?>
        {
            ["name"] = "John",
            ["age"] = 30,
            ["active"] = true,
            ["scores"] = new List<int> { 90, 95, 100 },
            ["settings"] = new Dictionary<string, object?>
            {
                ["darkMode"] = true,
                ["language"] = "en"
            }
        };

        var json = Json.Serialize(data);

        Contains(json, "\"name\":\"John\"");
        Contains(json, "\"age\":30");
        Contains(json, "\"active\":true");
        Contains(json, "\"scores\":[90,95,100]");
        Contains(json, "\"settings\":{\"darkMode\":true,\"language\":\"en\"}");
    }

    public static void Deserialization()
    {
        const string json = "{\"Id\":1,\"Name\":\"John\",\"IsActive\":true}";
        var user = Json.Deserialize<User>(json)!;
        Equal(1, user.Id); Equal("John", user.Name); Equal(true, user.IsActive);
    }

    public static void SpecialTypes()
    {
        var source = new SpecialModel { Date = new DateTime(2026, 9, 10, 12, 30, 0, DateTimeKind.Utc), Id = Guid.Parse("4d3c2f1e-1234-4567-89ab-0123456789ab"), Status = Status.Active };
        var json = Json.Serialize(source);
        var copy = Json.Deserialize<SpecialModel>(json)!;
        Equal(source.Date, copy.Date); Equal(source.Id, copy.Id); Equal(source.Status, copy.Status);
    }
    public static void NullableValues()
    {
        var item = Json.Deserialize<NullableModel>("{\"Count\":null}")!;
        Equal(null, item.Count);
        var item2 = Json.Deserialize<NullableModel>("{\"Count\":5}")!;
        Equal(5, item2.Count);
    }

    public static void EscapedStrings()
    {
        var value = "quote=\" slash=\\ newline=\n";
        var json = Json.Serialize(value);
        var copy = Json.Deserialize<string>(json);
        Equal(value, copy);
    }

    public static void MalformedJson()
    {
        Throws<JsonException>(() => Json.Deserialize<User>("{\"Id\":1"));
        Throws<JsonException>(() => Json.Deserialize<User>("{\"Id\":01}"));
        Throws<JsonException>(() => Json.Deserialize<User>("{\"Id\":1,}"));
    }

    public static void TypeMismatch()
    {
        var ex = Throws<JsonException>(() => Json.Deserialize<User>("{\"Id\":\"one\"}"));
        Contains(ex.Message, "Id");
    }

    public static void CircularReferenceThrow()
    {
        var person = new Person();
        person.Friend = person;
        var ex = Throws<JsonException>(() => Json.Serialize(person));
        Contains(ex.Message, "Circular reference");
    }

    public static void CircularReferenceWriteNull()
    {
        var person = new Person();
        // person.Friend = person;
        var json = Json.Serialize(person, new JsonSerializerOptions { CircularReferenceHandling = CircularReferenceHandling.WriteNull });
        Equal("{\"Friend\":null}", json);
    }

    public static void CaseInsensitiveProperties()
    {
        var user = Json.Deserialize<User>("{\"id\":5,\"name\":\"A\",\"isactive\":true}",
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
        Equal(5, user.Id); Equal("A", user.Name); Equal(true, user.IsActive);
    }

    public static void Equal<T>(T expected, T? actual)
    {
        if (!EqualityComparer<T>.Default.Equals(expected, actual))
            throw new Exception(
                $"Expected '{expected}', got '{actual}'.");
    }

    public static void Contains(string value, string expected)
    {
        if (!value.Contains(expected, StringComparison.Ordinal)) throw new Exception($"Expected '{value}' to contain '{expected}'.");
    }

    public static TException Throws<TException>(Action action) where TException : Exception
    {
        try { action(); }
        catch (TException ex) { return ex; }
        throw new Exception($"Expected {typeof(TException).Name}.");
    }
}

public sealed class User
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public sealed class Order
{
    public int Id { get; set; }
    public User Customer { get; set; } = new();
}

public sealed class CollectionModel
{
    public List<int> Values { get; set; } = [];
    public List<User> Users { get; set; } = [];
}

public sealed class SpecialModel
{
    public DateTime Date { get; set; }
    public Guid Id { get; set; }
    public Status Status { get; set; }
}

public enum Status { Inactive, Active }

public sealed class NullableModel
{
    public int? Count { get; set; }
}

public sealed class Person
{
    public Person? Friend { get; set; }
}