using BetaSharp.Blocks;
using BetaSharp.Items;
using BetaSharp.Stats;

namespace BetaSharp.Registry;

public static class Registries
{
    public static readonly Registry<Block> Blocks = new();
    public static readonly Registry<Item> Items = new();
    public static readonly Registry<StatBase> Stats = new();
}

