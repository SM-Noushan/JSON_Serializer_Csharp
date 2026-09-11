using System.Text;
using System.Collections;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace JSONSerializer.libs;

public static class Json
{
    public static string Serialize<T>(T value, JsonSerializerOptions? options = null)
        => Serialize((object?)value, options);

    public static string Serialize(object? value, JsonSerializerOptions? options = null)
    {
        options ??= new JsonSerializerOptions();
        var jsonWriter = new JsonWriter(options);
        jsonWriter.WriteValue(value);
        return jsonWriter.ToString();
    }

    public static T? Deserialize<T>(string json, JsonSerializerOptions? options = null)
    {
        var result = Deserialize(json, typeof(T), options);
        return result is null ? default : (T)result;
    }

    public static object? Deserialize(string json, Type targetType, JsonSerializerOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(targetType);
        options ??= new JsonSerializerOptions();
        var root = new JsonParser(json).Parse();
        return JsonValueConverter.Convert(root, targetType, options);
    }
}

internal sealed class JsonWriter
{
    private readonly StringBuilder _builder = new StringBuilder();
    private readonly JsonSerializerOptions _options;
    private readonly HashSet<object> _activeReferences = new(ReferenceEqualityComparer.Instance);
    private int _depth;

    public JsonWriter(JsonSerializerOptions options)
        => _options = options;

    public override string ToString() => _builder.ToString();

    public void WriteValue(object? value)
    {
        if (value == null) { _builder.Append("null"); return; }

        var type = Nullable.GetUnderlyingType(value.GetType()) ?? value.GetType();

        if (type == typeof(string)) { WriteString((string)value); return; }
        if (type == typeof(char)) { WriteString(value.ToString()!); return; }
        if (type == typeof(bool)) { _builder.Append((bool)value ? "true" : "false"); return; }
        if (type.IsEnum) { WriteString(value.ToString()!); return; }
        if (type == typeof(DateTime)) { WriteString(((DateTime)value).ToString("O", CultureInfo.InvariantCulture)); return; }
        if (type == typeof(DateTimeOffset)) { WriteString(((DateTimeOffset)value).ToString("O", CultureInfo.InvariantCulture)); return; }
        if (type == typeof(Guid)) { WriteString(value.ToString()!); return; }
        if (IsNumeric(type)) { WriteNumeric(value); return; }

        // Dictionaries must be checked before Enumerables because they are also Enumerables.Both must be checked before objects, because they are objects as well.
        if (value is IDictionary dictionary) { WriteDictionary(dictionary); return; }
        if (value is IEnumerable enumerable) { WriteEnumerable(enumerable, value); return; }

        WriteObject(value);
    }

    private void WriteObject(object value)
    {
        if (!TryEnterReference(value)) return;

        try
        {
            _builder.Append('{');
            _depth++;
            var properties = ReflectionMetadataCache.GetSerializableProperties(value.GetType());

            for (int i = 0; i < properties.Length; i++)
            {
                if (i > 0) _builder.Append(',');
                WriteNewLineAndIndentIfNeeded();
                WriteString(properties[i].Name);
                _builder.Append(_options.WriteIndented ? ": " : ":");
                WriteValue(properties[i].Property.GetValue(value));
            }

            _depth--;
            if (properties.Length > 0) WriteNewLineAndIndentIfNeeded();
            _builder.Append('}');
        }
        finally
        {
            _activeReferences.Remove(value);
        }
    }

    private void WriteEnumerable(IEnumerable enumerable, object? reference = null)
    {
        if (!TryEnterReference(reference)) return;
        try
        {
            _builder.Append('[');
            _depth++;
            var first = true;

            foreach (var item in enumerable)
            {
                if (!first) _builder.Append(',');
                first = false;
                WriteNewLineAndIndentIfNeeded();
                WriteValue(item);
            }

            _depth--;
            if (!first) WriteNewLineAndIndentIfNeeded();
            _builder.Append(']');
        }
        finally
        {
            _activeReferences.Remove(reference);
        }
    }

