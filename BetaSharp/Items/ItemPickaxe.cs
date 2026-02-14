using BetaSharp.Blocks;
using BetaSharp.Blocks.Materials;

namespace BetaSharp.Items;

public class ItemPickaxe : ItemTool
{

    private static Block[] blocksEffectiveAgainst = new Block[] { Block.COBBLESTONE, Block.DOUBLE_SLAB, Block.SLAB, Block.STONE, Block.SANDSTONE, Block.MOSSY_COBBLESTONE, Block.IRON_ORE, Block.IRON_BLOCK, Block.COAL_ORE, Block.GOLD_BLOCK, Block.GOLD_ORE, Block.DIAMOND_ORE, Block.DIAMOND_BLOCK, Block.ICE, Block.NETHERRACK, Block.LAPIS_ORE, Block.LAPIS_BLOCK };

    public ItemPickaxe(int id, EnumToolMaterial enumToolMaterial) : base(id, 2, enumToolMaterial, blocksEffectiveAgainst)
    {
    }

    public override bool isSuitableFor(Block block)
    {
        return block == Block.OBSIDIAN ? toolMaterial.getHarvestLevel() == 3 : (block != Block.DIAMOND_BLOCK && block != Block.DIAMOND_ORE ? (block != Block.GOLD_BLOCK && block != Block.GOLD_ORE ? (block != Block.IRON_BLOCK && block != Block.IRON_ORE ? (block != Block.LAPIS_BLOCK && block != Block.LAPIS_ORE ? (block != Block.REDSTONE_ORE && block != Block.LIT_REDSTONE_ORE ? (block.material == Material.Stone ? true : block.material == Material.Metal) : toolMaterial.getHarvestLevel() >= 2) : toolMaterial.getHarvestLevel() >= 1) : toolMaterial.getHarvestLevel() >= 1) : toolMaterial.getHarvestLevel() >= 2) : toolMaterial.getHarvestLevel() >= 2);
    }
}