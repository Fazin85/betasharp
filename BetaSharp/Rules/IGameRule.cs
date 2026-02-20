namespace BetaSharp.Rules;

public interface IRuleValue { }

public interface IGameRule
{
    ResourceLocation Key { get; }
    Type ValueType { get; }
    IRuleValue DefaultValue { get; }
    string Category { get; }
    string Description { get; }

    IRuleValue Deserialize(string raw);
    string Serialize(IRuleValue value);
}

public interface IGameRule<T> : IGameRule where T : IRuleValue
{
    new T DefaultValue { get; }
}