    private void WriteDictionary(IDictionary dictionary)
    {
        if (!TryEnterReference(dictionary)) return;
        try
        {
            _builder.Append('{');
            _depth++;
            var first = true;

            foreach (DictionaryEntry entry in dictionary)
            {
                if (entry.Key is not string key)
                    throw new JsonException("Dictionary keys must be strings for JSON object serialization.");
                if (!first) _builder.Append(',');
                first = false;
                WriteNewLineAndIndentIfNeeded();
                WriteString(key);
                _builder.Append(_options.WriteIndented ? ": " : ":");
                WriteValue(entry.Value);
            }

            _depth--;
            if (!first) WriteNewLineAndIndentIfNeeded();
            _builder.Append('}');
        }
        finally
        {
            _activeReferences.Remove(dictionary);
        }
    }

    private bool TryEnterReference(object value)
    {
        if (_activeReferences.Add(value)) return true;

        if (_options.CircularReferenceHandling == CircularReferenceHandling.WriteNull)
        {
            _builder.Append("null");
            return false;
        }

        throw new JsonException($"Circular reference detected while serializing '{value.GetType().FullName}'.");
    }

    private void WriteString(string value)
    {
        _builder.Append('"');
        foreach (var ch in value)
            switch (ch)
            {
                case '"': _builder.Append("\\\""); break;
                case '\\': _builder.Append("\\\\"); break;
                case '\b': _builder.Append("\\b"); break;
                case '\f': _builder.Append("\\f"); break;
                case '\n': _builder.Append("\\n"); break;
                case '\r': _builder.Append("\\r"); break;
                case '\t': _builder.Append("\\t"); break;
                default:
                    //control characters 0-31 => \uXXXX
                    if (ch < 0x20) _builder.Append("\\u").Append(((int)ch).ToString("x4", CultureInfo.InvariantCulture));
                    else _builder.Append(ch);
                    break;
            }
        _builder.Append('"');
    }

    private void WriteNumeric(object value)
    {
        switch (value)
        {
            case byte v: _builder.Append(v); break;
            case sbyte v: _builder.Append(v); break;
            case short v: _builder.Append(v); break;
            case ushort v: _builder.Append(v); break;
            case int v: _builder.Append(v); break;
            case uint v: _builder.Append(v); break;
            case long v: _builder.Append(v); break;
            case ulong v: _builder.Append(v); break;
            case decimal d: _builder.Append(d.ToString(CultureInfo.InvariantCulture)); break;
            case float f when float.IsFinite(f): _builder.Append(f.ToString("R", CultureInfo.InvariantCulture)); break;
            case double d when double.IsFinite(d): _builder.Append(d.ToString("R", CultureInfo.InvariantCulture)); break;
            case float: throw new JsonException("NaN/Infinity are not valid JSON numbers.");
            case double: throw new JsonException("NaN/Infinity are not valid JSON numbers.");
            default: throw new JsonException($"Unsupported numeric type '{value.GetType().FullName}'.");
        }
    }

    private void WriteNewLineAndIndentIfNeeded()
    {
        if (!_options.WriteIndented) return;
        _builder.AppendLine();
        _builder.Append(' ', _depth * 2);
    }

    private static bool IsNumeric(Type type)
        => Type.GetTypeCode(type) is TypeCode.Byte or TypeCode.SByte or TypeCode.Int16 or TypeCode.UInt16 or TypeCode.Int32 or TypeCode.UInt32 or TypeCode.Int64 or TypeCode.UInt64 or TypeCode.Single or TypeCode.Double or TypeCode.Decimal;
}

internal static class JsonValueConverter
{
    public static object? Convert(JsonValue node, Type targetType, JsonSerializerOptions options)
    {
        if (Nullable.GetUnderlyingType(targetType) is { } nullableType)
        {
            if (node is JsonNullValue) return null;
            return Convert(node, nullableType, options);
        }

