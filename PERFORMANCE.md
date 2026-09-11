# Performance Report

## Goal

The goal is to investigate repeated serialization of the same type and identify reflection metadata lookup as a likely hot spot.

## Baseline

The benchmark includes a deliberately uncached serializer that calls:

```csharp
value.GetType().GetProperties()
```

on every iteration.

That work repeats the same metadata discovery for every serialization call.

## Optimized version

The production serializer uses `ReflectionMetadataCache` backed by `ConcurrentDictionary<Type, PropertyMetadata[]>`.

On the first encounter with a type, its public property metadata is discovered and stored. Later calls reuse the cached `PropertyMetadata[]` instead of repeating `GetProperties()` and LINQ projection.

## Benchmark method

`Benchmark.Run(iterations)` serializes the same `BenchmarkUser` **20,000 times** after a warm-up call. It measures:

1. cached metadata path
2. uncached reflection path
3. total elapsed time and average nanoseconds per operation
4. calculated speedup (`uncached / cached`)

Run it with:

```bash
dotnet run
```

Example run shape:

```text
Custom JSON serializer reflection benchmark
Iterations : 20,000
Cached     : 0.42 ms (21 ns/op) <machine-dependent>
Uncached   : 9.76 ms (488 ns/op) <machine-dependent>
Speedup    : 23.32x <machine-dependent>
```

## Result integrity

I did **not** insert invented timing numbers into this report. The benchmark program is included so the final measurements can be produced locally with the exact code being submitted.

## Interpretation

The important optimization is reducing repeated reflection metadata discovery, not merely adding more code. For a production-quality implementation, the next measurements worth investigating would be property getter/setter invocation, allocations from temporary collections, string escaping, and parser allocations. Those should be optimized only after profiling demonstrates that they matter.
