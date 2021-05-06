using System;

namespace Audyssey.ViewModels
{
    public class TabViewModel : ViewModelBase
    {
        public Type Type { get; set; }

        public string Header { get; set; }

        public TabViewModel(Type type, string header)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
            Header = header ?? throw new ArgumentNullException(nameof(header));
        }
    }
}
