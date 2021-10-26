using System;
using System.Reactive.Disposables;
using System.Windows;
using H.ReactiveUI;
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
                        static viewModel => viewModel.StatusViewModel,
                        static view => view.StatusView.ViewModel)
                    .DisposeWith(disposable);
#if WPF_APP
                _ = this.OneWayBind(ViewModel,
                        static viewModel => viewModel.ChannelsViewModel,
                        static view => view.ChannelsView.ViewModel)
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
#endif

                // Drag and drop
                _ = this.OneWayBind(ViewModel,
                        static viewModel => viewModel.PreviewDropViewModel,
                        static view => view.PreviewDropView.ViewModel)
                    .DisposeWith(disposable);
                _ = this.OneWayBind(ViewModel,
                        static viewModel => viewModel.PreviewDropViewModel.IsVisible,
                        static view => view.PreviewDropView.Visibility)
                    .DisposeWith(disposable);
                _ = ViewModel
                    .WhenAnyValue(static x => x.DragFilesEnter)
                    .Subscribe(command => DragAndDropExtensions.SetDragFilesEnterCommand(this, command))
                    .DisposeWith(disposable);
                _ = ViewModel
                    .WhenAnyValue(static x => x.DragLeave)
                    .Subscribe(command => DragAndDropExtensions.SetDragLeaveCommand(this, command))
                    .DisposeWith(disposable);
                _ = ViewModel
                    .WhenAnyValue(static x => x.DropFiles)
                    .Subscribe(command => DragAndDropExtensions.SetDropFilesCommand(this, command))
                    .DisposeWith(disposable);
            });
        }

        #endregion
    }
}
