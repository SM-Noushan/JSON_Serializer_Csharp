using System.Globalization;
using System.Text;
using JSONSerializer.libs;

namespace JsonSerializer.libs;

public static class Json
{
    public static string Serialize<T>(T value)
        => Serialize((object?)value);

    public static string Serialize(object? value)
    {
        var jsonWriter = new JsonWriter();
        jsonWriter.WriteValue(value);
        return jsonWriter.ToString();
    }

    internal sealed class JsonWriter
    {
        private readonly StringBuilder _builder = new StringBuilder();

        public override string ToString() => _builder.ToString();

        public void WriteValue(object? value)
        {
            if (value == null) { _builder.Append("null"); return; }

            var type = value.GetType();

            if (value is double d && (double.IsNaN(d) || double.IsInfinity(d)))
                throw new JsonException(
                    "NaN/Infinity are not valid JSON numbers.");

            if (value is float f && (float.IsNaN(f) || float.IsInfinity(f)))
                throw new JsonException(
                    "NaN/Infinity are not valid JSON numbers.");
            if (type == typeof(string)) { WriteString((string)value); return; }
            if (type == typeof(char)) { WriteString(value.ToString()!); return; }
            if (type == typeof(bool)) { _builder.Append((bool)value ? "true" : "false"); return; }

            _builder.Append(value switch
            {
                int json => json.ToString(CultureInfo.InvariantCulture),
                long json => json.ToString(CultureInfo.InvariantCulture),
                float json => json.ToString(CultureInfo.InvariantCulture),
                double json => json.ToString(CultureInfo.InvariantCulture),
                decimal json => json.ToString(CultureInfo.InvariantCulture),
                bool json => json.ToString(),
                _ => WriteObject(value),
                // _ => throw new NotSupportedException($"Type '{type.Name}' is not supported for serialization.")
            });
        }

        private string WriteObject(object value)
        {
            _builder.Append('{');
            var properties = value.GetType().GetProperties();
            for (int i = 0; i < properties.Length; i++)
            {
                if (i > 0) _builder.Append(',');

                var property = properties[i];
                WriteString(property.Name);
                _builder.Append(':');
                WriteValue(property.GetValue(value));
            }
            _builder.Append('}');
            return "";
        }

        public void WriteString(string value)
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
    }
}