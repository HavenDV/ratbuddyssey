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

            _ = this.WhenActivated(disposable =>
            {
                if (ViewModel == null)
                {
                    return;
                }

                // Commands.
                _ = this.BindCommand(ViewModel,
                        static viewModel => viewModel.OpenFile,
                        static view => view.OpenFileMenuItem)
                    .DisposeWith(disposable);
                _ = this.BindCommand(ViewModel,
                        static viewModel => viewModel.SaveFile,
                        static view => view.SaveFileMenuItem)
                    .DisposeWith(disposable);
                _ = this.BindCommand(ViewModel,
                        static viewModel => viewModel.SaveFileAs,
                        static view => view.SaveFileAsMenuItem)
                    .DisposeWith(disposable);
                _ = this.BindCommand(ViewModel,
                        static viewModel => viewModel.ReloadFile,
                        static view => view.ReloadFileMenuItem)
                    .DisposeWith(disposable);

                // Channels.
                _ = this.OneWayBind(ViewModel,
                        static viewModel => viewModel.Channels,
                        static view => view.ChannelsView.ItemsSource)
                    .DisposeWith(disposable);
                _ = this.Bind(ViewModel,
                        static viewModel => viewModel.SelectedChannel,
                        static view => view.ChannelsView.SelectedItem)
                    .DisposeWith(disposable);

                // Status.
                _ = this.OneWayBind(ViewModel,
                        static viewModel => viewModel.CurrentFile,
                        static view => view.CurrentFileLabel.Content)
                    .DisposeWith(disposable);
                _ = this.Bind(ViewModel,
                        static viewModel => viewModel.AudysseyApp.AmpAssignInfo,
                        static view => view.AmpAssignInfoTextBox.Text)
                    .DisposeWith(disposable);
                _ = this.Bind(ViewModel,
                        static viewModel => viewModel.AudysseyApp.Title,
                        static view => view.TitleTextBox.Text)
                    .DisposeWith(disposable);
                _ = this.Bind(ViewModel,
                        static viewModel => viewModel.AudysseyApp.TargetModelName,
                        static view => view.TargetModelNameTextBox.Text)
                    .DisposeWith(disposable);
                _ = this.Bind(ViewModel,
                        static viewModel => viewModel.AudysseyApp.InterfaceVersion,
                        static view => view.InterfaceVersionTextBox.Text)
                    .DisposeWith(disposable);
                _ = this.Bind(ViewModel,
                        static viewModel => viewModel.AudysseyApp.UpgradeInfo,
                        static view => view.UpgradeInfoTextBox.Text)
                    .DisposeWith(disposable);
                _ = this.Bind(ViewModel,
                        static viewModel => viewModel.AudysseyApp.AdcLineup,
                        static view => view.AdcLineupTextBox.Text)
                    .DisposeWith(disposable);
                _ = this.Bind(ViewModel,
                        static viewModel => viewModel.AudysseyApp.SystemDelay,
                        static view => view.SystemDelayTextBox.Text)
                    .DisposeWith(disposable);
                _ = this.OneWayBind(ViewModel,
                        static viewModel => viewModel.AmpAssignTypeList,
                        static view => view.AmpAssignTypeListComboBox.ItemsSource)
                    .DisposeWith(disposable);
                _ = this.Bind(ViewModel,
                        static viewModel => viewModel.AudysseyApp.EnAmpAssignType,
                        static view => view.AmpAssignTypeListComboBox.SelectedIndex)
                    .DisposeWith(disposable);
                _ = this.OneWayBind(ViewModel,
                        static viewModel => viewModel.MultEQTypeList,
                        static view => view.MultEqTypeListComboBox.ItemsSource)
                    .DisposeWith(disposable);
                _ = this.Bind(ViewModel,
                        static viewModel => viewModel.AudysseyApp.EnMultEQType,
                        static view => view.MultEqTypeListComboBox.SelectedIndex)
                    .DisposeWith(disposable);
                _ = this.OneWayBind(ViewModel,
                        static viewModel => viewModel.TargetCurveTypeList,
                        static view => view.TargetCurveTypeComboBox.ItemsSource)
                    .DisposeWith(disposable);
                _ = this.Bind(ViewModel,
                        static viewModel => viewModel.AudysseyApp.EnTargetCurveType,
                        static view => view.TargetCurveTypeComboBox.SelectedIndex)
                    .DisposeWith(disposable);
                _ = this.Bind(ViewModel,
                        static viewModel => viewModel.AudysseyApp.DynamicEq,
                        static view => view.DynamicEqCheckBox.IsChecked)
                    .DisposeWith(disposable);
                _ = this.Bind(ViewModel,
                        static viewModel => viewModel.AudysseyApp.DynamicVolume,
                        static view => view.DynamicVolumeCheckBox.IsChecked)
                    .DisposeWith(disposable);
                _ = this.Bind(ViewModel,
                        static viewModel => viewModel.AudysseyApp.Auro,
                        static view => view.AuroCheckBox.IsChecked)
                    .DisposeWith(disposable);
                _ = this.Bind(ViewModel,
                        static viewModel => viewModel.AudysseyApp.LfcSupport,
                        static view => view.LfcSupportCheckBox.IsChecked)
                    .DisposeWith(disposable);
                _ = this.Bind(ViewModel,
                        static viewModel => viewModel.AudysseyApp.Lfc,
                        static view => view.LfcCheckBox.IsChecked)
                    .DisposeWith(disposable);

                // Target Curve Points.
                _ = this.OneWayBind(ViewModel,
                        static viewModel => viewModel.IsChannelSelected,
                        static view => view.TargetCurvePointsGroupBox.IsEnabled)
                    .DisposeWith(disposable);
                _ = this.OneWayBind(ViewModel,
                        static viewModel => viewModel.CustomTargetCurvePoints,
                        static view => view.TargetCurvePointsDataGrid.ItemsSource)
                    .DisposeWith(disposable);
                _ = Observable.FromEventPattern<InitializingNewItemEventArgs>(
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
                _ = Observable.FromEventPattern<DataGridRowEditEndingEventArgs>(
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

                _ = this.OneWayBind(ViewModel,
                        static viewModel => viewModel.ChannelInformationViewModel,
                        static view => view.ChannelInformationView.ViewModel)
                    .DisposeWith(disposable);
                _ = this.OneWayBind(ViewModel,
                        static viewModel => viewModel.ChannelReportViewModel,
                        static view => view.ChannelReportView.ViewModel)
                    .DisposeWith(disposable);

                // Graph View.
                _ = this.OneWayBind(ViewModel,
                        static viewModel => viewModel.MeasurementPositions,
                        static view => view.MeasurementPositionsListView.ItemsSource)
                    .DisposeWith(disposable);
                _ = this.Bind(ViewModel,
                        static viewModel => viewModel.SelectAllMeasurementPositionsIsChecked,
                        static view => view.SelectAllMeasurementPositionsCheckBox.IsChecked)
                    .DisposeWith(disposable);
                _ = this.OneWayBind(ViewModel,
                        static viewModel => viewModel.PlotModel,
                        static view => view.PlotView.Model)
                    .DisposeWith(disposable);
                _ = this.OneWayBind(ViewModel,
                        static viewModel => viewModel.SmoothingFactors,
                        static view => view.SmoothingFactorsListView.ItemsSource)
                    .DisposeWith(disposable);
                _ = this.OneWayBind(ViewModel,
                        static viewModel => viewModel.Ranges,
                        static view => view.RangesListView.ItemsSource)
                    .DisposeWith(disposable);
                _ = this.Bind(ViewModel,
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
