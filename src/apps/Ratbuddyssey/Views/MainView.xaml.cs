using System.Windows.Controls;
using Audyssey.ViewModels;

#nullable enable

namespace Ratbuddyssey.Views
{
    public partial class MainView
    {
        #region Constructors

        public MainView()
        {
            InitializeComponent();

            ViewModel = new MainViewModel();
            FileView.ViewModel = ViewModel.File;
            EthernetView.ViewModel = ViewModel.Ethernet;
        }

        #endregion

        #region Event Handlers

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //var currentTab = ((TabControl)sender).SelectedIndex;

            //switch (currentTab)
            //{
            //    case 0:
            //        if (FileView.AudysseyApp == null)
            //        {
            //            if (EthernetView.AvrAdapter != null)
            //            {
            //                FileView.DataContext = EthernetView.AvrAdapter;
            //            }
            //        }
            //        else
            //        {
            //            FileView.DataContext = FileView.AudysseyApp;
            //        }
            //        break;
            //    case 1:
            //        if (EthernetView.Avr != null)
            //        {
            //            EthernetView.DataContext = EthernetView.Avr;
            //        }
            //        break;
            //}
        }

        #endregion
    }
}