namespace BetaSharp;

public class TranslationStorage
{
    public static TranslationStorage Instance { get; } = new();

    /// <summary>Available languages: (locale code, display name). Only locales with a .lang asset load; others keep current language.</summary>
    private static readonly (string Code, string DisplayName)[] AvailableLocales =
    [
        ("en_US", "English (US)"),
        ("en_GB", "English (UK)"),
        ("de_DE", "Deutsch"),
        ("fr_FR", "Français"),
        ("es_ES", "Español"),
        ("pt_BR", "Português (Brasil)"),
        ("pt_PT", "Português (Portugal)"),
        ("it_IT", "Italiano"),
        ("nl_NL", "Nederlands"),
        ("pl_PL", "Polski"),
        ("ru_RU", "Русский"),
        ("ja_JP", "日本語"),
        ("zh_CN", "简体中文"),
        ("zh_TW", "繁體中文"),
        ("ko_KR", "한국어"),
    ];

    private readonly Dictionary<string, string> _translations = new();

    public string CurrentLocale { get; private set; } = "en_US";

    private TranslationStorage()
    {
        SetLanguage("en_US");
    }

    /// <summary>Returns (locale code, display name) for each available language.</summary>
    public static IReadOnlyList<(string Code, string DisplayName)> GetAvailableLocales() => AvailableLocales;

    /// <summary>Switch to a locale by code (e.g. "en_US"). Loads lang/{code}.lang and stats. If the .lang asset is missing, keeps current language.</summary>
    public void SetLanguage(string localeCode)
    {
        var temp = new Dictionary<string, string>();
        if (!TryLoadLanguageFile("lang/" + localeCode + ".lang", temp))
        {
            Console.WriteLine($"Language not available: {localeCode}");
            return;
        }
        _translations.Clear();
        foreach (var kvp in temp)
            _translations[kvp.Key] = kvp.Value;
        TryLoadLanguageFile("lang/stats_US.lang", _translations);
        CurrentLocale = localeCode;
    }

    private static bool TryLoadLanguageFile(string path, Dictionary<string, string> into)
    {
        try
        {
            var content = AssetManager.Instance.getAsset(path).getTextContent();
            using var reader = new StringReader(content);
            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#")) continue;
                var parts = line.Split('=', 2);
                if (parts.Length == 2)
                    into[parts[0].Trim()] = parts[1].Trim();
            }
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to load language file {path}: {ex.Message}");
            return false;
        }
    }

    public string TranslateKey(string key)
    {
        return _translations.TryGetValue(key, out string? value) ? value : key;
    }

    public string TranslateKeyFormat(string key, params object[] values)
    {
        string template = TranslateKey(key);
        
        for (int i = 0; i < values.Length; i++)
        {
            template = template.Replace($"%{i + 1}$s", values[i]?.ToString() ?? string.Empty);
        }
        return template;
    }

    public string TranslateNamedKey(string key) => TranslateKey($"{key}.name");
}
