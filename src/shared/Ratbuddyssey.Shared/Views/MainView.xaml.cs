using System.Reactive.Disposables;
using ReactiveUI;

#nullable enable

namespace Ratbuddyssey.Views
{
    public partial class MainView
    {
        #region Constructors

        public MainView()
        {
            InitializeComponent();

#if WPF_APP
            _ = this.WhenActivated(disposable =>
            {
                if (ViewModel == null)
                {
                    return;
                }

                _ = this.OneWayBind(ViewModel,
                        static viewModel => viewModel.Tabs,
                        static view => view.TabsListView.ItemsSource)
                    .DisposeWith(disposable);
                _ = this.Bind(ViewModel,
                        static viewModel => viewModel.SelectedTab,
                        static view => view.TabsListView.SelectedItem)
                    .DisposeWith(disposable);

                _ = this.OneWayBind(ViewModel,
                        static viewModel => viewModel.Router,
                        static view => view.RoutedViewHost.Router)
                    .DisposeWith(disposable);
            });
#endif
        }

        #endregion
    }
}