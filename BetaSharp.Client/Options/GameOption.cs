namespace BetaSharp.Client.Options;

public abstract class GameOption<T> : GameOption
{
    public T Value { get; set; } = default!;
    public Action<T>? OnChanged { get; init; }
    public Func<T, TranslationStorage, string>? Formatter { get; init; }

    protected GameOption(string translationKey, string saveKey)
    {
        TranslationKey = translationKey;
        SaveKey = saveKey;
    }
}

public abstract class GameOption
{
    public string TranslationKey { get; init; }
    public string SaveKey { get; init; }
    public string? LabelOverride { get; init; }

    public string GetLabel(TranslationStorage translations) =>
        LabelOverride ?? translations.TranslateKey(TranslationKey);

    public string GetDisplayString(TranslationStorage translations) =>
        GetLabel(translations) + ": " + FormatValue(translations);

    public abstract string FormatValue(TranslationStorage translations);
    public abstract void Load(string raw);
    public abstract string Save();
}
