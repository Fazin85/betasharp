using betareborn.Worlds;

namespace betareborn.Blocks
{
    public class BlockMushroom : BlockFlower
    {
        public BlockMushroom(int var1, int var2) : base(var1, var2)
        {
            float var3 = 0.2F;
            setBoundingBox(0.5F - var3, 0.0F, 0.5F - var3, 0.5F + var3, var3 * 2.0F, 0.5F + var3);
            setTickRandomly(true);
        }

        public override void onTick(World var1, int var2, int var3, int var4, java.util.Random var5)
        {
            if (var5.nextInt(100) == 0)
            {
                int var6 = var2 + var5.nextInt(3) - 1;
                int var7 = var3 + var5.nextInt(2) - var5.nextInt(2);
                int var8 = var4 + var5.nextInt(3) - 1;
                if (var1.isAir(var6, var7, var8) && canBlockStay(var1, var6, var7, var8))
                {
                    int var10000 = var2 + (var5.nextInt(3) - 1);
                    var10000 = var4 + (var5.nextInt(3) - 1);
                    if (var1.isAir(var6, var7, var8) && canBlockStay(var1, var6, var7, var8))
                    {
                        var1.setBlockWithNotify(var6, var7, var8, id);
                    }
                }
            }

        }

        protected override bool canThisPlantGrowOnThisBlockID(int var1)
        {
            return Block.BLOCKS_OPAQUE[var1];
        }

        public override bool canBlockStay(World var1, int var2, int var3, int var4)
        {
            return var3 >= 0 && var3 < 128 ? var1.getFullBlockLightValue(var2, var3, var4) < 13 && canThisPlantGrowOnThisBlockID(var1.getBlockId(var2, var3 - 1, var4)) : false;
        }
    }

}