        if (node is JsonNullValue)
        {
            if (targetType.IsValueType) throw new JsonException($"Cannot assign JSON null to non-nullable type '{targetType.FullName}'.");
            return null;
        }

        if (targetType == typeof(object)) return ConvertToUntyped(node, options);
        if (targetType == typeof(string)) return RequireString(node);
        if (targetType == typeof(bool)) return RequireBoolean(node);
        if (targetType == typeof(char)) return RequireString(node) switch
        {
            { Length: 1 } text => text[0],
            _ => throw new JsonException("A JSON string must contain exactly one character when deserializing char.")
        };
        if (targetType.IsEnum) return ConvertEnum(node, targetType);
        if (targetType == typeof(DateTime)) return ParseDateTime(node);
        if (targetType == typeof(DateTimeOffset)) return ParseDateTimeOffset(node);
        if (targetType == typeof(Guid)) return ParseGuid(node);

        if (IsNumeric(targetType)) return ConvertNumber(node, targetType);

        if (TryGetDictionaryShape(targetType, out var keyType, out var valueType))
            return ConvertDictionary(node, targetType, keyType!, valueType!, options);

        if (TryGetCollectionElementType(targetType, out var elementType))
            return ConvertCollection(node, targetType, elementType!, options);

        if (node is not JsonObjectValue objectNode)
            throw new JsonException($"Cannot deserialize JSON {Describe(node)} into '{targetType.FullName}'.");

        return ConvertObject(objectNode, targetType, options);
    }

    private static object ConvertObject(JsonObjectValue node, Type targetType, JsonSerializerOptions options)
    {
        if (targetType.IsAbstract || targetType.IsInterface)
            throw new JsonException($"Cannot create an instance of '{targetType.FullName}'. A concrete target type is required.");

        object instance;
        try
        {
            instance = Activator.CreateInstance(targetType)
                ?? throw new JsonException($"Could not create an instance of '{targetType.FullName}'.");
        }
        catch (JsonException) { throw; }
        catch (Exception ex)
        {
            throw new JsonException($"Could not create '{targetType.FullName}'. It must have a public parameterless constructor.", ex);
        }

        var properties = ReflectionMetadataCache.GetDeserializableProperties(targetType);
        var propertyMap = properties.ToDictionary(p => p.Name,
            p => p,
            options.PropertyNameCaseInsensitive ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal);

        foreach (var (name, value) in node.Properties)
        {
            if (!propertyMap.TryGetValue(name, out var property))
                continue;

            try
            {
                var converted = Convert(value, property.PropertyType, options);
                property.Property.SetValue(instance, converted);
            }
            catch (JsonException ex)
            {
                throw new JsonException($"Failed to deserialize property '{name}' of '{targetType.FullName}': {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new JsonException($"Failed to assign property '{name}' of '{targetType.FullName}'.", ex);
            }
        }

        return instance;
    }

    private static object ConvertCollection(JsonValue node, Type targetType, Type elementType, JsonSerializerOptions options)
    {
        if (node is not JsonArrayValue array)
            throw new JsonException($"Expected a JSON array for '{targetType.FullName}'.");

        var values = array.Items.Select(item => Convert(item, elementType, options)).ToList();

        if (targetType.IsArray)
        {
            var result = Array.CreateInstance(elementType, values.Count);
            for (var i = 0; i < values.Count; i++) result.SetValue(values[i], i);
            return result;
        }

        if (targetType.IsInterface || targetType.IsAbstract)
        {
            if (targetType.IsAssignableFrom(typeof(List<>).MakeGenericType(elementType)))
                return CreateList(elementType, values);
            throw new JsonException($"Collection type '{targetType.FullName}' is not supported.");
        }

        if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(List<>))
            return CreateList(elementType, values);

