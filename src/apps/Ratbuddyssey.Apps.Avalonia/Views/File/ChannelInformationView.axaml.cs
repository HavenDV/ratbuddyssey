using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Ratbuddyssey.Apps.Views;

public partial class ChannelInformationView : UserControl
{
    public ChannelInformationView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
