using betareborn.Chunks;
using betareborn.Entities;
using betareborn.Packets;
using betareborn.Worlds.Chunks;
using betareborn.Worlds.Dimensions;
using betareborn.Worlds.Storage;
using java.util;

namespace betareborn.Worlds
{
    public class ClientWorld : World
    {

        private readonly LinkedList blockResets = new LinkedList();
        private readonly NetClientHandler networkHandler;
        private MultiplayerChunkCache chunkCache;
        private readonly MCHash entitiesByNetworkId = new MCHash();
        private readonly Set forcedEntities = new HashSet();
        private readonly Set pendingEntities = new HashSet();

        public ClientWorld(NetClientHandler netHandler, long seed, int dimId) : base(new EmptyWorldStorage(), "MpServer", Dimension.fromId(dimId), seed)
        {
            networkHandler = netHandler;
            setSpawnPoint(new Vec3i(8, 64, 8));
            persistentStateManager = netHandler.clientPersistentStateManager;
        }

        public override void tick(int _)
        {
            setWorldTime(getTime() + 1L);
            int var1 = calculateSkylightSubtracted(1.0F);
            int var2;
            if (var1 != skylightSubtracted)
            {
                skylightSubtracted = var1;

                for (var2 = 0; var2 < worldAccesses.Count; ++var2)
                {
                    worldAccesses[var2].updateAllRenderers();
                }
            }

            for (var2 = 0; var2 < 10 && !pendingEntities.isEmpty(); ++var2)
            {
                Entity var3 = (Entity)pendingEntities.iterator().next();
                if (!loadedEntityList.Contains(var3))
                {
                    spawnEntity(var3);
                }
            }

            networkHandler.tick();

            for (var2 = 0; var2 < blockResets.size(); ++var2)
            {
                BlockReset var4 = (BlockReset)blockResets.get(var2);
                if (--var4.delay == 0)
                {
                    base.setBlockAndMetadata(var4.x, var4.y, var4.z, var4.block, var4.meta);
                    base.markBlockNeedsUpdate(var4.x, var4.y, var4.z);
                    blockResets.remove(var2--);
                }
            }

        }

        public void clearBlockResets(int var1, int var2, int var3, int var4, int var5, int var6)
        {
            for (int var7 = 0; var7 < blockResets.size(); ++var7)
            {
                BlockReset var8 = (BlockReset)blockResets.get(var7);
                if (var8.x >= var1 && var8.y >= var2 && var8.z >= var3 && var8.x <= var4 && var8.y <= var5 && var8.z <= var6)
                {
                    blockResets.remove(var7--);
                }
            }

        }

        protected override ChunkSource createChunkCache()
        {
            chunkCache = new MultiplayerChunkCache(this);
            return chunkCache;
        }

        public override void updateSpawnPosition()
        {
            setSpawnPoint(new Vec3i(8, 64, 8));
        }

        protected override void updateBlocksAndPlayCaveSounds()
        {
        }

        public override void scheduleBlockUpdate(int var1, int var2, int var3, int var4, int var5)
        {
        }

        public override bool TickUpdates(bool flush)
        {
            return false;
        }

        public void updateChunk(int chunkX, int chunkZ, bool load)
        {
            if (load)
            {
                chunkCache.loadChunk(chunkX, chunkZ);
            }
            else
            {
                chunkCache.unloadChunk(chunkX, chunkZ);
            }

            if (!load)
            {
                setBlocksDirty(chunkX * 16, 0, chunkZ * 16, chunkX * 16 + 15, 128, chunkZ * 16 + 15);
            }

        }

        public override bool spawnEntity(Entity entity)
        {
            bool var2 = base.spawnEntity(entity);
            forcedEntities.add(entity);
            if (!var2)
            {
                pendingEntities.add(entity);
            }

            return var2;
        }

        public override void setEntityDead(Entity var1)
        {
            base.setEntityDead(var1);
            forcedEntities.remove(var1);
        }

