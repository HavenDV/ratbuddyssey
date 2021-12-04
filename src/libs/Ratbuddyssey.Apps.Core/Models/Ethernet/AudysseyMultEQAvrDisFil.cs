using System.Collections.ObjectModel;
using System.ComponentModel;
using Newtonsoft.Json;

#nullable disable
#pragma warning disable

namespace Ratbuddyssey.MultEQAvr;

internal interface IDis
{
    #region Properties
    string EqType { get; set; }
    string ChData { get; set; }
    #endregion
}

internal interface IFil
{
    #region Properties
    ObservableCollection<sbyte> FilData { get; set; }
    ObservableCollection<sbyte> DispData { get; set; }
    #endregion
}

public class AvrDisFil : IDis, IFil, INotifyPropertyChanged
{
    private string _EqType = null;
    private string _ChData = null;
    private ObservableCollection<sbyte> _FilData = new();
    private ObservableCollection<sbyte> _DispData = new();

    #region Properties
    public string EqType
    {
        get
        {
            return _EqType;
        }
        set
        {
            _EqType = value;
            RaisePropertyChanged("EqType");
        }
    }
    public string ChData
    {
        get
        {
            return _ChData;
        }
        set
        {
            _ChData = value;
            RaisePropertyChanged("ChData");
        }
    }
    public ObservableCollection<sbyte> FilData
    {
        get
        {
            return _FilData;
        }
        set
        {
            _FilData = value;
            RaisePropertyChanged("FilData");
        }
    }
    public ObservableCollection<sbyte> DispData
    {
        get
        {
            return _DispData;
        }
        set
        {
            _DispData = value;
            RaisePropertyChanged("DispData");
        }
    }
    #endregion

    #region INotifyPropertyChanged implementation
    public event PropertyChangedEventHandler PropertyChanged = delegate { };
    #endregion

    #region methods
    private void RaisePropertyChanged(string propertyName)
    {
        if (PropertyChanged != null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    #endregion
}

public partial class AudysseyMultEQAvr : INotifyPropertyChanged
{
    private ObservableCollection<AvrDisFil> _AvrDisFil = new();
    private ObservableCollection<int[]> _AvrCoefData = new();

    #region Properties
    public ObservableCollection<AvrDisFil> DisFil
    {
        get
        {
            return _AvrDisFil;
        }
        set
        {
            _AvrDisFil = value;
            RaisePropertyChanged("DisFil");
        }
    }
    [JsonIgnore]
    public AvrDisFil SelectedDisFil
    {
        get
        {
            if (_SelectedChannel != null)
            {
                foreach (var avrDisFil in _AvrDisFil)
                {
                    if ((avrDisFil.ChData.Equals(_SelectedChannel)) &&
                        (avrDisFil.EqType.Equals(_AudyEqSet)))
                    {
                        return avrDisFil;
                    }
                }
            }
            return null;
        }
        set
        {
        }
    }
    public ObservableCollection<int[]> CoefData
    {
        get
        {
            return _AvrCoefData;
        }
        set
        {
            _AvrCoefData = value;
            RaisePropertyChanged("CoefData");
        }
    }
    [JsonIgnore]
    public int[] SelectedCoefData
    {
        get
        {
            if (_SelectedChannel != null)
            {
                foreach (var avrDisFil in _AvrDisFil)
                {
                    if ((avrDisFil.ChData.Equals(_SelectedChannel)) &&
                        (avrDisFil.EqType.Equals(_AudyEqSet)))
                    {
                        return CoefData[_AvrDisFil.IndexOf(avrDisFil)];
                    }
                }
            }
            return null;
        }
        set
        {
        }
    }
    #endregion
}
