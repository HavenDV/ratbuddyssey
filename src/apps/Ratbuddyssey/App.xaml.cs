using System.Reactive;
using System.Windows;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using Ratbuddyssey.Initialization;
using Ratbuddyssey.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using HostBuilder = Ratbuddyssey.Initialization.HostBuilder;

#nullable enable

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

                MessageBox.Show(
                    message, 
                    "Warning:", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Warning);

                context.SetOutput(Unit.Default);
            });
            Interactions.Exception.RegisterHandler(static context =>
            {
                var exception = context.Input;

                MessageBox.Show(
                    $"{exception}", 
                    "Error:", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Error);

                context.SetOutput(Unit.Default);
            });
            Interactions.OpenFile.RegisterHandler(static context =>
            {
                var extension = context.Input;

                var dialog = new Microsoft.Win32.OpenFileDialog
                {
                    FileName = extension switch
                    {
                        ".aud" => "AudysseySniffer.aud",
                        ".ady" => string.Empty,
                        _ => string.Empty,
                    },
                    DefaultExt = extension,
                    Filter = extension switch
                    {
                        ".aud" => "Audyssey sniffer (*.aud)|*.aud",
                        ".ady" => "Audyssey files (*.ady)|*.ady",
                        _ => string.Empty,
                    },
                };
                var fileName = dialog.ShowDialog() == true
                    ? dialog.FileName
                    : null;

                context.SetOutput(fileName);
            });
            Interactions.SaveFile.RegisterHandler(static context =>
            {
                var extension = context.Input;

                var dialog = new Microsoft.Win32.SaveFileDialog
                {
                    FileName = extension switch
                    {
                        ".aud" => "AudysseySniffer.aud",
                        _ => string.Empty,
                    },
                    DefaultExt = extension,
                    Filter = extension switch
                    {
                        ".aud" => "Audyssey sniffer (.aud)|*.aud",
                        ".ady" => "Audyssey calibration (.ady)|*.ady",
                        _ => string.Empty,
                    },
                };
                var fileName = dialog.ShowDialog() == true
                    ? dialog.FileName
                    : null;

                context.SetOutput(fileName);
            });
            Interactions.Question.RegisterHandler(static context =>
            {
                var message = context.Input;
                
                var result = MessageBox.Show(
                    message, 
                    "Are you sure?", 
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question,
                    MessageBoxResult.No);
                
                context.SetOutput(result == MessageBoxResult.Yes);
            });

            DispatcherUnhandledException += static (_, e) =>
            {
                e.Handled = true;

                Interactions.Exception.Handle(e.Exception).Subscribe();
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

                    fileViewModel.Open(path);
                }
            }

            mainView.ShowDialog();
        }

        #endregion
    }
}
