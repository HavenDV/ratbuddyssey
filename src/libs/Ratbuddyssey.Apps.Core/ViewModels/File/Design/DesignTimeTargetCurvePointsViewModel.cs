using System.Collections.ObjectModel;

namespace Ratbuddyssey.ViewModels;

public class DesignTimeTargetCurvePointsViewModel : TargetCurvePointsViewModel
{
    #region Constructors

    public DesignTimeTargetCurvePointsViewModel()
    {
        CustomTargetCurvePoints = new ObservableCollection<TargetCurvePointViewModel>()
        {
            new(0.0, 1.0),
            new(1.0, 2.0),
            new(2.0, 3.0),
        };
    }

    #endregion
}
