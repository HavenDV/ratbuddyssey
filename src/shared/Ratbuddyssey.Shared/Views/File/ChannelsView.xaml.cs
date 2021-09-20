using System.Reactive.Disposables;
using ReactiveUI;

#nullable enable

namespace Ratbuddyssey.Views
{
    public partial class ChannelsView
    {
        #region Constructors

        public ChannelsView()
        {
            InitializeComponent();

            _ = this.WhenActivated(disposable =>
            {
                if (ViewModel == null)
                {
                    return;
                }

                _ = this.OneWayBind(ViewModel,
                        static viewModel => viewModel.Channels,
                        static view => view.ChannelsListView.ItemsSource)
                    .DisposeWith(disposable);
                _ = this.Bind(ViewModel,
                        static viewModel => viewModel.SelectedChannel,
                        static view => view.ChannelsListView.SelectedItem)
                    .DisposeWith(disposable);
            });
        }

        #endregion
    }
}
