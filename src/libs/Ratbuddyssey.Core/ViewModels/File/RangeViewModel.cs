using System;
using ReactiveUI.Fody.Helpers;

namespace Audyssey.ViewModels
{
    public class RangeViewModel : ViewModelBase
    {
        [Reactive]
        public bool IsChecked { get; set; }

        public string Title { get; set; }

        public Range Value { get; set; }

        public RangeViewModel()
        {
        }

        public RangeViewModel(string title, Range value, bool isChecked = false)
        {
            Title = title ?? throw new ArgumentNullException(nameof(Title));
            Value = value;
            IsChecked = isChecked;
        }
    }

    public enum Range
    {
        Chirp,
        Subwoofer,
        Full,
    }
}
