namespace Ratbuddyssey.Views;

public partial class GraphView
{
#if HAS_WPF
    partial void AfterInitializeComponent()
    {
        PlotView.PreviewMouseWheel += (_, args) => args.Handled = true;
    }
#endif
}