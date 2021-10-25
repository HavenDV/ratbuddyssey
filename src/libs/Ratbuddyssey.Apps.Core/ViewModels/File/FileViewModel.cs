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

    #endregion

    #region Commands

    public ReactiveCommand<Unit, Unit> OpenFile { get; }
    public ReactiveCommand<Unit, Unit> SaveFile { get; }
    public ReactiveCommand<Unit, Unit> SaveFileAs { get; }
    public ReactiveCommand<Unit, Unit> ReloadFile { get; }

    public ReactiveCommand<string[], Unit> Drop { get; }

    #endregion

    #endregion

    #region Constructors

    public FileViewModel(IScreen hostScreen) : base(hostScreen, "App")
    {
        OpenFile = ReactiveCommand.CreateFromTask(async _ =>
        {
            var fileName = await Interactions.OpenFile.Handle(".ady");
            if (fileName == null)
            {
                return;
            }

            Open(fileName);
        });
        SaveFile = ReactiveCommand.Create(() =>
        {
#if DEBUG
            CurrentFile = Path.ChangeExtension(CurrentFile, ".json");
#endif
            SaveApp(CurrentFile);
        });
        SaveFileAs = ReactiveCommand.CreateFromTask(async _ =>
        {
            var fileName = await Interactions.SaveFile.Handle(".ady");
            if (fileName == null)
            {
                return;
            }

            CurrentFile = fileName;
            SaveApp(CurrentFile);
        });
        ReloadFile = ReactiveCommand.CreateFromTask(async _ =>
        {
            var value = await Interactions.Question.Handle(
                "This will reload the .ady file and discard all changes since last save");
            if (!value)
            {
                return;
            }

            LoadApp(CurrentFile);
        });
        Drop = ReactiveCommand.Create<string[]>(paths =>
        {
            if (paths.Any())
            {
                Open(paths.First());
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

    public void Open(string filePath)
    {
        if (!File.Exists(filePath))
        {
            return;
        }

        CurrentFile = filePath;
        LoadApp(CurrentFile);
    }

    private void LoadApp(string fileName)
    {
        if (!File.Exists(fileName))
        {
            return;
        }

        var json = File.ReadAllText(fileName);
        AudysseyApp = JsonConvert.DeserializeObject<AudysseyMultEQApp>(json, new JsonSerializerSettings
        {
            FloatParseHandling = FloatParseHandling.Decimal
        }) ?? throw new InvalidOperationException("Invalid file.");
        ChannelsViewModel.Channels = AudysseyApp.DetectedChannels
            .Select(static channel => new ChannelViewModel(channel))
            .ToArray();
    }

    private void SaveApp(string fileName)
    {
        var json = JsonConvert.SerializeObject(AudysseyApp, new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        });
        if (!string.IsNullOrEmpty(fileName))
        {
            File.WriteAllText(fileName, json);
        }
    }

    #endregion
}
