using betareborn.Entities;
using betareborn.Materials;
using betareborn.Worlds;

namespace betareborn.Blocks
{
    public class BlockLever : Block
    {

        public BlockLever(int var1, int var2) : base(var1, var2, Material.PISTON_BREAKABLE)
        {
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
            return 12;
        }

        public override bool canPlaceBlockOnSide(World var1, int var2, int var3, int var4, int var5)
        {
            return var5 == 1 && var1.shouldSuffocate(var2, var3 - 1, var4) ? true : (var5 == 2 && var1.shouldSuffocate(var2, var3, var4 + 1) ? true : (var5 == 3 && var1.shouldSuffocate(var2, var3, var4 - 1) ? true : (var5 == 4 && var1.shouldSuffocate(var2 + 1, var3, var4) ? true : var5 == 5 && var1.shouldSuffocate(var2 - 1, var3, var4))));
        }

        public override bool canPlaceBlockAt(World var1, int var2, int var3, int var4)
        {
            return var1.shouldSuffocate(var2 - 1, var3, var4) ? true : (var1.shouldSuffocate(var2 + 1, var3, var4) ? true : (var1.shouldSuffocate(var2, var3, var4 - 1) ? true : (var1.shouldSuffocate(var2, var3, var4 + 1) ? true : var1.shouldSuffocate(var2, var3 - 1, var4))));
        }

        public override void onBlockPlaced(World var1, int var2, int var3, int var4, int var5)
        {
            int var6 = var1.getBlockMeta(var2, var3, var4);
            int var7 = var6 & 8;
            var6 &= 7;
            var6 = -1;
            if (var5 == 1 && var1.shouldSuffocate(var2, var3 - 1, var4))
            {
                var6 = 5 + var1.random.nextInt(2);
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

            if (var6 == -1)
            {
                dropBlockAsItem(var1, var2, var3, var4, var1.getBlockMeta(var2, var3, var4));
                var1.setBlockWithNotify(var2, var3, var4, 0);
            }
            else
            {
                var1.setBlockMeta(var2, var3, var4, var6 + var7);
            }
        }

        public override void neighborUpdate(World var1, int var2, int var3, int var4, int var5)
        {
            if (checkIfAttachedToBlock(var1, var2, var3, var4))
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

                if (!var1.shouldSuffocate(var2, var3 - 1, var4) && var6 == 5)
                {
                    var7 = true;
                }

                if (!var1.shouldSuffocate(var2, var3 - 1, var4) && var6 == 6)
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

        private bool checkIfAttachedToBlock(World var1, int var2, int var3, int var4)
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
            int var5 = var1.getBlockMeta(var2, var3, var4) & 7;
            float var6 = 3.0F / 16.0F;
            if (var5 == 1)
            {
                setBoundingBox(0.0F, 0.2F, 0.5F - var6, var6 * 2.0F, 0.8F, 0.5F + var6);
            }
            else if (var5 == 2)
            {
                setBoundingBox(1.0F - var6 * 2.0F, 0.2F, 0.5F - var6, 1.0F, 0.8F, 0.5F + var6);
            }
            else if (var5 == 3)
            {
                setBoundingBox(0.5F - var6, 0.2F, 0.0F, 0.5F + var6, 0.8F, var6 * 2.0F);
            }
            else if (var5 == 4)
            {
                setBoundingBox(0.5F - var6, 0.2F, 1.0F - var6 * 2.0F, 0.5F + var6, 0.8F, 1.0F);
            }
            else
            {
                var6 = 0.25F;
                setBoundingBox(0.5F - var6, 0.0F, 0.5F - var6, 0.5F + var6, 0.6F, 0.5F + var6);
            }

        }

        public override void onBlockClicked(World var1, int var2, int var3, int var4, EntityPlayer var5)
        {
            onUse(var1, var2, var3, var4, var5);
        }

        public override bool onUse(World var1, int var2, int var3, int var4, EntityPlayer var5)
        {
            if (var1.multiplayerWorld)
            {
                return true;
            }
            else
            {
                int var6 = var1.getBlockMeta(var2, var3, var4);
                int var7 = var6 & 7;
                int var8 = 8 - (var6 & 8);
                var1.setBlockMeta(var2, var3, var4, var7 + var8);
                var1.markBlocksDirty(var2, var3, var4, var2, var3, var4);
                var1.playSoundEffect((double)var2 + 0.5D, (double)var3 + 0.5D, (double)var4 + 0.5D, "random.click", 0.3F, var8 > 0 ? 0.6F : 0.5F);
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
                return var7 == 6 && var5 == 1 ? true : (var7 == 5 && var5 == 1 ? true : (var7 == 4 && var5 == 2 ? true : (var7 == 3 && var5 == 3 ? true : (var7 == 2 && var5 == 4 ? true : var7 == 1 && var5 == 5))));
            }
        }

        public override bool canProvidePower()
        {
            return true;
        }
    }

}