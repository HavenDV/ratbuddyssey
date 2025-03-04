﻿using System.Diagnostics;

namespace Ratbuddyssey.Converters
{
    public class DebugDataBindingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (Debugger.IsAttached)
            {
                Debugger.Break();
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (Debugger.IsAttached)
            {
                Debugger.Break();
            }

            return value;
        }
    }
}
