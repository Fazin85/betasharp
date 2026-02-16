namespace BetaSharp.Stats;

public class StatCollector
{
    private static readonly TranslationStorage localizedName = TranslationStorage.getInstance();

    public static string translateToLocal(string var0)
    {
        return localizedName.translateKey(var0);
    }

    public static string translateToLocalFormatted(string var0, params object[] var1)
    {
        return localizedName.translateKeyFormat(var0, var1);
    }
}