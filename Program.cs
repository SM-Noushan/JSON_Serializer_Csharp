using JsonSerializer.libs;
using JSONSerializer.libs;
using JsonSerializer.tests;
// using static JsonSerializer.tests.Equal;

public class Program
{
    public static void Main(string[] args)
    {
        var tests = new (string name, Action run)[]
        {
            ("Primitive serialization", TestCase.PrimitiveSerialization),
            ("Object serialization", TestCase.ObjectSerialization),
            ("Nested objects", TestCase.NestedObjects),
            ("Collections", TestCase.Collections),
            ("DictionarySerialization", TestCase.DictionarySerialization),
            ("Special types", TestCase.SpecialTypes),
            ("Escapes", TestCase.EscapedStrings),
        };
        int passed = 0;
        foreach (var (name, run) in tests)
        {
            try
            {
                run();
                Console.WriteLine($"PASS  {name}");
                passed++;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"FAIL  {name}: {ex.Message}");
                Environment.ExitCode = 1;
            }
        }
        Console.WriteLine($"{passed}/{tests.Length} tests passed.");
        //Test case for null
        // Test.Equal<object?>(null, null);

        // // Test cases for SerializeInt
        // Console.WriteLine("Testing SerializeInt...");
        // Test.Equal(0, 0);
        // Test.Equal(42, 42);
        // Test.Equal(-25, -25);
        // Test.Equal(2147483647, int.MaxValue);
        // Test.Equal(-2147483648, int.MinValue);

        // // Test cases for SerializeLong
        // Console.WriteLine("\nTesting SerializeLong...");
        // Test.Equal(42L, 42);
        // Test.Equal(-100L, -100);
        // Test.Equal(long.MaxValue, 9223372036854775807);
        // Test.Equal(long.MinValue, -9223372036854775808);

        // //Test cases for SerializeFloat
        // Console.WriteLine("\nTesting SerializeFloat...");
        // Test.Equal(3.14f, 3.14f);
        // Test.Equal(-2.5f, -2.5f);
        // Test.Equal(float.PositiveInfinity, float.PositiveInfinity);
        // Test.Equal(float.NaN, float.NaN);
        // Test.Equal(float.NegativeInfinity, float.NegativeInfinity);

        // //Test cases for SerializeDouble
        // Console.WriteLine("\nTesting SerializeDouble...");
        // Test.Equal(3.14159d, 3.14159);
        // Test.Equal(double.NegativeInfinity, double.NegativeInfinity);
        // Test.Equal(double.NegativeInfinity, double.NegativeInfinity);
        // Test.Equal(double.NaN, double.NaN);

        // //Test cases for SerializeDecimal
        // Console.WriteLine("\nTesting SerializeDecimal...");
        // Test.Equal(123.45m, 123.45m);
        // Test.Equal(-987.65m, -987.65m);

        // //Test cases for SerializeBool
        // Console.WriteLine("\nTesting SerializeBool...");
        // Test.Equal(true, true);
        // Test.Equal(false, false);

        // //Test cases for SerializeString
        // Console.WriteLine("\nTesting SerializeString...");
        // Test.Equal("", "");
        // Test.Equal("123", "123");
        // Test.Equal("Hello, World!", "Hello, World!");
        // Test.Equal("Special characters: !@#$%^&*()", "Special characters: !@#$%^&*()");
        // Test.Equal("He said \"Hello, World!\"", "He said \"Hello, World!\""); //quote
        // Test.Equal("This is a backslash: \\", "This is a backslash: \\"); //backslash
        // Test.Equal("This is a newline:\n", "This is a newline:\n"); //newline
        // Test.Equal("This is a tab:\t", "This is a tab:\t"); //tab
        // Test.Equal("This is a carriage return:\r", "This is a carriage return:\r"); //carriage return
        // Test.Equal("This is a form feed:\f", "This is a form feed:\f"); //form feed
        // Test.Equal("This is a backspace:\b", "This is a backspace:\b"); //backspace
        // Test.Equal("Control character: \u0001", "Control character: \u0001"); //control character
        // // var user = new User
        // // {
        // //     Id = 1,
        // //     Name = null,
        // //     IsActive = true
        // // };
        // var user = new User
        // {
        //     Id = 1,
        //     Name = "John",
        //     IsActive = true,
        //     Address = new Address
        //     {
        //         City = "Dhaka",
        //         Country = "Bangladesh"
        //     }
        // };
        // var json = Json.Serialize(user);
        // // Console.WriteLine(json);
        // // Test.Equal("{\"Id\":1,\"Name\":null,\"IsActive\":true}", json);

