using JsonSerializer.libs;

namespace JsonSerializer.tests;

public static class Test
{
    // public static void TestSerialize(object? value, string excepted)
    // {
    //     var actual = Json.Serialize(value);

    //     if (actual != excepted)
    //         throw new Exception($"Expected '{excepted}', but got '{actual}' for value '{value}'.");

    //     Console.WriteLine($"PASS: {value ?? "null"} => {actual}");
    // }
    public static void Equal<T>(T expected, T? actual)
    {
        if (!EqualityComparer<T>.Default.Equals(expected, actual))
            throw new Exception(
                $"Expected '{expected}', got '{actual}'.");

        Console.WriteLine($"PASS: {actual} => {expected}");
    }
}

public sealed class User
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    public Address Address { get; set; } = new();
}

public sealed class Address
{
    public string City { get; set; } = string.Empty;

    public string Country { get; set; } = string.Empty;
}

public sealed class Company
{
    public string Name { get; set; } = string.Empty;

    public User Owner { get; set; } = new();
}

public sealed class MixedModel
{
    public int Id { get; set; }

    public long Count { get; set; }

    public double Price { get; set; }

    public bool Enabled { get; set; }

    public string Name { get; set; } = string.Empty;
}