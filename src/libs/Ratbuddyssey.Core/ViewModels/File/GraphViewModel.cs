using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using Ratbuddyssey.Models;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using Ratbuddyssey.MultEQApp;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

// ReSharper disable UnassignedGetOnlyAutoProperty

namespace Ratbuddyssey.ViewModels
{
    public class GraphViewModel : ActivatableViewModel
    {
        #region Properties

        private AudysseyMultEQReferenceCurveFilter ReferenceCurveFilter { get; } = new();

        private Dictionary<FrequencyRange, AxisLimit> AxisLimits { get; } = new()
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
            var model = new PlotModel();

            if (SelectedChannel != null)
            {
                PlotLine(model, SelectedChannel?.Data);
            }
            foreach (var channel in Channels.Where(static channel => channel.Sticky))
            {
                PlotLine(model, channel.Data, true);
            }
            switch (EnTargetCurveType)
            {
                case 0:
                    break;
                case 1:
                    PlotLine(model, null);
                    break;
                case 2:
                    PlotLine(model, null, true);
                    break;
                default:
                    PlotLine(model, null);
                    PlotLine(model, null, true);
                    break;

            }
            PlotAxis(model);

            PlotModel = model;
        }

        private void PlotAxis(PlotModel model)
        {
            model.Axes.Clear();
            var selectedRange = SelectedRange.Value;
            var limits = AxisLimits[selectedRange];
            if (selectedRange == FrequencyRange.Chirp)
            {
                if (LogarithmicAxisIsChecked)
                {
                    model.Axes.Add(new LogarithmicAxis { Position = AxisPosition.Bottom, Title = "ms", Minimum = limits.XMin, Maximum = limits.XMax, MajorGridlineStyle = LineStyle.Dot });
                }
                else
                {
                    model.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Title = "ms", Minimum = limits.XMin, Maximum = limits.XMax, MajorGridlineStyle = LineStyle.Dot });
                }
                model.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = "", Minimum = limits.YMin, Maximum = limits.YMax, MajorStep = limits.MajorStep, MinorStep = limits.MinorStep, MajorGridlineStyle = LineStyle.Solid });
            }
            else
            {
                if (LogarithmicAxisIsChecked)
                {
                    model.Axes.Add(new LogarithmicAxis { Position = AxisPosition.Bottom, Title = "Hz", Minimum = limits.XMin, Maximum = limits.XMax, MajorGridlineStyle = LineStyle.Dot });
                }
                else
                {
                    model.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Title = "Hz", Minimum = limits.XMin, Maximum = limits.XMax, MajorGridlineStyle = LineStyle.Dot });
                }
                model.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = "dB", Minimum = limits.YMin + limits.YShift, Maximum = limits.YMax + limits.YShift, MajorStep = limits.MajorStep, MinorStep = limits.MinorStep, MajorGridlineStyle = LineStyle.Solid });
            }
        }

        private void PlotLine(PlotModel model, DetectedChannel? selectedChannel, bool secondaryChannel = false)
        {
            var selectedRange = SelectedRange.Value;
            if (selectedChannel == null)
            {
                //time domain data
                if (selectedRange == FrequencyRange.Chirp)
                {
                }
                //frequency domain data
                else
                {
                    var points = secondaryChannel
                        ? ReferenceCurveFilter.HighFrequencyRollOff2Points
                        : ReferenceCurveFilter.HighFrequencyRollOff1Points;
                    if (!points.Any())
                    {
                        return;
                    }

                    var color = OxyColor.FromRgb(255, 0, 0);
                    var lineserie = new LineSeries
                    {
                        ItemsSource = points,
                        DataFieldX = "X",
                        DataFieldY = "Y",
                        StrokeThickness = 2,
                        MarkerSize = 0,
                        LineStyle = LineStyle.Solid,
                        Color = color,
                        MarkerType = MarkerType.None,
                    };
                    model.Series.Add(lineserie);
                }
            }
            else
            {
                foreach (var position in MeasurementPositions
                    .Where(static x => x.IsEnabled && x.IsChecked))
                {
                    if (!selectedChannel.ResponseData.TryGetValue($"{position.Value - 1}", out var values))
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
                            var d = Double.Parse(values[j], NumberStyles.AllowExponent | NumberStyles.Float, CultureInfo.InvariantCulture);
                            points.Add(new DataPoint(1000 * j * totalTime / count, d));
                        }
                    }
                    else
                    {
                        for (var j = 0; j < count; j++)
                        {
                            var d = Decimal.Parse(values[j], NumberStyles.AllowExponent | NumberStyles.Float, CultureInfo.InvariantCulture);
                            var cValue = (Complex)d;
                            cValues[j] = 100 * cValue;
                            xs[j] = (double)j / count * sampleRate;
                        }

                        MathNet.Numerics.IntegralTransforms.Fourier.Forward(cValues);

                        var x = 0;
                        var factor = SelectedSmoothingFactor.Value;
                        if (factor < 2.0)
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

                            LinSpacedFracOctaveSmooth(factor, ref smoothed, 1, 1d / 48);

                            foreach (var smoothetResult in smoothed)
                            {
                                points.Add(new DataPoint(xs[x++], limits.YShift + 20 * Math.Log10(smoothetResult)));
                                if (x == count / 2) break;
                            }
                        }
                    }

                    var color = OxyColor.FromArgb(
                        position.Color.A,
                        position.Color.R,
                        position.Color.G,
                        position.Color.B);
                    var lineserie = new LineSeries
                    {
                        ItemsSource = points,
                        DataFieldX = "X",
                        DataFieldY = "Y",
                        StrokeThickness = 1,
                        MarkerSize = 0,
                        LineStyle = secondaryChannel ? LineStyle.Dot : LineStyle.Solid,
                        Color = color,
                        MarkerType = MarkerType.None,
                    };

                    model.Series.Add(lineserie);
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
}
