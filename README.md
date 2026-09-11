# Custom JSON Serializer

A JSON serializer and deserializer built from scratch in C# using the .NET Base Class Library and reflection. The project is an educational implementation designed to demonstrate how a JSON library can convert C# values to JSON text and JSON text back into strongly typed C# values without using `System.Text.Json`, Newtonsoft.Json/Json.NET, or another third-party JSON library for the core JSON work.

## Project Information

- **Framework:** .NET 10
- **Language:** C# 14
- **Project type:** Console application
- **JSON libraries:** None
- **Reflection:** Used for generic object serialization/deserialization
- **Primary output:** JSON text
- **Testing:** Self-contained console test suite
- **Benchmarking:** Self-contained `Stopwatch` benchmark

The application contains the library implementation, tests, and benchmark in a single console project so the complete program can be built and run with one project.

## Project Structure

```text
JsonSerializer/
│
├── JsonSerializer/
│   │
│   ├── benchmark/
│   │   └── benchmark.cs
│   │
│   ├── libs/
│   │   ├── Json.cs
│   │   ├── JsonException.cs
│   │   ├── JsonParser.cs
│   │   ├── JsonSerializerOptions.cs
│   │   ├── JsonValue.cs
│   │   └── ReflectionMetadataCache.cs
│   │
│   ├── tests/
│   │   └── tests.cs
│   │
│   ├── PERFORMANCE.md
│   ├── PROJECT_MANIFEST.md
│   ├── README.md
│   ├── Program.cs
│   └── JsonSerializer.csproj
```

### Main responsibilities

| File                              | Responsibility                                                                                            |
| --------------------------------- | --------------------------------------------------------------------------------------------------------- |
| `Program.cs`                      | Application entry point; runs the complete test suite and benchmark                                       |
| `libs/Json.cs`                    | Main serializer/deserializer implementation, JSON writing, type conversion and recursive object handling  |
| `libs/JsonParser.cs`              | Hand-written JSON parser                                                                                  |
| `libs/JsonValue.cs`               | Internal representation of parsed JSON values                                                             |
| `libs/JsonException.cs`           | Custom exception for JSON parsing and conversion errors                                                   |
| `libs/JsonSerializerOptions.cs`   | Serializer configuration such as indentation, case-insensitive properties and circular-reference behavior |
| `libs/ReflectionMetadataCache.cs` | Caches reflection metadata to reduce repeated reflection overhead                                         |
| `tests/tests.cs`                  | Self-contained test cases for the project requirements                                                    |
| `benchmark/benchmark.cs`          | Reflection metadata performance benchmark                                                                 |
| `PERFORMANCE.md`                  | Benchmark methodology and results                                                                         |
| `PROJECT_MANIFEST.md`             | Project/requirement manifest                                                                              |

## How It Works

The implementation has two main directions.

### Serialization

```text
C# value/object
      ↓
Json.Serialize()
      ↓
JSON writer
      ↓
Reflection for objects
      ↓
Recursive value handling
      ↓
JSON text
```

### Deserialization

```text
JSON text
      ↓
JsonParser
      ↓
JsonValue tree
      ↓
Type conversion
      ↓
Reflection for objects
      ↓
Strongly typed C# value/object
```

The same recursive approach is used for nested objects, arrays, collections, dictionaries, and their combinations.

## Basic Usage

### Serialization

```csharp
var user = new User
{
    Id = 1,
    Name = "John",
    IsActive = true
};

var json = Json.Serialize(user);
```

Result:

```json
{ "Id": 1, "Name": "John", "IsActive": true }
```

### Deserialization

```csharp
var user = Json.Deserialize<User>(json);
```

The serializer discovers the `User` properties dynamically through reflection; no class-specific serialization code is required.

### Indented JSON

```csharp
var json = Json.Serialize(
    user,
    new JsonSerializerOptions
    {
        WriteIndented = true
    });
```

## Supported Types

### Serialization

The implementation supports the following JSON-compatible values:

