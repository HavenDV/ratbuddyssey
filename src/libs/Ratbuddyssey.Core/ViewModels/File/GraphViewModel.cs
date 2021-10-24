using System.Collections.ObjectModel;
using System.Drawing;
using System.Globalization;
using System.Numerics;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using Ratbuddyssey.Models;
using Ratbuddyssey.MultEQApp;

// ReSharper disable UnassignedGetOnlyAutoProperty

namespace Ratbuddyssey.ViewModels;

public class GraphViewModel : ActivatableViewModel
{
    #region Properties

    private AudysseyMultEQReferenceCurveFilter ReferenceCurveFilter { get; } = new();

    private static Dictionary<FrequencyRange, AxisLimit> AxisLimits { get; } = new()
    {
        {
            FrequencyRange.Full,
            new AxisLimit
            {
                XMin = 10,
                XMax = 24000,
                YMin = -35,
                YMax = 20,
                YShift = 0,
                MajorStep = 5,
                MinorStep = 1,
            }
        },
        {
            FrequencyRange.Subwoofer,
            new AxisLimit
            {
                XMin = 10,
                XMax = 1000,
                YMin = -35,
                YMax = 20,
                YShift = 0,
                MajorStep = 5,
                MinorStep = 1,
            }
        },
        {
            FrequencyRange.Chirp,
            new AxisLimit
            {
                XMin = 0,
                XMax = 350,
                YMin = -0.1,
                YMax = 0.1,
                YShift = 0,
                MajorStep = 0.01,
                MinorStep = 0.001,
            }
        }
    };

    #region Public

    [ObservableAsProperty]
    public int EnTargetCurveType { get; }

    [ObservableAsProperty]
    public IReadOnlyCollection<ChannelViewModel> Channels { get; } = Array.Empty<ChannelViewModel>();

    [ObservableAsProperty]
    public ChannelViewModel? SelectedChannel { get; }

    [Reactive]
    public IReadOnlyCollection<MeasurementPositionViewModel> MeasurementPositions { get; set; } = new MeasurementPositionViewModel[]
    {
            new(1, Color.Black, true),
            new(2, Color.Blue),
            new(3, Color.Violet),
            new(4, Color.Green),
            new(5, Color.Orange),
            new(6, Color.Red),
            new(7, Color.Cyan),
            new(8, Color.DeepPink),
    };

    [Reactive]
    public bool SelectAllMeasurementPositionsIsChecked { get; set; }

    [Reactive]
    public PlotModel PlotModel { get; set; } = new();

    public IReadOnlyCollection<SmoothingFactorViewModel> SmoothingFactors { get; set; } = new SmoothingFactorViewModel[]
    {
            new("No Smoothing", 1.0, true),
            new("1/2", 2.0),
            new("1/3", 3.0),
            new("1/6", 6.0),
            new("1/12", 12.0),
            new("1/24", 24.0),
            new("1/48", 48.0),
    };

    public SmoothingFactorViewModel SelectedSmoothingFactor => SmoothingFactors.First(static value => value.IsChecked);

    public IReadOnlyCollection<RangeViewModel> Ranges { get; set; } = new RangeViewModel[]
    {
            new("0-350ms", FrequencyRange.Chirp),
            new("10-1000Hz", FrequencyRange.Subwoofer),
            new("10Hz-24kHz", FrequencyRange.Full, true),
    };

    public RangeViewModel SelectedRange => Ranges.First(static value => value.IsChecked);

    [Reactive]
    public bool LogarithmicAxisIsChecked { get; set; } = true;

    #endregion

    #endregion

    #region Constructors

