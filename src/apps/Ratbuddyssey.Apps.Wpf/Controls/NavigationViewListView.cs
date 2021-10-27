namespace Ratbuddyssey.Controls;

/// <summary>
/// This ListView disallows the SelectedItem to be saved,
/// thus eliminating the bug where it is impossible to select
/// the previous item from FooterNavigationItems/NavigationTems
/// if a new item from the opposite collection was selected.
/// </summary>
public class NavigationViewListView : ListView
{
    #region Constructors

    static NavigationViewListView()
    {
        SelectedItemProperty.OverrideMetadata(
            typeof(NavigationViewListView),
            new FrameworkPropertyMetadata(null, null, (_, _) => null));
    }

    #endregion
}
