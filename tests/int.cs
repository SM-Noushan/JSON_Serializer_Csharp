namespace tests;

public class TestInt
{
    public void SerializeInt(int value, string excepted)
    {
        var actual = lib.JsonSerializer.Serialize(value);

        if (actual != excepted)
            throw new Exception($"Expected '{excepted}', but got '{actual}' for value '{value}'.");

        Console.WriteLine($"PASS: {value} => {actual}");
    }
}