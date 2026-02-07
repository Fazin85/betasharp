using betareborn.Blocks;

namespace betareborn.Items
{
    public class ItemSpade : ItemTool
    {

        private static Block[] blocksEffectiveAgainst = new Block[] { Block.GRASS_BLOCK, Block.DIRT, Block.SAND, Block.GRAVEL, Block.SNOW, Block.SNOW_BLOCK, Block.CLAY, Block.FARMLAND };

        public ItemSpade(int var1, EnumToolMaterial var2) : base(var1, 1, var2, blocksEffectiveAgainst)
        {
        }

        public override bool canHarvestBlock(Block var1)
        {
            return var1 == Block.SNOW ? true : var1 == Block.SNOW_BLOCK;
        }
    }

}