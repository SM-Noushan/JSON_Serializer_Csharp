using JsonSerializer.libs;
using JsonSerializer.tests;
// using static JsonSerializer.tests.Equal;

public class Program
{
    public static void Main(string[] args)
    {
        //Test case for null
        Test.Equal<object?>(null, null);

        // Test cases for SerializeInt
        Console.WriteLine("Testing SerializeInt...");
        Test.Equal(0, 0);
        Test.Equal(42, 42);
        Test.Equal(-25, -25);
        Test.Equal(2147483647, int.MaxValue);
        Test.Equal(-2147483648, int.MinValue);

        // Test cases for SerializeLong
        Console.WriteLine("\nTesting SerializeLong...");
        Test.Equal(42L, 42);
        Test.Equal(-100L, -100);
        Test.Equal(long.MaxValue, 9223372036854775807);
        Test.Equal(long.MinValue, -9223372036854775808);

        //Test cases for SerializeFloat
        Console.WriteLine("\nTesting SerializeFloat...");
        Test.Equal(3.14f, 3.14f);
        Test.Equal(-2.5f, -2.5f);
        Test.Equal(float.PositiveInfinity, float.PositiveInfinity);
        Test.Equal(float.NaN, float.NaN);
        Test.Equal(float.NegativeInfinity, float.NegativeInfinity);

        //Test cases for SerializeDouble
        Console.WriteLine("\nTesting SerializeDouble...");
        Test.Equal(3.14159d, 3.14159);
        Test.Equal(double.NegativeInfinity, double.NegativeInfinity);
        Test.Equal(double.NegativeInfinity, double.NegativeInfinity);
        Test.Equal(double.NaN, double.NaN);

        //Test cases for SerializeDecimal
        Console.WriteLine("\nTesting SerializeDecimal...");
        Test.Equal(123.45m, 123.45m);
        Test.Equal(-987.65m, -987.65m);

        //Test cases for SerializeBool
        Console.WriteLine("\nTesting SerializeBool...");
        Test.Equal(true, true);
        Test.Equal(false, false);

        //Test cases for SerializeString
        Console.WriteLine("\nTesting SerializeString...");
        Test.Equal("", "");
        Test.Equal("123", "123");
        Test.Equal("Hello, World!", "Hello, World!");
        Test.Equal("Special characters: !@#$%^&*()", "Special characters: !@#$%^&*()");
        Test.Equal("He said \"Hello, World!\"", "He said \"Hello, World!\""); //quote
        Test.Equal("This is a backslash: \\", "This is a backslash: \\"); //backslash
        Test.Equal("This is a newline:\n", "This is a newline:\n"); //newline
        Test.Equal("This is a tab:\t", "This is a tab:\t"); //tab
        Test.Equal("This is a carriage return:\r", "This is a carriage return:\r"); //carriage return
        Test.Equal("This is a form feed:\f", "This is a form feed:\f"); //form feed
        Test.Equal("This is a backspace:\b", "This is a backspace:\b"); //backspace
        Test.Equal("Control character: \u0001", "Control character: \u0001"); //control character
        // var user = new User
        // {
        //     Id = 1,
        //     Name = null,
        //     IsActive = true
        // };
        var user = new User
        {
            Id = 1,
            Name = "John",
            IsActive = true,
            Address = new Address
            {
                City = "Dhaka",
                Country = "Bangladesh"
            }
        };
        var json = Json.Serialize(user);
        // Console.WriteLine(json);
        // Test.Equal("{\"Id\":1,\"Name\":null,\"IsActive\":true}", json);

        var model = new MixedModel
        {
            Id = 10,
            Count = 5000L,
            Price = 19.95,
            Enabled = true,
            Name = "John \"Doe\""
            // Name = "Test"
        };
        // Console.WriteLine(Json.Serialize(model));
        var company = new Company
        {
            Name = "Example",
            Owner = new User
            {
                Id = 1,
                Name = "John",
                IsActive = true,
                Address = new Address
                {
                    City = "Dhaka",
                    Country = "Bangladesh"
                }
            }
        };
        var companyJson = Json.Serialize(company);
        // Console.WriteLine(companyJson);
        var user1 = new User
        {
            Id = 1,
            Name = "John",
            IsActive = true,
            Address = new Address
            {
                City = "Dhaka",
                Country = "Bangladesh"
            }
        };

        var json1 = Json.Serialize(
            user1,
            new JsonSerializerOptions
            {
                WriteIndented = true
            });
        Console.WriteLine(json1);
    }
}