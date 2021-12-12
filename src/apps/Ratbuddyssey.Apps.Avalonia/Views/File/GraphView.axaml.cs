using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Ratbuddyssey.Apps.Views;

public partial class GraphView : UserControl
{
    public GraphView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