    public GraphViewModel()
    {
        ReferenceCurveFilter.Load();

        this.WhenActivated(disposables =>
        {
            _ = MeasurementPositions
                .AsObservableChangeSet()
                .WhenPropertyChanged(static x => x.IsChecked, false)
                .Throttle(TimeSpan.FromMilliseconds(10), RxApp.MainThreadScheduler)
                .Subscribe(_ =>
                {
                    DrawChart();
                })
                .DisposeWith(disposables);
            _ = SmoothingFactors
                .AsObservableChangeSet()
                .WhenPropertyChanged(static x => x.IsChecked, false)
                .Where(static value => !value.Value)
                .Subscribe(_ =>
                {
                    DrawChart();
                })
                .DisposeWith(disposables);
            _ = Ranges
                .AsObservableChangeSet()
                .WhenPropertyChanged(static x => x.IsChecked, false)
                .Where(static value => !value.Value)
                .Subscribe(_ =>
                {
                    DrawChart();
                })
                .DisposeWith(disposables);

            _ = this
                .WhenAnyValue(static x => x.SelectAllMeasurementPositionsIsChecked)
                .Skip(1)
                .Subscribe(isChecked =>
                {
                    foreach (var position in MeasurementPositions)
                    {
                        position.IsChecked = isChecked;
                    }

                    DrawChart();
                })
                .DisposeWith(disposables);

            _ = this
                .WhenAnyValue(static x => x.EnTargetCurveType)
                .Skip(1)
                .Subscribe(_ =>
                {
                    DrawChart();
                })
                .DisposeWith(disposables);

            _ = this
                .WhenAnyValue(static x => x.LogarithmicAxisIsChecked)
                .Skip(1)
                .Subscribe(_ =>
                {
                    DrawChart();
                })
                .DisposeWith(disposables);

            _ = this
                .WhenAnyValue(static x => x.SelectedChannel)
                .Subscribe(channel =>
                {
                    var data = channel?.Data.ResponseData;
                    foreach (var position in MeasurementPositions)
                    {
                        position.IsEnabled = data != null && data.ContainsKey($"{position.Value - 1}");
                    }

                    DrawChart();
                })
                .DisposeWith(disposables);

            _ = Channels
                .AsObservableChangeSet()
                .WhenPropertyChanged(static x => x.Sticky, false)
                .Subscribe(_ =>
                {
                    DrawChart();
                })
                .DisposeWith(disposables);
        });
    }

    #endregion

    #region Methods

    private void DrawChart()
    {
        PlotModel = DrawChart(
            ReferenceCurveFilter,
            MeasurementPositions,
            Channels,
            SelectedChannel?.Data,
            SelectedRange.Value,
            SelectedSmoothingFactor.Value,
            LogarithmicAxisIsChecked,
            EnTargetCurveType);
    }

    private static PlotModel DrawChart(
        AudysseyMultEQReferenceCurveFilter filter,
        IReadOnlyCollection<MeasurementPositionViewModel> positions,
        IReadOnlyCollection<ChannelViewModel> channels,
        DetectedChannel? selectedChannel,
        FrequencyRange selectedRange,
        double selectedFactor,
        bool logarithmicAxisIsChecked,
        int enTargetCurveType)
    {
        var model = new PlotModel();

        if (selectedChannel != null)
        {
            PlotLine(model, filter, positions, selectedChannel, selectedRange, selectedFactor);
        }
        foreach (var channel in channels.Where(static channel => channel.Sticky))
        {
            PlotLine(model, filter, positions, channel.Data, selectedRange, selectedFactor, true);
        }
        switch (enTargetCurveType)
        {
            case 0:
                break;
            case 1:
                PlotLine(model, filter, positions, null, selectedRange, selectedFactor);
                break;
            case 2:
                PlotLine(model, filter, positions, null, selectedRange, selectedFactor, true);
                break;
            default:
                PlotLine(model, filter, positions, null, selectedRange, selectedFactor);
                PlotLine(model, filter, positions, null, selectedRange, selectedFactor, true);
                break;

        }
        PlotAxis(model, selectedRange, logarithmicAxisIsChecked);

        return model;
    }

    private static void PlotAxis(
        PlotModel model,
        FrequencyRange selectedRange,
        bool logarithmicAxisIsChecked)
    {
        model.Axes.Clear();

        var limits = AxisLimits[selectedRange];

        Axis xAxis = logarithmicAxisIsChecked
            ? new LogarithmicAxis()
            : new LinearAxis();
        xAxis.Position = AxisPosition.Bottom;
        xAxis.Title = selectedRange switch
        {
            FrequencyRange.Chirp => "ms",
            _ => "Hz",
        };
        xAxis.Minimum = limits.XMin;
        xAxis.Maximum = limits.XMax;
        xAxis.MajorGridlineStyle = LineStyle.Dot;

        model.Axes.Add(xAxis);

        model.Axes.Add(new LinearAxis
        {
            Position = AxisPosition.Left,
            Title = selectedRange switch
            {
                FrequencyRange.Chirp => string.Empty,
                _ => "dB",
            },
            Minimum = selectedRange switch
            {
                FrequencyRange.Chirp => limits.YMin,
                _ => limits.YMin + limits.YShift,
            },
            Maximum = selectedRange switch
            {
                FrequencyRange.Chirp => limits.YMax,
                _ => limits.YMax + limits.YShift,
            },
            MajorStep = limits.MajorStep,
            MinorStep = limits.MinorStep,
            MajorGridlineStyle = LineStyle.Solid,
        });
    }

