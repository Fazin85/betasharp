namespace BetaSharp.Stats;

public class IntegerStatFormatter : StatFormatter
{
    public string Format(int value)
    {
        return StatBase.defaultNumberFormat().format(value);
    }
}