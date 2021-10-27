#nullable enable

namespace Ratbuddyssey.Views;

public partial class PreviewDropView
{
    #region Constructors

    public PreviewDropView()
    {
        InitializeComponent();

        _ = this.WhenActivated(disposable =>
        {
            if (ViewModel == null)
            {
                return;
            }

            _ = this.OneWayBind(ViewModel,
                    static viewModel => viewModel.Names,
                    static view => view.NamesListView.ItemsSource)
                .DisposeWith(disposable);
        });
    }

    #endregion
}
