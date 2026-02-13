using BetaSharp.Util.Maths;
using BetaSharp.Worlds;
using BetaSharp.Worlds.Chunks;
using BetaSharp.Worlds.Chunks.Storage;

namespace BetaSharp.Server.Worlds;

public class ServerChunkCache : ChunkSource
{
    private readonly HashSet<int> chunksToUnload = [];
    private readonly Chunk empty;
    private readonly ChunkSource generator;
    private readonly ChunkStorage storage;
    public bool forceLoad = false;
    private readonly Dictionary<int, Chunk> chunksByPos = [];
    private readonly List<Chunk> chunks = [];
    private readonly ServerWorld world;

    public ServerChunkCache(ServerWorld world, ChunkStorage storage, ChunkSource generator)
    {
        empty = new EmptyChunk(world, new byte[32768], 0, 0);
        this.world = world;
        this.storage = storage;
        this.generator = generator;
    }


    public bool isChunkLoaded(int x, int z)
    {
        return chunksByPos.ContainsKey(ChunkPos.hashCode(x, z));
    }

    public void isLoaded(int chunkX, int chunkZ)
    {
        Vec3i var3 = world.getSpawnPos();
        int var4 = chunkX * 16 + 8 - var3.x;
        int var5 = chunkZ * 16 + 8 - var3.z;
        short var6 = 128;
        if (var4 < -var6 || var4 > var6 || var5 < -var6 || var5 > var6)
        {
            chunksToUnload.Add(ChunkPos.hashCode(chunkX, chunkZ));
        }
    }


    public Chunk loadChunk(int chunkX, int chunkZ)
    {
        int var3 = ChunkPos.hashCode(chunkX, chunkZ);
        chunksToUnload.Remove(var3);
        chunksByPos.TryGetValue(var3, out Chunk? var4);
        if (var4 == null)
        {
            var4 = loadChunkFromStorage(chunkX, chunkZ);
            if (var4 == null)
            {
                if (generator == null)
                {
                    var4 = empty;
                }
                else
                {
                    var4 = generator.getChunk(chunkX, chunkZ);
                }
            }

            chunksByPos.Add(var3, var4);
            chunks.Add(var4);
            if (var4 != null)
            {
                var4.populateBlockLight();
                var4.load();
            }

            if (!var4.terrainPopulated
                && isChunkLoaded(chunkX + 1, chunkZ + 1)
                && isChunkLoaded(chunkX, chunkZ + 1)
                && isChunkLoaded(chunkX + 1, chunkZ))
            {
                decorate(this, chunkX, chunkZ);
            }

            if (isChunkLoaded(chunkX - 1, chunkZ)
                && !getChunk(chunkX - 1, chunkZ).terrainPopulated
                && isChunkLoaded(chunkX - 1, chunkZ + 1)
                && isChunkLoaded(chunkX, chunkZ + 1)
                && isChunkLoaded(chunkX - 1, chunkZ))
            {
                decorate(this, chunkX - 1, chunkZ);
            }

            if (isChunkLoaded(chunkX, chunkZ - 1)
                && !getChunk(chunkX, chunkZ - 1).terrainPopulated
                && isChunkLoaded(chunkX + 1, chunkZ - 1)
                && isChunkLoaded(chunkX, chunkZ - 1)
                && isChunkLoaded(chunkX + 1, chunkZ))
            {
                decorate(this, chunkX, chunkZ - 1);
            }

            if (isChunkLoaded(chunkX - 1, chunkZ - 1)
                && !getChunk(chunkX - 1, chunkZ - 1).terrainPopulated
                && isChunkLoaded(chunkX - 1, chunkZ - 1)
                && isChunkLoaded(chunkX, chunkZ - 1)
                && isChunkLoaded(chunkX - 1, chunkZ))
            {
                decorate(this, chunkX - 1, chunkZ - 1);
            }
        }

        return var4;
    }


    public Chunk getChunk(int chunkX, int chunkZ)
    {
        chunksByPos.TryGetValue(ChunkPos.hashCode(chunkX, chunkZ), out Chunk? var3);
        if (var3 == null)
        {
            return !world.eventProcessingEnabled && !forceLoad ? empty : loadChunk(chunkX, chunkZ);
        }
        else
        {
            return var3;
        }
    }

    private Chunk loadChunkFromStorage(int chunkX, int chunkZ)
    {
        if (storage == null)
        {
            return null;
        }
        else
        {
            try
            {
                Chunk var3 = storage.loadChunk(world, chunkX, chunkZ);
                if (var3 != null)
                {
                    var3.lastSaveTime = world.getTime();
                }

                return var3;
            }
            catch (java.lang.Exception var4)
            {
                var4.printStackTrace();
                return null;
            }
        }
    }

    private void saveEntities(Chunk chunk)
    {
        if (storage != null)
        {
            try
            {
                storage.saveEntities(world, chunk);
            }
            catch (java.lang.Exception var3)
            {
                var3.printStackTrace();
            }
        }
    }

    private void saveChunk(Chunk chunk)
    {
        if (storage != null)
        {
            try
            {
                chunk.lastSaveTime = world.getTime();
                storage.saveChunk(world, chunk, null, -1);
            }
            catch (java.io.IOException var3)
            {
                var3.printStackTrace();
            }
        }
    }


    public void decorate(ChunkSource source, int x, int z)
    {
        Chunk var4 = getChunk(x, z);
        if (!var4.terrainPopulated)
        {
            var4.terrainPopulated = true;
            if (generator != null)
            {
                generator.decorate(source, x, z);
                var4.markDirty();
            }
        }
    }


    public bool save(bool saveEntities, LoadingDisplay display)
    {
        int var3 = 0;

        for (int var4 = 0; var4 < chunks.Count; var4++)
        {
            Chunk var5 = chunks[var4];
            if (saveEntities && !var5.empty)
            {
                this.saveEntities(var5);
            }

            if (var5.shouldSave(saveEntities))
            {
                saveChunk(var5);
                var5.dirty = false;
                if (++var3 == 24 && !saveEntities)
                {
                    return false;
                }
            }
        }

        if (saveEntities)
        {
            if (storage == null)
            {
                return true;
            }

            storage.flush();
        }

        return true;
    }


    public bool tick()
    {
        if (!world.savingDisabled)
        {
            for (int var1 = 0; var1 < 100; var1++)
            {
                if (chunksToUnload.Count > 0)
                {
                    int var2 = chunksToUnload.First();
                    Chunk var3 = chunksByPos[var2];
                    var3.unload();
                    saveChunk(var3);
                    saveEntities(var3);
                    chunksToUnload.Remove(var2);
                    chunksByPos.Remove(var2);
                    chunks.Remove(var3);
                }
            }

            if (storage != null)
            {
                storage.tick();
            }
        }

        return generator.tick();
    }


    public bool canSave()
    {
        return !world.savingDisabled;
    }

    public void markChunksForUnload(int renderDistanceChunks)
    {
    }

    public string getDebugInfo()
    {
        return "NOP";
    }
}