using System;
using Ratbuddyssey.Models;
using ReactiveUI.Fody.Helpers;

namespace Ratbuddyssey.ViewModels
{
    public class RangeViewModel : ViewModelBase
    {
        [Reactive]
        public bool IsChecked { get; set; }

        public string Title { get; set; }

        public FrequencyRange Value { get; set; }

        public RangeViewModel(string title, FrequencyRange value, bool isChecked = false)
        {
            Title = title ?? throw new ArgumentNullException(nameof(title));
            Value = value;
            IsChecked = isChecked;
        }
    }
}
