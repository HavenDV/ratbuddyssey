using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using Ratbuddyssey.ViewModels;
using ReactiveUI;

#nullable enable

namespace Ratbuddyssey.Views
{
    public partial class FileView
    {
        #region Constructors

        public FileView()
        {
            InitializeComponent();

            PlotView.PreviewMouseWheel += (_, args) => args.Handled = true;

            this.WhenActivated(disposable =>
            {
                if (ViewModel == null)
                {
                    return;
                }

                // Commands.
                this.BindCommand(ViewModel,
                        static viewModel => viewModel.OpenFile,
                        static view => view.OpenFileMenuItem)
                    .DisposeWith(disposable);
                this.BindCommand(ViewModel,
                        static viewModel => viewModel.SaveFile,
                        static view => view.SaveFileMenuItem)
                    .DisposeWith(disposable);
                this.BindCommand(ViewModel,
                        static viewModel => viewModel.SaveFileAs,
                        static view => view.SaveFileAsMenuItem)
                    .DisposeWith(disposable);
                this.BindCommand(ViewModel,
                        static viewModel => viewModel.ReloadFile,
                        static view => view.ReloadFileMenuItem)
                    .DisposeWith(disposable);

                // Channels.
                this.OneWayBind(ViewModel,
                        static viewModel => viewModel.Channels,
                        static view => view.ChannelsView.ItemsSource)
                    .DisposeWith(disposable);
                this.Bind(ViewModel,
                        static viewModel => viewModel.SelectedChannel,
                        static view => view.ChannelsView.SelectedItem)
                    .DisposeWith(disposable);

                // Status.
                this.OneWayBind(ViewModel,
                        static viewModel => viewModel.CurrentFile,
                        static view => view.CurrentFileLabel.Content)
                    .DisposeWith(disposable);
                this.Bind(ViewModel,
                        static viewModel => viewModel.AudysseyApp.AmpAssignInfo,
                        static view => view.AmpAssignInfoTextBox.Text)
                    .DisposeWith(disposable);
                this.Bind(ViewModel,
                        static viewModel => viewModel.AudysseyApp.Title,
                        static view => view.TitleTextBox.Text)
                    .DisposeWith(disposable);
                this.Bind(ViewModel,
                        static viewModel => viewModel.AudysseyApp.TargetModelName,
                        static view => view.TargetModelNameTextBox.Text)
                    .DisposeWith(disposable);
                this.Bind(ViewModel,
                        static viewModel => viewModel.AudysseyApp.InterfaceVersion,
                        static view => view.InterfaceVersionTextBox.Text)
                    .DisposeWith(disposable);
                this.Bind(ViewModel,
                        static viewModel => viewModel.AudysseyApp.UpgradeInfo,
                        static view => view.UpgradeInfoTextBox.Text)
                    .DisposeWith(disposable);
                this.Bind(ViewModel,
                        static viewModel => viewModel.AudysseyApp.AdcLineup,
                        static view => view.AdcLineupTextBox.Text)
                    .DisposeWith(disposable);
                this.Bind(ViewModel,
                        static viewModel => viewModel.AudysseyApp.SystemDelay,
                        static view => view.SystemDelayTextBox.Text)
                    .DisposeWith(disposable);
                this.OneWayBind(ViewModel,
                        static viewModel => viewModel.AmpAssignTypeList,
                        static view => view.AmpAssignTypeListComboBox.ItemsSource)
                    .DisposeWith(disposable);
                this.Bind(ViewModel,
                        static viewModel => viewModel.AudysseyApp.EnAmpAssignType,
                        static view => view.AmpAssignTypeListComboBox.SelectedIndex)
                    .DisposeWith(disposable);
                this.OneWayBind(ViewModel,
                        static viewModel => viewModel.MultEQTypeList,
                        static view => view.MultEqTypeListComboBox.ItemsSource)
                    .DisposeWith(disposable);
                this.Bind(ViewModel,
                        static viewModel => viewModel.AudysseyApp.EnMultEQType,
                        static view => view.MultEqTypeListComboBox.SelectedIndex)
                    .DisposeWith(disposable);
                this.OneWayBind(ViewModel,
                        static viewModel => viewModel.TargetCurveTypeList,
                        static view => view.TargetCurveTypeComboBox.ItemsSource)
                    .DisposeWith(disposable);
                this.Bind(ViewModel,
                        static viewModel => viewModel.AudysseyApp.EnTargetCurveType,
                        static view => view.TargetCurveTypeComboBox.SelectedIndex)
                    .DisposeWith(disposable);
                this.Bind(ViewModel,
                        static viewModel => viewModel.AudysseyApp.DynamicEq,
                        static view => view.DynamicEqCheckBox.IsChecked)
                    .DisposeWith(disposable);
                this.Bind(ViewModel,
                        static viewModel => viewModel.AudysseyApp.DynamicVolume,
                        static view => view.DynamicVolumeCheckBox.IsChecked)
                    .DisposeWith(disposable);
                this.Bind(ViewModel,
                        static viewModel => viewModel.AudysseyApp.Auro,
                        static view => view.AuroCheckBox.IsChecked)
                    .DisposeWith(disposable);
                this.Bind(ViewModel,
                        static viewModel => viewModel.AudysseyApp.LfcSupport,
                        static view => view.LfcSupportCheckBox.IsChecked)
                    .DisposeWith(disposable);
                this.Bind(ViewModel,
                        static viewModel => viewModel.AudysseyApp.Lfc,
                        static view => view.LfcCheckBox.IsChecked)
                    .DisposeWith(disposable);

                // Target Curve Points.
                this.OneWayBind(ViewModel,
                        static viewModel => viewModel.IsChannelSelected,
                        static view => view.TargetCurvePointsGroupBox.IsEnabled)
                    .DisposeWith(disposable);
                this.OneWayBind(ViewModel,
                        static viewModel => viewModel.CustomTargetCurvePoints,
                        static view => view.TargetCurvePointsDataGrid.ItemsSource)
                    .DisposeWith(disposable);
                Observable.FromEventPattern<InitializingNewItemEventArgs>(
                        TargetCurvePointsDataGrid,
                        nameof(TargetCurvePointsDataGrid.InitializingNewItem))
                    .Subscribe(value =>
                    {
                        if (value.EventArgs.NewItem is not TargetCurvePointViewModel point)
                        {
                            return;
                        }

                        point.Delete
                            .InvokeCommand(ViewModel.DeleteTargetCurvePoint);
                    })
                    .DisposeWith(disposable);
                Observable.FromEventPattern<DataGridRowEditEndingEventArgs>(
                        TargetCurvePointsDataGrid,
                        nameof(TargetCurvePointsDataGrid.RowEditEnding))
                    .Subscribe(static value =>
                    {
                        if (!value.EventArgs.Row.IsNewItem ||
                            value.EventArgs.Row.Item is not TargetCurvePointViewModel point)
                        {
                            return;
                        }

                        point.IsNew = false;
                    })
                    .DisposeWith(disposable);

                // Channel Info.
                this.OneWayBind(ViewModel,
                        static viewModel => viewModel.IsChannelSelected,
                        static view => view.ChannelInfoGroupBox.IsEnabled)
                    .DisposeWith(disposable);
                this.OneWayBind(ViewModel,
                        static viewModel => viewModel.CrossoverList,
                        static view => view.CustomCrossoverComboBox.ItemsSource)
                    .DisposeWith(disposable);
                this.OneWayBind(ViewModel,
                        static viewModel => viewModel.SpeakerTypeList,
                        static view => view.CustomSpeakerTypeComboBox.ItemsSource)
                    .DisposeWith(disposable);
                this.Bind(ViewModel,
                        static viewModel => viewModel.Channel.Data.TrimAdjustment,
                        static view => view.TrimAdjustmentTextBox.Text)
                    .DisposeWith(disposable);
                this.Bind(ViewModel,
                        static viewModel => viewModel.Channel.Data.DelayAdjustment,
                        static view => view.DelayAdjustmentTextBox.Text)
                    .DisposeWith(disposable);
                this.Bind(ViewModel,
                        static viewModel => viewModel.Channel.Data.FrequencyRangeRolloff,
                        static view => view.FrequencyRangeRolloffTextBox.Text)
                    .DisposeWith(disposable);
                this.Bind(ViewModel,
                        static viewModel => viewModel.Channel.Data.EnChannelType,
                        static view => view.EnChannelTypeTextBox.Text)
                    .DisposeWith(disposable);
                this.Bind(ViewModel,
                        static viewModel => viewModel.Channel.Data.CommandId,
                        static view => view.CommandIdTextBox.Text)
                    .DisposeWith(disposable);
                this.Bind(ViewModel,
                        static viewModel => viewModel.Channel.Data.CustomLevel,
                        static view => view.CustomLevelTextBox.Text)
                    .DisposeWith(disposable);
                this.Bind(ViewModel,
                        static viewModel => viewModel.Channel.Data.CustomDistance,
                        static view => view.CustomDistanceTextBox.Text)
                    .DisposeWith(disposable);
                this.Bind(ViewModel,
                        static viewModel => viewModel.Channel.Data.CustomCrossover,
                        static view => view.CustomCrossoverComboBox.Text)
                    .DisposeWith(disposable);
                this.Bind(ViewModel,
                        static viewModel => viewModel.Channel.Data.CustomSpeakerType,
                        static view => view.CustomSpeakerTypeComboBox.Text)
                    .DisposeWith(disposable);
                this.Bind(ViewModel,
                        static viewModel => viewModel.Channel.Data.IsSkipMeasurement,
                        static view => view.IsSkipMeasurementCheckBox.IsChecked)
                    .DisposeWith(disposable);
                this.Bind(ViewModel,
                        static viewModel => viewModel.Channel.Data.MidrangeCompensation,
                        static view => view.IsSkipMeasurementCheckBox.IsChecked)
                    .DisposeWith(disposable);

                // Channel Report.
                this.OneWayBind(ViewModel,
                        static viewModel => viewModel.IsChannelSelected,
                        static view => view.ChannelReportGroupBox.IsEnabled)
                    .DisposeWith(disposable);
                this.Bind(ViewModel,
                        static viewModel => viewModel.Channel.Data.ChannelReport.EnSpeakerConnect,
                        static view => view.EnSpeakerConnectTextBox.Text)
                    .DisposeWith(disposable);
                this.Bind(ViewModel,
                        static viewModel => viewModel.Channel.Data.ChannelReport.CustomEnSpeakerConnect,
                        static view => view.CustomEnSpeakerConnectTextBox.Text)
                    .DisposeWith(disposable);
                this.Bind(ViewModel,
                        static viewModel => viewModel.Channel.Data.ChannelReport.Distance,
                        static view => view.DistanceTextBox.Text)
                    .DisposeWith(disposable);
                this.Bind(ViewModel,
                        static viewModel => viewModel.Channel.Data.ChannelReport.IsReversePolarity,
                        static view => view.IsReversePolarityCheckBox.IsChecked)
                    .DisposeWith(disposable);

                // Graph View.
                this.OneWayBind(ViewModel,
                        static viewModel => viewModel.MeasurementPositions,
                        static view => view.MeasurementPositionsListView.ItemsSource)
                    .DisposeWith(disposable);
                this.Bind(ViewModel,
                        static viewModel => viewModel.SelectAllMeasurementPositionsIsChecked,
                        static view => view.SelectAllMeasurementPositionsCheckBox.IsChecked)
                    .DisposeWith(disposable);
                this.OneWayBind(ViewModel,
                        static viewModel => viewModel.PlotModel,
                        static view => view.PlotView.Model)
                    .DisposeWith(disposable);
                this.OneWayBind(ViewModel,
                        static viewModel => viewModel.SmoothingFactors,
                        static view => view.SmoothingFactorsListView.ItemsSource)
                    .DisposeWith(disposable);
                this.OneWayBind(ViewModel,
                        static viewModel => viewModel.Ranges,
                        static view => view.RangesListView.ItemsSource)
                    .DisposeWith(disposable);
                this.Bind(ViewModel,
                        static viewModel => viewModel.LogarithmicAxisIsChecked,
                        static view => view.LogarithmicAxisCheckBox.IsChecked)
                    .DisposeWith(disposable);
            });
        }

        #endregion

        #region Event Handlers

        public void HandleDroppedFile(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                return;
            }

            // Note that you can have more than one file.
            if (e.Data.GetData(DataFormats.FileDrop) is not string[] files)
            {
                return;
            }

            ViewModel?.DragFiles.Execute(files).Subscribe();
        }

        #endregion
    }
}
