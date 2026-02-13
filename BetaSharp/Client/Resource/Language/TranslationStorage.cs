namespace BetaSharp.Client.Resource.Language;

public class TranslationStorage : java.lang.Object
{
    private static readonly TranslationStorage instance = new();
    private readonly java.util.Properties translateTable = new();

    private TranslationStorage()
    {
        try
        {
            translateTable.load(new java.io.StringReader(AssetManager.Instance.getAsset("lang/en_US.lang").getTextContent()));
            translateTable.load(new java.io.StringReader(AssetManager.Instance.getAsset("lang/stats_US.lang").getTextContent()));
        }
        catch (java.io.IOException err)
        {
            err.printStackTrace();
        }

    }

    public static TranslationStorage getInstance()
    {
        return instance;
    }

    public string translateKey(string key)
    {
        return translateTable.getProperty(key, key);
    }

    public string translateKeyFormat(string key, params object[] values)
    {
        string str = translateTable.getProperty(key, key);
        for (int i = 0; i < values.Length; i++)
        {
            str = str.Replace($"%{i + 1}$s", values[i].ToString() ?? string.Empty);
        }
        return str;
    }

    public string translateNamedKey(string key)
    {
        return translateTable.getProperty(key + ".name", "");
    }
}