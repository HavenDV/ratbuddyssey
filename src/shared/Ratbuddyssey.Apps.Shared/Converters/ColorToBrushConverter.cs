#if WPF_APP
using System.Windows.Media;
#else
using Windows.UI;
using Windows.UI.Xaml.Media;
#endif

#nullable enable

namespace Ratbuddyssey.Converters
{
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
}
