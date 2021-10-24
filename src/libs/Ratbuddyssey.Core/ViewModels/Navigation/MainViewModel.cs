namespace Ratbuddyssey.ViewModels;

public class MainViewModel : ActivatableViewModel, IScreen
{
    #region Properties

    private IServiceProvider Services { get; }

    public RoutingState Router { get; } = new();

    public IReadOnlyCollection<Type> Tabs { get; } = new Type[]
    {
        typeof(FileViewModel),
        //typeof(EthernetViewModel),
    };

    [Reactive]
    public Type? SelectedTab { get; set; }

    #endregion

    #region Constructors

    public MainViewModel(IServiceProvider services)
    {
        Services = services ?? throw new ArgumentNullException(nameof(services));

        this.WhenActivated(disposables =>
        {
            _ = this
                .WhenAnyValue(static viewModel => viewModel.SelectedTab)
                .WhereNotNull()
                .Select(type => (IRoutableViewModel)Services.GetRequiredService(type))
                .InvokeCommand(Router.Navigate)
                .DisposeWith(disposables);

            SelectedTab = Tabs.First();
        });
    }

    #endregion
}
