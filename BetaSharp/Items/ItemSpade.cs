using BetaSharp.Blocks;

namespace BetaSharp.Items;

public class ItemSpade : ItemTool
{

    private static Block[] blocksEffectiveAgainst = new Block[] { Block.GRASS_BLOCK, Block.DIRT, Block.SAND, Block.GRAVEL, Block.SNOW, Block.SNOW_BLOCK, Block.CLAY, Block.FARMLAND };

    public ItemSpade(int id, EnumToolMaterial enumToolMaterial) : base(id, 1, enumToolMaterial, blocksEffectiveAgainst)
    {
    }

    public override bool isSuitableFor(Block block)
    {
        return block == Block.SNOW ? true : block == Block.SNOW_BLOCK;
    }
}