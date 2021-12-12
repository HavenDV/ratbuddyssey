using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Ratbuddyssey.Apps.Views;

public partial class ChannelsView : UserControl
{
    public ChannelsView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
