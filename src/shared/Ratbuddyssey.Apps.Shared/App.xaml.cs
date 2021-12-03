using H.ReactiveUI.CommonInteractions.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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
        InitializeLogging();

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
        window.Activate();
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

    /// <summary>
    /// Configures global Uno Platform logging
    /// </summary>
    private static void InitializeLogging()
    {
        var factory = LoggerFactory.Create(builder =>
        {
#if __WASM__
            builder.AddProvider(new global::Uno.Extensions.Logging.WebAssembly.WebAssemblyConsoleLoggerProvider());
#elif __IOS__
                builder.AddProvider(new global::Uno.Extensions.Logging.OSLogLoggerProvider());
#elif NETFX_CORE
                builder.AddDebug();
#else
                builder.AddConsole();
#endif

            // Exclude logs below this level
            builder.SetMinimumLevel(LogLevel.Information);

            // Default filters for Uno Platform namespaces
            builder.AddFilter("Uno", LogLevel.Warning);
            builder.AddFilter("Windows", LogLevel.Warning);
            builder.AddFilter("Microsoft", LogLevel.Warning);

            // Generic Xaml events
            // builder.AddFilter("Windows.UI.Xaml", LogLevel.Debug );
            // builder.AddFilter("Windows.UI.Xaml.VisualStateGroup", LogLevel.Debug );
            // builder.AddFilter("Windows.UI.Xaml.StateTriggerBase", LogLevel.Debug );
            // builder.AddFilter("Windows.UI.Xaml.UIElement", LogLevel.Debug );
            // builder.AddFilter("Windows.UI.Xaml.FrameworkElement", LogLevel.Trace );

            // Layouter specific messages
            // builder.AddFilter("Windows.UI.Xaml.Controls", LogLevel.Debug );
            // builder.AddFilter("Windows.UI.Xaml.Controls.Layouter", LogLevel.Debug );
            // builder.AddFilter("Windows.UI.Xaml.Controls.Panel", LogLevel.Debug );

            // builder.AddFilter("Windows.Storage", LogLevel.Debug );

            // Binding related messages
            // builder.AddFilter("Windows.UI.Xaml.Data", LogLevel.Debug );
            // builder.AddFilter("Windows.UI.Xaml.Data", LogLevel.Debug );

            // Binder memory references tracking
            // builder.AddFilter("Uno.UI.DataBinding.BinderReferenceHolder", LogLevel.Debug );

            // RemoteControl and HotReload related
            // builder.AddFilter("Uno.UI.RemoteControl", LogLevel.Information);

            // Debug JS interop
            // builder.AddFilter("Uno.Foundation.WebAssemblyRuntime", LogLevel.Debug );
        });

        global::Uno.Extensions.LogExtensionPoint.AmbientLoggerFactory = factory;

#if HAS_UNO
        global::Uno.UI.Adapter.Microsoft.Extensions.Logging.LoggingAdapter.Initialize();
#endif
    }

    #endregion
}
