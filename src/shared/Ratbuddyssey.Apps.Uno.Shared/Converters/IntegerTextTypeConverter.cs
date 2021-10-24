using Ratbuddyssey.Converters;

#nullable enable

namespace ReactiveUI;

public class IntegerTextTypeConverter : BaseConverter<int, string>
{
    public IntegerTextTypeConverter()
        : base(
            static value => $"{value}",
            static value => int.TryParse(value, out var result) ? result : 0)
    {
    }
}
