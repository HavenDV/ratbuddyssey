#nullable enable

namespace Ratbuddyssey.Views
{
    public partial class MainView
    {
        #region Constructors

        public MainView()
        {
            InitializeComponent();

            _ = this.WhenActivated(disposable =>
            {
                if (ViewModel == null)
                {
                    return;
                }

                _ = this.OneWayBind(ViewModel,
                        static viewModel => viewModel.Tabs,
                        static view => view.NavigationControl.MenuItemsSource)
                    .DisposeWith(disposable);
                _ = this.Bind(ViewModel,
                        static viewModel => viewModel.SelectedTab,
                        static view => view.NavigationControl.SelectedItem)
                    .DisposeWith(disposable);

                _ = this.OneWayBind(ViewModel,
                        static viewModel => viewModel.Router,
                        static view => view.RoutedViewHost.Router)
                    .DisposeWith(disposable);
            });
        }

        #endregion
    }
}