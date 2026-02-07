using betareborn.Entities;
using betareborn.Materials;
using betareborn.Worlds;

namespace betareborn.Blocks
{
    public class BlockButton : Block
    {
        public BlockButton(int var1, int var2) : base(var1, var2, Material.PISTON_BREAKABLE)
        {
            setTickRandomly(true);
        }

        public override Box getCollisionShape(World var1, int var2, int var3, int var4)
        {
            return null;
        }

        public override int tickRate()
        {
            return 20;
        }

        public override bool isOpaque()
        {
            return false;
        }

        public override bool isFullCube()
        {
            return false;
        }

        public override bool canPlaceBlockOnSide(World var1, int var2, int var3, int var4, int var5)
        {
            return var5 == 2 && var1.shouldSuffocate(var2, var3, var4 + 1) ? true : (var5 == 3 && var1.shouldSuffocate(var2, var3, var4 - 1) ? true : (var5 == 4 && var1.shouldSuffocate(var2 + 1, var3, var4) ? true : var5 == 5 && var1.shouldSuffocate(var2 - 1, var3, var4)));
        }

        public override bool canPlaceBlockAt(World var1, int var2, int var3, int var4)
        {
            return var1.shouldSuffocate(var2 - 1, var3, var4) ? true : (var1.shouldSuffocate(var2 + 1, var3, var4) ? true : (var1.shouldSuffocate(var2, var3, var4 - 1) ? true : var1.shouldSuffocate(var2, var3, var4 + 1)));
        }

        public override void onBlockPlaced(World var1, int var2, int var3, int var4, int var5)
        {
            int var6 = var1.getBlockMeta(var2, var3, var4);
            int var7 = var6 & 8;
            var6 &= 7;
            if (var5 == 2 && var1.shouldSuffocate(var2, var3, var4 + 1))
            {
                var6 = 4;
            }
            else if (var5 == 3 && var1.shouldSuffocate(var2, var3, var4 - 1))
            {
                var6 = 3;
            }
            else if (var5 == 4 && var1.shouldSuffocate(var2 + 1, var3, var4))
            {
                var6 = 2;
            }
            else if (var5 == 5 && var1.shouldSuffocate(var2 - 1, var3, var4))
            {
                var6 = 1;
            }
            else
            {
                var6 = getOrientation(var1, var2, var3, var4);
            }

            var1.setBlockMeta(var2, var3, var4, var6 + var7);
        }

        private int getOrientation(World var1, int var2, int var3, int var4)
        {
            return var1.shouldSuffocate(var2 - 1, var3, var4) ? 1 : (var1.shouldSuffocate(var2 + 1, var3, var4) ? 2 : (var1.shouldSuffocate(var2, var3, var4 - 1) ? 3 : (var1.shouldSuffocate(var2, var3, var4 + 1) ? 4 : 1)));
        }

        public override void neighborUpdate(World var1, int var2, int var3, int var4, int var5)
        {
            if (func_305_h(var1, var2, var3, var4))
            {
                int var6 = var1.getBlockMeta(var2, var3, var4) & 7;
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

                if (var7)
                {
                    dropBlockAsItem(var1, var2, var3, var4, var1.getBlockMeta(var2, var3, var4));
                    var1.setBlockWithNotify(var2, var3, var4, 0);
                }
            }

        }

        private bool func_305_h(World var1, int var2, int var3, int var4)
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

        public override void updateBoundingBox(BlockView var1, int var2, int var3, int var4)
        {
            int var5 = var1.getBlockMeta(var2, var3, var4);
            int var6 = var5 & 7;
            bool var7 = (var5 & 8) > 0;
            float var8 = 6.0F / 16.0F;
            float var9 = 10.0F / 16.0F;
            float var10 = 3.0F / 16.0F;
            float var11 = 2.0F / 16.0F;
            if (var7)
            {
                var11 = 1.0F / 16.0F;
            }

            if (var6 == 1)
            {
                setBoundingBox(0.0F, var8, 0.5F - var10, var11, var9, 0.5F + var10);
            }
            else if (var6 == 2)
            {
                setBoundingBox(1.0F - var11, var8, 0.5F - var10, 1.0F, var9, 0.5F + var10);
            }
            else if (var6 == 3)
            {
                setBoundingBox(0.5F - var10, var8, 0.0F, 0.5F + var10, var9, var11);
            }
            else if (var6 == 4)
            {
                setBoundingBox(0.5F - var10, var8, 1.0F - var11, 0.5F + var10, var9, 1.0F);
            }

        }

        public override void onBlockClicked(World var1, int var2, int var3, int var4, EntityPlayer var5)
        {
            onUse(var1, var2, var3, var4, var5);
        }

