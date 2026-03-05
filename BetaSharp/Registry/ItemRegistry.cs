using BetaSharp.Blocks;
using BetaSharp.Items;
using Microsoft.Extensions.Logging;

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

        var id = ResourceLocation.Parse(name);

        if (Registries.Items.TryGet(id, out var item))
        {
            return item;
        }

        if (Registries.Blocks.TryGet(id, out var block))
        {
            return block;
        }

        // log avaliable items and blocks
        Log.Instance.For(nameof(ItemRegistry)).LogInformation($"Available items: {string.Join(", ", Registries.Items.GetEntries().Select(x => x.Key.ToString()))}");
        Log.Instance.For(nameof(ItemRegistry)).LogInformation($"Available blocks: {string.Join(", ", Registries.Blocks.GetEntries().Select(x => x.Key.ToString()))}");
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