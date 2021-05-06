using System.Drawing;
using ReactiveUI.Fody.Helpers;

namespace Audyssey.ViewModels
{
    public class MeasurementPositionViewModel : ViewModelBase
    {
        [Reactive] 
        public bool IsEnabled { get; set; } = true;

        [Reactive]
        public bool IsChecked { get; set; }

        public Color Color { get; set; }

        public int Value { get; set; }

        public MeasurementPositionViewModel()
        {
        }

        public MeasurementPositionViewModel(int value, Color color, bool isChecked = false)
        {
            Value = value;
            Color = color;
            IsChecked = isChecked;
        }
    }
}
