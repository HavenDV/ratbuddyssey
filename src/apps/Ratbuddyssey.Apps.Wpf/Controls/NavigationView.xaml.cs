using System;
using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Ratbuddyssey.Controls;

public partial class NavigationView : HeaderedContentControl
{
    #region Dependency Properties

    public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.Register(
        nameof(ItemTemplate),
        typeof(DataTemplate),
        typeof(NavigationView),
        new PropertyMetadata(null));

    public static readonly DependencyProperty MenuItemsSourceProperty = DependencyProperty.Register(
        nameof(MenuItemsSource),
        typeof(IEnumerable),
        typeof(NavigationView),
        new PropertyMetadata(Array.Empty<string>()));

    public static readonly DependencyProperty FooterMenuItemsSourceProperty = DependencyProperty.Register(
        nameof(FooterMenuItemsSource),
        typeof(IEnumerable),
        typeof(NavigationView),
        new PropertyMetadata(Array.Empty<string>()));

    public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register(
        nameof(SelectedItem),
        typeof(object),
        typeof(NavigationView),
        new PropertyMetadata(null));

    #endregion

    #region Properties

    [Bindable(true)]
    public DataTemplate? ItemTemplate { get; set; }

    [Bindable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IEnumerable MenuItemsSource {
        get => (IEnumerable)GetValue(MenuItemsSourceProperty);
        set => SetValue(MenuItemsSourceProperty, value);
    }

    [Bindable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IEnumerable FooterMenuItemsSource {
        get => (IEnumerable)GetValue(FooterMenuItemsSourceProperty);
        set => SetValue(FooterMenuItemsSourceProperty, value);
    }

    [Bindable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public object? SelectedItem {
        get => GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }

    #endregion
}