| Type/category                      | Support                                                |
| ---------------------------------- | ------------------------------------------------------ |
| `null`                             | Yes                                                    |
| `string`                           | Yes, with JSON escaping                                |
| `bool`                             | Yes                                                    |
| `byte`, `sbyte`, `short`, `ushort` | Yes                                                    |
| `int`, `uint`, `long`, `ulong`     | Yes                                                    |
| `float`, `double`, `decimal`       | Yes                                                    |
| `char`                             | Yes, written as a JSON string                          |
| `enum`                             | Yes, written using the enum name                       |
| `DateTime`                         | Yes, round-trip `O` format string                      |
| `DateTimeOffset`                   | Yes, round-trip format string                          |
| `Guid`                             | Yes, standard `D` string representation                |
| Nullable value types               | Yes                                                    |
| Plain objects                      | Yes, through public readable properties and reflection |
| Arrays                             | Yes                                                    |
| `List<T>`                          | Yes                                                    |
| `IEnumerable<T>`                   | Yes                                                    |
| Dictionaries with string keys      | Yes, written as JSON objects                           |
| Nested combinations                | Yes                                                    |

### Deserialization

Typed deserialization supports the same major categories when the requested target type is known:

- primitives and numeric types
- strings and booleans
- nullable value types
- enums
- `DateTime`
- `DateTimeOffset`
- `Guid`
- arrays
- `List<T>`
- compatible `IEnumerable<T>` collection targets
- string-key dictionaries
- nested objects
- nested collections and dictionaries

For `object` values, JSON is converted to an untyped .NET representation. JSON objects become dictionaries, arrays become lists, strings become `string`, booleans become `bool`, integer-looking numbers become `long`, other valid finite numbers become `decimal`, and JSON `null` becomes `null`.

## Reflection-Based Object Serialization

The serializer does not contain special code for individual models.

For a normal object it:

1. Gets the runtime type.
2. Retrieves the public readable properties.
3. Gets each property's value with `PropertyInfo.GetValue()`.
4. Recursively writes the property value.
5. Produces the corresponding JSON object.

Deserialization performs the reverse operation by creating the requested concrete type and assigning converted JSON values to writable public properties with `PropertyInfo.SetValue()`.

This allows the same implementation to work with different user-defined classes without adding serializer logic for each class.

## JSON Parser

`JsonParser` is implemented from scratch using a recursive-descent approach.

It recognizes:

```text
Object   { ... }
Array    [ ... ]
String   "..."
Number   123 / -12.5 / 1e3
Boolean  true / false
Null     null
```

The parser processes the input character by character and validates the JSON grammar instead of trying to repair malformed input.

### Invalid input examples that are rejected

```text
{
[1,]
{"Id"}
{"Id":}
{"a":1 "b":2}
{"a":01}
"unterminated
tru
1e
```

Errors use the custom `JsonException` and include useful context such as the JSON location when parsing fails.

## JSON Value Model

`JsonValue.cs` provides the intermediate representation between raw JSON text and typed C# values.

Conceptually:

```text
JsonValue
├── JsonObjectValue
├── JsonArrayValue
├── JsonStringValue
├── JsonNumberValue
├── JsonBooleanValue
└── JsonNullValue
```

This separation keeps parsing independent from the target C# type. The parser only needs to understand JSON; the conversion layer decides how the parsed value maps to a requested .NET type.

## Collections

Collections are processed recursively. This means the same `WriteValue`/conversion logic can handle combinations such as:

```text
List<int>
List<User>
int[]
IEnumerable<User>
List<List<int>>
Dictionary<string, object>
Dictionary<string, User>
Dictionary<string, List<int>>
```

Dictionaries are serialized as JSON objects because JSON object property names are strings. Therefore non-string dictionary keys are rejected with a descriptive `JsonException`.

## Special Type Decisions

### Enums

Enums are serialized by name rather than by underlying numeric value.

Example:

```csharp
Status.Active
```

becomes:

```json
"Active"
```

Enum name deserialization is case-insensitive.

### DateTime and DateTimeOffset

`DateTime` uses the round-trip `O` format. `DateTimeOffset` is also serialized using a round-trip ISO-style representation so the value can be reconstructed reliably.

