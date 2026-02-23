namespace BetaSharp.Client.Options;

public abstract class GameOption
{
    public string TranslationKey { get; }
    public string SaveKey { get; }
    public string? LabelOverride { get; init; }
    public bool IsSlider => this is FloatOption;

    protected GameOption(string translationKey, string saveKey)
    {
        TranslationKey = translationKey;
        SaveKey = saveKey;
    }

    public string GetLabel(TranslationStorage translations) =>
        LabelOverride ?? translations.TranslateKey(TranslationKey);

    public string GetDisplayString(TranslationStorage translations) =>
        GetLabel(translations) + ": " + FormatValue(translations);

    public abstract string FormatValue(TranslationStorage translations);
    public abstract void Load(string raw);
    public abstract string Save();
}
