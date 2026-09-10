using JsonSerialize.lib;

namespace JsonSerialize.tests;

public class Test
{
    public static void TestSerialize(object? value, string excepted)
    {
        var actual = JsonSerializer.Serialize(value);

        if (actual != excepted)
            throw new Exception($"Expected '{excepted}', but got '{actual}' for value '{value}'.");

        Console.WriteLine($"PASS: {value ?? "null"} => {actual}");
    }
}