        var collection = Activator.CreateInstance(targetType);
        if (collection is null) throw new JsonException($"Could not create collection '{targetType.FullName}'.");
        var add = targetType.GetMethod("Add", [elementType]);
        if (add is null) throw new JsonException($"Collection type '{targetType.FullName}' must expose an Add method.");
        foreach (var value in values) add.Invoke(collection, [value]);
        return collection;
    }

    private static object CreateList(Type elementType, List<object?> values)
    {
        var listType = typeof(List<>).MakeGenericType(elementType);
        var list = Activator.CreateInstance(listType)!;
        var add = listType.GetMethod("Add", [elementType])!;
        foreach (var value in values) add.Invoke(list, [value]);
        return list;
    }

    private static object ConvertDictionary(JsonValue node, Type targetType, Type keyType, Type valueType, JsonSerializerOptions options)
    {
        if (keyType != typeof(string))
            throw new JsonException("Only dictionaries with string keys can be represented as JSON objects.");
        if (node is not JsonObjectValue objectNode)
            throw new JsonException($"Expected a JSON object for dictionary type '{targetType.FullName}'.");

        var concreteType = targetType;
        if (targetType.IsInterface || targetType.IsAbstract)
            concreteType = typeof(Dictionary<,>).MakeGenericType(keyType, valueType);

        var dictionary = Activator.CreateInstance(concreteType) as IDictionary;
        if (dictionary is null)
            throw new JsonException($"Dictionary type '{targetType.FullName}' could not be created.");

        foreach (var (key, jsonValue) in objectNode.Properties)
            dictionary.Add(key, Convert(jsonValue, valueType, options));
        return dictionary;
    }

    private static object? ConvertToUntyped(JsonValue node, JsonSerializerOptions options) => node switch
    {
        JsonNullValue => null,
        JsonStringValue s => s.Value,
        JsonBooleanValue b => b.Value,
        JsonNumberValue n when n.IsInteger && n.TryGetInt64(out var integer) => integer,
        JsonNumberValue n when decimal.TryParse(n.RawText, NumberStyles.Float, CultureInfo.InvariantCulture, out var decimalValue) => decimalValue,
        JsonNumberValue n => throw new JsonException($"Invalid number '{n.RawText}'."),
        JsonArrayValue a => a.Items.Select(item => ConvertToUntyped(item, options)).ToList(),
        JsonObjectValue o => o.Properties.ToDictionary(p => p.Key, p => ConvertToUntyped(p.Value, options), StringComparer.Ordinal),
        _ => throw new JsonException("Unsupported JSON value.")
    };

    private static object ConvertNumber(JsonValue node, Type targetType)
    {
        if (node is not JsonNumberValue number)
            throw new JsonException($"Expected a JSON number for '{targetType.FullName}'.");

        try
        {
            return targetType == typeof(byte) ? byte.Parse(number.RawText, CultureInfo.InvariantCulture) :
                   targetType == typeof(sbyte) ? sbyte.Parse(number.RawText, CultureInfo.InvariantCulture) :
                   targetType == typeof(short) ? short.Parse(number.RawText, CultureInfo.InvariantCulture) :
                   targetType == typeof(ushort) ? ushort.Parse(number.RawText, CultureInfo.InvariantCulture) :
                   targetType == typeof(int) ? int.Parse(number.RawText, CultureInfo.InvariantCulture) :
                   targetType == typeof(uint) ? uint.Parse(number.RawText, CultureInfo.InvariantCulture) :
                   targetType == typeof(long) ? long.Parse(number.RawText, CultureInfo.InvariantCulture) :
                   targetType == typeof(ulong) ? ulong.Parse(number.RawText, CultureInfo.InvariantCulture) :
                   targetType == typeof(float) ? float.Parse(number.RawText, CultureInfo.InvariantCulture) :
                   targetType == typeof(double) ? double.Parse(number.RawText, CultureInfo.InvariantCulture) :
                   targetType == typeof(decimal) ? decimal.Parse(number.RawText, CultureInfo.InvariantCulture) :
                   throw new JsonException($"Unsupported numeric type '{targetType.FullName}'.");
        }
        catch (JsonException) { throw; }
        catch (Exception ex)
        {
            throw new JsonException($"JSON number '{number.RawText}' cannot be converted to '{targetType.FullName}'.", ex);
        }
    }

