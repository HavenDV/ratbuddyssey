using System;
using ReactiveUI.Fody.Helpers;

namespace Audyssey.ViewModels
{
    public class SmoothingFactorViewModel : ViewModelBase
    {
        [Reactive]
        public bool IsChecked { get; set; }

        public string Title { get; set; }

        public double Value { get; set; }

        public SmoothingFactorViewModel(string title, double value, bool isChecked = false)
        {
            Title = title ?? throw new ArgumentNullException(nameof(Title));
            Value = value;
            IsChecked = isChecked;
        }
    }
}
