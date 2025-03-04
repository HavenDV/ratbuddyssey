﻿using System.Runtime.Serialization;

namespace Ratbuddyssey.MultEQApp;

[DataContract]
public class DetectedChannel : ReactiveObject
{
    #region Properties

    [DataMember]
    [Reactive]
    public int EnChannelType { get; set; }

    [DataMember]
    [Reactive]
    public bool? IsSkipMeasurement { get; set; }

    [DataMember]
    [Reactive]
    public string DelayAdjustment { get; set; } = string.Empty;

    [DataMember]
    [Reactive]
    public string CommandId { get; set; } = string.Empty;

    [DataMember]
    [Reactive]
    public string TrimAdjustment { get; set; } = string.Empty;

    [DataMember]
    [Reactive]
    public ChannelReport ChannelReport { get; set; } = new();

    [DataMember]
    [Reactive]
    public Dictionary<string, string[]> ResponseData { get; set; } = new();

    [DataMember]
    [Reactive]
    public ICollection<string> CustomTargetCurvePoints { get; set; } = Array.Empty<string>();

    [DataMember]
    [Reactive]
    public bool? MidrangeCompensation { get; set; }

    [DataMember]
    [Reactive]
    public decimal? FrequencyRangeRolloff { get; set; }

    [DataMember]
    [Reactive]
    public string CustomLevel { get; set; } = string.Empty;

    [DataMember]
    [Reactive]
    public string CustomSpeakerType { get; set; } = string.Empty;

    [DataMember]
    [Reactive]
    public decimal? CustomDistance { get; set; }

    [DataMember]
    [Reactive]
    public string CustomCrossover { get; set; } = string.Empty;

    #endregion

    #region Serialization

    /// <summary>
    /// Do not serialize second subwoofer (channeltype 55) custom curve points since they are not used by Audyssey.
    /// TODO: I think it's worth transferring this logic to the ViewModel with the additional disabling of the UI when necessary, additionally commenting.
    /// </summary>
    /// <returns></returns>
    public bool ShouldSerializeCustomTargetCurvePoints()
    {
        return EnChannelType != 55;
    }

    #endregion
}