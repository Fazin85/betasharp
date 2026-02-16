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
    public ServerChunkCache ChunkCache;
    public bool BypassSpawnProtection = false;
    public bool IsSavingDisabled;
    private readonly MinecraftServer Server;
    private readonly Dictionary<int, Entity> EntitiesById = [];

    // Lowercase aliases for compatibility
    public ServerChunkCache chunkCache { get => ChunkCache; set => ChunkCache = value; }
    public bool bypassSpawnProtection { get => BypassSpawnProtection; set => BypassSpawnProtection = value; }
    public bool savingDisabled { get => IsSavingDisabled; set => IsSavingDisabled = value; }

    public ServerWorld(MinecraftServer Server, WorldStorage Storage, String Name, int DimensionId, long Seed) : base(Storage, Name, Seed, Dimension.fromId(DimensionId))
    {
        this.Server = Server;
    }


    public override void updateEntity(Entity Entity, bool RequireLoaded)
    {
        if (!Server.spawnAnimals && (Entity is EntityAnimal || Entity is EntityWaterMob))
        {
            Entity.markDead();
        }

        if (Entity.passenger == null || !(Entity.passenger is EntityPlayer))
        {
            base.updateEntity(Entity, RequireLoaded);
        }
    }

    public void tickVehicle(Entity Vehicle, bool RequireLoaded)
    {
        base.updateEntity(Vehicle, RequireLoaded);
    }


    protected override ChunkSource CreateChunkCache()
    {
        ChunkStorage Chunkstorage = storage.getChunkStorage(Dimension);
        ChunkCache = new ServerChunkCache(this, Chunkstorage, Dimension.createChunkGenerator());
        return ChunkCache;
    }

    public List<BlockEntity> getBlockEntities(int MinX, int MinY, int MinZ, int MaxX, int MaxY, int MaxZ)
    {
        List<BlockEntity> var7 = [];

        for (int var8 = 0; var8 < blockEntities.Count; var8++)
        {
            BlockEntity var9 = blockEntities[var8];
            if (var9.x >= MinX && var9.y >= MinY && var9.z >= MinZ && var9.x < MaxX && var9.y < MaxY && var9.z < MaxZ)
            {
                var7.Add(var9);
            }
        }

        return var7;
    }


    public override bool canInteract(EntityPlayer Player, int X, int Y, int Z)
    {
        int var5 = (int)MathHelper.abs(X - properties.SpawnX);
        int var6 = (int)MathHelper.abs(Z - properties.SpawnZ);
        if (var5 > var6)
        {
            var6 = var5;
        }

        return var6 > 16 || Server.playerManager.isOperator(Player.name) || Server is InternalServer;
    }


    protected override void NotifyEntityAdded(Entity Entity)
    {
        base.NotifyEntityAdded(Entity);
        EntitiesById.Add(Entity.id, Entity);
    }


    protected override void NotifyEntityRemoved(Entity Entity)
    {
        base.NotifyEntityRemoved(Entity);
        EntitiesById.Remove(Entity.id);
    }

    public Entity getEntity(int Id)
    {
        EntitiesById.TryGetValue(Id, out Entity? Entity);
        return Entity;
    }


    public override bool spawnGlobalEntity(Entity entity)
    {
        if (base.spawnGlobalEntity(entity))
        {
            Server.playerManager.sendToAround(entity.x, entity.y, entity.z, 512.0, Dimension.id, new GlobalEntitySpawnS2CPacket(entity));
            return true;
        }
        else
        {
            return false;
        }
    }


    public override void broadcastEntityEvent(Entity Entity, byte Event)
    {
        EntityStatusS2CPacket var3 = new EntityStatusS2CPacket(Entity.id, Event);
        Server.getEntityTracker(Dimension.id).sendToAround(Entity, var3);
    }


    public override Explosion createExplosion(Entity Source, double X, double Y, double Z, float Power, bool Fire)
    {
        Explosion var10 = new Explosion(this, Source, X, Y, Z, Power)
        {
            isFlaming = Fire
        };
        var10.doExplosionA();
        var10.doExplosionB(false);
        Server.playerManager.sendToAround(X, Y, Z, 64.0, Dimension.id, new ExplosionS2CPacket(X, Y, Z, Power, var10.destroyedBlockPositions));
        return var10;
    }


    public override void playNoteBlockActionAt(int X, int Y, int Z, int SoundType, int Pitch)
    {
        base.playNoteBlockActionAt(X, Y, Z, SoundType, Pitch);
        Server.playerManager.sendToAround(X, Y, Z, 64.0, Dimension.id, new PlayNoteSoundS2CPacket(X, Y, Z, SoundType, Pitch));
    }

    public void forceSave()
    {
        storage.forceSave();
    }


    protected override void UpdateWeatherCycles()
    {
        bool wasRaining = isRaining();
        base.UpdateWeatherCycles();
        if (wasRaining != isRaining())
        {
            switch (wasRaining)
            {
                case true:
                    Server.playerManager.sendToAll(new GameStateChangeS2CPacket(2));
                    break;
                default:
                    Server.playerManager.sendToAll(new GameStateChangeS2CPacket(1));
                    break;
            }
        }
    }
}