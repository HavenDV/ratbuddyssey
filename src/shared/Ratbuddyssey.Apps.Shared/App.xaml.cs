using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ratbuddyssey.Initialization;
using Windows.ApplicationModel.Activation;

#nullable enable

namespace Ratbuddyssey;

public sealed partial class App
{
    #region Properties

    public IHost Host { get; }

    #endregion

    #region Constructors

    public App()
    {
        Host = Initialization.HostBuilder
            .Create()
            .AddViews()
            .AddPlatformSpecificLoggers()
#if __WASM__
            .RemoveFileWatchers()
#endif
            .Build();

        InitializeComponent();
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

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
#if HAS_WINUI
        var window = new Window();
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

#endregion
    }
