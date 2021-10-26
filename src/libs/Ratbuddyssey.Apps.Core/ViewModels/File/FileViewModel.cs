using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Ratbuddyssey.Models.MultEQApp;

// ReSharper disable UnassignedGetOnlyAutoProperty

namespace Ratbuddyssey.ViewModels;

public class FileViewModel : RoutableViewModel
{
    #region Properties

    #region Public

    [Reactive]
    public AudysseyMultEQApp AudysseyApp { get; set; } = new();

    [Reactive]
    public string CurrentFile { get; set; } = string.Empty;

    public ChannelsViewModel ChannelsViewModel { get; } = new();
    public StatusViewModel StatusViewModel { get; } = new();
    public TargetCurvePointsViewModel TargetCurvePointsViewModel { get; } = new();
    public ChannelInformationViewModel ChannelInformationViewModel { get; } = new();
    public ChannelReportViewModel ChannelReportViewModel { get; } = new();
    public GraphViewModel GraphViewModel { get; } = new();
    public PreviewDropViewModel PreviewDropViewModel { get; } = new();

    #endregion

    #region Commands

    public ReactiveCommand<Unit, Unit> OpenFile { get; }
    public ReactiveCommand<Unit, Unit> SaveFile { get; }
    public ReactiveCommand<Unit, Unit> SaveFileAs { get; }
    public ReactiveCommand<Unit, Unit> ReloadFile { get; }

    public ReactiveCommand<IReadOnlyCollection<FileData>, Unit> DragFilesEnter { get; }
    public ReactiveCommand<Unit, Unit> DragLeave { get; }
    public ReactiveCommand<IReadOnlyCollection<FileData>, Unit> DropFiles { get; }

    #endregion

    #endregion

    #region Constructors

    public FileViewModel(IScreen hostScreen) : base(hostScreen, "App")
    {
        OpenFile = ReactiveCommand.CreateFromTask(async cancellationToken =>
        {
            var data = await FileInteractions.OpenFile.Handle(new OpenFileArguments
            {
                Extensions = new[] { ".ady" },
                FilterName = "Audyssey files",
            });
            if (data == null)
            {
                return;
            }

            Open(data);
        });
        SaveFile = ReactiveCommand.CreateFromTask(async cancellationToken =>
        {
            _ = await FileInteractions.SaveOpenFile.Handle(new SaveOpenFileArguments
            {
                FullPath = CurrentFile,
                Text = SaveApp(),
            });
        });
        SaveFileAs = ReactiveCommand.CreateFromTask(async cancellationToken =>
        {
            var fileName = await FileInteractions.SaveFile.Handle(new SaveFileArguments
            {
                SuggestedFileName = Path.GetFileName(CurrentFile),
                Extension = ".ady",
                FilterName = "Audyssey files",
                TextFunc = () => Task.FromResult(SaveApp()),
            });
            if (fileName == null)
            {
                return;
            }

            CurrentFile = fileName;
        });
        ReloadFile = ReactiveCommand.CreateFromTask(async cancellationToken =>
        {
            var value = await MessageInteractions.Question.Handle(new(
                "This will reload the .ady file and discard all changes since last save"));
            if (!value)
            {
                return;
            }

            LoadApp(CurrentFile);
        });
        DragFilesEnter = ReactiveCommand.Create<IReadOnlyCollection<FileData>>(files =>
        {
            PreviewDropViewModel.Names = new[] { files.First().FullPath };
            PreviewDropViewModel.IsVisible = true;
        });
        DragLeave = ReactiveCommand.Create(() =>
        {
            PreviewDropViewModel.IsVisible = false;
        });
        DropFiles = ReactiveCommand.Create<IReadOnlyCollection<FileData>>(files =>
        {
            PreviewDropViewModel.IsVisible = false;

            if (files.Any())
            {
                Open(files.First());
            }
        });

        this.WhenActivated(disposables =>
        {
            _ = this
                .WhenAnyValue(static x => x.AudysseyApp)
                .BindTo(StatusViewModel, static x => x.AudysseyApp)
                .DisposeWith(disposables);
            _ = this
                .WhenAnyValue(static x => x.CurrentFile)
                .BindTo(StatusViewModel, static x => x.CurrentFile)
                .DisposeWith(disposables);

            _ = ChannelsViewModel
                .WhenAnyValue(static x => x.Channel)
                .ToPropertyEx(
                    TargetCurvePointsViewModel,
                    static x => x.Channel,
                    new ChannelViewModel())
                .DisposeWith(disposables);
            _ = ChannelsViewModel
                .WhenAnyValue(static x => x.IsChannelSelected)
                .ToPropertyEx(
                    TargetCurvePointsViewModel,
                    static x => x.IsChannelSelected)
                .DisposeWith(disposables);

            _ = ChannelsViewModel
                .WhenAnyValue(static x => x.Channel)
                .ToPropertyEx(
                    ChannelInformationViewModel,
                    static x => x.Channel,
                    new ChannelViewModel())
                .DisposeWith(disposables);
            _ = ChannelsViewModel
                .WhenAnyValue(static x => x.IsChannelSelected)
                .ToPropertyEx(
                    ChannelInformationViewModel,
                    static x => x.IsChannelSelected)
                .DisposeWith(disposables);

            _ = ChannelsViewModel
                .WhenAnyValue(static x => x.Channel)
                .ToPropertyEx(
                    ChannelReportViewModel,
                    static x => x.Channel,
                    new ChannelViewModel())
                .DisposeWith(disposables);
            _ = ChannelsViewModel
                .WhenAnyValue(static x => x.IsChannelSelected)
                .ToPropertyEx(
                    ChannelReportViewModel,
                    static x => x.IsChannelSelected)
                .DisposeWith(disposables);

            _ = this
                .WhenAnyValue(static x => x.AudysseyApp.EnTargetCurveType)
                .ToPropertyEx(
                    GraphViewModel,
                    static x => x.EnTargetCurveType)
                .DisposeWith(disposables);
            _ = ChannelsViewModel
                .WhenAnyValue(static x => x.Channels)
                .ToPropertyEx(
                    GraphViewModel,
                    static x => x.Channels,
                    Array.Empty<ChannelViewModel>())
                .DisposeWith(disposables);
            _ = ChannelsViewModel
                .WhenAnyValue(static x => x.SelectedChannel)
                .ToPropertyEx(
                    GraphViewModel,
                    static x => x.SelectedChannel)
                .DisposeWith(disposables);
        });
    }

    #endregion

    #region Methods

    public void Open(FileData data)
    {
        CurrentFile = data.FullPath;
        LoadApp(data.Text);
    }

    private void LoadApp(string json)
    {
        AudysseyApp = JsonConvert.DeserializeObject<AudysseyMultEQApp>(json, new JsonSerializerSettings
        {
            FloatParseHandling = FloatParseHandling.Decimal
        }) ?? throw new InvalidOperationException("Invalid file.");
        ChannelsViewModel.Channels = AudysseyApp.DetectedChannels
            .Select(static channel => new ChannelViewModel(channel))
            .ToArray();
    }

    private string SaveApp()
    {
        return JsonConvert.SerializeObject(AudysseyApp, new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        });
    }

    #endregion
}
