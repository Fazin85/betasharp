namespace BetaSharp.Rules;

internal sealed class DefaultRules : IRulesProvider
{
    public void RegisterAll(RuleRegistry r)
    {
        r.Register(new BoolRule("doFireTick", true, description: "Whether fire should spread and naturally extinguish."));
    }
}
