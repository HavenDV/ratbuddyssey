namespace Ratbuddyssey.Controls;

internal class GroupBox : ContentControl
{
    #region Dependency Properties

    #region Header

    public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(
        nameof(Header),
        typeof(string),
        typeof(GroupBox),
        new PropertyMetadata(string.Empty));
    
    public string? Header
    {
        get => (string?)GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    #endregion

    #endregion
}
