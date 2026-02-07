using betareborn.Entities;
using betareborn.Materials;
using betareborn.Worlds;

namespace betareborn.Blocks
{
    public class BlockCactus : Block
    {

        public BlockCactus(int var1, int var2) : base(var1, var2, Material.CACTUS)
        {
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

        public override Box getCollisionShape(World var1, int var2, int var3, int var4)
        {
            float var5 = 1.0F / 16.0F;
            return Box.createCached((double)((float)var2 + var5), (double)var3, (double)((float)var4 + var5), (double)((float)(var2 + 1) - var5), (double)((float)(var3 + 1) - var5), (double)((float)(var4 + 1) - var5));
        }

        public override Box getBoundingBox(World var1, int var2, int var3, int var4)
        {
            float var5 = 1.0F / 16.0F;
            return Box.createCached((double)((float)var2 + var5), (double)var3, (double)((float)var4 + var5), (double)((float)(var2 + 1) - var5), (double)(var3 + 1), (double)((float)(var4 + 1) - var5));
        }

        public override int getTexture(int var1)
        {
            return var1 == 1 ? textureId - 1 : (var1 == 0 ? textureId + 1 : textureId);
        }

        public override bool isFullCube()
        {
            return false;
        }

        public override bool isOpaque()
        {
            return false;
        }

        public override int getRenderType()
        {
            return 13;
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
            if (var1.getMaterial(var2 - 1, var3, var4).isSolid())
            {
                return false;
            }
            else if (var1.getMaterial(var2 + 1, var3, var4).isSolid())
            {
                return false;
            }
            else if (var1.getMaterial(var2, var3, var4 - 1).isSolid())
            {
                return false;
            }
            else if (var1.getMaterial(var2, var3, var4 + 1).isSolid())
            {
                return false;
            }
            else
            {
                int var5 = var1.getBlockId(var2, var3 - 1, var4);
                return var5 == Block.CACTUS.id || var5 == Block.SAND.id;
            }
        }

        public override void onEntityCollidedWithBlock(World var1, int var2, int var3, int var4, Entity var5)
        {
            var5.attackEntityFrom((Entity)null, 1);
        }
    }

}