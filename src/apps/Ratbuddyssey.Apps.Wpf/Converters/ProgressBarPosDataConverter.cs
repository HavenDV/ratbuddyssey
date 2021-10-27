namespace Ratbuddyssey.Converters
{
    public class ProgressBarPosDataConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((sbyte)value > 0)
            {
                return (sbyte)value;
            }
            else
            {
                return (sbyte)0;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
