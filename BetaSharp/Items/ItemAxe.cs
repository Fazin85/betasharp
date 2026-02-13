using BetaSharp.Blocks;

namespace BetaSharp.Items;

public class ItemAxe : ItemTool
{

    private static Block[] blocksEffectiveAgainst = new Block[] { Block.PLANKS, Block.BOOKSHELF, Block.LOG, Block.CHEST };

    public ItemAxe(int id, EnumToolMaterial enumToolMaterial) : base(id, 3, enumToolMaterial, blocksEffectiveAgainst)
    {
    }
}