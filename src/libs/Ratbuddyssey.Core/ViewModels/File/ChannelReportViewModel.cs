// ReSharper disable UnassignedGetOnlyAutoProperty

namespace Ratbuddyssey.ViewModels
{
    public class ChannelReportViewModel : ActivatableViewModel
    {
        #region Properties

        #region Public

        [ObservableAsProperty]
        public ChannelViewModel Channel { get; } = new();

        [ObservableAsProperty]
        public bool IsChannelSelected { get; }

        #endregion

        #endregion
    }
}
