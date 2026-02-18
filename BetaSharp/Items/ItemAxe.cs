using BetaSharp.Blocks;

namespace BetaSharp.Items;

public class ItemAxe : ItemTool
{

    private static Block[] blocksEffectiveAgainst = [Block.Planks, Block.Bookshelf, Block.Log, Block.Chest];

    public ItemAxe(int id, EnumToolMaterial enumToolMaterial) : base(id, 3, enumToolMaterial, blocksEffectiveAgainst)
    {
    }
}