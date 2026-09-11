using System.Reflection;
using System.Diagnostics;
using JSONSerializer.libs;

namespace JSONSerializer.benchmark;

public static class Benchmark
{
    public static TimeSpan Measure(int count, Action action)
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        var stopwatch = Stopwatch.StartNew();

        for (var i = 0; i < count; i++)
            action();

        stopwatch.Stop();
        return stopwatch.Elapsed;
    }

    public static void Run(int iterations)
    {
        var type = typeof(BenchmarkUser);

        // Warm-up JIT and populate the metadata cache.
        _ = ReflectionMetadataCache.GetSerializableProperties(type);

        var cached = Measure(
            iterations,
            () => _ = ReflectionMetadataCache.GetSerializableProperties(type));

        var uncached = Measure(
            iterations,
            () =>
            {
                _ = type
                    .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .Where(p => p.CanRead && p.GetIndexParameters().Length == 0)
                    .Select(p => new PropertyMetadata(p))
                    .ToArray();
            });

        Console.WriteLine("\nReflection metadata benchmark");
        Console.WriteLine($"Iterations : {iterations:N0}");
        Console.WriteLine($"Cached     : {cached.TotalMilliseconds:N2} ms ({cached.TotalMilliseconds / iterations * 1_000_000:N0} ns/op)");
        Console.WriteLine($"Uncached   : {uncached.TotalMilliseconds:N2} ms ({uncached.TotalMilliseconds / iterations * 1_000_000:N0} ns/op)");
        Console.WriteLine($"Speedup    : {uncached.TotalMilliseconds / cached.TotalMilliseconds:N2}x");
        Console.WriteLine();
        Console.WriteLine("Note: results vary by CPU/runtime. Run Release mode and repeat after system warm-up.");
    }
}

public sealed class BenchmarkUser
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}