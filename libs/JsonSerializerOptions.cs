namespace JSONSerializer.libs;

public sealed class JsonSerializerOptions
{
    // init can be used to set the property value during object initialization, but cannot be modified afterward.
    public bool WriteIndented { get; init; }
}