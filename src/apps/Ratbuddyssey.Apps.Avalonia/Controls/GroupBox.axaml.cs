using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml;

namespace Ratbuddyssey.Apps.Controls;

public partial class GroupBox : HeaderedContentControl
{
    public GroupBox()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