    private static object ConvertEnum(JsonValue node, Type targetType)
    {
        var text = RequireString(node);
        if (!Enum.TryParse(targetType, text, ignoreCase: true, out var result))
            throw new JsonException($"'{text}' is not a valid {targetType.Name} value.");
        return result!;
    }

    private static DateTime ParseDateTime(JsonValue node)
    {
        var text = RequireString(node);
        if (!DateTime.TryParse(text, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var result))
            throw new JsonException($"'{text}' is not a valid ISO-8601 DateTime.");
        return result;
    }

    private static DateTimeOffset ParseDateTimeOffset(JsonValue node)
    {
        var text = RequireString(node);
        if (!DateTimeOffset.TryParse(text, CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
            throw new JsonException($"'{text}' is not a valid ISO-8601 DateTimeOffset.");
        return result;
    }

    private static Guid ParseGuid(JsonValue node)
    {
        var text = RequireString(node);
        if (!Guid.TryParse(text, out var result)) throw new JsonException($"'{text}' is not a valid Guid.");
        return result;
    }

    private static string RequireString(JsonValue node) =>
        node is JsonStringValue value
            ? value.Value
            : throw new JsonException("Expected a JSON string.");

    private static bool RequireBoolean(JsonValue node) => node is JsonBooleanValue value
       ? value.Value
       : throw new JsonException("Expected a JSON boolean.");

    private static string Describe(JsonValue node) => node switch
    {
        JsonObjectValue => "object",
        JsonArrayValue => "array",
        JsonStringValue => "string",
        JsonNumberValue => "number",
        JsonBooleanValue => "boolean",
        JsonNullValue => "null",
        _ => "value"
    };

    private static bool TryGetCollectionElementType(Type type, out Type? elementType)
    {
        if (type.IsArray)
        {
            elementType = type.GetElementType();
            return elementType is not null;
        }

        if (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(List<>) ||
                                   type.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
        {
            elementType = type.GetGenericArguments()[0];
            return true;
        }

        var enumerable = type.GetInterfaces()
            .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>));
        if (enumerable is not null)
        {
            elementType = enumerable.GetGenericArguments()[0];
            return true;
        }

        elementType = null;
        return false;
    }

    private static bool TryGetDictionaryShape(Type type, out Type? keyType, out Type? valueType)
    {
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
        {
            var args = type.GetGenericArguments();
            keyType = args[0]; valueType = args[1]; return true;
        }

        var dictionary = type.GetInterfaces()
            .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDictionary<,>));
        if (dictionary is not null)
        {
            var args = dictionary.GetGenericArguments();
            keyType = args[0]; valueType = args[1]; return true;
        }

        keyType = valueType = null;
        return false;
    }

    public static bool IsNumeric(Type type)
        => Type.GetTypeCode(type) is TypeCode.Byte or TypeCode.SByte or TypeCode.Int16 or TypeCode.UInt16 or TypeCode.Int32 or TypeCode.UInt32 or TypeCode.Int64 or TypeCode.UInt64 or TypeCode.Single or TypeCode.Double or TypeCode.Decimal;
}

internal sealed class ReferenceEqualityComparer : IEqualityComparer<object>
{
    public static ReferenceEqualityComparer Instance { get; } = new();
    private ReferenceEqualityComparer() { }
    public new bool Equals(object? x, object? y) => ReferenceEquals(x, y);
    public int GetHashCode(object obj) => RuntimeHelpers.GetHashCode(obj);
}