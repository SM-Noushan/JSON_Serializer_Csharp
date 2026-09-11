using System.Globalization;

namespace JSONSerializer.libs;

internal abstract class JsonValue;

internal sealed class JsonObjectValue : JsonValue
{
    public Dictionary<string, JsonValue> Properties { get; } = new(StringComparer.Ordinal);
}

internal sealed class JsonArrayValue : JsonValue
{
    public List<JsonValue> Items { get; } = [];
}

internal sealed class JsonStringValue(string value) : JsonValue
{
    public string Value { get; } = value;
}

internal sealed class JsonNumberValue(string rawText) : JsonValue
{
    public string RawText { get; } = rawText;

    public bool IsInteger =>
        !RawText.Contains('.', StringComparison.Ordinal) &&
        !RawText.Contains('e', StringComparison.OrdinalIgnoreCase);

    public bool TryGetInt64(out long value) =>
        long.TryParse(RawText, NumberStyles.Integer, CultureInfo.InvariantCulture, out value);

}
internal sealed class JsonBooleanValue(bool value) : JsonValue
{
    public bool Value { get; } = value;
}

internal sealed class JsonNullValue : JsonValue
{
    public static JsonNullValue Instance { get; } = new();
    private JsonNullValue() { }
}