        protected override void obtainEntitySkin(Entity var1)
        {
            base.obtainEntitySkin(var1);
            if (pendingEntities.contains(var1))
            {
                pendingEntities.remove(var1);
            }

        }

        protected override void releaseEntitySkin(Entity var1)
        {
            base.releaseEntitySkin(var1);
            if (forcedEntities.contains(var1))
            {
                pendingEntities.add(var1);
            }

        }

        public void forceEntity(int var1, Entity var2)
        {
            Entity var3 = getEntity(var1);
            if (var3 != null)
            {
                setEntityDead(var3);
            }

            forcedEntities.add(var2);
            var2.entityId = var1;
            if (!spawnEntity(var2))
            {
                pendingEntities.add(var2);
            }

            entitiesByNetworkId.addKey(var1, var2);
        }

        public Entity getEntity(int var1)
        {
            return (Entity)entitiesByNetworkId.lookup(var1);
        }

        public Entity removeEntityFromWorld(int var1)
        {
            Entity var2 = (Entity)entitiesByNetworkId.removeObject(var1);
            if (var2 != null)
            {
                forcedEntities.remove(var2);
                setEntityDead(var2);
            }

            return var2;
        }

        public override bool setBlockMetadata(int var1, int var2, int var3, int var4)
        {
            int var5 = getBlockId(var1, var2, var3);
            int var6 = getBlockMeta(var1, var2, var3);
            if (base.setBlockMetadata(var1, var2, var3, var4))
            {
                blockResets.add(new BlockReset(this, var1, var2, var3, var5, var6));
                return true;
            }
            else
            {
                return false;
            }
        }

        public override bool setBlockAndMetadata(int var1, int var2, int var3, int var4, int var5)
        {
            int var6 = getBlockId(var1, var2, var3);
            int var7 = getBlockMeta(var1, var2, var3);
            if (base.setBlockAndMetadata(var1, var2, var3, var4, var5))
            {
                blockResets.add(new BlockReset(this, var1, var2, var3, var6, var7));
                return true;
            }
            else
            {
                return false;
            }
        }

        public override bool setBlock(int var1, int var2, int var3, int var4)
        {
            int var5 = getBlockId(var1, var2, var3);
            int var6 = getBlockMeta(var1, var2, var3);
            if (base.setBlock(var1, var2, var3, var4))
            {
                blockResets.add(new BlockReset(this, var1, var2, var3, var5, var6));
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool setBlockWithMetaFromPacket(int var1, int var2, int var3, int var4, int var5)
        {
            clearBlockResets(var1, var2, var3, var1, var2, var3);
            if (base.setBlockAndMetadata(var1, var2, var3, var4, var5))
            {
                notifyBlockChange(var1, var2, var3, var4);
                return true;
            }
            else
            {
                return false;
            }
        }

        public override void sendQuittingDisconnectingPacket()
        {
            networkHandler.sendPacketAndDisconnect(new Packet255KickDisconnect("Quitting"));
        }

        protected override void updateWeather()
        {
            if (!dimension.hasCeiling)
            {
                if (ticksSinceLightning > 0)
                {
                    --ticksSinceLightning;
                }

                prevRainingStrength = rainingStrength;
                if (worldInfo.getRaining())
                {
                    rainingStrength = (float)((double)rainingStrength + 0.01D);
                }
                else
                {
                    rainingStrength = (float)((double)rainingStrength - 0.01D);
                }

                if (rainingStrength < 0.0F)
                {
                    rainingStrength = 0.0F;
                }

                if (rainingStrength > 1.0F)
                {
                    rainingStrength = 1.0F;
                }

                prevThunderingStrength = thunderingStrength;
                if (worldInfo.getThundering())
                {
                    thunderingStrength = (float)((double)thunderingStrength + 0.01D);
                }
                else
                {
                    thunderingStrength = (float)((double)thunderingStrength - 0.01D);
                }

                if (thunderingStrength < 0.0F)
                {
                    thunderingStrength = 0.0F;
                }

                if (thunderingStrength > 1.0F)
                {
                    thunderingStrength = 1.0F;
                }

            }
        }
    }

}