using ReactiveUI;
using H.ReactiveUI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ratbuddyssey.Initialization;
using Ratbuddyssey.ViewModels;
#if WPF_APP
using System.Windows;
#else
using System;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml.Controls;
#endif

#nullable enable

namespace Ratbuddyssey;

public sealed partial class App
{
    #region Properties

    public IHost Host { get; }
    private InteractionManager InteractionManager { get; } = new();

    #endregion

    #region Constructors

    public App()
    {
        InteractionManager.Register();
        InteractionManager.CatchUnhandledExceptions(this);

        Host = Initialization.HostBuilder
            .Create()
            .AddViews()
            .AddConverters()
            .AddPlatformSpecificLoggers()
#if __WASM__
            .RemoveFileWatchers()
#endif
            .Build();

#if !WPF_APP
        InitializeComponent();
#endif
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

#if WPF_APP

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

#else

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
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

        if (frame.Content is null)
        {
            frame.Content = GetView<MainViewModel>(out var _);
        }

        window.Activate();
    }

#endif

    #endregion
}
