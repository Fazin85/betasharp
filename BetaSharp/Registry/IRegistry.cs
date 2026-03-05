using System.Collections.Generic;

namespace BetaSharp.Registry;

public interface IRegistry<T>
{
    void Register(ResourceLocation id, T value);

    bool TryGet(ResourceLocation id, out T value);

    T Get(ResourceLocation id);

    IEnumerable<KeyValuePair<ResourceLocation, T>> GetEntries();
}