### Guid

`Guid` uses the standard `D` representation.

Example:

```text
550e8400-e29b-41d4-a716-446655440000
```

### Nullable values

A nullable value is serialized as its underlying value when present and as JSON `null` when it has no value.

During deserialization, JSON `null` is accepted for nullable value types but rejected for non-nullable value types.

## String Escaping

JSON strings are written with the required surrounding quotation marks and escaping for special characters, including:

```text
"
\\
\b
\f
\n
\r
\t
```

Control characters below `0x20` are represented using `\uXXXX` escapes.

The parser performs the corresponding unescaping when reading JSON strings.

## Error Handling

The project uses a custom `JsonException` instead of allowing low-level implementation errors to become the main user-facing error for JSON problems.

Examples include:

- malformed JSON syntax
- unexpected tokens
- incomplete strings or escapes
- malformed numbers
- invalid literals such as incomplete `true`/`false`/`null`
- duplicate object property names
- type mismatches during deserialization
- numeric overflow or invalid numeric conversion
- invalid enum values
- invalid `DateTime`, `DateTimeOffset`, or `Guid` values
- `null` assigned to a non-nullable value type
- invalid dictionary key types
- unsupported target collection/object types
- circular references when configured to throw

Errors are intended to explain what failed and, where appropriate, identify the relevant property/type or JSON location.

## Circular References

A circular object graph can otherwise cause infinite recursion:

```text
Person
  ↓
Friend
  ↓
Person
  ↓
Friend
  ↓
...
```

The serializer tracks objects currently being serialized using reference identity. This is important because two different objects with equal values are not the same object reference.

Two behaviors are available through `JsonSerializerOptions`:

```csharp
CircularReferenceHandling.Throw
```

The default. A circular reference causes `JsonException`.

```csharp
CircularReferenceHandling.WriteNull
```

The recursive reference is written as JSON `null`, preventing infinite recursion.

The tracking state is removed in a `finally` block so exceptions cannot leave stale references in the writer.

## Reflection Metadata Caching

Reflection metadata can be expensive to discover repeatedly. Without caching, serialization would repeatedly perform operations such as:

```csharp
type.GetProperties(...)
```

The project therefore uses `ReflectionMetadataCache` with `ConcurrentDictionary<Type, PropertyMetadata[]>`.

The first lookup for a type discovers and stores its metadata. Later operations reuse the cached metadata instead of rebuilding it.

### Why caching is useful

Without caching:

```text
Serialize object
    ↓
GetProperties()
    ↓
Create metadata

Serialize same type again
    ↓
GetProperties()
    ↓
Create metadata again
```

With caching:

```text
First call
    ↓
GetProperties()
    ↓
Create metadata
    ↓
Store in cache

Later calls
    ↓
Dictionary lookup
    ↓
Reuse metadata
```

## Performance Benchmark

The benchmark isolates the reflection metadata lookup so that the cached and uncached paths perform equivalent metadata work.

### Measured result

Using **20,000 iterations**:

```text
Reflection metadata benchmark
Iterations : 20,000
Cached     : 0.42 ms (21 ns/op)
Uncached   : 9.76 ms (488 ns/op)
Speedup    : 23.32x
```

The result means that, in this particular environment and benchmark, repeatedly retrieving already-cached metadata was approximately **23.32 times faster** than rebuilding the metadata on every iteration.

This does **not** mean the complete JSON serializer is 23.32 times faster. The benchmark specifically measures the reflection metadata operation being optimized.

Results vary with CPU, operating system, .NET runtime version, system load, and JIT/runtime state. For meaningful measurements, use Release mode and repeat the benchmark after normal system warm-up.

See `PERFORMANCE.md` for the benchmark discussion.

## Running the Project

Because tests and the benchmark are part of the same console application, run the application from the project directory:

```bash
dotnet run
```

The program will:

1. Execute the complete test suite.
2. Print PASS/FAIL results for each test case.
3. Print the total number of passing tests.
4. Run the reflection metadata benchmark.

The test runner currently covers:

