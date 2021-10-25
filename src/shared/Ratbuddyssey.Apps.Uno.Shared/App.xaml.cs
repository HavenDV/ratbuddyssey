using System;
using System.Diagnostics;
using Ratbuddyssey.Initialization;
using Ratbuddyssey.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ReactiveUI;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Popups;
using System.Reactive;
using Windows.Storage.Pickers;
using System.Collections.Generic;
using System.IO;
using Ratbuddyssey.Extensions;

#nullable enable

namespace Ratbuddyssey;

public sealed partial class App
{
    #region Properties

    private IHost Host { get; }
    //private InteractionManager InteractionManager { get; } = new();

    #endregion

    #region Constructors

    public App()
    {
        _ = Interactions.Warning.RegisterHandler(static async context =>
        {
            var warning = context.Input;

            Trace.WriteLine($"Warning: {warning}");
            var dialog = new MessageDialog(warning, "Warning:");

            context.SetOutput(Unit.Default);

            await dialog.ShowAsync();
        });
        _ = Interactions.Exception.RegisterHandler(static async context =>
        {
            var exception = context.Input;

            Trace.WriteLine($"Exception: {exception}");
            var dialog = new MessageDialog($"{exception}", "Exception:");

            context.SetOutput(Unit.Default);

            await dialog.ShowAsync();
        });
        _ = Interactions.Question.RegisterHandler(static async context =>
        {
            var question = context.Input;

            Trace.WriteLine($@"Question: {question}");

            var dialog = new ContentDialog
            {
                Title = "Question",
                Content = question,
                PrimaryButtonText = "Yes",
                CloseButtonText = "No",
            };
            var result = await dialog.ShowAsync();

            context.SetOutput(result == ContentDialogResult.Primary);
        });
        _ = Interactions.OpenFile.RegisterHandler(static async context =>
        {
            var (fileName, extensions, filterName) = context.Input;

            var picker = new FileOpenPicker();
            foreach (var extension in extensions)
            {
                picker.FileTypeFilter.Add(extension);
            }

            var file = await picker.PickSingleFileAsync();
            if (file == null)
            {
                context.SetOutput(new(string.Empty, string.Empty, Array.Empty<byte>()));
                return;
            }

            var model = await file.ToFileAsync();

            context.SetOutput(model);
        });
        _ = Interactions.SaveFile.RegisterHandler(async context =>
        {
            var (fileName, extension, filterName, bytesTask) = context.Input;

            var picker = new FileSavePicker
            {
                SuggestedStartLocation = PickerLocationId.Downloads,
                SuggestedFileName = fileName,
                FileTypeChoices =
                {
                    { extension, new List<string> { extension } },
                },
            };
            var file = await picker.PickSaveFileAsync();
            if (file == null)
            {
                context.SetOutput(null);
                return;
            }

            var bytes = await bytesTask().ConfigureAwait(false);

            using (var stream = await file.OpenStreamForWriteAsync().ConfigureAwait(false))
            using (var writer = new BinaryWriter(stream))
            {
                writer.Write(bytes);
            }

            context.SetOutput(file.Path);
        });

        //InteractionManager.Register();
        UnhandledException += static (sender, args) =>
        {
            args.Handled = true;

            _ = Interactions.Exception
                .Handle(args.Exception)
                .Subscribe();
        };

        Host = Initialization.HostBuilder
            .Create()
            .AddViews()
            .AddConverters()
            .AddPlatformSpecificLoggers()
#if __WASM__
            .RemoveFileWatchers()
#endif
            .Build();

        InitializeComponent();
    }

    #endregion

    #region Event Handlers

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        // Uno.Material.Resources.Init(this, null);

        // Uno.iOS requires full namespace.
        var window = Windows.UI.Xaml.Window.Current;
        if (window.Content is not Frame frame)
        {
            frame = new Frame();

            window.Content = frame;
        }

        if (args.PrelaunchActivated)
        {
            return;
        }

        var mainViewModel = Host.Services
                .GetRequiredService<MainViewModel>();
        if (frame.Content is null)
        {
            var viewModel = mainViewModel;
            var view = Host.Services
                .GetRequiredService<IViewLocator>()
                .ResolveView(viewModel) ??
                throw new InvalidOperationException("view is null.");

            view.ViewModel = viewModel;
            frame.Content = view;
        }

        window.Activate();
    }

    #endregion
}
