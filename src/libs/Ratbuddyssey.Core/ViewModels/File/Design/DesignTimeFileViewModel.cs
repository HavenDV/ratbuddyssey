using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Ratbuddyssey.Models;

namespace Ratbuddyssey.ViewModels
{
    public class DesignTimeFileViewModel
    {
        #region Properties

        public IReadOnlyCollection<ChannelViewModel> Channels { get; set; } = new ChannelViewModel[]
        {
            new(0, "FL"),
            new(1, "C"),
            new(2, "FR"),
            new(5, "SRA"),
            new(13, "SLA"),
            new(19, "TFR"),
            new(22, "TRR"),
            new(36, "TRL"),
            new(39, "TFL"),
            new(42, "SW1"),
        };

        public ChannelViewModel? SelectedChannel => Channels.First();

        public IReadOnlyCollection<MeasurementPositionViewModel> MeasurementPositions { get; set; } = new MeasurementPositionViewModel[]
        {
            new(1, Color.Black, true),
            new(2, Color.Blue),
            new(3, Color.Violet),
            new(4, Color.Green),
            new(5, Color.Orange),
            new(6, Color.Red),
            new(7, Color.Cyan),
            new(8, Color.DeepPink),
        };

        public IReadOnlyCollection<SmoothingFactorViewModel> SmoothingFactors { get; set; } = new SmoothingFactorViewModel[]
        {
            new("No Smoothing", 1.0, true),
            new("1/2", 2.0),
            new("1/3", 3.0),
            new("1/6", 6.0),
            new("1/12", 12.0),
            new("1/24", 24.0),
            new("1/48", 48.0),
        };

        public IReadOnlyCollection<RangeViewModel> Ranges { get; set; } = new RangeViewModel[]
        {
            new("0-350ms", FrequencyRange.Chirp),
            new("10-1000Hz", FrequencyRange.Subwoofer),
            new("10Hz-24kHz", FrequencyRange.Full, true),
        };

        public List<TargetCurvePointViewModel> CustomTargetCurvePoints { get; set; } = new List<TargetCurvePointViewModel>()
        {
            new(0.0, 1.0),
            new(1.0, 2.0),
            new(2.0, 3.0),
        };

        #endregion
    }
}
