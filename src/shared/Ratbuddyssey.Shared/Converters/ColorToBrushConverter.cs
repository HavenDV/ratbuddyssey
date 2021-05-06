using System.Windows.Media;

#nullable enable

namespace Ratbuddyssey.Converters
{
    public class ColorToBrushConverter : BaseConverter<System.Drawing.Color, Brush>
    {
        public ColorToBrushConverter() : base(static color => new SolidColorBrush(Color.FromArgb(color.A, color.R, color.G, color.B)))
        {
        }
    }
}
