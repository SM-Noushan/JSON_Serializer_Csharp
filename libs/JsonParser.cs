using System.Text;
using JSONSerializer.libs;
using System.Globalization;

namespace JsonSerializer.libs;

internal sealed class JsonParser
{
    private readonly string _json;
    private int _index;
    private bool End => _index >= _json.Length;
    private char Current => _json[_index];

    public JsonParser(string json) =>
        _json = json ?? throw new ArgumentNullException(nameof(json));

    public JsonValue Parse()
    {
        SkipWhitespace();
        var value = ParseValue();
        SkipWhitespace();

        if (!End)
            Throw("Unexpected characters after the JSON value.");

        return value;
    }

    private JsonValue ParseValue()
    {
        if (End) Throw("Unexpected end of input while reading a JSON value.");

        return Current switch
        {
            '{' => ParseObject(),
            '[' => ParseArray(),
            '"' => new JsonStringValue(ParseString()),
            't' => ParseLiteral("true", new JsonBooleanValue(true)),
            'f' => ParseLiteral("false", new JsonBooleanValue(false)),
            'n' => ParseLiteral("null", JsonNullValue.Instance),
            '-' or >= '0' and <= '9' => ParseNumber(),
            _ => throw Error("Unexpected token '{0}'.", Current)
        };
    }

    private JsonObjectValue ParseObject()
    {
        Expect('{');
        SkipWhitespace();
        var result = new JsonObjectValue();

        if (TryConsume('}')) return result;

        while (true)
        {
            SkipWhitespace();
            if (End || Current != '"')
                Throw("Expected a string property name.");

            var name = ParseString();
            SkipWhitespace();
            Expect(':');
            SkipWhitespace();

            if (!result.Properties.TryAdd(name, ParseValue()))
                Throw("Duplicate property '{0}'.", name);

            SkipWhitespace();
            if (TryConsume('}')) return result;
            Expect(',');
            SkipWhitespace();
        }
    }

    private JsonArrayValue ParseArray()
    {
        Expect('[');
        SkipWhitespace();
        var result = new JsonArrayValue();

        if (TryConsume(']')) return result;

        while (true)
        {
            SkipWhitespace();
            result.Items.Add(ParseValue());
            SkipWhitespace();

            if (TryConsume(']')) return result;
            Expect(',');
            SkipWhitespace();
        }
    }

    private JsonValue ParseLiteral(string literal, JsonValue value)
    {
        // if (_index + literal.Length > _json.Length ||
        // !string.Equals(_json.AsSpan(_index, literal.Length), literal.AsSpan(), StringComparison.Ordinal))
        if (_index + literal.Length > _json.Length || !_json.AsSpan(_index, literal.Length).SequenceEqual(literal.AsSpan()))
            Throw("Invalid literal. Expected '{0}'.", literal);

        _index += literal.Length;
        return value;
    }

    private JsonNumberValue ParseNumber()
    {
        var start = _index;

        if (TryConsume('-') && End)
            Throw("A minus sign must be followed by digits.");

        if (!End && Current == '0')
        {
            _index++;
            if (!End && char.IsDigit(Current))
                Throw("JSON numbers cannot contain leading zeros.");
        }
        else
        {
            if (End || Current is < '1' or > '9')
                Throw("Invalid number. Expected a digit after the sign.");

            while (!End && char.IsDigit(Current)) _index++;
        }

        if (TryConsume('.'))
        {
            var fractionStart = _index;
            while (!End && char.IsDigit(Current)) _index++;
            if (_index == fractionStart)
                Throw("A decimal point must be followed by at least one digit.");
        }

        if (!End && (Current == 'e' || Current == 'E'))
        {
            _index++;
            if (!End && (Current == '+' || Current == '-')) _index++;

            var exponentStart = _index;
            while (!End && char.IsDigit(Current)) _index++;
            if (_index == exponentStart)
                Throw("An exponent must contain at least one digit.");
        }

        return new JsonNumberValue(_json[start.._index]);
    }

    private string ParseString()
    {
        Expect('"');
        var builder = new StringBuilder();

        while (!End)
        {
            var ch = Current;
            _index++;

            if (ch == '"') return builder.ToString();
            if (ch < 0x20) Throw("Unescaped control character in JSON string.");

            if (ch != '\\')
            {
                builder.Append(ch);
                continue;
            }

            if (End) Throw("Unterminated escape sequence in JSON string.");

            var escape = Current;
            _index++;
            builder.Append(escape switch
            {
                '"' => '"',
                '\\' => '\\',
                '/' => '/',
                'b' => '\b',
                'f' => '\f',
                'n' => '\n',
                'r' => '\r',
                't' => '\t',
                'u' => ParseUnicodeEscape(),
                _ => Error<char>("Invalid escape sequence '\\{0}'.", escape)
            });
        }

        Throw("Unterminated JSON string.");
        return string.Empty;
    }

    private char ParseUnicodeEscape()
    {
        if (_index + 4 > _json.Length)
            Throw("Incomplete unicode escape sequence.");

        var hex = _json.AsSpan(_index, 4);
        if (!ushort.TryParse(hex, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out var value))
            Throw("Invalid unicode escape sequence.");

        _index += 4;
        return (char)value;
    }

    private void SkipWhitespace()
    {
        while (!End && Current is ' ' or '\t' or '\r' or '\n')
            _index++;
    }

    private bool TryConsume(char expected)
    {
        if (!End && Current == expected)
        {
            _index++;
            return true;
        }
        return false;
    }

    private void Expect(char expected)
    {
        if (End || Current != expected)
            Throw("Expected '{0}'.", expected);
        _index++;
    }

    private JsonException Error(string format, params object[] args)
    {
        var line = 1;
        var column = 1;

        for (var i = 0; i < Math.Min(_index, _json.Length); i++)
            if (_json[i] == '\n') { line++; column = 1; }
            else column++;

        return new JsonException(string.Format(CultureInfo.InvariantCulture, format, args) + $" Position: line {line}, column {column}.");
    }

    private void Throw(string format, params object[] args) => throw Error(format, args);
    private T Error<T>(string format, params object[] args) => throw Error(format, args);
}