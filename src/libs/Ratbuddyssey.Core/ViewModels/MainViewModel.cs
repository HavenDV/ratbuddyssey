namespace Audyssey.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public FileViewModel File { get; } = new();
        public EthernetViewModel Ethernet { get; } = new();
    }
}
