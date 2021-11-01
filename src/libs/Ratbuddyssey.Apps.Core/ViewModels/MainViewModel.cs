namespace Ratbuddyssey.ViewModels;

public class MainViewModel : ReactiveObject
{
    public FileViewModel FileViewModel { get; } = new();
}
