using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

// ReSharper disable UnassignedGetOnlyAutoProperty

namespace Ratbuddyssey.ViewModels
{
    public class TargetCurvePointsViewModel : ActivatableViewModel
    {
        #region Properties

        #region Public

        [ObservableAsProperty]
        public ChannelViewModel Channel { get; } = new();

        [ObservableAsProperty]
        public bool IsChannelSelected { get; }

        [Reactive]
        public ObservableCollection<TargetCurvePointViewModel> CustomTargetCurvePoints { get; set; } = new();

        #endregion

        #region Commands

        public ReactiveCommand<TargetCurvePointViewModel, Unit> DeleteTargetCurvePoint { get; }

        #endregion

        #endregion

        #region Constructors

        public TargetCurvePointsViewModel()
        {
            DeleteTargetCurvePoint = ReactiveCommand.Create<TargetCurvePointViewModel>(viewModel =>
            {
                _ = CustomTargetCurvePoints.Remove(viewModel);
            });

            this.WhenActivated(disposables =>
            {
                _ = this
                    .WhenAnyValue(static x => x.Channel)
                    .Subscribe(channel =>
                    {
                        CustomTargetCurvePoints = new ObservableCollection<TargetCurvePointViewModel>(channel.Data.CustomTargetCurvePoints
                            .Select(static value => value.Trim('{', '}').Split(','))
                            .Select(static values => new TargetCurvePointViewModel(
                                double.TryParse(values[0].Trim(), out var result1) ? result1 : default,
                                double.TryParse(values[1].Trim(), out var result2) ? result2 : default))
                            .Select(point =>
                            {
                                _ = point.Delete
                                    .InvokeCommand(DeleteTargetCurvePoint);

                                return point;
                            })
                            .OrderBy(static x => x.Key));

                        _ = CustomTargetCurvePoints
                            .ToObservableChangeSet()
                            .Subscribe(_ =>
                            {
                                channel.Data.CustomTargetCurvePoints = CustomTargetCurvePoints
                                    .Select(static value => $"{{{value.Key}, {value.Value}}}")
                                    .ToArray();
                            });
                    })
                    .DisposeWith(disposables);
            });
        }

        #endregion
    }
}
