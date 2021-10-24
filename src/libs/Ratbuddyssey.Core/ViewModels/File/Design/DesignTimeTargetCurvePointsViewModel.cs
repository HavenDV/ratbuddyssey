namespace Ratbuddyssey.ViewModels;

public class DesignTimeTargetCurvePointsViewModel
{
    #region Properties

    public List<TargetCurvePointViewModel> CustomTargetCurvePoints { get; set; } = new List<TargetCurvePointViewModel>()
        {
            new(0.0, 1.0),
            new(1.0, 2.0),
            new(2.0, 3.0),
        };

    #endregion
}
