namespace BetaSharp;

public class TranslationStorage
{
    public static TranslationStorage Instance { get; } = new();
    private readonly Dictionary<string, string> _translations = new();

    private TranslationStorage()
    {
        LoadLanguageFile("lang/en_US.lang");
        LoadLanguageFile("lang/stats_US.lang");
    }

    private void LoadLanguageFile(string path)
    {
        try
        {
            var content = AssetManager.Instance.getAsset(path).getTextContent();
            using var reader = new StringReader(content);
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                // Skip comments or empty lines
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#")) continue;

                var parts = line.Split('=', 2);
                if (parts.Length == 2)
                {
                    _translations[parts[0].Trim()] = parts[1].Trim();
                }
            }
        }
        catch (Exception ex)
        {
            // Use standard C# Console or your logger instead of printStackTrace
            Console.WriteLine($"Failed to load language file {path}: {ex.Message}");
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
