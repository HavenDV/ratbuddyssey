using System.Collections.Generic;
using ReactiveUI.Fody.Helpers;

// ReSharper disable UnassignedGetOnlyAutoProperty

namespace Ratbuddyssey.ViewModels
{
    public class ChannelInformationViewModel : ActivatableViewModel
    {
        #region Properties

        #region Public

        [ObservableAsProperty]
        public ChannelViewModel Channel { get; } = new();

        [ObservableAsProperty]
        public bool IsChannelSelected { get; }

        public IReadOnlyCollection<string> CrossoverList { get; } = new[]
        {
            " ", "40", "60", "80", "90", "100", "110", "120", "150", "180", "200", "250", "F",
        };

        public IReadOnlyCollection<string> SpeakerTypeList { get; } = new[]
        {
            " ", "S", "L",
        };

        #endregion

        #endregion
    }
}
