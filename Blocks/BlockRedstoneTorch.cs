using betareborn.Worlds;
using java.util;

namespace betareborn.Blocks
{
    public class BlockRedstoneTorch : BlockTorch
    {

        private bool torchActive = false;
        private static List torchUpdates = new ArrayList();

        public override int getTexture(int var1, int var2)
        {
            return var1 == 1 ? Block.REDSTONE_WIRE.getTexture(var1, var2) : base.getTexture(var1, var2);
        }

        private bool checkForBurnout(World var1, int var2, int var3, int var4, bool var5)
        {
            if (var5)
            {
                torchUpdates.add(new RedstoneUpdateInfo(var2, var3, var4, var1.getWorldTime()));
            }

            int var6 = 0;

            for (int var7 = 0; var7 < torchUpdates.size(); ++var7)
            {
                RedstoneUpdateInfo var8 = (RedstoneUpdateInfo)torchUpdates.get(var7);
                if (var8.x == var2 && var8.y == var3 && var8.z == var4)
                {
                    ++var6;
                    if (var6 >= 8)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public BlockRedstoneTorch(int var1, int var2, bool var3) : base(var1, var2)
        {
            torchActive = var3;
            setTickRandomly(true);
        }

        public override int tickRate()
        {
            return 2;
        }

        public override void onBlockAdded(World var1, int var2, int var3, int var4)
        {
            if (var1.getBlockMeta(var2, var3, var4) == 0)
            {
                base.onBlockAdded(var1, var2, var3, var4);
            }

            if (torchActive)
            {
                var1.notifyBlocksOfNeighborChange(var2, var3 - 1, var4, id);
                var1.notifyBlocksOfNeighborChange(var2, var3 + 1, var4, id);
                var1.notifyBlocksOfNeighborChange(var2 - 1, var3, var4, id);
                var1.notifyBlocksOfNeighborChange(var2 + 1, var3, var4, id);
                var1.notifyBlocksOfNeighborChange(var2, var3, var4 - 1, id);
                var1.notifyBlocksOfNeighborChange(var2, var3, var4 + 1, id);
            }

        }

        public override void onBlockRemoval(World var1, int var2, int var3, int var4)
        {
            if (torchActive)
            {
                var1.notifyBlocksOfNeighborChange(var2, var3 - 1, var4, id);
                var1.notifyBlocksOfNeighborChange(var2, var3 + 1, var4, id);
                var1.notifyBlocksOfNeighborChange(var2 - 1, var3, var4, id);
                var1.notifyBlocksOfNeighborChange(var2 + 1, var3, var4, id);
                var1.notifyBlocksOfNeighborChange(var2, var3, var4 - 1, id);
                var1.notifyBlocksOfNeighborChange(var2, var3, var4 + 1, id);
            }

        }

        public override bool isPoweringTo(BlockView var1, int var2, int var3, int var4, int var5)
        {
            if (!torchActive)
            {
                return false;
            }
            else
            {
                int var6 = var1.getBlockMeta(var2, var3, var4);
                return var6 == 5 && var5 == 1 ? false : (var6 == 3 && var5 == 3 ? false : (var6 == 4 && var5 == 2 ? false : (var6 == 1 && var5 == 5 ? false : var6 != 2 || var5 != 4)));
            }
        }

        private bool func_30002_h(World var1, int var2, int var3, int var4)
        {
            int var5 = var1.getBlockMeta(var2, var3, var4);
            return var5 == 5 && var1.isBlockIndirectlyProvidingPowerTo(var2, var3 - 1, var4, 0) ? true : (var5 == 3 && var1.isBlockIndirectlyProvidingPowerTo(var2, var3, var4 - 1, 2) ? true : (var5 == 4 && var1.isBlockIndirectlyProvidingPowerTo(var2, var3, var4 + 1, 3) ? true : (var5 == 1 && var1.isBlockIndirectlyProvidingPowerTo(var2 - 1, var3, var4, 4) ? true : var5 == 2 && var1.isBlockIndirectlyProvidingPowerTo(var2 + 1, var3, var4, 5))));
        }

        public override void onTick(World var1, int var2, int var3, int var4, java.util.Random var5)
        {
            bool var6 = func_30002_h(var1, var2, var3, var4);

            while (torchUpdates.size() > 0 && var1.getWorldTime() - ((RedstoneUpdateInfo)torchUpdates.get(0)).updateTime > 100L)
            {
                torchUpdates.remove(0);
            }

            if (torchActive)
            {
                if (var6)
                {
                    var1.setBlockAndMetadataWithNotify(var2, var3, var4, Block.REDSTONE_TORCH.id, var1.getBlockMeta(var2, var3, var4));
                    if (checkForBurnout(var1, var2, var3, var4, true))
                    {
                        var1.playSoundEffect((double)((float)var2 + 0.5F), (double)((float)var3 + 0.5F), (double)((float)var4 + 0.5F), "random.fizz", 0.5F, 2.6F + (var1.random.nextFloat() - var1.random.nextFloat()) * 0.8F);

                        for (int var7 = 0; var7 < 5; ++var7)
                        {
                            double var8 = (double)var2 + var5.nextDouble() * 0.6D + 0.2D;
                            double var10 = (double)var3 + var5.nextDouble() * 0.6D + 0.2D;
                            double var12 = (double)var4 + var5.nextDouble() * 0.6D + 0.2D;
                            var1.addParticle("smoke", var8, var10, var12, 0.0D, 0.0D, 0.0D);
                        }
                    }
                }
            }
            else if (!var6 && !checkForBurnout(var1, var2, var3, var4, false))
            {
                var1.setBlockAndMetadataWithNotify(var2, var3, var4, Block.LIT_REDSTONE_TORCH.id, var1.getBlockMeta(var2, var3, var4));
            }

        }

        public override void neighborUpdate(World var1, int var2, int var3, int var4, int var5)
        {
            base.neighborUpdate(var1, var2, var3, var4, var5);
            var1.scheduleBlockUpdate(var2, var3, var4, id, tickRate());
        }

        public override bool isIndirectlyPoweringTo(World var1, int var2, int var3, int var4, int var5)
        {
            return var5 == 0 ? isPoweringTo(var1, var2, var3, var4, var5) : false;
        }

        public override int getDroppedItemId(int var1, java.util.Random var2)
        {
            return Block.LIT_REDSTONE_TORCH.id;
        }

        public override bool canProvidePower()
        {
            return true;
        }

        public override void randomDisplayTick(World var1, int var2, int var3, int var4, java.util.Random var5)
        {
            if (torchActive)
            {
                int var6 = var1.getBlockMeta(var2, var3, var4);
                double var7 = (double)((float)var2 + 0.5F) + (double)(var5.nextFloat() - 0.5F) * 0.2D;
                double var9 = (double)((float)var3 + 0.7F) + (double)(var5.nextFloat() - 0.5F) * 0.2D;
                double var11 = (double)((float)var4 + 0.5F) + (double)(var5.nextFloat() - 0.5F) * 0.2D;
                double var13 = (double)0.22F;
                double var15 = (double)0.27F;
                if (var6 == 1)
                {
                    var1.addParticle("reddust", var7 - var15, var9 + var13, var11, 0.0D, 0.0D, 0.0D);
                }
                else if (var6 == 2)
                {
                    var1.addParticle("reddust", var7 + var15, var9 + var13, var11, 0.0D, 0.0D, 0.0D);
                }
                else if (var6 == 3)
                {
                    var1.addParticle("reddust", var7, var9 + var13, var11 - var15, 0.0D, 0.0D, 0.0D);
                }
                else if (var6 == 4)
                {
                    var1.addParticle("reddust", var7, var9 + var13, var11 + var15, 0.0D, 0.0D, 0.0D);
                }
                else
                {
                    var1.addParticle("reddust", var7, var9, var11, 0.0D, 0.0D, 0.0D);
                }

            }
        }
    }

}