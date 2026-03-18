using System.Collections.Concurrent;
using BetaSharp.NBT;

namespace BetaSharp.Rules;

public sealed class CvarSet(CvarRegistry registry)
{
    private readonly ConcurrentDictionary<ResourceLocation, ICvarValue> _values = new();

    public event Action<ResourceLocation, ICvarValue, ICvarValue>? RuleChanged;

    public T Get<T>(IGameRule<T> rule) where T : ICvarValue =>
        _values.TryGetValue(rule.Key, out ICvarValue? v) ? (T)v : rule.DefaultValue;

    public ICvarValue Get(ResourceLocation key)
    {
        Cvar rule = registry.Get(key);
        return _values.TryGetValue(key, out ICvarValue? v) ? v : rule.DefaultValue;
    }

    public bool GetBool(IGameRule<BoolValue> rule) => (bool)Get(rule);
    public int GetInt(IGameRule<IntValue> rule) => (int)Get(rule);
    public float GetFloat(IGameRule<FloatValue> rule) => (float)Get(rule);
    public string GetString(IGameRule<StringValue> rule) => (string)Get(rule);
    public TEnum GetEnum<TEnum>(IGameRule<EnumValue<TEnum>> rule) where TEnum : struct, Enum => (TEnum)Get(rule);

    public void Set<T>(IGameRule<T> rule, T value) where T : ICvarValue
    {
        T old = Get(rule);
        _values[rule.Key] = value;
        RuleChanged?.Invoke(rule.Key, old, value);
    }

    public bool TrySet(ResourceLocation key, string rawValue)
    {
        if (!registry.TryGet(key, out Cvar? rule)) return false;
        ICvarValue newValue = rule.Deserialize(rawValue);
        ICvarValue old = _values.TryGetValue(key, out ICvarValue? existing) ? existing : rule.DefaultValue;
        _values[key] = newValue;
        RuleChanged?.Invoke(key, old, newValue);
        return true;
    }

    public void Reset(ResourceLocation key) => _values.TryRemove(key, out _);

    public void ResetAll() => _values.Clear();

    public Dictionary<string, string> Serialize()
    {
        var result = new Dictionary<string, string>();
        foreach ((ResourceLocation? key, ICvarValue? value) in _values)
        {
            Cvar rule = registry.Get(key);
            result[key.ToString()] = rule.Serialize(value);
        }
        return result;
    }

    public void Deserialize(Dictionary<string, string> data)
    {
        foreach ((string? rawKey, string? rawValue) in data)
            TrySet(ResourceLocation.Parse(rawKey), rawValue);
    }

    public void WriteToNBT(NBTTagCompound nbt)
    {
        foreach (var (rule, value) in NonDefaultValues())
        {
            nbt.SetString(rule.Key.ToString(), rule.Serialize(value));
        }
    }

    public static CvarSet FromNBT(CvarRegistry registry, NBTTagCompound nbt)
    {
        CvarSet ruleSet = new(registry);
        foreach ((string key, NBTBase value) in nbt.Dictionary)
        {
            if (value is NBTTagString str)
                ruleSet.TrySet(ResourceLocation.Parse(key), str.Value);
        }
        return ruleSet;
    }

    public IEnumerable<(Cvar cvar, ICvarValue Value)> NonDefaultValues()
    {
        foreach ((ResourceLocation? key, ICvarValue? value) in _values)
        {
            Cvar rule = registry.Get(key);
            if (!value.Equals(rule.DefaultValue))
                yield return (rule, value);
        }
    }
}
