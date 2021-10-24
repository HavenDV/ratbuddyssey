#nullable enable

namespace Ratbuddyssey.Converters;

public class InvertBooleanConverter : BaseConverter<bool, bool>
{
    public InvertBooleanConverter()
        : base(static value => !value)
    {
    }
}
