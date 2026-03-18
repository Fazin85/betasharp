using System.Collections.Concurrent;

namespace BetaSharp.Rules;

public sealed class CvarRegistry
{
    public static CvarRegistry Instance { get; } = CreateDefault();

    private readonly ConcurrentDictionary<ResourceLocation, Cvar> _rules = new();

    private static CvarRegistry CreateDefault()
    {
        CvarRegistry registry = new();
        Cvars.Instance.RegisterAll(registry);
        return registry;
    }

    public IGameRule<T> Register<T>(IGameRule<T> rule) where T : ICvarValue
    {
        if (!_rules.TryAdd(rule.Key, rule))
            throw new InvalidOperationException($"Rule '{rule.Key}' is already registered.");
        return rule;
    }

    public CvarRegistrar For(string @namespace) => new(this, @namespace);

    public bool TryGet(ResourceLocation key, out Cvar rule) =>
        _rules.TryGetValue(key, out rule!);

    public Cvar Get(ResourceLocation key) =>
        _rules.TryGetValue(key, out Cvar? r) ? r
            : throw new KeyNotFoundException($"No rule registered for key '{key}'.");

    public IEnumerable<Cvar> All => _rules.Values;

    public IEnumerable<Cvar> ByCategory(string category) =>
        _rules.Values.Where(r => r.Category == category);

    public IEnumerable<string> Categories =>
        _rules.Values.Select(r => r.Category).Distinct().Order();
}

public sealed class CvarRegistrar(CvarRegistry registry, string ns)
{
    public BoolVar Bool(string name, bool defaultValue,
        string category = "general", string description = "") =>
        (BoolVar)registry.Register(new BoolVar(new ResourceLocation(ns, name), defaultValue, category, description));

    public IntVar Int(string name, int defaultValue, int min = int.MinValue, int max = int.MaxValue,
        string category = "general", string description = "") =>
        (IntVar)registry.Register(new IntVar(new ResourceLocation(ns, name), defaultValue, min, max, category, description));

    public FloatVar Float(string name, float defaultValue, float min = float.MinValue, float max = float.MaxValue,
        string category = "general", string description = "") =>
        (FloatVar)registry.Register(new FloatVar(new ResourceLocation(ns, name), defaultValue, min, max, category, description));

    public StringRule String(string name, string defaultValue,
        string category = "general", string description = "") =>
        (StringRule)registry.Register(new StringRule(new ResourceLocation(ns, name), defaultValue, category, description));

    public EnumRule<T> Enum<T>(string name, T defaultValue,
        string category = "general", string description = "") where T : struct, global::System.Enum =>
        (EnumRule<T>)registry.Register(new EnumRule<T>(new ResourceLocation(ns, name), defaultValue, category, description));
}
