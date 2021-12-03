namespace Ratbuddyssey.Controls;

#if HAS_WINUI
public class GridSplitter : CommunityToolkit.WinUI.UI.Controls.GridSplitter
#else
public class GridSplitter : Microsoft.Toolkit.Uwp.UI.Controls.GridSplitter
#endif
{
}
