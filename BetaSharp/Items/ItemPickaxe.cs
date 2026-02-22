using BetaSharp.Blocks;
using BetaSharp.Blocks.Materials;

namespace BetaSharp.Items;

public class ItemPickaxe : ItemTool
{

    private static Block[] blocksEffectiveAgainst =
    [
        Block.Cobblestone,
        Block.DoubleSlab,
        Block.Slab,
        Block.Stone,
        Block.Sandstone,
        Block.MossyCobblestone,
        Block.IronOre,
        Block.IronBlock,
        Block.CoalOre,
        Block.GoldBlock,
        Block.GoldOre,
        Block.DiamondOre,
        Block.DiamondBlock,
        Block.Ice,
        Block.Netherrack,
        Block.LapisOre,
        Block.LapisBlock,
        Block.Furnace,
    ];

    public ItemPickaxe(int id, EnumToolMaterial enumToolMaterial) : base(id, 2, enumToolMaterial, blocksEffectiveAgainst)
    {
    }

    public override bool isSuitableFor(Block block)
    {
        return toolMaterial.getHarvestLevel() >= block.getHarvestLevel();
    }
}
