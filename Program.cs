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
        var invalidJson = new[]
            {
                "",
                "{",
                "[",
                "{\"Name\"}",
                "{\"Name\":}",
                "[1,]",
                "{\"a\":1 \"b\":2}",
                "{\"a\":1,}",
                "tru",
                "nul",
                "\"unterminated",
                "{\"a\":01}"
            };

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
        var validJson = new[]
            {
                "0",
                "-1",
                "123.456",
                "1e5",
                "1.5e-2",
                "[]",
                "{}",
                "[[]]",
                "{\"nested\":{}}"
            };

        foreach (var json in validJson)
        {
            var result = new JsonParser(json).Parse();
            Console.WriteLine($"PASS: {json}");
        }
    }
}