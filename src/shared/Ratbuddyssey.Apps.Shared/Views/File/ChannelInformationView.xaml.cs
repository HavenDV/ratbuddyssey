#if HAS_WPF
using System.Reactive.Disposables;
using ReactiveUI;

#nullable enable

namespace Ratbuddyssey.Views
{
    public partial class ChannelInformationView
    {
        #region Constructors

        public ChannelInformationView()
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
                        static view => view.ChannelInformationGroupBox.IsEnabled)
                    .DisposeWith(disposable);
                _ = this.OneWayBind(ViewModel,
                        static viewModel => viewModel.CrossoverList,
                        static view => view.CustomCrossoverComboBox.ItemsSource)
                    .DisposeWith(disposable);
                _ = this.OneWayBind(ViewModel,
                        static viewModel => viewModel.SpeakerTypeList,
                        static view => view.CustomSpeakerTypeComboBox.ItemsSource)
                    .DisposeWith(disposable);
                _ = this.Bind(ViewModel,
                        static viewModel => viewModel.Channel.Data.TrimAdjustment,
                        static view => view.TrimAdjustmentTextBox.Text)
                    .DisposeWith(disposable);
                _ = this.Bind(ViewModel,
                        static viewModel => viewModel.Channel.Data.DelayAdjustment,
                        static view => view.DelayAdjustmentTextBox.Text)
                    .DisposeWith(disposable);
                _ = this.Bind(ViewModel,
                        static viewModel => viewModel.Channel.Data.FrequencyRangeRolloff,
                        static view => view.FrequencyRangeRolloffTextBox.Text)
                    .DisposeWith(disposable);
                _ = this.Bind(ViewModel,
                        static viewModel => viewModel.Channel.Data.EnChannelType,
                        static view => view.EnChannelTypeTextBox.Text)
                    .DisposeWith(disposable);
                _ = this.Bind(ViewModel,
                        static viewModel => viewModel.Channel.Data.CommandId,
                        static view => view.CommandIdTextBox.Text)
                    .DisposeWith(disposable);
                _ = this.Bind(ViewModel,
                        static viewModel => viewModel.Channel.Data.CustomLevel,
                        static view => view.CustomLevelTextBox.Text)
                    .DisposeWith(disposable);
                _ = this.Bind(ViewModel,
                        static viewModel => viewModel.Channel.Data.CustomDistance,
                        static view => view.CustomDistanceTextBox.Text)
                    .DisposeWith(disposable);
                _ = this.Bind(ViewModel,
                        static viewModel => viewModel.Channel.Data.CustomCrossover,
                        static view => view.CustomCrossoverComboBox.Text)
                    .DisposeWith(disposable);
                _ = this.Bind(ViewModel,
                        static viewModel => viewModel.Channel.Data.CustomSpeakerType,
                        static view => view.CustomSpeakerTypeComboBox.Text)
                    .DisposeWith(disposable);
                _ = this.Bind(ViewModel,
                        static viewModel => viewModel.Channel.Data.IsSkipMeasurement,
                        static view => view.IsSkipMeasurementCheckBox.IsChecked)
                    .DisposeWith(disposable);
                _ = this.Bind(ViewModel,
                        static viewModel => viewModel.Channel.Data.MidrangeCompensation,
                        static view => view.MidrangeCompensationCheckBox.IsChecked)
                    .DisposeWith(disposable);
            });
        }

        #endregion
    }
}
#endif