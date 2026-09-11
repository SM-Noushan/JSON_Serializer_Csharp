namespace JSONSerializer.libs;

// Invoked when JSON is syntactically invalid or cannot be mapped to the requested .NET type.
public sealed class JsonException : Exception
{
    public JsonException(string message) : base(message) { }
    public JsonException(string message, Exception innerException) : base(message, innerException) { }
}