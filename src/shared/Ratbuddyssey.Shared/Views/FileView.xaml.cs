using System;
using System.Reactive.Disposables;
using System.Windows;
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
                        static viewModel => viewModel.AudysseyApp.AmpAssignTypeList,
                        static view => view.AmpAssignTypeListComboBox.ItemsSource)
                    .DisposeWith(disposable);
                this.Bind(ViewModel,
                        static viewModel => viewModel.AudysseyApp.EnAmpAssignType,
                        static view => view.AmpAssignTypeListComboBox.SelectedIndex)
                    .DisposeWith(disposable);
                this.OneWayBind(ViewModel,
                        static viewModel => viewModel.AudysseyApp.MultEQTypeList,
                        static view => view.MultEqTypeListComboBox.ItemsSource)
                    .DisposeWith(disposable);
                this.Bind(ViewModel,
                        static viewModel => viewModel.AudysseyApp.EnMultEQType,
                        static view => view.MultEqTypeListComboBox.SelectedIndex)
                    .DisposeWith(disposable);
                this.OneWayBind(ViewModel,
                        static viewModel => viewModel.AudysseyApp.TargetCurveTypeList,
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

                this.OneWayBind(ViewModel,
                        static viewModel => viewModel.PlotModel,
                        static view => view.PlotView.Model)
                    .DisposeWith(disposable);
                this.OneWayBind(ViewModel,
                        static viewModel => viewModel.Channels,
                        static view => view.ChannelsView.ItemsSource)
                    .DisposeWith(disposable);
                this.Bind(ViewModel,
                        static viewModel => viewModel.SelectedChannel,
                        static view => view.ChannelsView.SelectedItem)
                    .DisposeWith(disposable);

                this.Bind(ViewModel,
                        static viewModel => viewModel.AddTargetCurvePointKey,
                        static view => view.AddTargetCurvePointKeyTextBox.Text)
                    .DisposeWith(disposable);
                this.Bind(ViewModel,
                        static viewModel => viewModel.AddTargetCurvePointValue,
                        static view => view.AddTargetCurvePointValueTextBox.Text)
                    .DisposeWith(disposable);

                this.OneWayBind(ViewModel,
                        static viewModel => viewModel.MeasurementPositions,
                        static view => view.MeasurementPositionsListView.ItemsSource)
                    .DisposeWith(disposable);
                this.Bind(ViewModel,
                        static viewModel => viewModel.SelectAllMeasurementPositionsIsChecked,
                        static view => view.SelectAllMeasurementPositionsCheckBox.IsChecked)
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
                this.BindCommand(ViewModel,
                        static viewModel => viewModel.AddTargetCurvePoint,
                        static view => view.AddTargetCurvePointButton)
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
