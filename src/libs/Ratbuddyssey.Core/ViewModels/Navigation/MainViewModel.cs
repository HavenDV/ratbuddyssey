using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Ratbuddyssey.ViewModels
{
    public class MainViewModel : ActivatableViewModel, IScreen
    {
        #region Properties

        private IServiceProvider Services { get; }

        public RoutingState Router { get; } = new();

        public IReadOnlyCollection<TabViewModel> Tabs { get; } = new TabViewModel[]
        {
            new(typeof(FileViewModel), "App"),
            new(typeof(EthernetViewModel), "Ethernet"),
        };

        [Reactive]
        public TabViewModel? SelectedTab { get; set; }

        #endregion

        #region Constructors

        public MainViewModel(IServiceProvider services)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));

            this.WhenActivated(disposables =>
            {
                _ = this
                    .WhenAnyValue(static viewModel => viewModel.SelectedTab)
                    .WhereNotNull()
                    .Select(static item => item.Type)
                    .Select(type => (IRoutableViewModel)Services.GetRequiredService(type))
                    .InvokeCommand(Router.Navigate)
                    .DisposeWith(disposables);

                SelectedTab = Tabs.First();
            });
        }

        #endregion
    }
}