    private static void PlotLine(
        PlotModel model,
        AudysseyMultEQReferenceCurveFilter filter,
        IReadOnlyCollection<MeasurementPositionViewModel> positions,
        DetectedChannel? selectedChannel,
        FrequencyRange selectedRange,
        double selectedFactor,
        bool secondaryChannel = false)
    {
        if (selectedChannel == null)
        {
            //time domain data
            if (selectedRange == FrequencyRange.Chirp)
            {
            }
            //frequency domain data
            else
            {
                model.Series.Add(new LineSeries
                {
                    ItemsSource = secondaryChannel
                        ? filter.HighFrequencyRollOff2Points
                        : filter.HighFrequencyRollOff1Points,
                    DataFieldX = "X",
                    DataFieldY = "Y",
                    StrokeThickness = 2,
                    MarkerSize = 0,
                    LineStyle = LineStyle.Solid,
                    Color = OxyColor.FromRgb(255, 0, 0),
                    MarkerType = MarkerType.None,
                });
            }
        }
        else
        {
            foreach (var position in positions
                .Where(static x => x.IsEnabled && x.IsChecked))
            {
                if (!selectedChannel.ResponseData.TryGetValue(
                    $"{position.Value - 1}", out var values))
                {
                    position.IsChecked = false;
                    continue;
                }

                var points = new Collection<DataPoint>();
                var count = values.Length;
                var cValues = new Complex[count];
                var xs = new double[count];
                const float sampleRate = 48000;
                var totalTime = count / sampleRate;

                var limits = AxisLimits[selectedRange];
                if (selectedRange == FrequencyRange.Chirp)
                {
                    limits.XMax = 1000 * totalTime; // horizotal scale: s to ms
                    for (var j = 0; j < count; j++)
                    {
                        var d = double.Parse(values[j], NumberStyles.AllowExponent | NumberStyles.Float, CultureInfo.InvariantCulture);

                        points.Add(new DataPoint(
                            1000 * j * totalTime / count,
                            d));
                    }
                }
                else
                {
                    for (var j = 0; j < count; j++)
                    {
                        var d = decimal.Parse(values[j], NumberStyles.AllowExponent | NumberStyles.Float, CultureInfo.InvariantCulture);
                        var cValue = (Complex)d;
                        cValues[j] = 100 * cValue;
                        xs[j] = (double)j / count * sampleRate;
                    }

                    MathNet.Numerics.IntegralTransforms.Fourier.Forward(cValues);

                    var x = 0;
                    if (selectedFactor < 2.0)
                    {
                        foreach (var cValue in cValues)
                        {
                            points.Add(new DataPoint(xs[x++], limits.YShift + 20 * Math.Log10(cValue.Magnitude)));
                            if (x == count / 2) break;
                        }
                    }
                    else
                    {
                        var smoothed = new double[count];
                        for (var j = 0; j < count; j++)
                        {
                            smoothed[j] = cValues[j].Magnitude;
                        }

                        LinSpacedFracOctaveSmooth(selectedFactor, ref smoothed, 1, 1d / 48);

                        foreach (var smoothetResult in smoothed)
                        {
                            points.Add(new DataPoint(
                                xs[x++],
                                limits.YShift + 20 * Math.Log10(smoothetResult)));

                            if (x == count / 2)
                            {
                                break;
                            }
                        }
                    }
                }

                model.Series.Add(new LineSeries
                {
                    ItemsSource = points,
                    DataFieldX = "X",
                    DataFieldY = "Y",
                    StrokeThickness = 1,
                    MarkerSize = 0,
                    LineStyle = secondaryChannel ? LineStyle.Dot : LineStyle.Solid,
                    Color = OxyColor.FromArgb(
                        position.Color.A,
                        position.Color.R,
                        position.Color.G,
                        position.Color.B),
                    MarkerType = MarkerType.None,
                });
            }
        }
    }

    private static void LinSpacedFracOctaveSmooth(double frac, ref double[] smoothed, float startFreq, double freqStep)
    {
        var passes = 8;
        // Scale octave frac to allow for number of passes
        var scaledFrac = 7.5 * frac; //Empirical tweak to better match Gaussian smoothing
        var octMult = Math.Pow(2, 0.5 / scaledFrac);
        var bwFactor = (octMult - 1 / octMult);
        var b = 0.5 + bwFactor * startFreq / freqStep;
        var n = smoothed.Length;
        double xp;
        double yp;
        // Smooth from HF to LF to avoid artificial elevation of HF data
        for (var pass = 0; pass < passes; pass++)
        {
            xp = smoothed[n - 1];
            yp = xp;
            // reverse pass
            for (var i = n - 2; i >= 0; i--)
            {
                var a = 1 / (b + i * bwFactor);
                yp += ((xp + smoothed[i]) / 2 - yp) * a;
                xp = smoothed[i];
                smoothed[i] = (float)yp;
            }
            // forward pass
            for (var i = 1; i < n; i++)
            {
                var a = 1 / (b + i * bwFactor);
                yp += ((xp + smoothed[i]) / 2 - yp) * a;
                xp = smoothed[i];
                smoothed[i] = (float)yp;
            }
        }
    }

    #endregion
}
