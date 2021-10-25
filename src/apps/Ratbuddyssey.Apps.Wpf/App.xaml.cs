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
                var (fileName, extensions, filterName) = context.Input;

                var wildcards = extensions
                    .Select(static extension => $"*{extension}")
                    .ToArray();
                var filter = $@"{filterName} ({string.Join(", ", wildcards)})|{string.Join(";", wildcards)}";

                var dialog = new OpenFileDialog
                {
                    FileName = fileName,
                    CheckFileExists = true,
                    CheckPathExists = true,
                    Filter = filter,
                };
                if (dialog.ShowDialog() != true)
                {
                    context.SetOutput(null);
                    return;
                }

                var path = dialog.FileName;
                var model = path.ToFile();

                context.SetOutput(model);
            });
            Interactions.SaveFile.RegisterHandler(static async context =>
            {
                var (fileName, extension, filterName, bytesFunc) = context.Input;

                var wildcards = new [] { $"*{extension}" };
                var filter = $@"{filterName} ({string.Join(", ", wildcards)})|{string.Join(";", wildcards)}";

                var dialog = new SaveFileDialog
                {
                    FileName = fileName,
                    DefaultExt = extension,
                    AddExtension = true,
                    Filter = filter,
                };
                if (dialog.ShowDialog() != true)
                {
                    context.SetOutput(null);
                    return;
                }

                var bytes = await bytesFunc().ConfigureAwait(false);
                var path = dialog.FileName;

                File.WriteAllBytes(path, bytes);

                context.SetOutput(path);
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

                    fileViewModel.Open(path.ToFile());
                }
            }

            mainView.ShowDialog();
        }

        #endregion
    }
}
