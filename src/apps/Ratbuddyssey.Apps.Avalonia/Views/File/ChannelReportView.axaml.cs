using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Ratbuddyssey.Apps.Views;

public partial class ChannelReportView : UserControl
{
    public ChannelReportView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
