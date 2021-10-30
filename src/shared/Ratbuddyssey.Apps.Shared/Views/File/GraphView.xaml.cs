#if HAS_WPF
using System.Reactive.Disposables;
using ReactiveUI;

#nullable enable

namespace Ratbuddyssey.Views
{
    public partial class GraphView
    {
        #region Constructors

        public GraphView()
        {
            InitializeComponent();

            PlotView.PreviewMouseWheel += (_, args) => args.Handled = true;

            _ = this.WhenActivated(disposable =>
            {
                if (ViewModel == null)
                {
                    return;
                }

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
    }
}
#endif