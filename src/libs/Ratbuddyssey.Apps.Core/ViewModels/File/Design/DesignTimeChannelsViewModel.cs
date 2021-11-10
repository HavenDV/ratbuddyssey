namespace Ratbuddyssey.ViewModels;

public class DesignTimeChannelsViewModel : ChannelsViewModel
{
    #region Constructors

    public DesignTimeChannelsViewModel()
    {
        Channels = new ChannelViewModel[]
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
        SelectedChannel = Channels.First();
    }

    #endregion
}
