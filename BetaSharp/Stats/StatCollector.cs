namespace BetaSharp.Stats;

public class StatCollector
{
    private static readonly TranslationStorage localizedName = TranslationStorage.Instance;

    public static string translateToLocal(string var0)
    {
        return localizedName.TranslateKey(var0);
    }

    public static string translateToLocalFormatted(string var0, params object[] var1)
    {
        return localizedName.TranslateKeyFormat(var0, var1);
    }
}