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

    private readonly List<BlockReset> _blockResets = new();
    private readonly ClientNetworkHandler _networkHandler;
    private MultiplayerChunkCache _chunkCache;
    private readonly IntHashMap entitiesByNetworkId = new IntHashMap();
    private readonly HashSet<Entity> forcedEntities = new();
    private readonly HashSet<Entity> pendingEntities = new();

    public ClientWorld(ClientNetworkHandler netHandler, long seed, int dimId) : base(new EmptyWorldStorage(), "MpServer", Dimension.fromId(dimId), seed)
    {
        _networkHandler = netHandler;
        setSpawnPos(new Vec3i(8, 64, 8));
        persistentStateManager = netHandler.clientPersistentStateManager;
    }

    public override void tick(int _)
    {
        setTime(getTime() + 1L);
        int ambient = getAmbientDarkness(1.0F);

        if (ambient != ambientDarkness)
        {
            ambientDarkness = ambient;
            for (int j = 0; j < eventListeners.Count; ++j)
            {
                eventListeners[j].notifyAmbientDarknessChanged();
            }
        }

        for (int i = 0; i < 10 && pendingEntities.Count > 0; ++i)
        {
            Entity entity = pendingEntities.First();
            if (!entities.Contains(entity))
            {
                spawnEntity(entity);
            }
        }

        _networkHandler.tick();

        for (int i = 0; i < _blockResets.Count; ++i)
        {
            BlockReset blockReset = _blockResets[i];
            if (--blockReset.Delay == 0)
            {
                base.setBlockWithoutNotifyingNeighbors(blockReset.X, blockReset.Y, blockReset.Z, blockReset.BlockId, blockReset.Meta);
                blockUpdateEvent(blockReset.X, blockReset.Y, blockReset.Z);
                _blockResets.RemoveAt(i--);
            }
        }

    }

    public void clearBlockResets(int minX, int minY, int minZ, int maxX, int maxY, int maxZ)
    {
        for (int i = 0; i < _blockResets.Count; ++i)
        {
            BlockReset blockReset = _blockResets[i];
            if (blockReset.X >= minX && blockReset.Y >= minY && blockReset.Z >= minZ && blockReset.X <= maxX && blockReset.Y <= maxY && blockReset.Z <= maxZ)
            {
                _blockResets.RemoveAt(i--);
            }
        }

    }

    protected override ChunkSource createChunkCache()
    {
        _chunkCache = new MultiplayerChunkCache(this);
        return _chunkCache;
    }

    public override void updateSpawnPosition()
    {
        setSpawnPos(new Vec3i(8, 64, 8));
    }

    protected override void manageChunkUpdatesAndEvents()
    {
    }

    public override void scheduleBlockUpdate(int x, int y, int z, int blockId, int delay)
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
            _chunkCache.loadChunk(chunkX, chunkZ);
        }
        else
        {
            _chunkCache.unloadChunk(chunkX, chunkZ);
        }

        if (!load)
        {
            setBlocksDirty(chunkX * 16, 0, chunkZ * 16, chunkX * 16 + 15, 128, chunkZ * 16 + 15);
        }

    }

    public override bool spawnEntity(Entity entity)
    {
        bool var2 = base.spawnEntity(entity);
        forcedEntities.Add(entity);
        if (!var2)
        {
            pendingEntities.Add(entity);
        }

        return var2;
    }

    public override void remove(Entity ent)
    {
        base.remove(ent);
        forcedEntities.Remove(ent);
    }

    protected override void notifyEntityAdded(Entity ent)
    {
        base.notifyEntityAdded(ent);
        if (pendingEntities.Contains(ent))
        {
            pendingEntities.Remove(ent);
        }

    }

    protected override void notifyEntityRemoved(Entity ent)
    {
        base.notifyEntityRemoved(ent);
        if (forcedEntities.Contains(ent))
        {
            pendingEntities.Add(ent);
        }

    }

    public void forceEntity(int networkId, Entity ent)
    {
        Entity existingEnt = getEntity(networkId);
        if (existingEnt != null)
        {
            remove(existingEnt);
        }

        forcedEntities.Add(ent);
        ent.id = networkId;
        if (!spawnEntity(ent))
        {
            pendingEntities.Add(ent);
        }

        entitiesByNetworkId.put(networkId, ent);
    }

    public Entity getEntity(int networkId)
    {
        return (Entity)entitiesByNetworkId.get(networkId);
    }

    public Entity removeEntityFromWorld(int networkId)
    {
        Entity var2 = (Entity)entitiesByNetworkId.remove(networkId);
        if (var2 != null)
        {
            forcedEntities.Remove(var2);
            remove(var2);
        }

        return var2;
    }

    public override bool setBlockMetaWithoutNotifyingNeighbors(int x, int y, int z, int meta)
    {
        int blockId = getBlockId(x, y, z);
        int previousMeta = getBlockMeta(x, y, z);
        if (base.setBlockMetaWithoutNotifyingNeighbors(x, y, z, meta))
        {
            _blockResets.Add(new BlockReset(this, x, y, z, blockId, previousMeta));
            return true;
        }
        else
        {
            return false;
        }
    }

    public override bool setBlockWithoutNotifyingNeighbors(int x, int y, int z, int blockId, int meta)
    {
        int previousBlockId = getBlockId(x, y, z);
        int previousMeta = getBlockMeta(x, y, z);
        if (base.setBlockWithoutNotifyingNeighbors(x, y, z, blockId, meta))
        {
            _blockResets.Add(new BlockReset(this, x, y, z, previousBlockId, previousMeta));
            return true;
        }
        else
        {
            return false;
        }
    }

    public override bool setBlockWithoutNotifyingNeighbors(int x, int y, int z, int blockId)
    {
        int previousBlockId = getBlockId(x, y, z);
        int previousMeta = getBlockMeta(x, y, z);
        if (base.setBlockWithoutNotifyingNeighbors(x, y, z, blockId))
        {
            _blockResets.Add(new BlockReset(this, x, y, z, previousBlockId, previousMeta));
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool setBlockWithMetaFromPacket(int minX, int minY, int minZ, int blockId, int meta)
    {
        clearBlockResets(minX, minY, minZ, minX, minY, minZ);
        if (base.setBlockWithoutNotifyingNeighbors(minX, minY, minZ, blockId, meta))
        {
            blockUpdate(minX, minY, minZ, blockId);
            return true;
        }
        else
        {
            return false;
        }
    }

    public override void disconnect()
    {
        _networkHandler.sendPacketAndDisconnect(new DisconnectPacket("Quitting"));
    }

    protected override void updateWeatherCycles()
    {
        if (dimension.hasCeiling) return;

        if (ticksSinceLightning > 0) --ticksSinceLightning;

        prevRainingStrength = rainingStrength;
        rainingStrength = Math.Clamp(rainingStrength + (properties.IsRaining ? 0.01f : -0.01f), 0.0f, 1.0f);

        prevThunderingStrength = thunderingStrength;
        thunderingStrength = Math.Clamp(thunderingStrength + (properties.IsThundering ? 0.01f : -0.01f), 0.0f, 1.0f);
    }
}