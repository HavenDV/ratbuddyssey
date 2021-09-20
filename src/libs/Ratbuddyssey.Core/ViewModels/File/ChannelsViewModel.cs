using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

// ReSharper disable UnassignedGetOnlyAutoProperty

namespace Ratbuddyssey.ViewModels
{
    public class ChannelsViewModel : ActivatableViewModel
    {
        #region Properties

        #region Public

        [Reactive]
        public IReadOnlyCollection<ChannelViewModel> Channels { get; set; } = Array.Empty<ChannelViewModel>();

        [Reactive]
        public ChannelViewModel? SelectedChannel { get; set; }

        [ObservableAsProperty]
        public ChannelViewModel Channel { get; } = new();

        [ObservableAsProperty]
        public bool IsChannelSelected { get; }

        #endregion

        #endregion

        #region Constructors

        public ChannelsViewModel()
        {
            this.WhenActivated(disposables =>
            {
                _ = this
                    .WhenAnyValue(static x => x.SelectedChannel)
                    .WhereNotNull()
                    .ToPropertyEx(
                        this,
                        static x => x.Channel,
                        new ChannelViewModel())
                    .DisposeWith(disposables);
                _ = this
                    .WhenAnyValue(static x => x.SelectedChannel)
                    .Select(static value => value != null)
                    .ToPropertyEx(
                        this,
                        static x => x.IsChannelSelected,
                        false,
                        false)
                    .DisposeWith(disposables);
            });
        }

        #endregion

    }
}
