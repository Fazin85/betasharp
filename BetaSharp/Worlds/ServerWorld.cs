using BetaSharp.Blocks.Entities;
using BetaSharp.Entities;
using BetaSharp.Network.Packets.S2CPlay;
using BetaSharp.Server;
using BetaSharp.Server.Internal;
using BetaSharp.Server.Worlds;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds.Chunks;
using BetaSharp.Worlds.Chunks.Storage;
using BetaSharp.Worlds.Dimensions;
using BetaSharp.Worlds.Storage;

namespace BetaSharp.Worlds;

public class ServerWorld : World
{
    public ServerChunkCache chunkCache;
    public bool bypassSpawnProtection = false;
    public bool savingDisabled;
    private readonly MinecraftServer server;
    private readonly Dictionary<int, Entity> entitiesById = [];

    public ServerWorld(MinecraftServer server, WorldStorage storage, String name, int dimensionId, long seed) : base(storage, name, seed, Dimension.fromId(dimensionId))
    {
        this.server = server;
    }


    public override void updateEntity(Entity entity, bool requireLoaded)
    {
        if (!server.spawnAnimals && (entity is EntityAnimal || entity is EntityWaterMob))
        {
            entity.markDead();
        }

        if (entity.passenger == null || entity.passenger is not EntityPlayer)
        {
            base.updateEntity(entity, requireLoaded);
        }
    }

    public void tickVehicle(Entity vehicle, bool requireLoaded)
    {
        base.updateEntity(vehicle, requireLoaded);
    }


    protected override ChunkSource CreateChunkCache()
    {
        ChunkStorage var1 = storage.getChunkStorage(dimension);
        chunkCache = new ServerChunkCache(this, var1, dimension.createChunkGenerator());
        return chunkCache;
    }

    public List<BlockEntity> getBlockEntities(int minX, int minY, int minZ, int maxX, int maxY, int maxZ)
    {
        List<BlockEntity> var7 = [];

        for (int var8 = 0; var8 < blockEntities.Count; var8++)
        {
            BlockEntity var9 = blockEntities[var8];
            if (var9.x >= minX && var9.y >= minY && var9.z >= minZ && var9.x < maxX && var9.y < maxY && var9.z < maxZ)
            {
                var7.Add(var9);
            }
        }

        return var7;
    }


    public override bool canInteract(EntityPlayer player, int x, int y, int z)
    {
        int var5 = (int)MathHelper.abs(x - properties.SpawnX);
        int var6 = (int)MathHelper.abs(z - properties.SpawnZ);
        if (var5 > var6)
        {
            var6 = var5;
        }

        return var6 > 16 || server.playerManager.isOperator(player.name) || server is InternalServer;
    }


    protected override void NotifyEntityAdded(Entity entity)
    {
        base.NotifyEntityAdded(entity);
        entitiesById.Add(entity.id, entity);
    }


    protected override void NotifyEntityRemoved(Entity entity)
    {
        base.NotifyEntityRemoved(entity);
        entitiesById.Remove(entity.id);
    }

    public Entity getEntity(int id)
    {
        entitiesById.TryGetValue(id, out Entity? entity);
        return entity;
    }


    public override bool spawnGlobalEntity(Entity entity)
    {
        if (base.spawnGlobalEntity(entity))
        {
            server.playerManager.sendToAround(entity.x, entity.y, entity.z, 512.0, dimension.id, new GlobalEntitySpawnS2CPacket(entity));
            return true;
        }
        else
        {
            return false;
        }
    }


    public override void broadcastEntityEvent(Entity entity, byte @event)
    {
        EntityStatusS2CPacket var3 = new EntityStatusS2CPacket(entity.id, @event);
        server.getEntityTracker(dimension.id).sendToAround(entity, var3);
    }


    public override Explosion createExplosion(Entity source, double x, double y, double z, float power, bool fire)
    {
        Explosion var10 = new Explosion(this, source, x, y, z, power)
        {
            isFlaming = fire
        };
        var10.doExplosionA();
        var10.doExplosionB(false);
        server.playerManager.sendToAround(x, y, z, 64.0, dimension.id, new ExplosionS2CPacket(x, y, z, power, var10.destroyedBlockPositions));
        return var10;
    }


    public override void playNoteBlockActionAt(int x, int y, int z, int soundType, int pitch)
    {
        base.playNoteBlockActionAt(x, y, z, soundType, pitch);
        server.playerManager.sendToAround(x, y, z, 64.0, dimension.id, new PlayNoteSoundS2CPacket(x, y, z, soundType, pitch));
    }

    public void forceSave()
    {
        storage.forceSave();
    }


    protected override void UpdateWeatherCycles()
    {
        bool var1 = isRaining();
        base.UpdateWeatherCycles();
        if (var1 != isRaining())
        {
            if (var1)
            {
                server.playerManager.sendToAll(new GameStateChangeS2CPacket(2));
            }
            else
            {
                server.playerManager.sendToAll(new GameStateChangeS2CPacket(1));
            }
        }
    }
}