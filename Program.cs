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

        var parser = new JsonParser("true");
        var result1 = parser.Parse();
        Console.WriteLine(result1.GetType().Name);


        var jsonValues = new[] { "null", "true", "false", "\"Hello\"", "123", "-45.67" };
        foreach (var json in jsonValues)
        {
            var result2 = new JsonParser(json).Parse();
            Console.WriteLine(
                $"{json} -> {result2.GetType().Name}");
        }

    }
}