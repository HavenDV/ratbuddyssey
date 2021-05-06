using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Ratbuddyssey.Converters
{
    public class ColorToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not System.Drawing.Color color)
            {
                throw new NotImplementedException();
            }

            return new SolidColorBrush(Color.FromArgb(color.A, color.R, color.G, color.B));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
