using System;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Audyssey.ViewModels
{
    public abstract class ViewModelBase : ReactiveObject
    {
    }

    public abstract class SelectableViewModel : ReactiveObject
    {
        [Reactive]
        public bool IsSelected { get; set; }
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

        public ReactiveCommand<Type, IRoutableViewModel> NavigateByType { get; } = ReactiveCommand.Create<Type, IRoutableViewModel>(
            static type => throw new InvalidOperationException("HostScreen is not NavigationViewModel."));
        public ReactiveCommand<IRoutableViewModel, IRoutableViewModel> NavigateByViewModel { get; } = ReactiveCommand.Create<IRoutableViewModel, IRoutableViewModel>(
            static type => throw new InvalidOperationException("HostScreen is not NavigationViewModel."));

        #endregion

        #region Constructors

        protected RoutableViewModel(IScreen hostScreen, string? name = null)
        {
            HostScreen = hostScreen ?? throw new ArgumentNullException(nameof(hostScreen));
            UrlPathSegment = name;

            //if (HostScreen is NavigationViewModel navigationViewModel)
            //{
            //    NavigateByType = navigationViewModel.Navigate;
            //    NavigateByViewModel = navigationViewModel.Router.Navigate;
            //}
        }

        #endregion
    }
}
