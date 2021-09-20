using System.Collections.Generic;
using Ratbuddyssey.Models.MultEQApp;
using ReactiveUI.Fody.Helpers;

// ReSharper disable UnassignedGetOnlyAutoProperty

namespace Ratbuddyssey.ViewModels
{
    public class StatusViewModel : ActivatableViewModel
    {
        #region Properties

        #region Public

        [ObservableAsProperty]
        public AudysseyMultEQApp AudysseyApp { get; } = new();

        [ObservableAsProperty]
        public string CurrentFile { get; } = string.Empty;

        public IReadOnlyCollection<string> AmpAssignTypeList { get; } = new[]
        {
            "FrontA", "FrontB", "Type3", "Type4",
            "Type5", "Type6", "Type7", "Type8",
            "Type9", "Type10", "Type11", "Type12",
            "Type13", "Type14", "Type15", "Type16",
            "Type17", "Type18", "Type19", "Type20",
        };

        public IReadOnlyCollection<string> TargetCurveTypeList { get; } = new[]
        {
            " ", "High Frequency Roll Off 1", "High Frequency Roll Off 2",
        };

        public IReadOnlyCollection<string> MultEQTypeList { get; } = new[]
        {
            "MultEQ", "MultEQXT", "MultEQXT32",
        };

        #endregion

        #endregion
    }
}
