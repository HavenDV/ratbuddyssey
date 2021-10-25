using System.IO;
using System.Reactive;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Win32;
using Ratbuddyssey.Extensions;
using Ratbuddyssey.Initialization;
using Ratbuddyssey.ViewModels;
using ReactiveUI;
using HostBuilder = Ratbuddyssey.Initialization.HostBuilder;

namespace Ratbuddyssey
{
    public partial class App
    {
        #region Properties

        public IHost Host { get; }
        private InteractionManager InteractionManager { get; } = new();

        #endregion

        #region Constructors

        public App()
        {
            InteractionManager.Register();
            DispatcherUnhandledException += static (_, e) =>
            {
                e.Handled = true;

                MessageInteractions.Exception.Handle(e.Exception).Subscribe();
            };

            Host = HostBuilder
                .Create()
                .AddViews()
                .AddConverters()
                .AddPlatformSpecificLoggers()
                .Build();
        }

        #endregion

        #region Event Handlers

        private IViewFor GetView<T>(out T viewModel) where T : notnull
        {
            viewModel = Host.Services.GetRequiredService<T>();
            var view = Host.Services
                           .GetRequiredService<IViewLocator>()
                           .ResolveView(viewModel) ??
                       throw new InvalidOperationException("View is null.");

            view.ViewModel = viewModel;
            return view;
        }

        private void Application_Startup(object _, StartupEventArgs e)
        {
            var mainView = (Window)GetView<MainViewModel>(out var _);

            if (e.Args.Any())
            {
                var path = e.Args.ElementAtOrDefault(0);
                if (path != null && !string.IsNullOrWhiteSpace(path))
                {
                    var fileViewModel = Host.Services.GetRequiredService<FileViewModel>();

                    fileViewModel.Open(path.ToFile());
                }
            }

            mainView.ShowDialog();
        }

        #endregion
    }
}
