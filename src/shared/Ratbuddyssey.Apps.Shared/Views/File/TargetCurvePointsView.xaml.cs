#if HAS_WPF
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Controls;
using Ratbuddyssey.ViewModels;
using ReactiveUI;

#nullable enable

namespace Ratbuddyssey.Views
{
    public partial class TargetCurvePointsView
    {
        #region Constructors

        public TargetCurvePointsView()
        {
            InitializeComponent();

            _ = this.WhenActivated(disposable =>
            {
                if (ViewModel == null)
                {
                    return;
                }

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

                        _ = point.Delete
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
            });
        }

        #endregion
    }
}
#endif