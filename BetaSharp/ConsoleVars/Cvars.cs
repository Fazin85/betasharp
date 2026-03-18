namespace BetaSharp.Rules;

internal sealed class Cvars : ICVarProvider
{
    internal static Cvars Instance { get; } = new();
    internal static BoolVar sv_firetick { get; private set; } = null!;
    internal static BoolVar sv_tiledrops { get; private set; } = null!;
    internal static BoolVar sv_enabletnt { get; private set; } = null!;
    internal static FloatVar i_timescale { get; private set; } = null!;
    private Cvars()
    {
    }

    public void RegisterAll(CvarRegistry registry)
    {
        CvarRegistrar r = registry.For(ResourceLocation.DefaultNamespace);

        sv_firetick = r.Bool("sv_firetick", true, description: " Server-side:  Whether fire should spread and naturally extinguish.");
        sv_tiledrops = r.Bool("sv_tiledrops", true, description: " Server-side: Whether blocks should have drops.");
        sv_enabletnt = r.Bool("sv_enabletnt", true, description: " Server-side: Whether TNT explodes after activation.");
        i_timescale = r.Float("i_timescale", 1.0f,description: " world internal tick Rate." );
    }
}
