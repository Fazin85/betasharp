using Microsoft.Extensions.Logging;

namespace BetaSharp.Registry;

public class Registry<T> : IRegistry<T>
{
    private readonly Dictionary<ResourceLocation, T> _entries = new();
    private readonly ILogger<Registry<T>> _logger = Log.Instance.For<Registry<T>>();

    public void Register(ResourceLocation id, T value)
    {
        if (_entries.ContainsKey(id))
        {
            _logger.LogError("Duplicate registry entry for id {Id}", id.ToString());
            throw new ArgumentException($"Duplicate registry id '{id}'", nameof(id));
        }

        _entries[id] = value;
    }

    public bool TryGet(ResourceLocation id, out T value) => _entries.TryGetValue(id, out value!);

    public T Get(ResourceLocation id)
    {
        if (_entries.TryGetValue(id, out var value))
        {
            return value;
        }

        throw new KeyNotFoundException($"Unknown registry id '{id}'");
    }

    public IEnumerable<KeyValuePair<ResourceLocation, T>> GetEntries() => _entries;

    public bool TryGet(ResourceLocation id, out T? value, bool allowNull)
    {
        return _entries.TryGetValue(id, out value!);
    }
}

