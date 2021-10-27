﻿#nullable enable

namespace Ratbuddyssey.Converters;

public class BaseConverter<TFrom, TTo> : IValueConverter, IBindingTypeConverter
{
    #region Properties

    protected Func<TFrom, object?, TTo> ConvertFunc { get; set; }
    protected Func<TTo, object?, TFrom>? ConvertBackFunc { get; set; }

    public int Affinity { get; set; } = 100;

    #endregion

    #region Constructors

    public BaseConverter(
        Func<TFrom, object?, TTo> convertFunc,
        Func<TTo, object?, TFrom>? convertBackFunc = null)
    {
        ConvertFunc = convertFunc ?? throw new ArgumentNullException(nameof(convertFunc));
        ConvertBackFunc = convertBackFunc;
    }

    public BaseConverter(
        Func<TFrom, TTo> convertFunc,
        Func<TTo, TFrom>? convertBackFunc = null)
    {
        convertFunc = convertFunc ?? throw new ArgumentNullException(nameof(convertFunc));

        ConvertFunc = (value, _) => convertFunc(value);
        if (convertBackFunc != null)
        {
            ConvertBackFunc = (value, _) => convertBackFunc(value);
        }
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
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
#else
    public object? ConvertBack(object? value, Type targetType, object? parameter, string language)
#endif
    {
        return TryConvert(value, targetType, parameter, out var result)
            ? result
            : DependencyProperty.UnsetValue;
    }

    public int GetAffinityForObjects(Type fromType, Type toType)
    {
        if (fromType == typeof(TFrom) &&
            (toType == typeof(TTo) || toType == typeof(object)))
        {
            return Affinity;
        }
        if (ConvertBackFunc != null &&
            fromType == typeof(TTo) &&
            toType == typeof(TFrom))
        {
            return Affinity;
        }

        return 0;
    }

    public bool TryConvert(object? from, Type toType, object? conversionHint, out object? result)
    {
        if (toType == typeof(TTo) || toType == typeof(object))
        {
            if (from == null)
            {
                result = null;
                return true;
            }
            if (from is TFrom fromCasted)
            {
                result = ConvertFunc(fromCasted, conversionHint);
                return true;
            }
            if (typeof(TFrom).IsEnum)
            {
                result = ConvertFunc((TFrom)from, conversionHint);
                return true;
            }
        }
        if (ConvertBackFunc != null &&
            toType == typeof(TFrom))
        {
            if (from == null)
            {
                result = null;
                return true;
            }
            if (from is TTo toCasted)
            {
                result = ConvertBackFunc(toCasted, conversionHint);
                return true;
            }
            if (typeof(TTo).IsEnum)
            {
                result = ConvertBackFunc((TTo)from, conversionHint);
                return true;
            }
        }

        result = null;
        return false;
    }

    #endregion
}
