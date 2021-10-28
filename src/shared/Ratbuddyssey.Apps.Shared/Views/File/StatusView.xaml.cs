#nullable enable

namespace Ratbuddyssey.Views;

public partial class StatusView
{
    partial void AfterWhenActivated(CompositeDisposable disposables)
    {
        _ = this.OneWayBind(ViewModel,
               static viewModel => viewModel.CurrentFile,
               static view => view.CurrentFileLabel.Text)
           .DisposeWith(disposables);
        _ = this.Bind(ViewModel,
                static viewModel => viewModel.AudysseyApp.AmpAssignInfo,
                static view => view.AmpAssignInfoTextBox.Text)
            .DisposeWith(disposables);
        _ = this.Bind(ViewModel,
                static viewModel => viewModel.AudysseyApp.Title,
                static view => view.TitleTextBox.Text)
            .DisposeWith(disposables);
        _ = this.Bind(ViewModel,
                static viewModel => viewModel.AudysseyApp.TargetModelName,
                static view => view.TargetModelNameTextBox.Text)
            .DisposeWith(disposables);
        _ = this.Bind(ViewModel,
                static viewModel => viewModel.AudysseyApp.InterfaceVersion,
                static view => view.InterfaceVersionTextBox.Text)
            .DisposeWith(disposables);
        _ = this.Bind(ViewModel,
                static viewModel => viewModel.AudysseyApp.UpgradeInfo,
                static view => view.UpgradeInfoTextBox.Text)
            .DisposeWith(disposables);
#if WPF_APP
        _ = this.Bind(ViewModel,
            static viewModel => viewModel.AudysseyApp.AdcLineup,
            static view => view.AdcLineupTextBox.Text)
        .DisposeWith(disposables);
        _ = this.Bind(ViewModel,
                static viewModel => viewModel.AudysseyApp.SystemDelay,
                static view => view.SystemDelayTextBox.Text)
            .DisposeWith(disposables);
#endif
        _ = this.OneWayBind(ViewModel,
            static viewModel => viewModel.AmpAssignTypeList,
            static view => view.AmpAssignTypeListComboBox.ItemsSource)
        .DisposeWith(disposables);
        _ = this.Bind(ViewModel,
                static viewModel => viewModel.AudysseyApp.EnAmpAssignType,
                static view => view.AmpAssignTypeListComboBox.SelectedIndex)
            .DisposeWith(disposables);
        _ = this.OneWayBind(ViewModel,
                static viewModel => viewModel.MultEQTypeList,
                static view => view.MultEqTypeListComboBox.ItemsSource)
            .DisposeWith(disposables);
        _ = this.Bind(ViewModel,
                static viewModel => viewModel.AudysseyApp.EnMultEQType,
                static view => view.MultEqTypeListComboBox.SelectedIndex)
            .DisposeWith(disposables);
        _ = this.OneWayBind(ViewModel,
                static viewModel => viewModel.TargetCurveTypeList,
                static view => view.TargetCurveTypeComboBox.ItemsSource)
            .DisposeWith(disposables);
        _ = this.Bind(ViewModel,
                static viewModel => viewModel.AudysseyApp.EnTargetCurveType,
                static view => view.TargetCurveTypeComboBox.SelectedIndex)
            .DisposeWith(disposables);
        _ = this.Bind(ViewModel,
                static viewModel => viewModel.AudysseyApp.DynamicEq,
                static view => view.DynamicEqCheckBox.IsChecked)
            .DisposeWith(disposables);
        _ = this.Bind(ViewModel,
                static viewModel => viewModel.AudysseyApp.DynamicVolume,
                static view => view.DynamicVolumeCheckBox.IsChecked)
            .DisposeWith(disposables);
        _ = this.Bind(ViewModel,
                static viewModel => viewModel.AudysseyApp.Auro,
                static view => view.AuroCheckBox.IsChecked)
            .DisposeWith(disposables);
        _ = this.Bind(ViewModel,
                static viewModel => viewModel.AudysseyApp.LfcSupport,
                static view => view.LfcSupportCheckBox.IsChecked)
            .DisposeWith(disposables);
        _ = this.Bind(ViewModel,
                static viewModel => viewModel.AudysseyApp.Lfc,
                static view => view.LfcCheckBox.IsChecked)
            .DisposeWith(disposables);
    }
}