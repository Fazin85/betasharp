using BetaSharp.Blocks;

namespace BetaSharp.Items;

public class ItemSpade : ItemTool
{

    private static Block[] blocksEffectiveAgainst =
    [
        Block.GrassBlock,
        Block.Dirt,
        Block.Sand,
        Block.Gravel,
        Block.Snow,
        Block.SnowBlock,
        Block.Clay,
        Block.Farmland,
    ];

    public ItemSpade(int id, EnumToolMaterial enumToolMaterial) : base(id, 1, enumToolMaterial, blocksEffectiveAgainst)
    {
    }

    public override bool isSuitableFor(Block block)
    {
        return block == Block.Snow ? true : block == Block.SnowBlock;
    }
}
