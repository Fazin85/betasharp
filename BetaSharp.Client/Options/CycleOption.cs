namespace BetaSharp.Client.Options;

public class CycleOption : GameOption<int>
{
    public string[] Labels { get; }

    public CycleOption(string translationKey, string saveKey, string[] labels, int defaultValue = 0) : base(translationKey, saveKey)
    {
        Labels = labels;
        Value = defaultValue;
    }

    public void Cycle(int increment = 1)
    {
        Value = ((Value + increment) % Labels.Length + Labels.Length) % Labels.Length;
        OnChanged?.Invoke(Value);
    }

    public override string FormatValue(TranslationStorage translations)
    {
        if (Formatter != null)
        {
            return Formatter(Value, translations);
        }

        return translations.TranslateKey(Labels[Value]);
    }

    public override void Load(string raw) => Value = int.Parse(raw);

    public override string Save() => Value.ToString();
}
