using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Ratbuddyssey.Models.MultEQApp;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

// ReSharper disable UnassignedGetOnlyAutoProperty

namespace Ratbuddyssey.ViewModels
{
    public class FileViewModel : RoutableViewModel
    {
        #region Properties

        #region Public

        [Reactive]
        public AudysseyMultEQApp AudysseyApp { get; set; } = new();

        [Reactive]
        public IReadOnlyCollection<ChannelViewModel> Channels { get; set; } = Array.Empty<ChannelViewModel>();

        [Reactive]
        public ChannelViewModel? SelectedChannel { get; set; }

        [ObservableAsProperty]
        public ChannelViewModel Channel { get; } = new();

        [ObservableAsProperty]
        public bool IsChannelSelected { get; }

        [Reactive]
        public ObservableCollection<TargetCurvePointViewModel> CustomTargetCurvePoints { get; set; } = new();

        [Reactive]
        public string CurrentFile { get; set; } = string.Empty;

        public IReadOnlyCollection<string> AmpAssignTypeList { get; } = new[]
        {
            "FrontA", "FrontB", "Type3", "Type4",
            "Type5", "Type6", "Type7", "Type8",
            "Type9", "Type10", "Type11", "Type12",
            "Type13", "Type14", "Type15", "Type16",
            "Type17", "Type18", "Type19", "Type20",
        };

        public IReadOnlyCollection<string> TargetCurveTypeList { get; } = new[]
        {
            " ", "High Frequency Roll Off 1", "High Frequency Roll Off 2",
        };

        public IReadOnlyCollection<string> MultEQTypeList { get; } = new[]
        {
            "MultEQ", "MultEQXT", "MultEQXT32",
        };

        public ChannelInformationViewModel ChannelInformationViewModel { get; } = new();
        public ChannelReportViewModel ChannelReportViewModel { get; } = new();
        public GraphViewModel GraphViewModel { get; } = new();

        #endregion

        #region Commands

        public ReactiveCommand<Unit, Unit> OpenFile { get; }
        public ReactiveCommand<Unit, Unit> SaveFile { get; }
        public ReactiveCommand<Unit, Unit> SaveFileAs { get; }
        public ReactiveCommand<Unit, Unit> ReloadFile { get; }

        public ReactiveCommand<string[], Unit> DragFiles { get; }

        public ReactiveCommand<TargetCurvePointViewModel, Unit> DeleteTargetCurvePoint { get; }

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
            DragFiles = ReactiveCommand.Create<string[]>(paths =>
            {
                // Assuming you have one file that you care about, pass it off to whatever
                // handling code you have defined.
                if (paths.Any())
                {
                    Open(paths.First());
                }
            });
            DeleteTargetCurvePoint = ReactiveCommand.Create<TargetCurvePointViewModel>(viewModel =>
            {
                CustomTargetCurvePoints.Remove(viewModel);
            });

            _ = this
                .WhenAnyValue(static x => x.SelectedChannel)
                .WhereNotNull()
                .ToPropertyEx(
                    this,
                    static x => x.Channel,
                    new ChannelViewModel());

            _ = this
                .WhenAnyValue(static x => x.SelectedChannel)
                .Select(static value => value != null)
                .ToPropertyEx(
                    this,
                    static x => x.IsChannelSelected,
                    false,
                    false);

            _ = this
                .WhenAnyValue(static x => x.Channel)
                .Subscribe(channel =>
                {
                    CustomTargetCurvePoints = new ObservableCollection<TargetCurvePointViewModel>(channel.Data.CustomTargetCurvePoints
                        .Select(static value => value.Trim('{', '}').Split(','))
                        .Select(static values => new TargetCurvePointViewModel(
                            double.TryParse(values[0].Trim(), out var result1) ? result1 : default,
                            double.TryParse(values[1].Trim(), out var result2) ? result2 : default))
                        .Select(point =>
                        {
                            point.Delete
                                .InvokeCommand(DeleteTargetCurvePoint);

                            return point;
                        })
                        .OrderBy(static x => x.Key));

                    CustomTargetCurvePoints
                        .ToObservableChangeSet()
                        .Subscribe(_ =>
                        {
                            channel.Data.CustomTargetCurvePoints = CustomTargetCurvePoints
                                .Select(static value => $"{{{value.Key}, {value.Value}}}")
                                .ToArray();
                        });
                });

            this.WhenActivated(disposables =>
            {
                _ = this
                    .WhenAnyValue(static x => x.Channel)
                    .ToPropertyEx(
                        ChannelInformationViewModel,
                        static x => x.Channel,
                        new ChannelViewModel())
                    .DisposeWith(disposables);
                _ = this
                    .WhenAnyValue(static x => x.IsChannelSelected)
                    .ToPropertyEx(
                        ChannelInformationViewModel,
                        static x => x.IsChannelSelected)
                    .DisposeWith(disposables);

                _ = this
                    .WhenAnyValue(static x => x.Channel)
                    .ToPropertyEx(
                        ChannelReportViewModel,
                        static x => x.Channel,
                        new ChannelViewModel())
                    .DisposeWith(disposables);
                _ = this
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
                _ = this
                    .WhenAnyValue(static x => x.Channels)
                    .ToPropertyEx(
                        GraphViewModel,
                        static x => x.Channels,
                        Array.Empty<ChannelViewModel>())
                    .DisposeWith(disposables);
                _ = this
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
            Channels = AudysseyApp.DetectedChannels
                .Select(static channel => new ChannelViewModel(channel))
                .ToArray();
            _ = Channels
                .AsObservableChangeSet()
                .WhenPropertyChanged(static x => x.Sticky, false)
                .Subscribe(_ =>
                {
                    GraphViewModel.DrawChart();
                });
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
}
