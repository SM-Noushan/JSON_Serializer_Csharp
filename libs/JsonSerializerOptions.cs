namespace JSONSerializer.libs;

public enum CircularReferenceHandling
{
    Throw,
    WriteNull
}

public sealed class JsonSerializerOptions
{
    // init can be used to set the property value during object initialization, but cannot be modified afterward.
    public bool WriteIndented { get; init; }
    public bool PropertyNameCaseInsensitive { get; init; }
    public CircularReferenceHandling CircularReferenceHandling { get; init; } = CircularReferenceHandling.Throw;
}