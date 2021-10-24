using System;
using System.Reactive.Disposables;
using System.Windows;
using ReactiveUI;

#nullable enable

namespace Ratbuddyssey.Views
{
    public partial class FileView
    {
        #region Constructors

        public FileView()
        {
            InitializeComponent();

#if WPF_APP
            _ = this.WhenActivated(disposable =>
            {
                if (ViewModel == null)
                {
                    return;
                }

                _ = this.BindCommand(ViewModel,
                        static viewModel => viewModel.OpenFile,
                        static view => view.OpenFileMenuItem)
                    .DisposeWith(disposable);
                _ = this.BindCommand(ViewModel,
                        static viewModel => viewModel.SaveFile,
                        static view => view.SaveFileMenuItem)
                    .DisposeWith(disposable);
                _ = this.BindCommand(ViewModel,
                        static viewModel => viewModel.SaveFileAs,
                        static view => view.SaveFileAsMenuItem)
                    .DisposeWith(disposable);
                _ = this.BindCommand(ViewModel,
                        static viewModel => viewModel.ReloadFile,
                        static view => view.ReloadFileMenuItem)
                    .DisposeWith(disposable);

                _ = this.OneWayBind(ViewModel,
                        static viewModel => viewModel.ChannelsViewModel,
                        static view => view.ChannelsView.ViewModel)
                    .DisposeWith(disposable);
                _ = this.OneWayBind(ViewModel,
                        static viewModel => viewModel.StatusViewModel,
                        static view => view.StatusView.ViewModel)
                    .DisposeWith(disposable);
                _ = this.OneWayBind(ViewModel,
                        static viewModel => viewModel.TargetCurvePointsViewModel,
                        static view => view.TargetCurvePointsView.ViewModel)
                    .DisposeWith(disposable);
                _ = this.OneWayBind(ViewModel,
                        static viewModel => viewModel.ChannelInformationViewModel,
                        static view => view.ChannelInformationView.ViewModel)
                    .DisposeWith(disposable);
                _ = this.OneWayBind(ViewModel,
                        static viewModel => viewModel.ChannelReportViewModel,
                        static view => view.ChannelReportView.ViewModel)
                    .DisposeWith(disposable);
                _ = this.OneWayBind(ViewModel,
                        static viewModel => viewModel.GraphViewModel,
                        static view => view.GraphView.ViewModel)
                    .DisposeWith(disposable);
            });
#endif
        }

        #endregion

        #region Event Handlers

#if WPF_APP
        public void HandleDroppedFile(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop) ||
                e.Data.GetData(DataFormats.FileDrop) is not string[] files)
            {
                return;
            }

            _ = ViewModel?.Drop
                .Execute(files)
                .Subscribe();
        }
#endif

        #endregion
    }
}
