namespace BetaSharp.Stats;

public class DistanceStatFormatter : StatFormatter
{
    public string Format(int value)
    {
        double var3 = (double)value / 100.0D;
        double var5 = var3 / 1000.0D;
        return var5 > 0.5D ? StatBase.defaultDecimalFormat().format(var5) + " km" : (var3 > 0.5D ? StatBase.defaultDecimalFormat().format(var3) + " m" : value + " cm");
    }
}