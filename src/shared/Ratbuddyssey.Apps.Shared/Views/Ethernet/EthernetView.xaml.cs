#if HAS_WPF
using System.Reactive.Disposables;
using ReactiveUI;

#nullable enable

namespace Ratbuddyssey.Views
{
    public partial class EthernetView
    {
        #region Constructors

        public EthernetView()
        {
            InitializeComponent();

            _ = this.WhenActivated(disposable =>
            {
                if (ViewModel == null)
                {
                    return;
                }

                _ = this.OneWayBind(ViewModel,
                        static viewModel => viewModel.Ips,
                        static view => view.IpsComboBox.ItemsSource)
                    .DisposeWith(disposable);
                _ = this.Bind(ViewModel,
                        static viewModel => viewModel.SelectedIp,
                        static view => view.IpsComboBox.SelectedItem)
                    .DisposeWith(disposable);
                _ = this.OneWayBind(ViewModel,
                        static viewModel => viewModel.InterfaceIps,
                        static view => view.InterfaceIpsComboBox.ItemsSource)
                    .DisposeWith(disposable);
                _ = this.Bind(ViewModel,
                        static viewModel => viewModel.SelectedInterfaceIp,
                        static view => view.InterfaceIpsComboBox.SelectedItem)
                    .DisposeWith(disposable);
                _ = this.Bind(ViewModel,
                        static viewModel => viewModel.ConnectReceiverIsChecked,
                        static view => view.ConnectReceiverMenuItem.IsChecked)
                    .DisposeWith(disposable);
                _ = this.Bind(ViewModel,
                        static viewModel => viewModel.ConnectSnifferIsChecked,
                        static view => view.ConnectSnifferMenuItem.IsChecked)
                    .DisposeWith(disposable);
                _ = this.Bind(ViewModel,
                        static viewModel => viewModel.ConnectAudysseyIsChecked,
                        static view => view.ConnectAudysseyMenuItem.IsChecked)
                    .DisposeWith(disposable);
                _ = this.Bind(ViewModel,
                        static viewModel => viewModel.SelectedChannelSetupView,
                        static view => view.ChannelSetupView.SelectedItem)
                    .DisposeWith(disposable);

                _ = this.BindCommand(ViewModel,
                        static viewModel => viewModel.OpenFile,
                        static view => view.OpenFileMenuItem)
                    .DisposeWith(disposable);
                _ = this.BindCommand(ViewModel,
                        static viewModel => viewModel.SaveFile,
                        static view => view.SaveFileMenuItem)
                    .DisposeWith(disposable);
                _ = this.BindCommand(ViewModel,
                        static viewModel => viewModel.GetReceiverInfo,
                        static view => view.GetReceiverInfoMenuItem)
                    .DisposeWith(disposable);
                _ = this.BindCommand(ViewModel,
                        static viewModel => viewModel.GetReceiverStatus,
                        static view => view.GetReceiverStatusMenuItem)
                    .DisposeWith(disposable);
                _ = this.BindCommand(ViewModel,
                        static viewModel => viewModel.SetAvrSetAmp,
                        static view => view.SetAvrSetAmpMenuItem)
                    .DisposeWith(disposable);
                _ = this.BindCommand(ViewModel,
                        static viewModel => viewModel.SetAvrSetAudy,
                        static view => view.SetAvrSetAudyMenuItem)
                    .DisposeWith(disposable);
                _ = this.BindCommand(ViewModel,
                        static viewModel => viewModel.SetAvrSetDisFil,
                        static view => view.SetAvrSetDisFilMenuItem)
                    .DisposeWith(disposable);
                _ = this.BindCommand(ViewModel,
                        static viewModel => viewModel.SetAvrInitCoefs,
                        static view => view.SetAvrInitCoefsMenuItem)
                    .DisposeWith(disposable);
                _ = this.BindCommand(ViewModel,
                        static viewModel => viewModel.SetAvrSetCoefDt,
                        static view => view.SetAvrSetCoefDtMenuItem)
                    .DisposeWith(disposable);
                _ = this.BindCommand(ViewModel,
                        static viewModel => viewModel.SetAudysseyFinishedFlag,
                        static view => view.SetAudysseyFinishedFlagMenuItem)
                    .DisposeWith(disposable);

                _ = this.OneWayBind(ViewModel,
                        static viewModel => viewModel.Avr.ChSetup,
                        static view => view.ChannelSetupView.ItemsSource)
                    .DisposeWith(disposable);
            });
        }

        #endregion
    }
}
#endif