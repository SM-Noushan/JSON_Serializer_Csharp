using JSONSerializer.benchmark;
using JSONSerializer.libs;
using TestCase = JSONSerializer.tests.TestCase;

namespace JSONSerializer;

public class Program
{
    public static void Main(string[] args)
    {
        // Run tests
        var tests = new (string name, Action run)[]
        {
            ("Primitive serialization", TestCase.PrimitiveSerialization),
            ("Object serialization", TestCase.ObjectSerialization),
            ("Nested objects", TestCase.NestedObjects),
            ("Collections", TestCase.Collections),
            ("IEnumerable deserialization", TestCase.EnumerableDeserialization),
            ("Dictionary", TestCase.DictionarySerialization),
            ("Deserialization", TestCase.Deserialization),
            ("Special types", TestCase.SpecialTypes),
            ("Nullable", TestCase.NullableValues),
            ("Escapes", TestCase.EscapedStrings),
            ("Malformed JSON", TestCase.MalformedJson),
            ("Type mismatch", TestCase.TypeMismatch),
            ("Circular reference Throw", TestCase.CircularReferenceThrow),
            ("Circular reference WriteNull", TestCase.CircularReferenceWriteNull),
            ("Case insensitive properties", TestCase.CaseInsensitiveProperties)
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

        //Benchmark
        const int iterations = 20_000;
        Benchmark.Run(iterations);
    }
}