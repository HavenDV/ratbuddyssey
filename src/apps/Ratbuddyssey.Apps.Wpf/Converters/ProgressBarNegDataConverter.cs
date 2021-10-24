using System;
using System.Windows.Data;

namespace Ratbuddyssey.Converters
{
    public class ProgressBarNegDataConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((sbyte)value > 0)
            {
                return (sbyte)0;
            }
            else
            {
                return -(sbyte)value;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
