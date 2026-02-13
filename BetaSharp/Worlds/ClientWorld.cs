using BetaSharp.Client.Chunks;
using BetaSharp.Client.Network;
using BetaSharp.Entities;
using BetaSharp.Network.Packets.Play;
using BetaSharp.Util;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds.Chunks;
using BetaSharp.Worlds.Dimensions;
using BetaSharp.Worlds.Storage;
using java.util;

namespace BetaSharp.Worlds;

public class ClientWorld : World
{

    private readonly LinkedList blockResets = new LinkedList();
    private readonly ClientNetworkHandler networkHandler;
    private MultiplayerChunkCache chunkCache;
    private readonly IntHashMap entitiesByNetworkId = new IntHashMap();
    private readonly Set forcedEntities = new HashSet();
    private readonly Set pendingEntities = new HashSet();

    public ClientWorld(ClientNetworkHandler netHandler, long seed, int dimId) : base(new EmptyWorldStorage(), "MpServer", Dimension.fromId(dimId), seed)
    {
        networkHandler = netHandler;
        setSpawnPos(new Vec3i(8, 64, 8));
        persistentStateManager = netHandler.clientPersistentStateManager;
    }

    public override void tick(int _)
    {
        setTime(getTime() + 1L);
        int var1 = getAmbientDarkness(1.0F);
        int var2;
        if (var1 != ambientDarkness)
        {
            ambientDarkness = var1;

            for (var2 = 0; var2 < eventListeners.Count; ++var2)
            {
                eventListeners[var2].notifyAmbientDarknessChanged();
            }
        }

        for (var2 = 0; var2 < 10 && !pendingEntities.isEmpty(); ++var2)
        {
            Entity var3 = (Entity)pendingEntities.iterator().next();
            if (!entities.Contains(var3))
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
                base.setBlockWithoutNotifyingNeighbors(var4.x, var4.y, var4.z, var4.block, var4.meta);
                base.blockUpdateEvent(var4.x, var4.y, var4.z);
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
        setSpawnPos(new Vec3i(8, 64, 8));
    }

    protected override void manageChunkUpdatesAndEvents()
    {
    }

    public override void scheduleBlockUpdate(int var1, int var2, int var3, int var4, int var5)
    {
    }

    public override bool processScheduledTicks(bool flush)
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

    public override void remove(Entity var1)
    {
        base.remove(var1);
        forcedEntities.remove(var1);
    }

    protected override void notifyEntityAdded(Entity var1)
    {
        base.notifyEntityAdded(var1);
        if (pendingEntities.contains(var1))
        {
            pendingEntities.remove(var1);
        }

    }

    protected override void notifyEntityRemoved(Entity var1)
    {
        base.notifyEntityRemoved(var1);
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
            remove(var3);
        }

        forcedEntities.add(var2);
        var2.id = var1;
        if (!spawnEntity(var2))
        {
            pendingEntities.add(var2);
        }

        entitiesByNetworkId.put(var1, var2);
    }

    public Entity getEntity(int var1)
    {
        return (Entity)entitiesByNetworkId.get(var1);
    }

    public Entity removeEntityFromWorld(int var1)
    {
        Entity var2 = (Entity)entitiesByNetworkId.remove(var1);
        if (var2 != null)
        {
            forcedEntities.remove(var2);
            remove(var2);
        }

        return var2;
    }

    public override bool setBlockMetaWithoutNotifyingNeighbors(int var1, int var2, int var3, int var4)
    {
        int var5 = getBlockId(var1, var2, var3);
        int var6 = getBlockMeta(var1, var2, var3);
        if (base.setBlockMetaWithoutNotifyingNeighbors(var1, var2, var3, var4))
        {
            blockResets.add(new BlockReset(this, var1, var2, var3, var5, var6));
            return true;
        }
        else
        {
            return false;
        }
    }

    public override bool setBlockWithoutNotifyingNeighbors(int var1, int var2, int var3, int var4, int var5)
    {
        int var6 = getBlockId(var1, var2, var3);
        int var7 = getBlockMeta(var1, var2, var3);
        if (base.setBlockWithoutNotifyingNeighbors(var1, var2, var3, var4, var5))
        {
            blockResets.add(new BlockReset(this, var1, var2, var3, var6, var7));
            return true;
        }
        else
        {
            return false;
        }
    }

    public override bool setBlockWithoutNotifyingNeighbors(int var1, int var2, int var3, int var4)
    {
        int var5 = getBlockId(var1, var2, var3);
        int var6 = getBlockMeta(var1, var2, var3);
        if (base.setBlockWithoutNotifyingNeighbors(var1, var2, var3, var4))
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
        if (base.setBlockWithoutNotifyingNeighbors(var1, var2, var3, var4, var5))
        {
            blockUpdate(var1, var2, var3, var4);
            return true;
        }
        else
        {
            return false;
        }
    }

    public override void disconnect()
    {
        networkHandler.sendPacketAndDisconnect(new DisconnectPacket("Quitting"));
    }

    protected override void updateWeatherCycles()
    {
        if (!dimension.hasCeiling)
        {
            if (ticksSinceLightning > 0)
            {
                --ticksSinceLightning;
            }

            prevRainingStrength = rainingStrength;
            if (properties.getRaining())
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
            if (properties.getThundering())
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