using System;
using System.Windows.Data;

namespace Ratbuddyssey.Converters
{
    public class ValueToAngleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((sbyte)value > 0)
                return (double)10.0;
            else
                return (double)20.0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
