// Remove after fix: https://github.com/unoplatform/uno/discussions/8175

using ReactiveUI;
using System.Reactive.Disposables;

#nullable enable

namespace Ratbuddyssey.Views
{

    public partial class MainView
    {
        partial void BeforeInitializeComponent();
        partial void AfterInitializeComponent();


        partial void AfterWhenActivated(CompositeDisposable disposables);

        public MainView()
        {
            BeforeInitializeComponent();

            InitializeComponent();

            AfterInitializeComponent();

            _ = this.WhenActivated(disposables =>
            {
                DataContext = ViewModel;

                if (ViewModel == null)
                {
                    return;
                }

                AfterWhenActivated(disposables);
            });
        }

    }


    public partial class ChannelsView
    {
        partial void BeforeInitializeComponent();
        partial void AfterInitializeComponent();


        partial void AfterWhenActivated(CompositeDisposable disposables);

        public ChannelsView()
        {
            BeforeInitializeComponent();

            InitializeComponent();

            AfterInitializeComponent();

            _ = this.WhenActivated(disposables =>
            {
                DataContext = ViewModel;

                if (ViewModel == null)
                {
                    return;
                }

                AfterWhenActivated(disposables);
            });
        }

    }


    public partial class GraphView
    {
        partial void BeforeInitializeComponent();
        partial void AfterInitializeComponent();


        partial void AfterWhenActivated(CompositeDisposable disposables);

        public GraphView()
        {
            BeforeInitializeComponent();

            InitializeComponent();

            AfterInitializeComponent();

            _ = this.WhenActivated(disposables =>
            {
                DataContext = ViewModel;

                if (ViewModel == null)
                {
                    return;
                }

                AfterWhenActivated(disposables);
            });
        }

    }


    public partial class FileView
    {
        partial void BeforeInitializeComponent();
        partial void AfterInitializeComponent();


        partial void AfterWhenActivated(CompositeDisposable disposables);

        public FileView()
        {
            BeforeInitializeComponent();

            InitializeComponent();

            AfterInitializeComponent();

            _ = this.WhenActivated(disposables =>
            {
                DataContext = ViewModel;

                if (ViewModel == null)
                {
                    return;
                }

                AfterWhenActivated(disposables);
            });
        }

    }


    public partial class TargetCurvePointsView
    {
        partial void BeforeInitializeComponent();
        partial void AfterInitializeComponent();


        partial void AfterWhenActivated(CompositeDisposable disposables);

        public TargetCurvePointsView()
        {
            BeforeInitializeComponent();

            InitializeComponent();

            AfterInitializeComponent();

            _ = this.WhenActivated(disposables =>
            {
                DataContext = ViewModel;

                if (ViewModel == null)
                {
                    return;
                }

                AfterWhenActivated(disposables);
            });
        }

    }


    public partial class ChannelReportView
    {
        partial void BeforeInitializeComponent();
        partial void AfterInitializeComponent();


        partial void AfterWhenActivated(CompositeDisposable disposables);

        public ChannelReportView()
        {
            BeforeInitializeComponent();

            InitializeComponent();

            AfterInitializeComponent();

            _ = this.WhenActivated(disposables =>
            {
                DataContext = ViewModel;

                if (ViewModel == null)
                {
                    return;
                }

                AfterWhenActivated(disposables);
            });
        }

    }


    public partial class StatusView
    {
        partial void BeforeInitializeComponent();
        partial void AfterInitializeComponent();


        partial void AfterWhenActivated(CompositeDisposable disposables);

        public StatusView()
        {
            BeforeInitializeComponent();

            InitializeComponent();

            AfterInitializeComponent();

            _ = this.WhenActivated(disposables =>
            {
                DataContext = ViewModel;

                if (ViewModel == null)
                {
                    return;
                }

                AfterWhenActivated(disposables);
            });
        }

    }


    public partial class ChannelInformationView
    {
        partial void BeforeInitializeComponent();
        partial void AfterInitializeComponent();


        partial void AfterWhenActivated(CompositeDisposable disposables);

        public ChannelInformationView()
        {
            BeforeInitializeComponent();

            InitializeComponent();

            AfterInitializeComponent();

            _ = this.WhenActivated(disposables =>
            {
                DataContext = ViewModel;

                if (ViewModel == null)
                {
                    return;
                }

                AfterWhenActivated(disposables);
            });
        }

    }


    public partial class PreviewDropView
    {
        partial void BeforeInitializeComponent();
        partial void AfterInitializeComponent();


        partial void AfterWhenActivated(CompositeDisposable disposables);

        public PreviewDropView()
        {
            BeforeInitializeComponent();

            InitializeComponent();

            AfterInitializeComponent();

            _ = this.WhenActivated(disposables =>
            {
                DataContext = ViewModel;

                if (ViewModel == null)
                {
                    return;
                }

                AfterWhenActivated(disposables);
            });
        }

    }

}
