using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using H.ReactiveUI;
using H.XamlExtensions;
using Microsoft.Extensions.Hosting;
using Ratbuddyssey.Apps.Views;
using Ratbuddyssey.Initialization;
using Ratbuddyssey.ViewModels;
using Splat;

namespace Ratbuddyssey.Apps;

public class App : Application
{
    #region Properties

    private InteractionManager InteractionManager { get; } = new();

    #endregion

    public override void Initialize()
    {
#if DEBUG
        GC.KeepAlive(typeof(GridExtensions));
#endif

        AvaloniaXamlLoader.Load(this);

        InteractionManager.Register();
        InteractionManager.CatchUnhandledExceptions(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var view = new MainView();
            desktop.MainWindow = view;
            FileInteractionManager.Window = view;
        }
        base.OnFrameworkInitializationCompleted();
    }
}
