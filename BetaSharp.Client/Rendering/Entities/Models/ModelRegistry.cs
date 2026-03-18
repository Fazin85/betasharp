namespace BetaSharp.Client.Rendering.Entities.Models;


public static class ModelRegistry
{
    private static readonly Dictionary<string, Func<ModelBase>> _models = new();

    public static void Register(string id, Func<ModelBase> factory)
        => _models[id] = factory;

    public static ModelBase Get(string id)
        => _models.TryGetValue(id, out var factory) ? factory() : null;
}
