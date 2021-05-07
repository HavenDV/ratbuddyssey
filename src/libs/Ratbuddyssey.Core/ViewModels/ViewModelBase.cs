using System;
using ReactiveUI;

namespace Ratbuddyssey.ViewModels
{
    public abstract class ViewModelBase : ReactiveObject
    {
    }

    public abstract class ActivatableViewModel : ViewModelBase, IActivatableViewModel
    {
        #region Properties

        public ViewModelActivator Activator { get; } = new();

        #endregion
    }

    public abstract class RoutableViewModel : ActivatableViewModel, IRoutableViewModel
    {
        #region Properties

        public string? UrlPathSegment { get; set; }
        public IScreen HostScreen { get; }

        #endregion

        #region Constructors

        protected RoutableViewModel(IScreen hostScreen, string? name = null)
        {
            HostScreen = hostScreen ?? throw new ArgumentNullException(nameof(hostScreen));
            UrlPathSegment = name;
        }

        #endregion
    }
}
