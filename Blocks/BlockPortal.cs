using betareborn.Entities;
using betareborn.Materials;
using betareborn.Worlds;

namespace betareborn.Blocks
{
    public class BlockPortal : BlockBreakable
    {

        public BlockPortal(int var1, int var2) : base(var1, var2, Material.NETHER_PORTAL, false)
        {
        }

        public override Box getCollisionShape(World var1, int var2, int var3, int var4)
        {
            return null;
        }

        public override void updateBoundingBox(BlockView var1, int var2, int var3, int var4)
        {
            float var5;
            float var6;
            if (var1.getBlockId(var2 - 1, var3, var4) != id && var1.getBlockId(var2 + 1, var3, var4) != id)
            {
                var5 = 2.0F / 16.0F;
                var6 = 0.5F;
                setBoundingBox(0.5F - var5, 0.0F, 0.5F - var6, 0.5F + var5, 1.0F, 0.5F + var6);
            }
            else
            {
                var5 = 0.5F;
                var6 = 2.0F / 16.0F;
                setBoundingBox(0.5F - var5, 0.0F, 0.5F - var6, 0.5F + var5, 1.0F, 0.5F + var6);
            }

        }

        public override bool isOpaque()
        {
            return false;
        }

        public override bool isFullCube()
        {
            return false;
        }

        public bool tryToCreatePortal(World var1, int var2, int var3, int var4)
        {
            sbyte var5 = 0;
            sbyte var6 = 0;
            if (var1.getBlockId(var2 - 1, var3, var4) == Block.OBSIDIAN.id || var1.getBlockId(var2 + 1, var3, var4) == Block.OBSIDIAN.id)
            {
                var5 = 1;
            }

            if (var1.getBlockId(var2, var3, var4 - 1) == Block.OBSIDIAN.id || var1.getBlockId(var2, var3, var4 + 1) == Block.OBSIDIAN.id)
            {
                var6 = 1;
            }

            if (var5 == var6)
            {
                return false;
            }
            else
            {
                if (var1.getBlockId(var2 - var5, var3, var4 - var6) == 0)
                {
                    var2 -= var5;
                    var4 -= var6;
                }

                int var7;
                int var8;
                for (var7 = -1; var7 <= 2; ++var7)
                {
                    for (var8 = -1; var8 <= 3; ++var8)
                    {
                        bool var9 = var7 == -1 || var7 == 2 || var8 == -1 || var8 == 3;
                        if (var7 != -1 && var7 != 2 || var8 != -1 && var8 != 3)
                        {
                            int var10 = var1.getBlockId(var2 + var5 * var7, var3 + var8, var4 + var6 * var7);
                            if (var9)
                            {
                                if (var10 != Block.OBSIDIAN.id)
                                {
                                    return false;
                                }
                            }
                            else if (var10 != 0 && var10 != Block.FIRE.id)
                            {
                                return false;
                            }
                        }
                    }
                }

                var1.editingBlocks = true;

                for (var7 = 0; var7 < 2; ++var7)
                {
                    for (var8 = 0; var8 < 3; ++var8)
                    {
                        var1.setBlockWithNotify(var2 + var5 * var7, var3 + var8, var4 + var6 * var7, Block.NETHER_PORTAL.id);
                    }
                }

                var1.editingBlocks = false;
                return true;
            }
        }

