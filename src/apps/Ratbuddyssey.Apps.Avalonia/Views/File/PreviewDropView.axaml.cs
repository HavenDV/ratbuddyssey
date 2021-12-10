using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Ratbuddyssey.Apps.Views;

public partial class PreviewDropView : UserControl
{
    public PreviewDropView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
