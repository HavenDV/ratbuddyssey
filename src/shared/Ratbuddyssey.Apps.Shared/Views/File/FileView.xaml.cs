namespace Ratbuddyssey.Views;

public partial class FileView
{
    // Workaround for issue: 
    partial void AfterWhenActivated(CompositeDisposable disposables)
    {
        _ = this.OneWayBind(ViewModel,
                static viewModel => viewModel.StatusViewModel,
                static view => view.StatusView.ViewModel)
            .DisposeWith(disposables);
        _ = this.OneWayBind(ViewModel,
                static viewModel => viewModel.ChannelsViewModel,
                static view => view.ChannelsView.ViewModel)
            .DisposeWith(disposables);
        _ = this.OneWayBind(ViewModel,
                static viewModel => viewModel.ChannelInformationViewModel,
                static view => view.ChannelInformationView.ViewModel)
            .DisposeWith(disposables);
        _ = this.OneWayBind(ViewModel,
                static viewModel => viewModel.ChannelReportViewModel,
                static view => view.ChannelReportView.ViewModel)
            .DisposeWith(disposables);
        _ = this.OneWayBind(ViewModel,
                static viewModel => viewModel.TargetCurvePointsViewModel,
                static view => view.TargetCurvePointsView.ViewModel)
            .DisposeWith(disposables);
#if HAS_WPF
        _ = this.OneWayBind(ViewModel,
                static viewModel => viewModel.GraphViewModel,
                static view => view.GraphView.ViewModel)
            .DisposeWith(disposables);
#endif
        _ = this.OneWayBind(ViewModel,
                static viewModel => viewModel.PreviewDropViewModel,
                static view => view.PreviewDropView.ViewModel)
            .DisposeWith(disposables);
    }
}
