using betareborn.Entities;
using betareborn.Materials;
using betareborn.Worlds;

namespace betareborn.Blocks
{
    public class BlockFarmland : Block
    {

        public BlockFarmland(int var1) : base(var1, Material.SOIL)
        {
            textureId = 87;
            setTickRandomly(true);
            setBoundingBox(0.0F, 0.0F, 0.0F, 1.0F, 15.0F / 16.0F, 1.0F);
            setOpacity(255);
        }

        public override Box getCollisionShape(World var1, int var2, int var3, int var4)
        {
            return Box.createCached((double)(var2 + 0), (double)(var3 + 0), (double)(var4 + 0), (double)(var2 + 1), (double)(var3 + 1), (double)(var4 + 1));
        }

        public override bool isOpaque()
        {
            return false;
        }

        public override bool isFullCube()
        {
            return false;
        }

        public override int getTexture(int var1, int var2)
        {
            return var1 == 1 && var2 > 0 ? textureId - 1 : (var1 == 1 ? textureId : 2);
        }

        public override void onTick(World var1, int var2, int var3, int var4, java.util.Random var5)
        {
            if (var5.nextInt(5) == 0)
            {
                if (!isWaterNearby(var1, var2, var3, var4) && !var1.canBlockBeRainedOn(var2, var3 + 1, var4))
                {
                    int var6 = var1.getBlockMeta(var2, var3, var4);
                    if (var6 > 0)
                    {
                        var1.setBlockMeta(var2, var3, var4, var6 - 1);
                    }
                    else if (!isCropsNearby(var1, var2, var3, var4))
                    {
                        var1.setBlockWithNotify(var2, var3, var4, Block.DIRT.id);
                    }
                }
                else
                {
                    var1.setBlockMeta(var2, var3, var4, 7);
                }
            }

        }

        public override void onEntityWalking(World var1, int var2, int var3, int var4, Entity var5)
        {
            if (var1.random.nextInt(4) == 0)
            {
                var1.setBlockWithNotify(var2, var3, var4, Block.DIRT.id);
            }

        }

        private bool isCropsNearby(World var1, int var2, int var3, int var4)
        {
            sbyte var5 = 0;

            for (int var6 = var2 - var5; var6 <= var2 + var5; ++var6)
            {
                for (int var7 = var4 - var5; var7 <= var4 + var5; ++var7)
                {
                    if (var1.getBlockId(var6, var3 + 1, var7) == Block.WHEAT.id)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool isWaterNearby(World var1, int var2, int var3, int var4)
        {
            for (int var5 = var2 - 4; var5 <= var2 + 4; ++var5)
            {
                for (int var6 = var3; var6 <= var3 + 1; ++var6)
                {
                    for (int var7 = var4 - 4; var7 <= var4 + 4; ++var7)
                    {
                        if (var1.getMaterial(var5, var6, var7) == Material.WATER)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public override void neighborUpdate(World var1, int var2, int var3, int var4, int var5)
        {
            base.neighborUpdate(var1, var2, var3, var4, var5);
            Material var6 = var1.getMaterial(var2, var3 + 1, var4);
            if (var6.isSolid())
            {
                var1.setBlockWithNotify(var2, var3, var4, Block.DIRT.id);
            }

        }

        public override int getDroppedItemId(int var1, java.util.Random var2)
        {
            return Block.DIRT.getDroppedItemId(0, var2);
        }
    }

}