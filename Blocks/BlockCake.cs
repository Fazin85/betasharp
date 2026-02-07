using betareborn.Entities;
using betareborn.Materials;
using betareborn.Worlds;

namespace betareborn.Blocks
{
    public class BlockCake : Block
    {

        public BlockCake(int var1, int var2) : base(var1, var2, Material.CAKE)
        {
            setTickRandomly(true);
        }

        public override void updateBoundingBox(BlockView var1, int var2, int var3, int var4)
        {
            int var5 = var1.getBlockMeta(var2, var3, var4);
            float var6 = 1.0F / 16.0F;
            float var7 = (float)(1 + var5 * 2) / 16.0F;
            float var8 = 0.5F;
            setBoundingBox(var7, 0.0F, var6, 1.0F - var6, var8, 1.0F - var6);
        }

        public override void setBlockBoundsForItemRender()
        {
            float var1 = 1.0F / 16.0F;
            float var2 = 0.5F;
            setBoundingBox(var1, 0.0F, var1, 1.0F - var1, var2, 1.0F - var1);
        }

        public override Box getCollisionShape(World var1, int var2, int var3, int var4)
        {
            int var5 = var1.getBlockMeta(var2, var3, var4);
            float var6 = 1.0F / 16.0F;
            float var7 = (float)(1 + var5 * 2) / 16.0F;
            float var8 = 0.5F;
            return Box.createCached((double)((float)var2 + var7), (double)var3, (double)((float)var4 + var6), (double)((float)(var2 + 1) - var6), (double)((float)var3 + var8 - var6), (double)((float)(var4 + 1) - var6));
        }

        public override Box getBoundingBox(World var1, int var2, int var3, int var4)
        {
            int var5 = var1.getBlockMeta(var2, var3, var4);
            float var6 = 1.0F / 16.0F;
            float var7 = (float)(1 + var5 * 2) / 16.0F;
            float var8 = 0.5F;
            return Box.createCached((double)((float)var2 + var7), (double)var3, (double)((float)var4 + var6), (double)((float)(var2 + 1) - var6), (double)((float)var3 + var8), (double)((float)(var4 + 1) - var6));
        }

        public override int getTexture(int var1, int var2)
        {
            return var1 == 1 ? textureId : (var1 == 0 ? textureId + 3 : (var2 > 0 && var1 == 4 ? textureId + 2 : textureId + 1));
        }

        public override int getTexture(int var1)
        {
            return var1 == 1 ? textureId : (var1 == 0 ? textureId + 3 : textureId + 1);
        }

        public override bool isFullCube()
        {
            return false;
        }

        public override bool isOpaque()
        {
            return false;
        }

        public override bool onUse(World var1, int var2, int var3, int var4, EntityPlayer var5)
        {
            eatCakeSlice(var1, var2, var3, var4, var5);
            return true;
        }

        public override void onBlockClicked(World var1, int var2, int var3, int var4, EntityPlayer var5)
        {
            eatCakeSlice(var1, var2, var3, var4, var5);
        }

        private void eatCakeSlice(World var1, int var2, int var3, int var4, EntityPlayer var5)
        {
            if (var5.health < 20)
            {
                var5.heal(3);
                int var6 = var1.getBlockMeta(var2, var3, var4) + 1;
                if (var6 >= 6)
                {
                    var1.setBlockWithNotify(var2, var3, var4, 0);
                }
                else
                {
                    var1.setBlockMeta(var2, var3, var4, var6);
                    var1.markBlockAsNeedsUpdate(var2, var3, var4);
                }
            }

        }

        public override bool canPlaceBlockAt(World var1, int var2, int var3, int var4)
        {
            return !base.canPlaceBlockAt(var1, var2, var3, var4) ? false : canBlockStay(var1, var2, var3, var4);
        }

        public override void neighborUpdate(World var1, int var2, int var3, int var4, int var5)
        {
            if (!canBlockStay(var1, var2, var3, var4))
            {
                dropBlockAsItem(var1, var2, var3, var4, var1.getBlockMeta(var2, var3, var4));
                var1.setBlockWithNotify(var2, var3, var4, 0);
            }

        }

        public override bool canBlockStay(World var1, int var2, int var3, int var4)
        {
            return var1.getMaterial(var2, var3 - 1, var4).isSolid();
        }

        public override int quantityDropped(java.util.Random var1)
        {
            return 0;
        }

        public override int getDroppedItemId(int var1, java.util.Random var2)
        {
            return 0;
        }
    }

}