using System.Collections.Generic;
using System.Linq;

namespace Audyssey.ViewModels
{
    public class DesignTimeMainViewModel
    {
        #region Properties

        public IReadOnlyCollection<TabViewModel> Tabs { get; } = new TabViewModel[]
        {
            new(typeof(FileViewModel), "App"),
            new(typeof(EthernetViewModel), "Ethernet"),
        };

        public TabViewModel? SelectedTab { get; set; }

        #endregion

        #region Constructors

        public DesignTimeMainViewModel()
        {
            SelectedTab = Tabs.First();
        }

        #endregion
    }
}
