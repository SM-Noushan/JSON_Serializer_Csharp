# Project Manifest

## Core library

- `libs/Json.cs` — public API, recursive writer, typed converter, special types, collections, dictionaries, circular-reference handling.
- `libs/JsonParser.cs` — hand-written JSON parser with strict grammar validation and location-aware errors.
- `lib/JsonValue.cs` — internal JSON value model used between parsing and typed conversion.
- `lib/ReflectionMetadataCache.cs` — cached public-property metadata for serialization/deserialization.
- `lib/JsonSerializerOptions.cs` — formatting, case sensitivity and circular-reference policy.
- `lib/JsonException.cs` — domain-specific error type.

## Tests

`tests/tests.cs` is a dependency-free executable test suite. It is intentionally not tied to a third-party test runner so the project remains easy to run.

## Benchmark

`benchmarks/benchmarks.cs` compares cached metadata lookup with a deliberately uncached reflection path over 20,000 serializations.

## Documentation

- `README.md` — use, supported types, architecture, decisions, limitations.
- `REQUIREMENTS.md` — requirement-by-requirement marking cross-check.
- `PERFORMANCE.md` — optimization rationale and benchmark methodology.
