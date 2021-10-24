using System.Runtime.Serialization;

namespace Ratbuddyssey.MultEQApp;

[DataContract]
public class ChannelReport : ReactiveObject
{
    [DataMember]
    [Reactive]
    public int? EnSpeakerConnect { get; set; }

    [DataMember]
    [Reactive]
    public int? CustomEnSpeakerConnect { get; set; }

    [DataMember]
    [Reactive]
    public bool? IsReversePolarity { get; set; }

    [DataMember]
    [Reactive]
    public decimal? Distance { get; set; }
}
