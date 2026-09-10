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
            string json => json.ToString(),
            _ => throw new NotSupportedException($"Type '{value.GetType().Name}' is not supported for serialization.")
        };
    }
}