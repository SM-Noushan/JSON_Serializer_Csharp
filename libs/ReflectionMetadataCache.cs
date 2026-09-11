using System.Reflection;
using System.Collections.Concurrent;

namespace JSONSerializer.libs;

internal sealed class PropertyMetadata
{
    public PropertyInfo Property { get; }
    public string Name { get; }
    public Type PropertyType { get; }

    public PropertyMetadata(PropertyInfo property)
    {
        Property = property;
        PropertyType = property.PropertyType;
        Name = property.Name;
    }
}

// Caches reflection metadata so repeated serialization avoids repeated GetProperties calls.
internal static class ReflectionMetadataCache
{
    private static readonly ConcurrentDictionary<Type, PropertyMetadata[]> SerializableCache = new();

    public static PropertyMetadata[] GetSerializableProperties(Type type) =>
        SerializableCache.GetOrAdd(type, static t =>
            t.GetProperties(BindingFlags.Instance | BindingFlags.Public)
             .Where(p => p.CanRead && p.GetIndexParameters().Length == 0)
             .Select(p => new PropertyMetadata(p))
             .ToArray());
}