using JsonSerializer.libs;

namespace JsonSerializer.tests;

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

    public static void SpecialTypes()
    {
        var source = new SpecialModel { Date = new DateTime(2026, 9, 10, 12, 30, 0, DateTimeKind.Utc), Id = Guid.Parse("4d3c2f1e-1234-4567-89ab-0123456789ab"), Status = Status.Active };
        var json = Json.Serialize(source);
        Equal("{\"Date\":\"2026-09-10T12:30:00.0000000Z\",\"Id\":\"4d3c2f1e-1234-4567-89ab-0123456789ab\",\"Status\":\"Active\"}", json);
    }

    public static void EscapedStrings()
    {
        var value = "quote=\" slash=\\ newline=\n";
        var json = Json.Serialize(value);
        Equal("\"quote=\\\" slash=\\\\ newline=\\n\"", json);
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

    public static void Equal<T>(T expected, T? actual)
    {
        if (!EqualityComparer<T>.Default.Equals(expected, actual))
            throw new Exception(
                $"Expected '{expected}', got '{actual}'.");

        Console.WriteLine($"PASS: {actual} => {expected}");
    }

    public static void Contains(string value, string expected)
    {
        if (!value.Contains(expected, StringComparison.Ordinal)) throw new Exception($"Expected '{value}' to contain '{expected}'.");
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