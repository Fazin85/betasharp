using betareborn.Entities;
using betareborn.Items;
using betareborn.Materials;
using betareborn.Stats;
using betareborn.Worlds;

namespace betareborn.Blocks
{
    public class BlockSnow : Block
    {

        public BlockSnow(int var1, int var2) : base(var1, var2, Material.SNOW_LAYER)
        {
            setBoundingBox(0.0F, 0.0F, 0.0F, 1.0F, 2.0F / 16.0F, 1.0F);
            setTickRandomly(true);
        }

        public override Box getCollisionShape(World var1, int var2, int var3, int var4)
        {
            int var5 = var1.getBlockMeta(var2, var3, var4) & 7;
            return var5 >= 3 ? Box.createCached((double)var2 + minX, (double)var3 + minY, (double)var4 + minZ, (double)var2 + maxX, (double)((float)var3 + 0.5F), (double)var4 + maxZ) : null;
        }

        public override bool isOpaque()
        {
            return false;
        }

        public override bool isFullCube()
        {
            return false;
        }

        public override void updateBoundingBox(BlockView var1, int var2, int var3, int var4)
        {
            int var5 = var1.getBlockMeta(var2, var3, var4) & 7;
            float var6 = (float)(2 * (1 + var5)) / 16.0F;
            setBoundingBox(0.0F, 0.0F, 0.0F, 1.0F, var6, 1.0F);
        }

        public override bool canPlaceBlockAt(World var1, int var2, int var3, int var4)
        {
            int var5 = var1.getBlockId(var2, var3 - 1, var4);
            return var5 != 0 && Block.BLOCKS[var5].isOpaque() ? var1.getMaterial(var2, var3 - 1, var4).blocksMovement() : false;
        }

        public override void neighborUpdate(World var1, int var2, int var3, int var4, int var5)
        {
            func_314_h(var1, var2, var3, var4);
        }

        private bool func_314_h(World var1, int var2, int var3, int var4)
        {
            if (!canPlaceBlockAt(var1, var2, var3, var4))
            {
                dropBlockAsItem(var1, var2, var3, var4, var1.getBlockMeta(var2, var3, var4));
                var1.setBlockWithNotify(var2, var3, var4, 0);
                return false;
            }
            else
            {
                return true;
            }
        }

        public override void harvestBlock(World var1, EntityPlayer var2, int var3, int var4, int var5, int var6)
        {
            int var7 = Item.snowball.id;
            float var8 = 0.7F;
            double var9 = (double)(var1.random.nextFloat() * var8) + (double)(1.0F - var8) * 0.5D;
            double var11 = (double)(var1.random.nextFloat() * var8) + (double)(1.0F - var8) * 0.5D;
            double var13 = (double)(var1.random.nextFloat() * var8) + (double)(1.0F - var8) * 0.5D;
            EntityItem var15 = new EntityItem(var1, (double)var3 + var9, (double)var4 + var11, (double)var5 + var13, new ItemStack(var7, 1, 0));
            var15.delayBeforeCanPickup = 10;
            var1.spawnEntity(var15);
            var1.setBlockWithNotify(var3, var4, var5, 0);
            var2.addStat(StatList.mineBlockStatArray[id], 1);
        }

        public override int getDroppedItemId(int var1, java.util.Random var2)
        {
            return Item.snowball.id;
        }

        public override int quantityDropped(java.util.Random var1)
        {
            return 0;
        }

        public override void onTick(World var1, int var2, int var3, int var4, java.util.Random var5)
        {
            if (var1.getSavedLightValue(EnumSkyBlock.Block, var2, var3, var4) > 11)
            {
                dropBlockAsItem(var1, var2, var3, var4, var1.getBlockMeta(var2, var3, var4));
                var1.setBlockWithNotify(var2, var3, var4, 0);
            }

        }

        public override bool isSideVisible(BlockView var1, int var2, int var3, int var4, int var5)
        {
            return var5 == 1 ? true : base.isSideVisible(var1, var2, var3, var4, var5);
        }
    }

}