namespace BetaSharp.Rules;

public abstract class ConsoleVar<T>(ResourceLocation key, T defaultValue, string category, string description)
    : IGameRule<T> where T : ICvarValue
{
    public ResourceLocation Key { get; } = key;
    public Type ValueType => typeof(T);
    ICvarValue Cvar.DefaultValue => defaultValue;
    public T DefaultValue { get; } = defaultValue;
    public string Category { get; } = category;
    public string Description { get; } = description;

    public abstract T Deserialize(string raw);
    public abstract string Serialize(T value);

    ICvarValue Cvar.Deserialize(string raw) => Deserialize(raw);
    string Cvar.Serialize(ICvarValue value) => Serialize((T)value);
}

public sealed class BoolVar(ResourceLocation key, bool defaultValue, string category = "general", string description = "")
    : ConsoleVar<BoolValue>(key, defaultValue, category, description)
{
    public override BoolValue Deserialize(string raw) =>
        new(raw.Equals("true", StringComparison.OrdinalIgnoreCase));

    public override string Serialize(BoolValue value) => value.ToString();
}

public sealed class IntVar(ResourceLocation key, int defaultValue, int min = int.MinValue, int max = int.MaxValue,
    string category = "general", string description = "")
    : ConsoleVar<IntValue>(key, defaultValue, category, description)
{
    public int Min { get; } = min;
    public int Max { get; } = max;

    public override IntValue Deserialize(string raw)
    {
        int v = int.TryParse(raw, out int n) ? n : DefaultValue.Value;
        return new(Math.Clamp(v, Min, Max));
    }

    public override string Serialize(IntValue value) => value.ToString();
}

public sealed class FloatVar(ResourceLocation key, float defaultValue, float min = float.MinValue, float max = float.MaxValue,
    string category = "general", string description = "")
    : ConsoleVar<FloatValue>(key, defaultValue, category, description)
{
    public float Min { get; } = min;
    public float Max { get; } = max;

    public override FloatValue Deserialize(string raw)
    {
        float v = float.TryParse(raw, out float f) ? f : DefaultValue.Value;
        return new(Math.Clamp(v, Min, Max));
    }

    public override string Serialize(FloatValue value) => value.ToString();
}

public sealed class StringRule(ResourceLocation key, string defaultValue,
    string category = "general", string description = "")
    : ConsoleVar<StringValue>(key, defaultValue, category, description)
{
    public override StringValue Deserialize(string raw) => new(raw);
    public override string Serialize(StringValue value) => value.Value;
}

public sealed class EnumRule<T>(ResourceLocation key, T defaultValue,
    string category = "general", string description = "")
    : ConsoleVar<EnumValue<T>>(key, new EnumValue<T>(defaultValue), category, description)
    where T : struct, Enum
{
    public override EnumValue<T> Deserialize(string raw)
    {
        if (Enum.TryParse(raw, ignoreCase: true, out T result))
            return new(result);
        return DefaultValue;
    }

    public override string Serialize(EnumValue<T> value) => value.Value.ToString();
}
