#if WPF_APP
using System.Reactive.Disposables;
using ReactiveUI;

#nullable enable

namespace Ratbuddyssey.Views
{
    public partial class ChannelReportView
    {
        #region Constructors

        public ChannelReportView()
        {
            InitializeComponent();

            _ = this.WhenActivated(disposable =>
            {
                if (ViewModel == null)
                {
                    return;
                }

                _ = this.OneWayBind(ViewModel,
                        static viewModel => viewModel.IsChannelSelected,
                        static view => view.ChannelReportGroupBox.IsEnabled)
                    .DisposeWith(disposable);
                _ = this.Bind(ViewModel,
                        static viewModel => viewModel.Channel.Data.ChannelReport.EnSpeakerConnect,
                        static view => view.EnSpeakerConnectTextBox.Text)
                    .DisposeWith(disposable);
                _ = this.Bind(ViewModel,
                        static viewModel => viewModel.Channel.Data.ChannelReport.CustomEnSpeakerConnect,
                        static view => view.CustomEnSpeakerConnectTextBox.Text)
                    .DisposeWith(disposable);
                _ = this.Bind(ViewModel,
                        static viewModel => viewModel.Channel.Data.ChannelReport.Distance,
                        static view => view.DistanceTextBox.Text)
                    .DisposeWith(disposable);
                _ = this.Bind(ViewModel,
                        static viewModel => viewModel.Channel.Data.ChannelReport.IsReversePolarity,
                        static view => view.IsReversePolarityCheckBox.IsChecked)
                    .DisposeWith(disposable);
            });
        }

        #endregion
    }
}
#endif