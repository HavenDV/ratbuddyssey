using System;
using Ratbuddyssey.ViewModels;
#if WPF_APP
using Icon = MaterialDesignThemes.Wpf.PackIconKind;
#else
using Windows.UI.Xaml.Controls;
using Icon = Windows.UI.Xaml.Controls.Symbol;
#endif

#nullable enable

namespace Ratbuddyssey.Converters;

public class TypeToIconConverter : BaseConverter<Type?, Icon>
{
    #if WPF_APP
    /// <summary>
    /// https://materialdesignicons.com/.
    /// </summary>
    public static Icon Convert(Type? type)
    {
        if (type == typeof(FileViewModel))
        {
            return Icon.File;
        }
        if (type == typeof(EthernetViewModel))
        {
            return Icon.EthernetCable;
        }

        return Icon.None;
    }
    #else
    /// <summary>
    /// https://docs.microsoft.com/en-us/uwp/api/windows.ui.xaml.controls.symbol?view=winrt-19041.
    /// </summary>
    public static Icon Convert(Type? type)
    {
        if (type == typeof(FileViewModel))
        {
            return Icon.OpenFile;
        }

        if (type == typeof(EthernetViewModel))
        {
            return Icon.World;
        }

        return Icon.Remove;
    }
    #endif

    public TypeToIconConverter() : base(Convert)
    {
    }
}

#if !WPF_APP
public class TypeToSymbolIconConverter : BaseConverter<Type?, IconElement>
{
    public static SymbolIcon Convert(Type? type)
    {
        return new(TypeToIconConverter.Convert(type));
    }

    public TypeToSymbolIconConverter()
        : base(Convert)
    {
    }
}
#endif