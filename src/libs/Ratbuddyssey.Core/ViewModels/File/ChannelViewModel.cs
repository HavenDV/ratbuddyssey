using System;
using Ratbuddyssey.MultEQApp;
using ReactiveUI.Fody.Helpers;

namespace Ratbuddyssey.ViewModels
{
    public class ChannelViewModel : ViewModelBase
    {
        public DetectedChannel Data { get; set; }

        [Reactive]
        public bool Sticky { get; set; }

        public ChannelViewModel(int type, string commandId, bool sticky = false)
        {
            Data = new DetectedChannel
            {
                EnChannelType = type, 
                CommandId = commandId ?? throw new ArgumentNullException(nameof(commandId)),
            };

            Sticky = sticky;
        }

        public ChannelViewModel(DetectedChannel data)
        {
            Data = data ?? throw new ArgumentNullException(nameof(data));
        }
    }
}
