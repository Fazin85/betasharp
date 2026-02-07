using betareborn.Blocks;
using betareborn.Materials;

namespace betareborn.Items
{
    public class ItemPickaxe : ItemTool
    {

        private static Block[] blocksEffectiveAgainst = new Block[] { Block.COBBLESTONE, Block.DOUBLE_SLAB, Block.SLAB, Block.STONE, Block.SANDSTONE, Block.MOSSY_COBBLESTONE, Block.IRON_ORE, Block.IRON_BLOCK, Block.COAL_ORE, Block.GOLD_BLOCK, Block.GOLD_ORE, Block.DIAMOND_ORE, Block.DIAMOND_BLOCK, Block.ICE, Block.NETHERRACK, Block.LAPIS_ORE, Block.LAPIS_BLOCK };

        public ItemPickaxe(int var1, EnumToolMaterial var2) : base(var1, 2, var2, blocksEffectiveAgainst)
        {
        }

        public override bool canHarvestBlock(Block var1)
        {
            return var1 == Block.OBSIDIAN ? toolMaterial.getHarvestLevel() == 3 : (var1 != Block.DIAMOND_BLOCK && var1 != Block.DIAMOND_ORE ? (var1 != Block.GOLD_BLOCK && var1 != Block.GOLD_ORE ? (var1 != Block.IRON_BLOCK && var1 != Block.IRON_ORE ? (var1 != Block.LAPIS_BLOCK && var1 != Block.LAPIS_ORE ? (var1 != Block.REDSTONE_ORE && var1 != Block.LIT_REDSTONE_ORE ? (var1.material == Material.STONE ? true : var1.material == Material.METAL) : toolMaterial.getHarvestLevel() >= 2) : toolMaterial.getHarvestLevel() >= 1) : toolMaterial.getHarvestLevel() >= 1) : toolMaterial.getHarvestLevel() >= 2) : toolMaterial.getHarvestLevel() >= 2);
        }
    }

}