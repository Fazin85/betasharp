using betareborn.Items;
using betareborn.Materials;
using betareborn.Worlds;

namespace betareborn.Blocks
{
    public class BlockSnowBlock : Block
    {

        public BlockSnowBlock(int var1, int var2) : base(var1, var2, Material.SNOW_BLOCK)
        {
            setTickRandomly(true);
        }

        public override int getDroppedItemId(int var1, java.util.Random var2)
        {
            return Item.snowball.id;
        }

        public override int quantityDropped(java.util.Random var1)
        {
            return 4;
        }

        public override void onTick(World var1, int var2, int var3, int var4, java.util.Random var5)
        {
            if (var1.getSavedLightValue(EnumSkyBlock.Block, var2, var3, var4) > 11)
            {
                dropBlockAsItem(var1, var2, var3, var4, var1.getBlockMeta(var2, var3, var4));
                var1.setBlockWithNotify(var2, var3, var4, 0);
            }

        }
    }

}