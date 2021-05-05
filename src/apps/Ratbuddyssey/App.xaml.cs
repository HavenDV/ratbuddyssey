using Audyssey;
using System.Reactive;
using System.Windows;
using Microsoft.Extensions.Hosting;
using System;
using Audyssey.Initialization;
using HostBuilder = Audyssey.Initialization.HostBuilder;

namespace Ratbuddyssey
{
    public partial class App
    {
        #region Properties

        public IHost Host { get; }

        #endregion

        #region Constructors

        public App()
        {
            Interactions.Warning.RegisterHandler(static context =>
            {
                var message = context.Input;

                MessageBox.Show(message, "Warning:", MessageBoxButton.OK, MessageBoxImage.Warning);

                context.SetOutput(Unit.Default);
            });
            Interactions.Exception.RegisterHandler(static context =>
            {
                var exception = context.Input;

                MessageBox.Show($"{exception}", "Error:", MessageBoxButton.OK, MessageBoxImage.Error);

                context.SetOutput(Unit.Default);
            });

            DispatcherUnhandledException += static (_, e) =>
            {
                e.Handled = true;

                Interactions.Exception.Handle(e.Exception).Subscribe();
            };

            Host = HostBuilder
                .Create()
                .AddViews()
                .AddPlatformSpecificLoggers()
                .Build();
        }

        #endregion

        #region Event Handlers

        //private IViewFor GetView<T>(out T viewModel) where T : notnull
        //{
        //    viewModel = Host.Services.GetRequiredService<T>();
        //    var view = Host.Services
        //                   .GetRequiredService<IViewLocator>()
        //                   .ResolveView(viewModel) ??
        //               throw new InvalidOperationException("view is null.");

        //    view.ViewModel = viewModel;
        //    return view;
        //}

        private void Application_Startup(object _, StartupEventArgs e)
        {
            //var view = (Window)GetView<MainViewModel>(out var _);

            //if (e.Args.Any())
            //{
            //    var fileName = e.Args.ElementAtOrDefault(0);

            //    if (!string.IsNullOrWhiteSpace(fileName))
            //    {
            //        //var loginViewModel = Host.Services
            //        //    .GetRequiredService<LoginViewModel>();

            //        //loginViewModel.PerformLogin(username, password);
            //    }
            //}

            //view.ShowDialog();
        }

        #endregion
    }
}