        public override bool onUse(World var1, int var2, int var3, int var4, EntityPlayer var5)
        {
            int var6 = var1.getBlockMeta(var2, var3, var4);
            int var7 = var6 & 7;
            int var8 = 8 - (var6 & 8);
            if (var8 == 0)
            {
                return true;
            }
            else
            {
                var1.setBlockMeta(var2, var3, var4, var7 + var8);
                var1.markBlocksDirty(var2, var3, var4, var2, var3, var4);
                var1.playSoundEffect((double)var2 + 0.5D, (double)var3 + 0.5D, (double)var4 + 0.5D, "random.click", 0.3F, 0.6F);
                var1.notifyBlocksOfNeighborChange(var2, var3, var4, id);
                if (var7 == 1)
                {
                    var1.notifyBlocksOfNeighborChange(var2 - 1, var3, var4, id);
                }
                else if (var7 == 2)
                {
                    var1.notifyBlocksOfNeighborChange(var2 + 1, var3, var4, id);
                }
                else if (var7 == 3)
                {
                    var1.notifyBlocksOfNeighborChange(var2, var3, var4 - 1, id);
                }
                else if (var7 == 4)
                {
                    var1.notifyBlocksOfNeighborChange(var2, var3, var4 + 1, id);
                }
                else
                {
                    var1.notifyBlocksOfNeighborChange(var2, var3 - 1, var4, id);
                }

                var1.scheduleBlockUpdate(var2, var3, var4, id, tickRate());
                return true;
            }
        }

        public override void onBlockRemoval(World var1, int var2, int var3, int var4)
        {
            int var5 = var1.getBlockMeta(var2, var3, var4);
            if ((var5 & 8) > 0)
            {
                var1.notifyBlocksOfNeighborChange(var2, var3, var4, id);
                int var6 = var5 & 7;
                if (var6 == 1)
                {
                    var1.notifyBlocksOfNeighborChange(var2 - 1, var3, var4, id);
                }
                else if (var6 == 2)
                {
                    var1.notifyBlocksOfNeighborChange(var2 + 1, var3, var4, id);
                }
                else if (var6 == 3)
                {
                    var1.notifyBlocksOfNeighborChange(var2, var3, var4 - 1, id);
                }
                else if (var6 == 4)
                {
                    var1.notifyBlocksOfNeighborChange(var2, var3, var4 + 1, id);
                }
                else
                {
                    var1.notifyBlocksOfNeighborChange(var2, var3 - 1, var4, id);
                }
            }

            base.onBlockRemoval(var1, var2, var3, var4);
        }

        public override bool isPoweringTo(BlockView var1, int var2, int var3, int var4, int var5)
        {
            return (var1.getBlockMeta(var2, var3, var4) & 8) > 0;
        }

        public override bool isIndirectlyPoweringTo(World var1, int var2, int var3, int var4, int var5)
        {
            int var6 = var1.getBlockMeta(var2, var3, var4);
            if ((var6 & 8) == 0)
            {
                return false;
            }
            else
            {
                int var7 = var6 & 7;
                return var7 == 5 && var5 == 1 ? true : (var7 == 4 && var5 == 2 ? true : (var7 == 3 && var5 == 3 ? true : (var7 == 2 && var5 == 4 ? true : var7 == 1 && var5 == 5)));
            }
        }

        public override bool canProvidePower()
        {
            return true;
        }

        public override void onTick(World var1, int var2, int var3, int var4, java.util.Random var5)
        {
            if (!var1.multiplayerWorld)
            {
                int var6 = var1.getBlockMeta(var2, var3, var4);
                if ((var6 & 8) != 0)
                {
                    var1.setBlockMeta(var2, var3, var4, var6 & 7);
                    var1.notifyBlocksOfNeighborChange(var2, var3, var4, id);
                    int var7 = var6 & 7;
                    if (var7 == 1)
                    {
                        var1.notifyBlocksOfNeighborChange(var2 - 1, var3, var4, id);
                    }
                    else if (var7 == 2)
                    {
                        var1.notifyBlocksOfNeighborChange(var2 + 1, var3, var4, id);
                    }
                    else if (var7 == 3)
                    {
                        var1.notifyBlocksOfNeighborChange(var2, var3, var4 - 1, id);
                    }
                    else if (var7 == 4)
                    {
                        var1.notifyBlocksOfNeighborChange(var2, var3, var4 + 1, id);
                    }
                    else
                    {
                        var1.notifyBlocksOfNeighborChange(var2, var3 - 1, var4, id);
                    }

                    var1.playSoundEffect((double)var2 + 0.5D, (double)var3 + 0.5D, (double)var4 + 0.5D, "random.click", 0.3F, 0.5F);
                    var1.markBlocksDirty(var2, var3, var4, var2, var3, var4);
                }
            }
        }

        public override void setBlockBoundsForItemRender()
        {
            float var1 = 3.0F / 16.0F;
            float var2 = 2.0F / 16.0F;
            float var3 = 2.0F / 16.0F;
            setBoundingBox(0.5F - var1, 0.5F - var2, 0.5F - var3, 0.5F + var1, 0.5F + var2, 0.5F + var3);
        }
    }

}