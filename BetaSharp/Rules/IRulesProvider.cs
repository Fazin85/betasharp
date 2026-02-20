namespace BetaSharp.Rules;

public interface IRulesProvider
{
    void RegisterAll(RuleRegistry registry);
}
