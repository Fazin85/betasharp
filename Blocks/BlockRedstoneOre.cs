using betareborn.Entities;
using betareborn.Items;
using betareborn.Materials;
using betareborn.Worlds;

namespace betareborn.Blocks
{
    public class BlockRedstoneOre : Block
    {

        private bool field_468_a;

        public BlockRedstoneOre(int var1, int var2, bool var3) : base(var1, var2, Material.STONE)
        {
            if (var3)
            {
                setTickRandomly(true);
            }

            field_468_a = var3;
        }

        public override int tickRate()
        {
            return 30;
        }

        public override void onBlockClicked(World var1, int var2, int var3, int var4, EntityPlayer var5)
        {
            func_320_h(var1, var2, var3, var4);
            base.onBlockClicked(var1, var2, var3, var4, var5);
        }

        public override void onEntityWalking(World var1, int var2, int var3, int var4, Entity var5)
        {
            func_320_h(var1, var2, var3, var4);
            base.onEntityWalking(var1, var2, var3, var4, var5);
        }

        public override bool onUse(World var1, int var2, int var3, int var4, EntityPlayer var5)
        {
            func_320_h(var1, var2, var3, var4);
            return base.onUse(var1, var2, var3, var4, var5);
        }

        private void func_320_h(World var1, int var2, int var3, int var4)
        {
            func_319_i(var1, var2, var3, var4);
            if (id == Block.REDSTONE_ORE.id)
            {
                var1.setBlockWithNotify(var2, var3, var4, Block.LIT_REDSTONE_ORE.id);
            }

        }

        public override void onTick(World var1, int var2, int var3, int var4, java.util.Random var5)
        {
            if (id == Block.LIT_REDSTONE_ORE.id)
            {
                var1.setBlockWithNotify(var2, var3, var4, Block.REDSTONE_ORE.id);
            }

        }

        public override int getDroppedItemId(int var1, java.util.Random var2)
        {
            return Item.redstone.id;
        }

        public override int quantityDropped(java.util.Random var1)
        {
            return 4 + var1.nextInt(2);
        }

        public override void randomDisplayTick(World var1, int var2, int var3, int var4, java.util.Random var5)
        {
            if (field_468_a)
            {
                func_319_i(var1, var2, var3, var4);
            }

        }

        private void func_319_i(World var1, int var2, int var3, int var4)
        {
            java.util.Random var5 = var1.random;
            double var6 = 1.0D / 16.0D;

            for (int var8 = 0; var8 < 6; ++var8)
            {
                double var9 = (double)((float)var2 + var5.nextFloat());
                double var11 = (double)((float)var3 + var5.nextFloat());
                double var13 = (double)((float)var4 + var5.nextFloat());
                if (var8 == 0 && !var1.isOpaque(var2, var3 + 1, var4))
                {
                    var11 = (double)(var3 + 1) + var6;
                }

                if (var8 == 1 && !var1.isOpaque(var2, var3 - 1, var4))
                {
                    var11 = (double)(var3 + 0) - var6;
                }

                if (var8 == 2 && !var1.isOpaque(var2, var3, var4 + 1))
                {
                    var13 = (double)(var4 + 1) + var6;
                }

                if (var8 == 3 && !var1.isOpaque(var2, var3, var4 - 1))
                {
                    var13 = (double)(var4 + 0) - var6;
                }

                if (var8 == 4 && !var1.isOpaque(var2 + 1, var3, var4))
                {
                    var9 = (double)(var2 + 1) + var6;
                }

                if (var8 == 5 && !var1.isOpaque(var2 - 1, var3, var4))
                {
                    var9 = (double)(var2 + 0) - var6;
                }

                if (var9 < (double)var2 || var9 > (double)(var2 + 1) || var11 < 0.0D || var11 > (double)(var3 + 1) || var13 < (double)var4 || var13 > (double)(var4 + 1))
                {
                    var1.addParticle("reddust", var9, var11, var13, 0.0D, 0.0D, 0.0D);
                }
            }

        }
    }

}