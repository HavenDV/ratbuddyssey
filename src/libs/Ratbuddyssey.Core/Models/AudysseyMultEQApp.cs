using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Ratbuddyssey.MultEQApp;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Ratbuddyssey.Models
{
    namespace MultEQApp
    {
        [DataContract]
        public class AudysseyMultEQApp : ReactiveObject
        {
            #region Properties

            [DataMember]
            [Reactive]
            public string Title { get; set; } = string.Empty;

            [DataMember]
            [Reactive]
            public string TargetModelName { get; set; } = string.Empty;

            [DataMember]
            [Reactive]
            public string InterfaceVersion { get; set; } = string.Empty;

            [DataMember]
            [Reactive]
            public bool? DynamicEq { get; set; }

            [DataMember]
            [Reactive]
            public bool? DynamicVolume { get; set; }

            [DataMember]
            [Reactive]
            public bool? LfcSupport { get; set; }

            [DataMember]
            [Reactive]
            public bool? Lfc { get; set; }

            [DataMember]
            [Reactive]
            public int? SystemDelay { get; set; }

            [DataMember]
            [Reactive]
            public decimal? AdcLineup { get; set; }

            [DataMember]
            [Reactive]
            public int EnTargetCurveType { get; set; }
            
            [DataMember]
            [Reactive]
            public int EnAmpAssignType { get; set; }
            
            [DataMember]
            [Reactive]
            public int EnMultEQType { get; set; }
            
            [DataMember]
            [Reactive]
            public string AmpAssignInfo { get; set; } = string.Empty;

            [DataMember]
            [Reactive]
            public bool? Auro { get; set; }

            [DataMember]
            [Reactive]
            public string UpgradeInfo { get; set; } = string.Empty;

            [DataMember]
            [Reactive]
            public ObservableCollection<DetectedChannel> DetectedChannels { get; set; } = new();

            #endregion
        }
    }
}