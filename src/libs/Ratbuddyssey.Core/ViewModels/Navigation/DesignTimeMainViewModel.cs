using System.Collections.Generic;
using System.Linq;

namespace Ratbuddyssey.ViewModels
{
    public class DesignTimeMainViewModel
    {
        #region Properties

        public IReadOnlyCollection<TabViewModel> Tabs { get; } = new TabViewModel[]
        {
            new(typeof(FileViewModel), "App"),
            new(typeof(EthernetViewModel), "Ethernet"),
        };

        public TabViewModel SelectedTab => Tabs.First();

        #endregion
    }
}
