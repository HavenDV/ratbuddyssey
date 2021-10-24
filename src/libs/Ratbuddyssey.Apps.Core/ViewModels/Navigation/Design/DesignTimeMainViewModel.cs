namespace Ratbuddyssey.ViewModels;

public class DesignTimeMainViewModel
{
    #region Properties

    public IReadOnlyCollection<Type> Tabs { get; } = new Type[]
    {
        typeof(FileViewModel),
        //typeof(EthernetViewModel),
    };

    public Type SelectedTab => Tabs.First();

    #endregion
}
