using static JsonSerialize.tests.Test;

public class Program
{
    public static void Main(string[] args)
    {
        //Test case for null
        TestSerialize(null, "null");

        // Test cases for SerializeInt
        Console.WriteLine("Testing SerializeInt...");
        TestSerialize(0, "0");
        TestSerialize(42, "42");
        TestSerialize(-25, "-25");
        TestSerialize(int.MaxValue, "2147483647");
        TestSerialize(int.MinValue, "-2147483648");

        // Test cases for SerializeLong
        Console.WriteLine("\nTesting SerializeLong...");
        TestSerialize(42L, "42");
        TestSerialize(-100L, "-100");
        TestSerialize(long.MaxValue, "9223372036854775807");
        TestSerialize(long.MinValue, "-9223372036854775808");

        //Test cases for SerializeFloat
        Console.WriteLine("\nTesting SerializeFloat...");
        TestSerialize(3.14f, "3.14");
        TestSerialize(-2.5f, "-2.5");

        //Test cases for SerializeDouble
        Console.WriteLine("\nTesting SerializeDouble...");
        TestSerialize(3.14159, "3.14159");
        TestSerialize(-2.71828, "-2.71828");

        //Test cases for SerializeDecimal
        Console.WriteLine("\nTesting SerializeDecimal...");
        TestSerialize(123.45m, "123.45");
        TestSerialize(-987.65m, "-987.65");

        //Test cases for SerializeBool
        Console.WriteLine("\nTesting SerializeBool...");
        TestSerialize(true, "True");
        TestSerialize(false, "False");

        //Test cases for SerializeString
        Console.WriteLine("\nTesting SerializeString...");
        TestSerialize("", "");
        TestSerialize("Hello, World!", "Hello, World!");
        TestSerialize("123", "123");
        TestSerialize("true", "true");
        TestSerialize("null", "null");
        TestSerialize("Special characters: !@#$%^&*()", "Special characters: !@#$%^&*()");
    }
}