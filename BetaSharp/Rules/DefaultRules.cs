namespace BetaSharp.Rules;

internal sealed class DefaultRules : IRulesProvider
{
    public static DefaultRules Instance { get; } = new();

    private DefaultRules()
    {
    }

    public void RegisterAll(RuleRegistry registry)
    {
        RuleRegistrar r = registry.For(ResourceLocation.DefaultNamespace);

        r.Bool("do_fire_tick", true, description: "Whether fire should spread and naturally extinguish.");
    }
}
