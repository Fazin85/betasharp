using betareborn.Blocks;
using betareborn.Entities;
using betareborn.Items;
using betareborn.Network.Packets.S2CPlay;
using betareborn.Worlds;

namespace betareborn.Server.Network
{
    public class ServerPlayerInteractionManager
    {
        private readonly ServerWorld world;
        public EntityPlayer player;
        private int failedMiningStartTime;
        private int failedMiningX;
        private int failedMiningY;
        private int failedMiningZ;
        private int tickCounter;
        private bool mining;
        private int miningX;
        private int miningY;
        private int miningZ;
        private int startMiningTime;

        public ServerPlayerInteractionManager(ServerWorld world)
        {
            this.world = world;
        }

        public void update()
        {
            tickCounter++;
            if (mining)
            {
                int var1 = tickCounter - startMiningTime;
                int var2 = world.GetBlockId(miningX, miningY, miningZ);
                if (var2 != 0)
                {
                    Block var3 = Block.BLOCKS[var2];
                    float var4 = var3.getHardness(player) * (var1 + 1);
                    if (var4 >= 1.0F)
                    {
                        mining = false;
                        tryBreakBlock(miningX, miningY, miningZ);
                    }
                }
                else
                {
                    mining = false;
                }
            }
        }

        public void onBlockBreakingAction(int x, int y, int z, int direction)
        {
            world.extinguishFire(null, x, y, z, direction);
            failedMiningStartTime = tickCounter;
            int var5 = world.GetBlockId(x, y, z);
            if (var5 > 0)
            {
                Block.BLOCKS[var5].onBlockBreakStart(world, x, y, z, player);
            }

            if (var5 > 0 && Block.BLOCKS[var5].getHardness(player) >= 1.0F)
            {
                tryBreakBlock(x, y, z);
            }
            else
            {
                failedMiningX = x;
                failedMiningY = y;
                failedMiningZ = z;
            }
        }

        public void continueMining(int x, int y, int z)
        {
            if (x == failedMiningX && y == failedMiningY && z == failedMiningZ)
            {
                int var4 = tickCounter - failedMiningStartTime;
                int var5 = world.GetBlockId(x, y, z);
                if (var5 != 0)
                {
                    Block var6 = Block.BLOCKS[var5];
                    float var7 = var6.getHardness(player) * (var4 + 1);
                    if (var7 >= 0.7F)
                    {
                        tryBreakBlock(x, y, z);
                    }
                    else if (!mining)
                    {
                        mining = true;
                        miningX = x;
                        miningY = y;
                        miningZ = z;
                        startMiningTime = failedMiningStartTime;
                    }
                }
            }
        }

        public bool finishMining(int x, int y, int z)
        {
            Block var4 = Block.BLOCKS[world.GetBlockId(x, y, z)];
            int var5 = world.GetBlockMeta(x, y, z);
            bool var6 = world.setBlock(x, y, z, 0);
            if (var4 != null && var6)
            {
                var4.onMetadataChange(world, x, y, z, var5);
            }

            return var6;
        }

        public bool tryBreakBlock(int x, int y, int z)
        {
            int blockId = world.GetBlockId(x, y, z);
            int var5 = world.GetBlockMeta(x, y, z);
            world.worldEvent(player, 2001, x, y, z, blockId + world.GetBlockMeta(x, y, z) * 256);
            bool var6 = finishMining(x, y, z);
            ItemStack var7 = player.getHand();
            if (var7 != null)
            {
                var7.postMine(blockId, x, y, z, player);
                if (var7.count == 0)
                {
                    var7.onRemoved(player);
                    player.clearStackInHand();
                }
            }

            if (var6 && player.canHarvest(Block.BLOCKS[blockId]))
            {
                Block.BLOCKS[blockId].afterBreak(world, player, x, y, z, var5);
                ((ServerPlayerEntity)player).networkHandler.sendPacket(new BlockUpdateS2CPacket(x, y, z, world));
            }

            return var6;
        }

        public bool interactItem(EntityPlayer player, World world, ItemStack stack)
        {
            int var4 = stack.count;
            ItemStack var5 = stack.use(world, player);
            if (var5 != stack || var5 != null && var5.count != var4)
            {
                player.inventory.main[player.inventory.selectedSlot] = var5;
                if (var5.count == 0)
                {
                    player.inventory.main[player.inventory.selectedSlot] = null;
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        public bool interactBlock(EntityPlayer player, World world, ItemStack stack, int x, int y, int z, int side)
        {
            int var8 = world.GetBlockId(x, y, z);
            if (var8 > 0 && Block.BLOCKS[var8].onUse(world, x, y, z, player))
            {
                return true;
            }
            else
            {
                return stack == null ? false : stack.useOnBlock(player, world, x, y, z, side);
            }
        }
    }
}
