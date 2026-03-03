using BetaSharp.Blocks;
using BetaSharp.Items;

namespace BetaSharp.Registry;

public static class Registries
{
    public static readonly Registry<Block> Blocks = new();
    public static readonly Registry<Item> Items = new();
}

