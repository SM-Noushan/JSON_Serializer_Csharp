using JSONSerializer.libs;
using JSONSerializer.tests;

namespace JSONSerializer;

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
                // run();
                // Console.WriteLine($"PASS  {name}");
                passed++;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"FAIL  {name}: {ex.Message}");
                Environment.ExitCode = 1;
            }
        }
        // Console.WriteLine($"{passed}/{tests.Length} tests passed.");


        // test cases for string/numeric/boolean/null parsing
        // var jsonValues = new[] { "null", "true", "false", "\"Hello\"", "123", "-45.67" };
        // var jsonValues = new[] { "null", "true", "false", "123", "-45.67" };
        // var jsonValues = new[] { "\"Hello\"", "\"Hello \\\"John\\\"\"", "\"C:\\\\Test\"", "\"Line1\\nLine2\"", "\"\\u0041\"" };
        // foreach (var json in jsonValues)
        // {
        //     var result = new JsonParser(json).Parse();

        //     if (result is JsonStringValue stringValue)
        //         Console.WriteLine(stringValue.Value);

        //     // Console.WriteLine($"{json} -> {result.GetType().Name}");
        // }

        //test cases for object parsing
        // var json = "{\"Name\":\"John\",\"Age\":25,\"Active\":true}";

        // var result =
        //     new JsonParser(json).Parse();

        // if (result is JsonObjectValue objectValue)
        // {
        //     foreach (var property in objectValue.Properties)
        //     {
        //         Console.WriteLine(
        //             $"{property.Key} -> " +
        //             $"{property.Value.GetType().Name}");
        //     }
        // }

        //test cases for array parsing
        // var json = "[1,true,\"hello\",null]";

        // var result =
        //     new JsonParser(json).Parse();

        // if (result is JsonArrayValue array)
        // {
        //     foreach (var item in array.Items)
        //     {
        //         Console.WriteLine(
        //             item.GetType().Name);
        //     }
        // }

        // test cases for invalid JSON
        // var invalidJson = new[]
        //     {
        //         "",
        //         "{",
        //         "[",
        //         "{\"Name\"}",
        //         "{\"Name\":}",
        //         "[1,]",
        //         "{\"a\":1 \"b\":2}",
        //         "{\"a\":1,}",
        //         "tru",
        //         "nul",
        //         "\"unterminated",
        //         "{\"a\":01}"
        //     };

        // foreach (var json in invalidJson)
        // {
        //     try
        //     {
        //         new JsonParser(json).Parse();
        //         Console.WriteLine($"FAIL: Accepted invalid JSON: {json}");
        //     }
        //     catch (JsonException ex)
        //     {
        //         Console.WriteLine($"PASS: Rejected '{json}'");
        //         Console.WriteLine($"{ex.Message}");
        //     }
        // }
        // test cases for valid JSON
        // var validJson = new[]
        //     {
        //         "0",
        //         "-1",
        //         "123.456",
        //         "1e5",
        //         "1.5e-2",
        //         "[]",
        //         "{}",
        //         "[[]]",
        //         "{\"nested\":{}}"
        //     };

        // foreach (var json in validJson)
        // {
        //     var result = new JsonParser(json).Parse();
        //     Console.WriteLine($"PASS: {json}");
        // }

        //Deserialize
        // var nullValue = Json.Deserialize<int?>("null");
        // Console.WriteLine(nullValue is null); //ok

        //passed
        // try
        // {
        //     Json.Deserialize<int>("null");
        //     Console.WriteLine("FAIL");
        // }
        // catch (JsonException ex)
        // {
        //     Console.WriteLine($"PASS: {ex.Message}");
        // }

        // Console.WriteLine(Json.Deserialize<int>("123")); // 123
        // Console.WriteLine(Json.Deserialize<double>("12.34")); // 12.34
        // Console.WriteLine(Json.Deserialize<bool>("true")); // True
        // Console.WriteLine(Json.Deserialize<string>("\"Hello\"")); // Hello
        // Console.WriteLine(Json.Deserialize<int?>("null") is null); // True
        // Console.WriteLine(Json.Deserialize<double?>("null") is null); // True
        // Console.WriteLine(Json.Deserialize<bool?>("null") is null); // True
        // Console.WriteLine(Json.Deserialize<long>("123456789")); // 123456789
        // try
        // {
        //     // Json.Deserialize<int>("\"hello\"");
        //     // Json.Deserialize<bool>("true");
        //     Json.Deserialize<bool>("123");
        //     Console.WriteLine("FAIL");
        // }
        // catch (JsonException ex)
        // {
        //     Console.WriteLine($"PASS: {ex.Message}");
        // }

        //deserialize object
        // var json =
        //     """
        //     {
        //         "Id": 1,
        //         "Name": "John",
        //         "IsActive": true
        //     }
        //     """;

        // var user = Json.Deserialize<User>(json);

        // Console.WriteLine(user!.Id);
        // Console.WriteLine(user.Name);
        // Console.WriteLine(user.IsActive);

        // var json =
        //     """
        //     {
        //         "Id": 1,
        //         "Name": "John",
        //         "IsActive": true,
        //         "Address": {
        //             "City": "Dhaka",
        //             "Country": "Bangladesh"
        //         }
        //     }
        //     """;

        // var user = Json.Deserialize<User>(json);

        // Console.WriteLine(user!.Address.City);
        // Console.WriteLine(user.Address.Country);

        //Testing collections
        // var numbers =Json.Deserialize<List<int>>(
        // "[1,2,3]");

        // foreach (var number in numbers!)
        // {
        //     Console.WriteLine(number);
        // }
        // var users =Json.Deserialize<List<User>>(
        // """
        //         [
        //             {
        //                 "Id": 1,
        //                 "Name": "John",
        //                 "IsActive": true
        //             },
        //             {
        //                 "Id": 2,
        //                 "Name": "Jane",
        //                 "IsActive": false
        //             }
        //         ]
        //     """);

        // foreach (var user in users!)
        // {
        //     Console.WriteLine(user.Name);
        // }

        //Test dictionary deserialization 
        var data = Json.Deserialize<Dictionary<string, object>>(
            """
            {
                "name": "John",
                "age": 25,
                "active": true
            }
            """);
        Console.WriteLine(data["name"]);
        Console.WriteLine(data["age"]);
        Console.WriteLine(data["active"]);

        //Test special types deserialization

        var date = Json.Deserialize<DateTime>("\"2026-09-11T12:00:00.0000000Z\"");
        var id = Json.Deserialize<Guid>("\"550e8400-e29b-41d4-a716-446655440000\"");
        var status = Json.Deserialize<Status>("\"Active\"");
        Console.WriteLine($"Date: {date}, \nID: {id}, \nStatus: {status}");

        var user = Json.Deserialize<User>(
            """
            {
                "id": 1,
                "name": "John",
                "isactive": true
            }
            """,
        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        Console.WriteLine($"User: {user.Id}, {user.Name}, {user.IsActive}");
        // Console.WriteLine($"User: {user.id}, {user.name}, {user.isactive}");

        var original = new User
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

        var json = Json.Serialize(original);
        var copy = Json.Deserialize<User>(json);

        Console.WriteLine(json);
        Console.WriteLine(copy!.Name);
        Console.WriteLine(copy.Address.City);
    }
    public enum Status { Active, Inactive }

}