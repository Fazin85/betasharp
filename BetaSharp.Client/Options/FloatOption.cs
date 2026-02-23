namespace BetaSharp.Client.Options;

public class FloatOption : GameOption<float>
{
    public float Step { get; init; }

    public FloatOption(string translationKey, string saveKey, float defaultValue = 0f) : base(translationKey, saveKey)
    {
        Value = defaultValue;
    }

    public void Set(float value)
    {
        Value = Math.Clamp(value, 0f, 1f);

        if (Step > 0)
        {
            Value = MathF.Round(Value / Step) * Step;
        }

        OnChanged?.Invoke(Value);
    }

    public override string FormatValue(TranslationStorage translations)
    {
        if (Formatter != null)
        {
            return Formatter(Value, translations);
        }

        return Value == 0.0F
            ? translations.TranslateKey("options.off")
            : $"{(int)(Value * 100.0F)}%";
    }

    public override void Load(string raw)
    {
        Value = raw switch
        {
            "true" => 1.0F,
            "false" => 0.0F,
            _ => float.Parse(raw)
        };
    }

    public override string Save() => Value.ToString();
}
