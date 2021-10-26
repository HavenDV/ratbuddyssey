namespace Ratbuddyssey.ViewModels;

public class PreviewDropViewModel : ActivatableViewModel
{
    #region Properties

    #region Public

    [Reactive]
    public bool IsVisible { get; set; }

    [Reactive]
    public IReadOnlyCollection<string> Names { get; set; } = Array.Empty<string>();

    #endregion

    #endregion
}
