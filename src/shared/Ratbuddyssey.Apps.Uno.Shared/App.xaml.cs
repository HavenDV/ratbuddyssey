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
    private InteractionManager InteractionManager { get; } = new();

    #endregion

    #region Constructors

    public App()
    {
        InteractionManager.Register();
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