        public override void neighborUpdate(World var1, int var2, int var3, int var4, int var5)
        {
            sbyte var6 = 0;
            sbyte var7 = 1;
            if (var1.getBlockId(var2 - 1, var3, var4) == id || var1.getBlockId(var2 + 1, var3, var4) == id)
            {
                var6 = 1;
                var7 = 0;
            }

            int var8;
            for (var8 = var3; var1.getBlockId(var2, var8 - 1, var4) == id; --var8)
            {
            }

            if (var1.getBlockId(var2, var8 - 1, var4) != Block.OBSIDIAN.id)
            {
                var1.setBlockWithNotify(var2, var3, var4, 0);
            }
            else
            {
                int var9;
                for (var9 = 1; var9 < 4 && var1.getBlockId(var2, var8 + var9, var4) == id; ++var9)
                {
                }

                if (var9 == 3 && var1.getBlockId(var2, var8 + var9, var4) == Block.OBSIDIAN.id)
                {
                    bool var10 = var1.getBlockId(var2 - 1, var3, var4) == id || var1.getBlockId(var2 + 1, var3, var4) == id;
                    bool var11 = var1.getBlockId(var2, var3, var4 - 1) == id || var1.getBlockId(var2, var3, var4 + 1) == id;
                    if (var10 && var11)
                    {
                        var1.setBlockWithNotify(var2, var3, var4, 0);
                    }
                    else if ((var1.getBlockId(var2 + var6, var3, var4 + var7) != Block.OBSIDIAN.id || var1.getBlockId(var2 - var6, var3, var4 - var7) != id) && (var1.getBlockId(var2 - var6, var3, var4 - var7) != Block.OBSIDIAN.id || var1.getBlockId(var2 + var6, var3, var4 + var7) != id))
                    {
                        var1.setBlockWithNotify(var2, var3, var4, 0);
                    }
                }
                else
                {
                    var1.setBlockWithNotify(var2, var3, var4, 0);
                }
            }
        }

        public override bool isSideVisible(BlockView var1, int var2, int var3, int var4, int var5)
        {
            if (var1.getBlockId(var2, var3, var4) == id)
            {
                return false;
            }
            else
            {
                bool var6 = var1.getBlockId(var2 - 1, var3, var4) == id && var1.getBlockId(var2 - 2, var3, var4) != id;
                bool var7 = var1.getBlockId(var2 + 1, var3, var4) == id && var1.getBlockId(var2 + 2, var3, var4) != id;
                bool var8 = var1.getBlockId(var2, var3, var4 - 1) == id && var1.getBlockId(var2, var3, var4 - 2) != id;
                bool var9 = var1.getBlockId(var2, var3, var4 + 1) == id && var1.getBlockId(var2, var3, var4 + 2) != id;
                bool var10 = var6 || var7;
                bool var11 = var8 || var9;
                return var10 && var5 == 4 ? true : (var10 && var5 == 5 ? true : (var11 && var5 == 2 ? true : var11 && var5 == 3));
            }
        }

        public override int quantityDropped(java.util.Random var1)
        {
            return 0;
        }

        public override int getRenderBlockPass()
        {
            return 1;
        }

        public override void onEntityCollidedWithBlock(World var1, int var2, int var3, int var4, Entity var5)
        {
            if (var5.ridingEntity == null && var5.riddenByEntity == null)
            {
                var5.setInPortal();
            }

        }

        public override void randomDisplayTick(World var1, int var2, int var3, int var4, java.util.Random var5)
        {
            if (var5.nextInt(100) == 0)
            {
                var1.playSoundEffect((double)var2 + 0.5D, (double)var3 + 0.5D, (double)var4 + 0.5D, "portal.portal", 1.0F, var5.nextFloat() * 0.4F + 0.8F);
            }

            for (int var6 = 0; var6 < 4; ++var6)
            {
                double var7 = (double)((float)var2 + var5.nextFloat());
                double var9 = (double)((float)var3 + var5.nextFloat());
                double var11 = (double)((float)var4 + var5.nextFloat());
                double var13 = 0.0D;
                double var15 = 0.0D;
                double var17 = 0.0D;
                int var19 = var5.nextInt(2) * 2 - 1;
                var13 = ((double)var5.nextFloat() - 0.5D) * 0.5D;
                var15 = ((double)var5.nextFloat() - 0.5D) * 0.5D;
                var17 = ((double)var5.nextFloat() - 0.5D) * 0.5D;
                if (var1.getBlockId(var2 - 1, var3, var4) != id && var1.getBlockId(var2 + 1, var3, var4) != id)
                {
                    var7 = (double)var2 + 0.5D + 0.25D * (double)var19;
                    var13 = (double)(var5.nextFloat() * 2.0F * (float)var19);
                }
                else
                {
                    var11 = (double)var4 + 0.5D + 0.25D * (double)var19;
                    var17 = (double)(var5.nextFloat() * 2.0F * (float)var19);
                }

                var1.addParticle("portal", var7, var9, var11, var13, var15, var17);
            }

        }
    }

}