namespace Ratbuddyssey.ViewModels;

public class MainViewModel : ActivatableViewModel, IScreen
{
    #region Properties

    private IServiceProvider Services { get; }

    public RoutingState Router { get; } = new();

    #endregion

    #region Constructors

    public MainViewModel(IServiceProvider services)
    {
        Services = services ?? throw new ArgumentNullException(nameof(services));

        this.WhenActivated(disposables =>
        {
            Router.Navigate
                .Execute(Services.GetRequiredService<FileViewModel>())
                .Subscribe()
                .DisposeWith(disposables);
        });
    }

    #endregion
}
