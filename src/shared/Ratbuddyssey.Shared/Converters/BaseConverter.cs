using System;
using ReactiveUI;
#if WPF_APP
using System.Globalization;
using System.Windows.Data;
using System.Windows;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
#endif

#nullable enable

namespace Ratbuddyssey.Converters
{
    public class BaseConverter<TFrom, TTo> : IValueConverter, IBindingTypeConverter
    {
        #region Properties

        protected Func<TFrom, TTo>? Func { get; set; }

        public int Affinity { get; set; } = 10;

        #endregion

        #region Constructors

        public BaseConverter(Func<TFrom, TTo>? func = null)
        {
            Func = func;
        }

        #endregion

        #region Methods

#if WPF_APP
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
#else
        public object? Convert(object? value, Type targetType, object? parameter, string language)
#endif
        {
            return TryConvert(value, targetType, parameter, out var result)
                ? result
                : DependencyProperty.UnsetValue;
        }

#if WPF_APP
        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
#else
        public object ConvertBack(object? value, Type targetType, object? parameter, string language)
#endif
        {
            return DependencyProperty.UnsetValue;
        }

        public int GetAffinityForObjects(Type fromType, Type toType)
        {
            if (fromType == typeof(TFrom) &&
                toType == typeof(TTo))
            {
                return Affinity;
            }

            return 0;
        }

        public bool TryConvert(object? from, Type toType, object? conversionHint, out object? result)
        {
            if ((from == null || from is TFrom || typeof(TFrom).IsEnum) &&
                toType == typeof(TTo))
            {
                result = Func != null ? Func((TFrom?)from!) : default;
                return true;
            }

            result = null;
            return false;
        }

        #endregion
    }
}
