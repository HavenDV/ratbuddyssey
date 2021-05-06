using System.Collections.Generic;
using System.Drawing;
using Audyssey.Models;

namespace Audyssey.ViewModels
{
    public class DesignTimeFileViewModel
    {
        #region Properties
        
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

        #endregion
    }
}
