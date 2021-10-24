#if WPF_APP
using System.Reactive.Disposables;
using ReactiveUI;

#nullable enable

namespace Ratbuddyssey.Views
{
    public partial class StatusView
    {
        #region Constructors

        public StatusView()
        {
            InitializeComponent();

            _ = this.WhenActivated(disposable =>
            {
                if (ViewModel == null)
                {
                    return;
                }

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
            });
        }

        #endregion
    }
}
#endif