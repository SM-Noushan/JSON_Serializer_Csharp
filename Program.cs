using JsonSerializer.libs;
using JsonSerializer.tests;
// using static JsonSerializer.tests.Equal;

public class Program
{
    public static void Main(string[] args)
    {
        //Test case for null
        // TestSerialize(null, "null");

        // Test cases for SerializeInt
        // Console.WriteLine("Testing SerializeInt...");
        // TestSerialize(0, "0");
        // TestSerialize(42, "42");
        // TestSerialize(-25, "-25");
        // TestSerialize(int.MaxValue, "2147483647");
        // TestSerialize(int.MinValue, "-2147483648");

        // Test cases for SerializeLong
        // Console.WriteLine("\nTesting SerializeLong...");
        // TestSerialize(42L, "42");
        // TestSerialize(-100L, "-100");
        // TestSerialize(long.MaxValue, "9223372036854775807");
        // TestSerialize(long.MinValue, "-9223372036854775808");

        //Test cases for SerializeFloat
        // Console.WriteLine("\nTesting SerializeFloat...");
        // TestSerialize(3.14f, "3.14");
        // TestSerialize(-2.5f, "-2.5");
        // try
        // {
        //     TestSerialize(float.PositiveInfinity, "");
        //     Console.WriteLine("FAIL");
        // }
        // catch (JsonException)
        // {
        //     Console.WriteLine("PASS: NaN/Infinity rejected");
        // }

        //Test cases for SerializeDouble
        // Console.WriteLine("\nTesting SerializeDouble...");
        // TestSerialize(3.14159, "3.14159");
        // TestSerialize(-2.71828, "-2.71828");
        // try
        // {
        //     TestSerialize(double.NegativeInfinity, "");
        //     Console.WriteLine("FAIL");
        // }
        // catch (JsonException)
        // {
        //     Console.WriteLine("PASS: NaN/Infinity rejected");
        // }


        //Test cases for SerializeDecimal
        // Console.WriteLine("\nTesting SerializeDecimal...");
        // TestSerialize(123.45m, "123.45");
        // TestSerialize(-987.65m, "-987.65");

        //Test cases for SerializeBool
        // Console.WriteLine("\nTesting SerializeBool...");
        // TestSerialize(true, "true");
        // TestSerialize(false, "false");

        //Test cases for SerializeString
        // Console.WriteLine("\nTesting SerializeString...");
        // TestSerialize("", "\"\"");
        // TestSerialize("123", "\"123\"");
        // TestSerialize("Hello, World!", "\"Hello, World!\"");
        // TestSerialize("Special characters: !@#$%^&*()", "\"Special characters: !@#$%^&*()\"");
        // TestSerialize("He said \"Hello, World!\"", "\"He said \\\"Hello, World!\\\"\""); //quote
        // TestSerialize("This is a backslash: \\", "\"This is a backslash: \\\\\""); //backslash
        // TestSerialize("This is a newline:\n", "\"This is a newline:\\n\""); //newline
        // TestSerialize("This is a tab:\t", "\"This is a tab:\\t\""); //tab
        // TestSerialize("This is a carriage return:\r", "\"This is a carriage return:\\r\""); //carriage return
        // TestSerialize("This is a form feed:\f", "\"This is a form feed:\\f\""); //form feed
        // TestSerialize("This is a backspace:\b", "\"This is a backspace:\\b\""); //backspace
        // TestSerialize("Control character: \u0001", "\"Control character: \\u0001\""); //control character
        var user = new User
        {
            Id = 1,
            Name = null,
            IsActive = true
        };
        var json = Json.Serialize(user);
        Test.Equal("{\"Id\":1,\"Name\":null,\"IsActive\":true}", json);

        var model = new MixedModel
        {
            Id = 10,
            Count = 5000L,
            Price = 19.95,
            Enabled = true,
            Name = "John \"Doe\""
            // Name = "Test"
        };
        Console.WriteLine(Json.Serialize(model));
    }
}