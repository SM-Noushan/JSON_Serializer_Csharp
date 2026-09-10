using System.Globalization;
using System.Text;

namespace JsonSerialize.lib;

public static class JsonSerializer
{
    public static string Serialize(object? value)
    {
        if (value == null) return "null";
        return value switch
        {
            int json => json.ToString(),
            long json => json.ToString(),
            float json => json.ToString(),
            double json => json.ToString(),
            decimal json => json.ToString(),
            bool json => json.ToString(),
            string json => HandleString(json),
            _ => throw new NotSupportedException($"Type '{value.GetType().Name}' is not supported for serialization.")
        };
    }
    public static string HandleString(string value)
    {
        var jsonWrite = new JsonWrite();
        return jsonWrite.WriteString(value);
    }

    internal sealed class JsonWrite
    {
        private readonly StringBuilder _builder = new StringBuilder();
        public string WriteString(string value)
        {
            _builder.Append('"');
            foreach (var ch in value)
            {
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
            }
            return _builder.Append('"').ToString();
            // return _builder.ToString();
        }

    }
}