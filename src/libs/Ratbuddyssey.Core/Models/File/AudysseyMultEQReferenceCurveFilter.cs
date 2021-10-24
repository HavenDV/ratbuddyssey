using System.Collections.ObjectModel;
using System.Drawing;
using MathNet.Numerics.Interpolation;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OxyPlot;

namespace Ratbuddyssey;

public class AudysseyMultEQReferenceCurveFilter
{
    #region Properties

    public IReadOnlyCollection<DataPoint> HighFrequencyRollOff1Points { get; private set; } = Array.Empty<DataPoint>();
    public IReadOnlyCollection<DataPoint> HighFrequencyRollOff2Points { get; private set; } = Array.Empty<DataPoint>();

    #endregion

    #region Methods

    public void Load()
    {
        var folder = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly()?.Location) ?? string.Empty;
        var path1 = Path.ChangeExtension(Path.Combine(folder, "high_frequency_roll_off_1"), "json");
        var path2 = Path.ChangeExtension(Path.Combine(folder, "high_frequency_roll_off_2"), "json");

        HighFrequencyRollOff1Points = LoadPoints(path1);
        HighFrequencyRollOff2Points = LoadPoints(path2);
    }

    public IReadOnlyCollection<DataPoint> LoadPoints(string path)
    {
        var points = LoadJsonFile(path);
        if (!points.Any())
        {
            Interactions.Warning.Handle($"Json file reader: {path} missing").Subscribe();

            var pngPath = Path.ChangeExtension(path, "png");
            points = GeneratePointsFromBitmap(pngPath);
            if (!points.Any())
            {
                Interactions.Warning.Handle($"Bitmap file reader: {pngPath} missing").Subscribe();

                return Array.Empty<DataPoint>();
            }
        }

        SaveJsonFile(path, points);

        return points;
    }

    public IReadOnlyCollection<DataPoint> LoadJsonFile(string path)
    {
        if (!File.Exists(path))
        {
            return Array.Empty<DataPoint>();
        }

        var json = File.ReadAllText(path);

        return JsonConvert.DeserializeObject<IReadOnlyCollection<DataPoint>>(json, new JsonSerializerSettings
        {
            FloatParseHandling = FloatParseHandling.Decimal
        }) ?? Array.Empty<DataPoint>();
    }

    public void SaveJsonFile(string filename, IReadOnlyCollection<DataPoint> points)
    {
        var json = JsonConvert.SerializeObject(points, new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        });
        File.WriteAllText(Path.ChangeExtension(filename, "json"), json);
    }

    public IReadOnlyCollection<DataPoint> GeneratePointsFromBitmap(string path)
    {
        if (!File.Exists(path))
        {
            return Array.Empty<DataPoint>();
        }

        const double fmax = 19000.0;
        const double fmin = 20.0;
        const double dbrange = 16.0;
        const double dbmax = 1.0;
        const double sampleRate = 48000.0;
        const int sampleSize = 16384;

        // Create a Bitmap object from an image file.
        var iBitmap = new Bitmap(path);
        var oBitmap = new Bitmap(iBitmap.Width, iBitmap.Height);

        var first = 0;
        //var last = 0;
        double min = iBitmap.Height;
        double max = 0;

        // Get the color of a pixel within myBitmap.
        var points = new Collection<KeyValuePair<int, double>>();
        for (var x = 0; x < iBitmap.Width; x++)
        {
            var list = new List<int>();
            for (var y = 0; y < iBitmap.Height; y++)
            {
                var pixelColor = iBitmap.GetPixel(x, y);
                if ((pixelColor.R > 200) && (pixelColor.G < 25) && (pixelColor.B < 25))
                {
                    if (first == 0)
                    {
                        first = x;
                    }
                    //else last = x;
                    oBitmap.SetPixel(x, y, Color.FromArgb(255, 0, 0));
                    list.Add(y);
                }
            }
            if (list.Count > 0)
            {
                var a = iBitmap.Height - list.Average();
                points.Add(new KeyValuePair<int, double>(x, a));
                if (a < min)
                {
                    min = a;
                }

                if (a > max)
                {
                    max = a;
                }
            }
        }

        //var size = last - first + 1;
        var mag = max - min + 1;

        //calculate gain & offset
        var scale = dbrange / mag;
        var shift = dbmax - max * scale;

        //check
        //var n_min = min * scale + shift;
        //var n_max = max * scale + shift;

        //convert bitmap to frequency spectrum            
        var ex = new List<double>();
        var ey = new List<double>();
        for (var x = 0; x < points.Count; x++)
        {
            var df = Math.Log10(fmax / fmin); //over aproximately 3 decades
            var f = 20.0 * Math.Pow(10.0, df * x / points.Count); //convert log horizontal image pixels to linear frequency scale in Hz
            var y = points[x].Value * scale + shift;//scale and shift vertical image pixels to correct magnitude in dB

            ex.Add(f);
            ey.Add(Math.Pow(10.0, y / 20.0));
        }

        //interpolate frequency spectrum to match Audyssey responseData length
        var frequency = Enumerable.Range(0, sampleSize).Select(p => p * sampleRate / sampleSize).ToArray();
        var spline = CubicSpline.InterpolateAkima(ex, ey);
        var frequencyPoints = new List<DataPoint>();
        foreach (var f in frequency)
        {
            if (f < fmin)
            {   // exptrapolate
                frequencyPoints.Add(new DataPoint(f, double.NegativeInfinity));
            }
            else
            {
                frequencyPoints.Add(f > fmax
                    ? new DataPoint(f, double.NegativeInfinity)
                    : new DataPoint(f, 20 * Math.Log10(spline.Interpolate(f))));
            }
        }

        oBitmap.Save(Path.ChangeExtension(path, "jpg"), System.Drawing.Imaging.ImageFormat.Jpeg);

        return frequencyPoints;
    }

    #endregion
}
