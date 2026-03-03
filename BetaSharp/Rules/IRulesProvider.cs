namespace BetaSharp.Rules;

internal interface IRulesProvider
{
    void RegisterAll(RuleRegistry registry);
}
