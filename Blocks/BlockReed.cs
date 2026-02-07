using betareborn.Items;
using betareborn.Materials;
using betareborn.Worlds;

namespace betareborn.Blocks
{
    public class BlockReed : Block
    {

        public BlockReed(int var1, int var2) : base(var1, Material.PLANT)
        {
            textureId = var2;
            float var3 = 6.0F / 16.0F;
            setBoundingBox(0.5F - var3, 0.0F, 0.5F - var3, 0.5F + var3, 1.0F, 0.5F + var3);
            setTickRandomly(true);
        }

        public override void onTick(World var1, int var2, int var3, int var4, java.util.Random var5)
        {
            if (var1.isAir(var2, var3 + 1, var4))
            {
                int var6;
                for (var6 = 1; var1.getBlockId(var2, var3 - var6, var4) == id; ++var6)
                {
                }

                if (var6 < 3)
                {
                    int var7 = var1.getBlockMeta(var2, var3, var4);
                    if (var7 == 15)
                    {
                        var1.setBlockWithNotify(var2, var3 + 1, var4, id);
                        var1.setBlockMeta(var2, var3, var4, 0);
                    }
                    else
                    {
                        var1.setBlockMeta(var2, var3, var4, var7 + 1);
                    }
                }
            }

        }

        public override bool canPlaceBlockAt(World var1, int var2, int var3, int var4)
        {
            int var5 = var1.getBlockId(var2, var3 - 1, var4);
            return var5 == id ? true : (var5 != Block.GRASS_BLOCK.id && var5 != Block.DIRT.id ? false : (var1.getMaterial(var2 - 1, var3 - 1, var4) == Material.WATER ? true : (var1.getMaterial(var2 + 1, var3 - 1, var4) == Material.WATER ? true : (var1.getMaterial(var2, var3 - 1, var4 - 1) == Material.WATER ? true : var1.getMaterial(var2, var3 - 1, var4 + 1) == Material.WATER))));
        }

        public override void neighborUpdate(World var1, int var2, int var3, int var4, int var5)
        {
            checkBlockCoordValid(var1, var2, var3, var4);
        }

        protected void checkBlockCoordValid(World var1, int var2, int var3, int var4)
        {
            if (!canBlockStay(var1, var2, var3, var4))
            {
                dropBlockAsItem(var1, var2, var3, var4, var1.getBlockMeta(var2, var3, var4));
                var1.setBlockWithNotify(var2, var3, var4, 0);
            }

        }

        public override bool canBlockStay(World var1, int var2, int var3, int var4)
        {
            return canPlaceBlockAt(var1, var2, var3, var4);
        }

        public override Box getCollisionShape(World var1, int var2, int var3, int var4)
        {
            return null;
        }

        public override int getDroppedItemId(int var1, java.util.Random var2)
        {
            return Item.reed.id;
        }

        public override bool isOpaque()
        {
            return false;
        }

        public override bool isFullCube()
        {
            return false;
        }

        public override int getRenderType()
        {
            return 1;
        }
    }

}