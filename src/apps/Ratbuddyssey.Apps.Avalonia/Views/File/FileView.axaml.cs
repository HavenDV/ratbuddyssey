using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Ratbuddyssey.ViewModels;
using Splat;

namespace Ratbuddyssey.Apps.Views;

public partial class FileView : UserControl
{
    public FileView()
    {
        InitializeComponent();

        DataContext = new FileViewModel(Locator.Current.GetService<MainViewModel>());
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
