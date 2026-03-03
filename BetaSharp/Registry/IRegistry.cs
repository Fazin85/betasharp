using System.Collections.Generic;

namespace BetaSharp.Registry;

public interface IRegistry<T>
{
    void Register(Identifier id, T value);

    bool TryGet(Identifier id, out T value);

    T Get(Identifier id);

    IEnumerable<KeyValuePair<Identifier, T>> GetEntries();
}

