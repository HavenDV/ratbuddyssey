#nullable enable

using Windows.UI;

namespace Ratbuddyssey.Converters;

public class ColorToBrushConverter : BaseConverter<System.Drawing.Color, Brush>
{
    public static SolidColorBrush Convert(System.Drawing.Color value)
    {
        return new SolidColorBrush(Color.FromArgb(value.A, value.R, value.G, value.B));
    }

    public ColorToBrushConverter() : base(Convert)
    {
    }
}
