using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using H.ReactiveUI;
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