        // var model = new MixedModel
        // {
        //     Id = 10,
        //     Count = 5000L,
        //     Price = 19.95,
        //     Enabled = true,
        //     Name = "John \"Doe\""
        //     // Name = "Test"
        // };
        // // Console.WriteLine(Json.Serialize(model));
        // var company = new Company
        // {
        //     Name = "Example",
        //     Owner = new User
        //     {
        //         Id = 1,
        //         Name = "John",
        //         IsActive = true,
        //         Address = new Address
        //         {
        //             City = "Dhaka",
        //             Country = "Bangladesh"
        //         }
        //     }
        // };
        // var companyJson = Json.Serialize(company);
        // // Console.WriteLine(companyJson);
        // var user1 = new User
        // {
        //     Id = 1,
        //     Name = "John",
        //     IsActive = true,
        //     Address = new Address
        //     {
        //         City = "Dhaka",
        //         Country = "Bangladesh"
        //     }
        // };

        // var json1 = Json.Serialize(
        //     user1,
        //     new JsonSerializerOptions
        //     {
        //         WriteIndented = true
        //     });
        // // Console.WriteLine(json1);

        // // var numbers = new List<int> { 1, 2, 3 };
        // // var numbers = new [] { 1, 2, 3 };
        // IEnumerable<int> numbers = new List<int> { 5, 10, 15 };
        // var enumJson = Json.Serialize(numbers);
        // // Console.WriteLine(enumJson);

        // var users = new List<User>
        // {
        //     new User{Id = 1,Name = "John",IsActive = true},
        //     new User{Id = 2,Name = "Jane",IsActive = false}
        // };

        // var enumUser = Json.Serialize(users, new JsonSerializerOptions
        // {
        //     WriteIndented = true
        // });
        // // Console.WriteLine(enumUser);
        // var nestedList = new List<List<int>>();
        // nestedList.Add(new List<int> { 1, 2 });
        // nestedList.Add(new List<int> { 3, 4 });
        // nestedList.Add(new List<int> { 5, 6 });
        // // Console.WriteLine(Json.Serialize(nestedList));

        // var model1 = new CollectionModel
        // {
        //     Values = [1, 2, 3],
        //     Users =
        //     [
        //         new User
        //         {
        //             Id = 1,
        //             Name = "A",
        //             IsActive = true
        //         }
        //     ]
        // };
        // // Console.WriteLine(Json.Serialize(model1));

        // var model2 = new CollectionModel
        // {
        //     Values = [1, 2, 3],
        //     Users =
        //     [
        //         new User
        //             {
        //                 Id = 2,
        //                 Name = "B",
        //                 IsActive = true
        //             }
        //     ]
        // };

        // var collectionJson2 = Json.Serialize(model2);
        // Console.WriteLine(collectionJson2);
        // Test.Contains(collectionJson2, "\"Values\":[1,2,3]");
        // Test.Contains(collectionJson2, "\"Users\":[{\"Id\":2");

        // Dictionary serialization test
        // Console.WriteLine(Json.Serialize(new Dictionary<string, object?>
        // {
        //     ["name"] = "John",
        //     ["age"] = 30,
        //     ["active"] = true
        // }));
        // Console.WriteLine(Json.Serialize(new Dictionary<string, object?>
        // {
        //     ["name"] = "John",
        //     ["user"] = new User
        //     {
        //         Id = 1,
        //         Name = "John",
        //         IsActive = true
        //     },
        //     ["numbers"] = new List<int> { 1, 2, 3 }
        // }));
        // try
        // {
        //     Console.WriteLine(Json.Serialize(new Dictionary<int, string>
        //     {
        //         [1] = "one",
        //         [2] = "two"
        //     }));
        //     Console.WriteLine("FAIL: Non-string dictionary key accepted.");
        // }
        // catch (JsonException ex)
        // {
        //     Console.WriteLine($"PASS: Invalid dictionary key rejected: {ex.Message}");
        // }
        // var dictionaryData1 = new Dictionary<string, object?>
        // {
        //     ["users"] = new List<User>
        //     {
        //         new User
        //         {
        //             Id = 1,
        //             Name = "John",
        //             IsActive = true
        //         }
        //     },
        //     ["scores"] = new List<int> { 90, 95, 100 },
        //     ["settings"] = new Dictionary<string, object?>
        //     {
        //         ["darkMode"] = true,
        //         ["language"] = "en"
        //     }
        // };

        // var dictionaryJson1 = Json.Serialize(dictionaryData1, new JsonSerializerOptions() { WriteIndented = true });
        // Console.WriteLine(dictionaryJson1);
    }
}