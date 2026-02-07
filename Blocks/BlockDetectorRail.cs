using betareborn.Entities;
using betareborn.Worlds;

namespace betareborn.Blocks
{
    public class BlockDetectorRail : BlockRail
    {
        public BlockDetectorRail(int var1, int var2) : base(var1, var2, true)
        {
            setTickRandomly(true);
        }

        public override int tickRate()
        {
            return 20;
        }

        public override bool canProvidePower()
        {
            return true;
        }

        public override void onEntityCollidedWithBlock(World var1, int var2, int var3, int var4, Entity var5)
        {
            if (!var1.multiplayerWorld)
            {
                int var6 = var1.getBlockMeta(var2, var3, var4);
                if ((var6 & 8) == 0)
                {
                    setStateIfMinecartInteractsWithRail(var1, var2, var3, var4, var6);
                }
            }
        }

        public override void onTick(World var1, int var2, int var3, int var4, java.util.Random var5)
        {
            if (!var1.multiplayerWorld)
            {
                int var6 = var1.getBlockMeta(var2, var3, var4);
                if ((var6 & 8) != 0)
                {
                    setStateIfMinecartInteractsWithRail(var1, var2, var3, var4, var6);
                }
            }
        }

        public override bool isPoweringTo(BlockView var1, int var2, int var3, int var4, int var5)
        {
            return (var1.getBlockMeta(var2, var3, var4) & 8) != 0;
        }

        public override bool isIndirectlyPoweringTo(World var1, int var2, int var3, int var4, int var5)
        {
            return (var1.getBlockMeta(var2, var3, var4) & 8) == 0 ? false : var5 == 1;
        }

        private void setStateIfMinecartInteractsWithRail(World var1, int var2, int var3, int var4, int var5)
        {
            bool var6 = (var5 & 8) != 0;
            bool var7 = false;
            float var8 = 2.0F / 16.0F;
            var var9 = var1.getEntitiesWithinAABB(EntityMinecart.Class, Box.createCached((double)((float)var2 + var8), (double)var3, (double)((float)var4 + var8), (double)((float)(var2 + 1) - var8), (double)var3 + 0.25D, (double)((float)(var4 + 1) - var8)));
            if (var9.Count > 0)
            {
                var7 = true;
            }

            if (var7 && !var6)
            {
                var1.setBlockMeta(var2, var3, var4, var5 | 8);
                var1.notifyBlocksOfNeighborChange(var2, var3, var4, id);
                var1.notifyBlocksOfNeighborChange(var2, var3 - 1, var4, id);
                var1.markBlocksDirty(var2, var3, var4, var2, var3, var4);
            }

            if (!var7 && var6)
            {
                var1.setBlockMeta(var2, var3, var4, var5 & 7);
                var1.notifyBlocksOfNeighborChange(var2, var3, var4, id);
                var1.notifyBlocksOfNeighborChange(var2, var3 - 1, var4, id);
                var1.markBlocksDirty(var2, var3, var4, var2, var3, var4);
            }

            if (var7)
            {
                var1.scheduleBlockUpdate(var2, var3, var4, id, tickRate());
            }

        }
    }

}