using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Audyssey;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using Audyssey.MultEQApp;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Ratbuddyssey.Views
{
    public partial class FileView
    {
        #region Properties

        private AudysseyMultEQReferenceCurveFilter ReferenceCurveFilter { get; } = new();
        public AudysseyMultEQApp AudysseyApp { get; set; }

        private PlotModel PlotModel { get; } = new();

        private List<int> Keys { get; } = new();
        private Dictionary<int, Brush> Colors { get; } = new();

        private double SmoothingFactor { get; set; }

        private List<DetectedChannel> Channels { get; } = new();
        private DetectedChannel SelectedChannel { get; set; }

        private Dictionary<string, AxisLimit> AxisLimits { get; } = new()
        {
            { "rbtnXRangeFull", new AxisLimit { XMin = 10, XMax = 24000, YMin = -35, YMax = 20, YShift = 0, MajorStep = 5, MinorStep = 1 } },
            { "rbtnXRangeSubwoofer", new AxisLimit { XMin = 10, XMax = 1000, YMin = -35, YMax = 20, YShift = 0, MajorStep = 5, MinorStep = 1 } },
            { "rbtnXRangeChirp", new AxisLimit { XMin = 0, XMax = 350, YMin = -0.1, YMax = 0.1, YShift = 0, MajorStep = 0.01, MinorStep = 0.001 } }
        };
        private string SelectedAxisLimits { get; set; } = "rbtnXRangeFull";

        #endregion

        public FileView()
        {
            InitializeComponent();

            channelsView.SelectionChanged += ChannelsView_SelectionChanged;
            plot.PreviewMouseWheel += Plot_PreviewMouseWheel;
        }


        public void HandleDroppedFile(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // Note that you can have more than one file.
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);

                // Assuming you have one file that you care about, pass it off to whatever
                // handling code you have defined.
                if (files != null && files.Length > 0)
                    OpenFile(files[0]);
            }
        }

        public void OpenFile_OnClick(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ".ady",
                Filter = "Audyssey files (*.ady)|*.ady",
            };
            var result = dlg.ShowDialog();

            if (result == true)
            {
                OpenFile(dlg.FileName);
            }
        }

        public void ReloadFile_OnClick(object sender, RoutedEventArgs e)
        {
            var messageBoxResult = MessageBox.Show("This will reload the .ady file and discard all changes since last save", "Are you sure?", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                if (File.Exists(currentFile.Content.ToString()))
                {
                    LoadApp(currentFile.Content.ToString());
                    if ((AudysseyApp != null))
                    {
                        DataContext = AudysseyApp;
                    }
                }
            }
        }

        public void SaveFile_OnClick(object sender, RoutedEventArgs e)
        {
#if DEBUG
            currentFile.Content = Path.ChangeExtension(currentFile.Content.ToString(), ".json");
#endif
            SaveApp(currentFile.Content.ToString());
        }

        public void SaveFileAs_OnClick(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.SaveFileDialog
            {
                FileName = currentFile.Content.ToString(),
                DefaultExt = ".ady",
                Filter = "Audyssey calibration (.ady)|*.ady"
            };
            var result = dlg.ShowDialog();

            if (result == true)
            {
                currentFile.Content = dlg.FileName;
                SaveApp(currentFile.Content.ToString());
            }
        }

        private void OpenFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return;
            }

            currentFile.Content = filePath;
            LoadApp(currentFile.Content.ToString());
            if ((AudysseyApp != null))
            {
                DataContext = AudysseyApp;
            }
        }

        private void LoadApp(string fileName)
        {
            if (File.Exists(fileName))
            {
                var json = File.ReadAllText(fileName);
                AudysseyApp = JsonConvert.DeserializeObject<AudysseyMultEQApp>(json, new JsonSerializerSettings
                {
                    FloatParseHandling = FloatParseHandling.Decimal
                });
            }
        }

        private void SaveApp(string fileName)
        {
            if (AudysseyApp != null)
            {
                var json = JsonConvert.SerializeObject(AudysseyApp, new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });
                if ((!string.IsNullOrEmpty(fileName)))
                {
                    File.WriteAllText(fileName, json);
                }
            }
        }

        private void DrawChart()
        {
            if (plot != null)
            {
                ClearPlot();
                if (SelectedChannel != null)
                {
                    PlotLine(SelectedChannel);
                }
                if (Channels != null)
                {
                    foreach (var channel in Channels)
                    {
                        if (channel.Sticky)
                        {
                            PlotLine(channel, true);
                        }
                    }
                }
                if (AudysseyApp != null)
                {
                    switch (AudysseyApp.EnTargetCurveType)
                    {
                        case 0:
                            break;
                        case 1:
                            PlotLine(null, false);
                            break;
                        case 2:
                            PlotLine(null, true);
                            break;
                        default:
                            PlotLine(null, false);
                            PlotLine(null, true);
                            break;

                    }
                }
                PlotAxis();
                PlotChart();
            }
        }
        private void ClearPlot()
        {
            if (plot.Model != null && plot.Model.Series != null)
            {
                plot.Model.Series.Clear();
                plot.Model = null;
            }
        }
        private void PlotChart()
        {
            plot.Model = PlotModel;
        }
        private void PlotAxis()
        {
            PlotModel.Axes.Clear();
            var limits = AxisLimits[SelectedAxisLimits];
            if (SelectedAxisLimits == "rbtnXRangeChirp")
            {
                if (chbxLogarithmicAxis != null)
                {
                    if (chbxLogarithmicAxis.IsChecked == true)
                    {
                        PlotModel.Axes.Add(new LogarithmicAxis { Position = AxisPosition.Bottom, Title = "ms", Minimum = limits.XMin, Maximum = limits.XMax, MajorGridlineStyle = LineStyle.Dot });
                    }
                    else
                    {
                        PlotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Title = "ms", Minimum = limits.XMin, Maximum = limits.XMax, MajorGridlineStyle = LineStyle.Dot });
                    }
                }
                PlotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = "", Minimum = limits.YMin, Maximum = limits.YMax, MajorStep = limits.MajorStep, MinorStep = limits.MinorStep, MajorGridlineStyle = LineStyle.Solid });
            }
            else
            {
                if (chbxLogarithmicAxis != null)
                {
                    if (chbxLogarithmicAxis.IsChecked == true)
                    {
                        PlotModel.Axes.Add(new LogarithmicAxis { Position = AxisPosition.Bottom, Title = "Hz", Minimum = limits.XMin, Maximum = limits.XMax, MajorGridlineStyle = LineStyle.Dot });
                    }
                    else
                    {
                        PlotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Title = "Hz", Minimum = limits.XMin, Maximum = limits.XMax, MajorGridlineStyle = LineStyle.Dot });
                    }
                }
                PlotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = "dB", Minimum = limits.YMin + limits.YShift, Maximum = limits.YMax + limits.YShift, MajorStep = limits.MajorStep, MinorStep = limits.MinorStep, MajorGridlineStyle = LineStyle.Solid });
            }
        }
        private void PlotLine(DetectedChannel selectedChannel, bool secondaryChannel = false)
        {
            if (selectedChannel == null)
            {
                //time domain data
                if (SelectedAxisLimits == "rbtnXRangeChirp")
                {
                }
                //frequency domain data
                else
                {
                    Collection<DataPoint> points = null;
                    if (secondaryChannel)
                    {
                        points = ReferenceCurveFilter.High_Frequency_Roll_Off_2();
                    }
                    else
                    {
                        points = ReferenceCurveFilter.High_Frequency_Roll_Off_1();
                    }

                    if (points != null)
                    {
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
                        PlotModel.Series.Add(lineserie);
                    }
                }
            }
            else
            {
                for (var i = 0; i < Keys.Count; i++)
                {
                    var points = new Collection<DataPoint>();

                    var s = Keys[i].ToString();
                    if (!selectedChannel.ResponseData.ContainsKey(s))
                        continue;

                    var values = selectedChannel.ResponseData[s];
                    var count = values.Length;
                    var cValues = new Complex[count];
                    var xs = new double[count];

                    float sample_rate = 48000;
                    var totalTime = count / sample_rate;

                    var limits = AxisLimits[SelectedAxisLimits];
                    if (SelectedAxisLimits == "rbtnXRangeChirp")
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
                            xs[j] = (double)j / count * sample_rate;
                        }

                        MathNet.Numerics.IntegralTransforms.Fourier.Forward(cValues);

                        var x = 0;
                        if (radioButtonSmoothingFactorNone.IsChecked.Value)
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

                            LinSpacedFracOctaveSmooth(SmoothingFactor, ref smoothed, 1, 1d / 48);

                            foreach (var smoothetResult in smoothed)
                            {
                                points.Add(new DataPoint(xs[x++], limits.YShift + 20 * Math.Log10(smoothetResult)));
                                if (x == count / 2) break;
                            }
                        }
                    }

                    var color = OxyColor.Parse(Colors[Keys[i]].ToString());
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

                    PlotModel.Series.Add(lineserie);
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
            var N = smoothed.Length;
            double xp;
            double yp;
            // Smooth from HF to LF to avoid artificial elevation of HF data
            for (var pass = 0; pass < passes; pass++)
            {
                xp = smoothed[N - 1];
                yp = xp;
                // reverse pass
                for (var i = N - 2; i >= 0; i--)
                {
                    var a = 1 / (b + i * bwFactor);
                    yp += ((xp + smoothed[i]) / 2 - yp) * a;
                    xp = smoothed[i];
                    smoothed[i] = (float)yp;
                }
                // forward pass
                for (var i = 1; i < N; i++)
                {
                    var a = 1 / (b + i * bwFactor);
                    yp += ((xp + smoothed[i]) / 2 - yp) * a;
                    xp = smoothed[i];
                    smoothed[i] = (float)yp;
                }
            }
        }
        private void Plot_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true;
        }
        private void CheckBoxMeasurementPositionUnchecked(object sender, RoutedEventArgs e)
        {
            var checkBox = sender as CheckBox;
            var val = int.Parse(checkBox.Content.ToString()) - 1;
            if (Keys.Contains(val))
            {
                Keys.Remove(val);
                Colors.Remove(val);
                DrawChart();
            }
        }
        private void CheckBoxMeasurementPositionChecked(object sender, RoutedEventArgs e)
        {
            var checkBox = sender as CheckBox;
            var val = int.Parse(checkBox.Content.ToString()) - 1;
            if (SelectedChannel != null && !SelectedChannel.ResponseData.ContainsKey(val.ToString()))
            {
                // This channel has not been measured in this Audyssey calibration. Don't attempt to plot it, and clear the checkbox.
                checkBox.IsChecked = false;
            }
            else if (!Keys.Contains(val))
            {
                Keys.Add(val);
                Colors.Add(val, checkBox.Foreground);
                DrawChart();
            }
        }
        private void AllCheckBoxMeasurementPositionChecked(object sender, RoutedEventArgs e)
        {
            chbx1.IsChecked = true;
            chbx2.IsChecked = true;
            chbx3.IsChecked = true;
            chbx4.IsChecked = true;
            chbx5.IsChecked = true;
            chbx6.IsChecked = true;
            chbx7.IsChecked = true;
            chbx8.IsChecked = true;
            DrawChart();
        }
        private void AllCheckBoxMeasurementPositionUnchecked(object sender, RoutedEventArgs e)
        {
            chbx1.IsChecked = false;
            chbx2.IsChecked = false;
            chbx3.IsChecked = false;
            chbx4.IsChecked = false;
            chbx5.IsChecked = false;
            chbx6.IsChecked = false;
            chbx7.IsChecked = false;
            chbx8.IsChecked = false;
            DrawChart();
        }
        private void ChannelsView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var checkBoxes = new List<CheckBox> {
                chbx1, chbx2, chbx3, chbx4, chbx5, chbx6, chbx7, chbx8
            };

            // Disable all the check boxes
            foreach (var checkBox in checkBoxes)
            {
                checkBox.IsEnabled = false;
            }

            var selectedValue = channelsView.SelectedValue as DetectedChannel;
            if (selectedValue != null && selectedValue.ResponseData != null)
            {
                // Enable the check boxes corresponding to those positions for which the measurement is available
                foreach (var measurementPosition in selectedValue.ResponseData)
                {
                    var positionIndex = int.Parse(measurementPosition.Key);
                    Debug.Assert(positionIndex >= 0 && positionIndex < checkBoxes.Count);
                    checkBoxes[positionIndex].IsEnabled = true;
                }

                if (selectedValue.ResponseData.Count > 0)
                {
                    SelectedChannel = (DetectedChannel)channelsView.SelectedValue;
                    DrawChart();
                }
            }

            // Un-check all the disabled check boxes
            foreach (var checkBox in checkBoxes)
            {
                if (!checkBox.IsEnabled && checkBox.IsChecked == true)
                    checkBox.IsChecked = false;
            }
        }
        private void ChannelsView_OnClickSticky(object sender, RoutedEventArgs e)
        {
            foreach (var channel in AudysseyApp.DetectedChannels)
            {
                if (channel.Sticky)
                {
                    Channels.Add(channel);
                    DrawChart();
                }
                else if (Channels.Contains(channel))
                {
                    Channels.Remove(channel);
                    DrawChart();
                }
            }
        }
        private void ButtonClickAddTargetCurvePoint(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(keyTbx.Text) && !string.IsNullOrEmpty(valueTbx.Text))
            {
                ((DetectedChannel)channelsView.SelectedValue).CustomTargetCurvePointsDictionary.Add(new MyKeyValuePair(keyTbx.Text, valueTbx.Text));
            }
        }
        private void ButtonClickRemoveTargetCurvePoint(object sender, RoutedEventArgs e)
        {
            var b = sender as Button;
            var pair = b.DataContext as MyKeyValuePair;
            ((DetectedChannel)channelsView.SelectedValue).CustomTargetCurvePointsDictionary.Remove(pair);
        }
        private void RadioButtonSmoothingFactorChecked(object sender, RoutedEventArgs e)
        {
            var radioButton = sender as RadioButton;
            switch (radioButton.Name)
            {
                case "radioButtonSmoothingFactorNone":
                    SmoothingFactor = 1;
                    break;
                case "radioButtonSmoothingFactor2":
                    SmoothingFactor = 2;
                    break;
                case "radioButtonSmoothingFactor3":
                    SmoothingFactor = 3;
                    break;
                case "radioButtonSmoothingFactor6":
                    SmoothingFactor = 6;
                    break;
                case "radioButtonSmoothingFactor12":
                    SmoothingFactor = 12;
                    break;
                case "radioButtonSmoothingFactor24":
                    SmoothingFactor = 24;
                    break;
                case "radioButtonSmoothingFactor48":
                    SmoothingFactor = 48;
                    break;
                default:
                    break;
            }
            DrawChart();
        }
        private void rbtnXRange_Checked(object sender, RoutedEventArgs e)
        {
            var radioButton = sender as RadioButton;
            SelectedAxisLimits = radioButton.Name;
            DrawChart();
        }
        private void chbxStickSubwoofer_Checked(object sender, RoutedEventArgs e)
        {
            DrawChart();
        }
        private void chbxLogarithmicAxis_Checked(object sender, RoutedEventArgs e)
        {
            DrawChart();
        }
        private void chbxLogarithmicAxis_Unchecked(object sender, RoutedEventArgs e)
        {
            DrawChart();
        }
        private void TargetCurveTypeSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DrawChart();
        }
    }

    class AxisLimit
    {
        public double XMin { get; set; }
        public double XMax { get; set; }
        public double YMin { get; set; }
        public double YMax { get; set; }
        public double YShift { get; set; }
        public double MajorStep { get; set; }
        public double MinorStep { get; set; }
    }
}
