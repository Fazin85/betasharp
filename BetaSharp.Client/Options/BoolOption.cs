namespace BetaSharp.Client.Options;

public class BoolOption : GameOption
{
    public bool Value { get; set; }
    public Func<bool, TranslationStorage, string>? Formatter { get; init; }
    public Action<bool>? OnChanged { get; init; }

    public BoolOption(string translationKey, string saveKey, bool defaultValue = false) : base(translationKey, saveKey)
    {
        Value = defaultValue;
    }

    public void Toggle()
    {
        Value = !Value;
        OnChanged?.Invoke(Value);
    }

    public override string FormatValue(TranslationStorage translations)
    {
        if (Formatter != null)
        {
            return Formatter(Value, translations);
        }

        return Value ? translations.TranslateKey("options.on") : translations.TranslateKey("options.off");
    }

    public override void Load(string raw) => Value = raw == "true";

    public override string Save() => Value.ToString().ToLower();
}
