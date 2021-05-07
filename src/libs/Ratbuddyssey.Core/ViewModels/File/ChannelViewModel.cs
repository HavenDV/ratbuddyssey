using System;
using Ratbuddyssey.MultEQApp;
using ReactiveUI.Fody.Helpers;

namespace Ratbuddyssey.ViewModels
{
    public class ChannelViewModel : ViewModelBase
    {
        #region Properties

        public DetectedChannel Data { get; set; }

        [Reactive]
        public bool Sticky { get; set; }
        
        #endregion

        #region Constructors

        public ChannelViewModel()
        {
            Data = new DetectedChannel();
        }

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

        #endregion
    }
}