```text
Primitive serialization
Object serialization
Nested objects
Collections
IEnumerable deserialization
Dictionary
Deserialization
Special types
Nullable
Escapes
Malformed JSON
Type mismatch
Circular reference Throw
Circular reference WriteNull
Case insensitive properties
```

## Design and OOP Choices

The implementation intentionally uses only abstractions that provide a real benefit for this project.

### Single Responsibility Principle

Responsibilities are separated among the serializer, parser, JSON value representation, options, exception type, metadata cache, tests, and benchmark.

### Open/Closed Principle

Type handling is centralized and recursive, allowing additional supported type conversions to be added without changing the basic parser structure.

### Liskov Substitution Principle

The project has little inheritance because the problem does not naturally require an inheritance-heavy design. LSP is not artificially introduced just to satisfy SOLID.

### Interface Segregation Principle

The implementation does not introduce unnecessary interfaces where they would add complexity without solving a real problem.

### Dependency Inversion Principle

The design uses direct composition where it keeps the small educational project understandable rather than adding abstraction layers solely for compliance. The important dependencies are kept localized by responsibility.

## Design Patterns and Techniques

The project uses several patterns/techniques where they naturally fit the problem:

- **Facade-like API:** the public `Json` entry point hides the details of parsing, writing and conversion.
- **Cache:** `ReflectionMetadataCache` avoids repeated reflection metadata discovery.
- **Recursive processing:** nested objects, collections and dictionaries reuse the same value-writing/conversion logic.
- **Composite-like JSON tree:** `JsonValue` and its object/array/value forms represent arbitrary JSON structures.
- **Configuration/strategy-like behavior:** `JsonSerializerOptions` controls behavior such as circular-reference handling and formatting.

No unnecessary factory or interface hierarchy is introduced merely to demonstrate a design-pattern name.

## Intentional Limitations

This is an educational implementation, not a production replacement for `System.Text.Json`.

Current limitations include:

- Public properties are the supported object members; fields are not serialized by the reflection mapper.
- Deserialization requires a concrete target type that can be instantiated by the implemented reflection approach.
- No custom converter/attribute system is implemented.
- No polymorphic type metadata system is implemented.
- No reference-preservation IDs are generated for circular graphs; cycles are handled by the configured `Throw`/`WriteNull` behavior.
- Standard JSON is targeted; comments and other non-standard JSON extensions are not accepted.
- Dictionary keys are required to be strings.
- Collection support focuses on arrays, `List<T>`, and compatible `IEnumerable<T>`/collection targets required by the project.
- The implementation intentionally prioritizes clarity and project requirements over the full feature set and performance optimizations of production JSON libraries.

## Project Requirement Coverage

The implementation covers the project requirements as follows:

| Requirement                             | Status |
| --------------------------------------- | ------ |
| Primitive serialization                 | ✅     |
| Reflection-based object serialization   | ✅     |
| Nested objects                          | ✅     |
| Arrays / `List<T>` / `IEnumerable<T>`   | ✅     |
| Dictionaries                            | ✅     |
| JSON parser                             | ✅     |
| Typed deserialization                   | ✅     |
| Malformed JSON errors                   | ✅     |
| Type mismatch errors                    | ✅     |
| `DateTime`                              | ✅     |
| `Guid`                                  | ✅     |
| Enum                                    | ✅     |
| Nullable value types                    | ✅     |
| Circular-reference handling             | ✅     |
| Reflection metadata caching             | ✅     |
| Before/after-style reflection benchmark | ✅     |
| Documentation                           | ✅     |

## Final Notes

The main learning objective of this project is not simply to reproduce JSON syntax. It is to understand the stages involved in a serializer/deserializer:

```text
C# value
  ↓
Reflection / type inspection
  ↓
Recursive writing
  ↓
JSON text

JSON text
  ↓
Lexical/syntactic parsing
  ↓
Intermediate JSON representation
  ↓
Type conversion
  ↓
Reflection-based object construction
  ↓
C# value
```

The implementation demonstrates these stages without delegating the core JSON work to an existing JSON package.
