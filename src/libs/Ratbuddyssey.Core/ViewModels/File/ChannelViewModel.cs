using System;
using Ratbuddyssey.MultEQApp;
using ReactiveUI.Fody.Helpers;

namespace Ratbuddyssey.ViewModels
{
    public class ChannelViewModel : ViewModelBase
    {
        internal DetectedChannel? Data { get; set; }

        public int Type { get; set; } 
        public string CommandId { get; set; }

        [Reactive]
        public bool Sticky { get; set; }

        public ChannelViewModel(int type, string commandId, bool sticky = false)
        {
            Type = type;
            CommandId = commandId ?? throw new ArgumentNullException(nameof(commandId));
            Sticky = sticky;
        }

        public ChannelViewModel(DetectedChannel data)
        {
            Data = data ?? throw new ArgumentNullException(nameof(data));
            Type = data.EnChannelType;
            CommandId = data.CommandId ?? throw new ArgumentNullException(nameof(data.CommandId));
        }
    }
}
