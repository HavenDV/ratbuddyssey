using System.Collections.Concurrent;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Ratbuddyssey;

public class InterfaceContractResolver : DefaultContractResolver
{
    private readonly Type[] _interfaceTypes;

    private readonly ConcurrentDictionary<Type, Type> _typeToSerializeMap;

    public InterfaceContractResolver(params Type[] interfaceTypes)
    {
        _interfaceTypes = interfaceTypes;

        _typeToSerializeMap = new ConcurrentDictionary<Type, Type>();
    }

    protected override IList<JsonProperty> CreateProperties(
        Type type,
        MemberSerialization memberSerialization)
    {
        var typeToSerialize = _typeToSerializeMap.GetOrAdd(
            type,
            t => _interfaceTypes.FirstOrDefault(
                it => it.IsAssignableFrom(t)) ?? t);

        var props = base.CreateProperties(typeToSerialize, memberSerialization);

        // mark all props as not ignored
        foreach (var prop in props)
        {
            prop.Ignored = false;
        }

        return props;
    }
}
