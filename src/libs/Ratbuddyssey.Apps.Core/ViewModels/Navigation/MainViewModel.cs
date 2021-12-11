namespace Ratbuddyssey.ViewModels;

public class MainViewModel : ActivatableViewModel, IScreen
{
    #region Properties

    public RoutingState Router { get; } = new();
    public FileViewModel FileViewModel { get; }

    #endregion

    #region Constructors

    public MainViewModel()
    {
        FileViewModel = new(this);

        this.WhenActivated(disposables =>
        {
            Router.Navigate
                .Execute(FileViewModel)
                .Subscribe()
                .DisposeWith(disposables);
        });
    }

    #endregion
}
