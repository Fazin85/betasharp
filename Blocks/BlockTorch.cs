using betareborn.Materials;
using betareborn.Worlds;

namespace betareborn.Blocks
{
    public class BlockTorch : Block
    {

        public BlockTorch(int var1, int var2) : base(var1, var2, Material.PISTON_BREAKABLE)
        {
            setTickRandomly(true);
        }

        public override Box getCollisionShape(World var1, int var2, int var3, int var4)
        {
            return null;
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
            return 2;
        }

        private bool func_31032_h(World var1, int var2, int var3, int var4)
        {
            return var1.shouldSuffocate(var2, var3, var4) || var1.getBlockId(var2, var3, var4) == Block.FENCE.id;
        }

        public override bool canPlaceBlockAt(World var1, int var2, int var3, int var4)
        {
            return var1.shouldSuffocate(var2 - 1, var3, var4) ? true : (var1.shouldSuffocate(var2 + 1, var3, var4) ? true : (var1.shouldSuffocate(var2, var3, var4 - 1) ? true : (var1.shouldSuffocate(var2, var3, var4 + 1) ? true : func_31032_h(var1, var2, var3 - 1, var4))));
        }

        public override void onBlockPlaced(World var1, int var2, int var3, int var4, int var5)
        {
            int var6 = var1.getBlockMeta(var2, var3, var4);
            if (var5 == 1 && func_31032_h(var1, var2, var3 - 1, var4))
            {
                var6 = 5;
            }

            if (var5 == 2 && var1.shouldSuffocate(var2, var3, var4 + 1))
            {
                var6 = 4;
            }

            if (var5 == 3 && var1.shouldSuffocate(var2, var3, var4 - 1))
            {
                var6 = 3;
            }

            if (var5 == 4 && var1.shouldSuffocate(var2 + 1, var3, var4))
            {
                var6 = 2;
            }

            if (var5 == 5 && var1.shouldSuffocate(var2 - 1, var3, var4))
            {
                var6 = 1;
            }

            var1.setBlockMeta(var2, var3, var4, var6);
        }

        public override void onTick(World var1, int var2, int var3, int var4, java.util.Random var5)
        {
            base.onTick(var1, var2, var3, var4, var5);
            if (var1.getBlockMeta(var2, var3, var4) == 0)
            {
                onBlockAdded(var1, var2, var3, var4);
            }

        }

        public override void onBlockAdded(World var1, int var2, int var3, int var4)
        {
            if (var1.shouldSuffocate(var2 - 1, var3, var4))
            {
                var1.setBlockMeta(var2, var3, var4, 1);
            }
            else if (var1.shouldSuffocate(var2 + 1, var3, var4))
            {
                var1.setBlockMeta(var2, var3, var4, 2);
            }
            else if (var1.shouldSuffocate(var2, var3, var4 - 1))
            {
                var1.setBlockMeta(var2, var3, var4, 3);
            }
            else if (var1.shouldSuffocate(var2, var3, var4 + 1))
            {
                var1.setBlockMeta(var2, var3, var4, 4);
            }
            else if (func_31032_h(var1, var2, var3 - 1, var4))
            {
                var1.setBlockMeta(var2, var3, var4, 5);
            }

            dropTorchIfCantStay(var1, var2, var3, var4);
        }

        public override void neighborUpdate(World var1, int var2, int var3, int var4, int var5)
        {
            if (dropTorchIfCantStay(var1, var2, var3, var4))
            {
                int var6 = var1.getBlockMeta(var2, var3, var4);
                bool var7 = false;
                if (!var1.shouldSuffocate(var2 - 1, var3, var4) && var6 == 1)
                {
                    var7 = true;
                }

                if (!var1.shouldSuffocate(var2 + 1, var3, var4) && var6 == 2)
                {
                    var7 = true;
                }

                if (!var1.shouldSuffocate(var2, var3, var4 - 1) && var6 == 3)
                {
                    var7 = true;
                }

                if (!var1.shouldSuffocate(var2, var3, var4 + 1) && var6 == 4)
                {
                    var7 = true;
                }

                if (!func_31032_h(var1, var2, var3 - 1, var4) && var6 == 5)
                {
                    var7 = true;
                }

                if (var7)
                {
                    dropBlockAsItem(var1, var2, var3, var4, var1.getBlockMeta(var2, var3, var4));
                    var1.setBlockWithNotify(var2, var3, var4, 0);
                }
            }

        }

        private bool dropTorchIfCantStay(World var1, int var2, int var3, int var4)
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

        public override MovingObjectPosition collisionRayTrace(World var1, int var2, int var3, int var4, Vec3D var5, Vec3D var6)
        {
            int var7 = var1.getBlockMeta(var2, var3, var4) & 7;
            float var8 = 0.15F;
            if (var7 == 1)
            {
                setBoundingBox(0.0F, 0.2F, 0.5F - var8, var8 * 2.0F, 0.8F, 0.5F + var8);
            }
            else if (var7 == 2)
            {
                setBoundingBox(1.0F - var8 * 2.0F, 0.2F, 0.5F - var8, 1.0F, 0.8F, 0.5F + var8);
            }
            else if (var7 == 3)
            {
                setBoundingBox(0.5F - var8, 0.2F, 0.0F, 0.5F + var8, 0.8F, var8 * 2.0F);
            }
            else if (var7 == 4)
            {
                setBoundingBox(0.5F - var8, 0.2F, 1.0F - var8 * 2.0F, 0.5F + var8, 0.8F, 1.0F);
            }
            else
            {
                var8 = 0.1F;
                setBoundingBox(0.5F - var8, 0.0F, 0.5F - var8, 0.5F + var8, 0.6F, 0.5F + var8);
            }

            return base.collisionRayTrace(var1, var2, var3, var4, var5, var6);
        }

        public override void randomDisplayTick(World var1, int var2, int var3, int var4, java.util.Random var5)
        {
            int var6 = var1.getBlockMeta(var2, var3, var4);
            double var7 = (double)((float)var2 + 0.5F);
            double var9 = (double)((float)var3 + 0.7F);
            double var11 = (double)((float)var4 + 0.5F);
            double var13 = (double)0.22F;
            double var15 = (double)0.27F;
            if (var6 == 1)
            {
                var1.addParticle("smoke", var7 - var15, var9 + var13, var11, 0.0D, 0.0D, 0.0D);
                var1.addParticle("flame", var7 - var15, var9 + var13, var11, 0.0D, 0.0D, 0.0D);
            }
            else if (var6 == 2)
            {
                var1.addParticle("smoke", var7 + var15, var9 + var13, var11, 0.0D, 0.0D, 0.0D);
                var1.addParticle("flame", var7 + var15, var9 + var13, var11, 0.0D, 0.0D, 0.0D);
            }
            else if (var6 == 3)
            {
                var1.addParticle("smoke", var7, var9 + var13, var11 - var15, 0.0D, 0.0D, 0.0D);
                var1.addParticle("flame", var7, var9 + var13, var11 - var15, 0.0D, 0.0D, 0.0D);
            }
            else if (var6 == 4)
            {
                var1.addParticle("smoke", var7, var9 + var13, var11 + var15, 0.0D, 0.0D, 0.0D);
                var1.addParticle("flame", var7, var9 + var13, var11 + var15, 0.0D, 0.0D, 0.0D);
            }
            else
            {
                var1.addParticle("smoke", var7, var9, var11, 0.0D, 0.0D, 0.0D);
                var1.addParticle("flame", var7, var9, var11, 0.0D, 0.0D, 0.0D);
            }

        }
    }

}