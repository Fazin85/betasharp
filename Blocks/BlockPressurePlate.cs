using betareborn.Entities;
using betareborn.Materials;
using betareborn.Worlds;

namespace betareborn.Blocks
{
    public class BlockPressurePlate : Block
    {

        private EnumMobType triggerMobType;

        public BlockPressurePlate(int var1, int var2, EnumMobType var3, Material var4) : base(var1, var2, var4)
        {
            triggerMobType = var3;
            setTickRandomly(true);
            float var5 = 1.0F / 16.0F;
            setBoundingBox(var5, 0.0F, var5, 1.0F - var5, 0.03125F, 1.0F - var5);
        }

        public override int tickRate()
        {
            return 20;
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

        public override bool canPlaceBlockAt(World var1, int var2, int var3, int var4)
        {
            return var1.shouldSuffocate(var2, var3 - 1, var4);
        }

        public override void onBlockAdded(World var1, int var2, int var3, int var4)
        {
        }

        public override void neighborUpdate(World var1, int var2, int var3, int var4, int var5)
        {
            bool var6 = false;
            if (!var1.shouldSuffocate(var2, var3 - 1, var4))
            {
                var6 = true;
            }

            if (var6)
            {
                dropBlockAsItem(var1, var2, var3, var4, var1.getBlockMeta(var2, var3, var4));
                var1.setBlockWithNotify(var2, var3, var4, 0);
            }

        }

        public override void onTick(World var1, int var2, int var3, int var4, java.util.Random var5)
        {
            if (!var1.multiplayerWorld)
            {
                if (var1.getBlockMeta(var2, var3, var4) != 0)
                {
                    setStateIfMobInteractsWithPlate(var1, var2, var3, var4);
                }
            }
        }

        public override void onEntityCollidedWithBlock(World var1, int var2, int var3, int var4, Entity var5)
        {
            if (!var1.multiplayerWorld)
            {
                if (var1.getBlockMeta(var2, var3, var4) != 1)
                {
                    setStateIfMobInteractsWithPlate(var1, var2, var3, var4);
                }
            }
        }

        private void setStateIfMobInteractsWithPlate(World var1, int var2, int var3, int var4)
        {
            bool var5 = var1.getBlockMeta(var2, var3, var4) == 1;
            bool var6 = false;
            float var7 = 2.0F / 16.0F;
            List<Entity> var8 = null;
            if (triggerMobType == EnumMobType.everything)
            {
                var8 = var1.getEntitiesWithinAABBExcludingEntity((Entity)null, Box.createCached((double)((float)var2 + var7), (double)var3, (double)((float)var4 + var7), (double)((float)(var2 + 1) - var7), (double)var3 + 0.25D, (double)((float)(var4 + 1) - var7)));
            }

            if (triggerMobType == EnumMobType.mobs)
            {
                var8 = var1.getEntitiesWithinAABB(EntityLiving.Class, Box.createCached((double)((float)var2 + var7), (double)var3, (double)((float)var4 + var7), (double)((float)(var2 + 1) - var7), (double)var3 + 0.25D, (double)((float)(var4 + 1) - var7)));
            }

            if (triggerMobType == EnumMobType.players)
            {
                var8 = var1.getEntitiesWithinAABB(EntityPlayer.Class, Box.createCached((double)((float)var2 + var7), (double)var3, (double)((float)var4 + var7), (double)((float)(var2 + 1) - var7), (double)var3 + 0.25D, (double)((float)(var4 + 1) - var7)));
            }

            if (var8.Count > 0)
            {
                var6 = true;
            }

            if (var6 && !var5)
            {
                var1.setBlockMeta(var2, var3, var4, 1);
                var1.notifyBlocksOfNeighborChange(var2, var3, var4, id);
                var1.notifyBlocksOfNeighborChange(var2, var3 - 1, var4, id);
                var1.markBlocksDirty(var2, var3, var4, var2, var3, var4);
                var1.playSoundEffect((double)var2 + 0.5D, (double)var3 + 0.1D, (double)var4 + 0.5D, "random.click", 0.3F, 0.6F);
            }

            if (!var6 && var5)
            {
                var1.setBlockMeta(var2, var3, var4, 0);
                var1.notifyBlocksOfNeighborChange(var2, var3, var4, id);
                var1.notifyBlocksOfNeighborChange(var2, var3 - 1, var4, id);
                var1.markBlocksDirty(var2, var3, var4, var2, var3, var4);
                var1.playSoundEffect((double)var2 + 0.5D, (double)var3 + 0.1D, (double)var4 + 0.5D, "random.click", 0.3F, 0.5F);
            }

            if (var6)
            {
                var1.scheduleBlockUpdate(var2, var3, var4, id, tickRate());
            }

        }

        public override void onBlockRemoval(World var1, int var2, int var3, int var4)
        {
            int var5 = var1.getBlockMeta(var2, var3, var4);
            if (var5 > 0)
            {
                var1.notifyBlocksOfNeighborChange(var2, var3, var4, id);
                var1.notifyBlocksOfNeighborChange(var2, var3 - 1, var4, id);
            }

            base.onBlockRemoval(var1, var2, var3, var4);
        }

        public override void updateBoundingBox(BlockView var1, int var2, int var3, int var4)
        {
            bool var5 = var1.getBlockMeta(var2, var3, var4) == 1;
            float var6 = 1.0F / 16.0F;
            if (var5)
            {
                setBoundingBox(var6, 0.0F, var6, 1.0F - var6, 0.03125F, 1.0F - var6);
            }
            else
            {
                setBoundingBox(var6, 0.0F, var6, 1.0F - var6, 1.0F / 16.0F, 1.0F - var6);
            }

        }

        public override bool isPoweringTo(BlockView var1, int var2, int var3, int var4, int var5)
        {
            return var1.getBlockMeta(var2, var3, var4) > 0;
        }

        public override bool isIndirectlyPoweringTo(World var1, int var2, int var3, int var4, int var5)
        {
            return var1.getBlockMeta(var2, var3, var4) == 0 ? false : var5 == 1;
        }

        public override bool canProvidePower()
        {
            return true;
        }

        public override void setBlockBoundsForItemRender()
        {
            float var1 = 0.5F;
            float var2 = 2.0F / 16.0F;
            float var3 = 0.5F;
            setBoundingBox(0.5F - var1, 0.5F - var2, 0.5F - var3, 0.5F + var1, 0.5F + var2, 0.5F + var3);
        }

        public override int getPistonBehavior()
        {
            return 1;
        }
    }

}