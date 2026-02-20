namespace BetaSharp.Rules;

internal sealed class DefaultRules : IRulesProvider
{
    internal static DefaultRules Instance { get; } = new();
    internal static BoolRule DoFireTick { get; private set; } = null!;
    internal static BoolRule DoTileDrops { get; private set; } = null!;
    internal static BoolRule TntExplodes { get; private set; } = null!;

    private DefaultRules()
    {
    }

    public void RegisterAll(RuleRegistry registry)
    {
        RuleRegistrar r = registry.For(ResourceLocation.DefaultNamespace);

        DoFireTick = r.Bool("do_fire_tick", true, description: "Whether fire should spread and naturally extinguish.");
        DoTileDrops = r.Bool("do_tile_drops", true, description: "Whether blocks should have drops.");
        TntExplodes = r.Bool("tnt_explodes", true, description: "Whether TNT explodes after activation.");
    }
}
