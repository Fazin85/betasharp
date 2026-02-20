using System.Reflection;
using BetaSharp.Blocks;
using BetaSharp.Items;

namespace BetaSharp.Registry;

public static class ItemRegistry
{
    private static readonly Dictionary<string, object> _registry = new(StringComparer.OrdinalIgnoreCase);
    private static bool _initialized = false;

    public static void Initialize()
    {
        if (_initialized) return;

        foreach (var field in typeof(Item).GetFields(BindingFlags.Public | BindingFlags.Static))
        {
            if (typeof(Item).IsAssignableFrom(field.FieldType) && field.GetValue(null) is Item item)
            {
                _registry[$"betasharp:{field.Name}"] = item;
            }
        }

        foreach (var field in typeof(Block).GetFields(BindingFlags.Public | BindingFlags.Static))
        {
            if (typeof(Block).IsAssignableFrom(field.FieldType) && field.GetValue(null) is Block block)
            {
                _registry[$"betasharp:{field.Name}"] = block;
            }
        }

        _initialized = true;
    }

    public static object Resolve(string name)
    {
        if (!_initialized) Initialize();
        
        if (_registry.TryGetValue(name, out var obj))
            return obj;
        
        throw new Exception($"[Registry] Unknown item or block requested: {name}");
    }

    public static ItemStack ResolveStack(string name, int count, int meta)
    {
        object obj = Resolve(name);
        return obj switch
        {
            Item i => new ItemStack(i, count, meta),
            Block b => new ItemStack(b, count, meta),
            _ => throw new Exception($"[Registry] Cannot create ItemStack from {name}")
        };
    }
}