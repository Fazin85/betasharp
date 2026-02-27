using System.Globalization;

namespace BetaSharp.Client.Options;

public class FloatOption : GameOption<float>
{
    public required float Min { get; init; }
    public required float Max { get; init; }
    public float Step { get; init; }

    public FloatOption(string translationKey, string saveKey, float defaultValue = 0f) : base(translationKey, saveKey)
    {
        DefaultValue = defaultValue;
        Value = defaultValue;
    }

    /// <summary>
    /// Gets the normalized value (0-1) for slider positioning.
    /// </summary>
    public float NormalizedValue => (Value - Min) / (Max - Min);

    public void Set(float value)
    {
        Value = Math.Clamp(value, Min, Max);

        if (Step > 0)
        {
            Value = MathF.Round(Value / Step) * Step;
        }

        OnChanged?.Invoke(Value);
    }

    /// <summary>
    /// Sets the value from a normalized (0-1) slider position.
    /// </summary>
    public void SetFromNormalized(float normalized)
    {
        float actualValue = Min + normalized * (Max - Min);
        Set(actualValue);
    }

    public void ResetToDefault()
    {
        Set(DefaultValue);
    }

    public override string FormatValue(TranslationStorage translations)
    {
        if (Formatter != null)
        {
            return Formatter(Value, translations);
        }

        // Default formatting: percentage based on position in range
        float percent = (Value - Min) / (Max - Min) * 100f;
        return percent == 0f
            ? translations.TranslateKey("options.off")
            : $"{(int)percent}%";
    }

    public override void Load(string raw)
    {
        Value = raw switch
        {
            "true" => Max,
            "false" => Min,
            _ => float.Parse(raw, CultureInfo.InvariantCulture)
        };
    }

    public override string Save() => Value.ToString(CultureInfo.InvariantCulture);
}
