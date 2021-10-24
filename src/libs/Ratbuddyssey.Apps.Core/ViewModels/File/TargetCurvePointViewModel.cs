namespace Ratbuddyssey.ViewModels;

public class TargetCurvePointViewModel : ReactiveObject
{
    #region Constants

    private const double KeyMin = 10; //10Hz Chris Kyriakakis
    private const double KeyMax = 24000; //24000Hz Chris Kyriakakis
    private const double ValueMin = -20; //-12dB Ratbuddyssey MultiEQ app -> -20dB Chris Kyriakakis
    private const double ValueMax = 12; //12dB Ratbuddyssey MultiEQ app -> +9dB Chris Kyriakakis -> +12 dB in ady afile!

    #endregion

    #region Properties

    [Reactive]
    public double Key { get; set; } = KeyMin;

    [Reactive]
    public double Value { get; set; } = ValueMin;

    [Reactive]
    public bool IsNew { get; set; } = true;

    #region Commands

    public ReactiveCommand<Unit, TargetCurvePointViewModel> Delete { get; }

    #endregion

    #endregion

    #region Constructors

    public TargetCurvePointViewModel()
    {
        Delete = ReactiveCommand.Create(
            () => this,
            this
                .WhenAnyValue(static x => x.IsNew)
                .Select(static value => !value));

        PropertyChanged += (_, args) =>
        {
            switch (args.PropertyName)
            {
                case nameof(Value):
                    {
                        Value = Value switch
                        {
                            > ValueMax => ValueMax,
                            < ValueMin => ValueMin,
                            _ => Value
                        };
                        break;
                    }
                case nameof(Key):
                    {
                        Key = Key switch
                        {
                            > KeyMax => KeyMax,
                            < KeyMin => KeyMin,
                            _ => Key
                        };
                        break;
                    }
            }
        };
    }

    public TargetCurvePointViewModel(double key, double value) : this()
    {
        Key = key;
        Value = value;
    }

    #endregion
}
