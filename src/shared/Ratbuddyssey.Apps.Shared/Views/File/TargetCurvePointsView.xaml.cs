#if HAS_WPF
using System.Reactive.Linq;
using Ratbuddyssey.ViewModels;
#endif

#nullable enable

namespace Ratbuddyssey.Views;

public partial class TargetCurvePointsView
{
#if HAS_WPF
    partial void AfterWhenActivated(CompositeDisposable disposables)
    {
        if (ViewModel == null)
        {
            return;
        }


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
                   .DisposeWith(disposables);
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
            .DisposeWith(disposables);
    }
#endif
}
