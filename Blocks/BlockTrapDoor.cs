using betareborn.Entities;
using betareborn.Materials;
using betareborn.Worlds;

namespace betareborn.Blocks
{
    public class BlockTrapDoor : Block
    {

        public BlockTrapDoor(int var1, Material var2) : base(var1, var2)
        {
            textureId = 84;
            if (var2 == Material.METAL)
            {
                ++textureId;
            }

            float var3 = 0.5F;
            float var4 = 1.0F;
            setBoundingBox(0.5F - var3, 0.0F, 0.5F - var3, 0.5F + var3, var4, 0.5F + var3);
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
            return 0;
        }

        public override Box getBoundingBox(World var1, int var2, int var3, int var4)
        {
            updateBoundingBox(var1, var2, var3, var4);
            return base.getBoundingBox(var1, var2, var3, var4);
        }

        public override Box getCollisionShape(World var1, int var2, int var3, int var4)
        {
            updateBoundingBox(var1, var2, var3, var4);
            return base.getCollisionShape(var1, var2, var3, var4);
        }

        public override void updateBoundingBox(BlockView var1, int var2, int var3, int var4)
        {
            setBlockBoundsForBlockRender(var1.getBlockMeta(var2, var3, var4));
        }

        public override void setBlockBoundsForItemRender()
        {
            float var1 = 3.0F / 16.0F;
            setBoundingBox(0.0F, 0.5F - var1 / 2.0F, 0.0F, 1.0F, 0.5F + var1 / 2.0F, 1.0F);
        }

        public void setBlockBoundsForBlockRender(int var1)
        {
            float var2 = 3.0F / 16.0F;
            setBoundingBox(0.0F, 0.0F, 0.0F, 1.0F, var2, 1.0F);
            if (isTrapdoorOpen(var1))
            {
                if ((var1 & 3) == 0)
                {
                    setBoundingBox(0.0F, 0.0F, 1.0F - var2, 1.0F, 1.0F, 1.0F);
                }

                if ((var1 & 3) == 1)
                {
                    setBoundingBox(0.0F, 0.0F, 0.0F, 1.0F, 1.0F, var2);
                }

                if ((var1 & 3) == 2)
                {
                    setBoundingBox(1.0F - var2, 0.0F, 0.0F, 1.0F, 1.0F, 1.0F);
                }

                if ((var1 & 3) == 3)
                {
                    setBoundingBox(0.0F, 0.0F, 0.0F, var2, 1.0F, 1.0F);
                }
            }

        }

        public override void onBlockClicked(World var1, int var2, int var3, int var4, EntityPlayer var5)
        {
            onUse(var1, var2, var3, var4, var5);
        }

        public override bool onUse(World var1, int var2, int var3, int var4, EntityPlayer var5)
        {
            if (material == Material.METAL)
            {
                return true;
            }
            else
            {
                int var6 = var1.getBlockMeta(var2, var3, var4);
                var1.setBlockMeta(var2, var3, var4, var6 ^ 4);
                var1.func_28107_a(var5, 1003, var2, var3, var4, 0);
                return true;
            }
        }

        public void onPoweredBlockChange(World var1, int var2, int var3, int var4, bool var5)
        {
            int var6 = var1.getBlockMeta(var2, var3, var4);
            bool var7 = (var6 & 4) > 0;
            if (var7 != var5)
            {
                var1.setBlockMeta(var2, var3, var4, var6 ^ 4);
                var1.func_28107_a((EntityPlayer)null, 1003, var2, var3, var4, 0);
            }
        }

        public override void neighborUpdate(World var1, int var2, int var3, int var4, int var5)
        {
            if (!var1.multiplayerWorld)
            {
                int var6 = var1.getBlockMeta(var2, var3, var4);
                int var7 = var2;
                int var8 = var4;
                if ((var6 & 3) == 0)
                {
                    var8 = var4 + 1;
                }

                if ((var6 & 3) == 1)
                {
                    --var8;
                }

                if ((var6 & 3) == 2)
                {
                    var7 = var2 + 1;
                }

                if ((var6 & 3) == 3)
                {
                    --var7;
                }

                if (!var1.shouldSuffocate(var7, var3, var8))
                {
                    var1.setBlockWithNotify(var2, var3, var4, 0);
                    dropBlockAsItem(var1, var2, var3, var4, var6);
                }

                if (var5 > 0 && Block.BLOCKS[var5].canProvidePower())
                {
                    bool var9 = var1.isBlockIndirectlyGettingPowered(var2, var3, var4);
                    onPoweredBlockChange(var1, var2, var3, var4, var9);
                }

            }
        }

        public override MovingObjectPosition collisionRayTrace(World var1, int var2, int var3, int var4, Vec3D var5, Vec3D var6)
        {
            updateBoundingBox(var1, var2, var3, var4);
            return base.collisionRayTrace(var1, var2, var3, var4, var5, var6);
        }

        public override void onBlockPlaced(World var1, int var2, int var3, int var4, int var5)
        {
            sbyte var6 = 0;
            if (var5 == 2)
            {
                var6 = 0;
            }

            if (var5 == 3)
            {
                var6 = 1;
            }

            if (var5 == 4)
            {
                var6 = 2;
            }

            if (var5 == 5)
            {
                var6 = 3;
            }

            var1.setBlockMeta(var2, var3, var4, var6);
        }

        public override bool canPlaceBlockOnSide(World var1, int var2, int var3, int var4, int var5)
        {
            if (var5 == 0)
            {
                return false;
            }
            else if (var5 == 1)
            {
                return false;
            }
            else
            {
                if (var5 == 2)
                {
                    ++var4;
                }

                if (var5 == 3)
                {
                    --var4;
                }

                if (var5 == 4)
                {
                    ++var2;
                }

                if (var5 == 5)
                {
                    --var2;
                }

                return var1.shouldSuffocate(var2, var3, var4);
            }
        }

        public static bool isTrapdoorOpen(int var0)
        {
            return (var0 & 4) != 0;
        }
    }

}