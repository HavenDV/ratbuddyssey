using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Ratbuddyssey.ViewModels;

namespace Ratbuddyssey.Apps.Views;

public partial class FileView : UserControl
{
    public FileView()
    {
        InitializeComponent();

        DataContext = new FileViewModel(new MainViewModel());
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
