using BetaSharp.Blocks;
using BetaSharp.Items;

namespace BetaSharp.Registry;

public static class ItemRegistry
{
    private static bool _initialized = false;

    public static void Initialize()
    {
        if (_initialized) return;
        _initialized = true;
    }

    public static object Resolve(string name)
    {
        if (!_initialized) Initialize();

        var id = Identifier.Parse(name);

        if (Registries.Items.TryGet(id, out var item))
        {
            return item;
        }

        if (Registries.Blocks.TryGet(id, out var block))
        {
            return block;
        }

        throw new Exception($"Unknown item or block requested: {name}");
    }

    public static ItemStack ResolveStack(string name, int count, int meta)
    {
        object obj = Resolve(name);
        return obj switch
        {
            Item i => new ItemStack(i, count, meta),
            Block b => new ItemStack(b, count, meta),
            _ => throw new Exception($"Cannot create ItemStack from {name}.")
        };
    }
}