using H.ReactiveUI.CommonInteractions.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ratbuddyssey.Initialization;
#if !HAS_WPF
using Windows.ApplicationModel.Activation;
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

#if !HAS_WPF
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

#if HAS_WPF

    private async void Application_Startup(object _, StartupEventArgs e)
    {
        var mainView = (Window)GetView<MainViewModel>(out var _);

        if (e.Args.Any())
        {
            var path = e.Args.ElementAtOrDefault(0);
            if (path != null && !string.IsNullOrWhiteSpace(path))
            {
                var fileViewModel = Host.Services.GetRequiredService<FileViewModel>();

                await fileViewModel.OpenAsync(new SystemIOApiFileData(path)).ConfigureAwait(true);
            }
        }

        mainView.ShowDialog();
    }

#else

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
#if HAS_WINUI
        var window = new Window();
        MessageInteractionManager.Window = window;
#else
        var window = Window.Current;
#endif
        if (window.Content is not Frame frame)
        {
            frame = new Frame();

            window.Content = frame;
        }

#if !HAS_WINUI
        if (args.PrelaunchActivated)
        {
            return;
        }
#endif

        if (frame.Content is null)
        {
            frame.Content = GetView<MainViewModel>(out var _);
        }

        window.Activate();
    }

#endif

    #endregion
}
