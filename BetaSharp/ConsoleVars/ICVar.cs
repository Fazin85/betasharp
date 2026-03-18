namespace BetaSharp.Rules;

public interface ICvarValue { }

public interface Cvar
{
    ResourceLocation Key { get; }
    Type ValueType { get; }
    ICvarValue DefaultValue { get; }
    string Category { get; }
    string Description { get; }

    ICvarValue Deserialize(string raw);
    string Serialize(ICvarValue value);
}

public interface IGameRule<T> : Cvar where T : ICvarValue
{
    new T DefaultValue { get; }
}
