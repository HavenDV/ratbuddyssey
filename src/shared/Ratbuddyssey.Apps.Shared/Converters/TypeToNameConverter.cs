using Ratbuddyssey.ViewModels;

#nullable enable

namespace Ratbuddyssey.Converters;

public class TypeToNameConverter : BaseConverter<Type?, string>
{
    /// <summary>
    /// https://materialdesignicons.com/.
    /// </summary>
    public static string Convert(Type? type)
    {
        if (type == typeof(FileViewModel))
        {
            return "File";
        }
        if (type == typeof(EthernetViewModel))
        {
            return "Ethernet";
        }

        return string.Empty;
    }

    public TypeToNameConverter() : base(Convert)
    {
    }
}
