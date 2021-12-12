using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Ratbuddyssey.ViewModels;

namespace Ratbuddyssey.Apps.Views;

public partial class StatusView : UserControl
{
    public StatusView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
