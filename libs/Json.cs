using System.Collections;
using System.Globalization;
using System.Text;
using JSONSerializer.libs;

namespace JsonSerializer.libs;

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

    internal sealed class JsonWriter
    {
        private readonly StringBuilder _builder = new StringBuilder();
        private readonly JsonSerializerOptions _options;
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
            if (value is IEnumerable enumerable) { WriteEnumerable(enumerable); return; }

            WriteObject(value);
        }

        private void WriteObject(object value)
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

        private void WriteEnumerable(IEnumerable enumerable)
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

        private void WriteDictionary(IDictionary dictionary)
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

}