using BetaSharp.Util.Maths;
using BetaSharp.Worlds;
using BetaSharp.Worlds.Chunks;
using BetaSharp.Worlds.Chunks.Storage;

namespace BetaSharp.Server.Worlds;

public class ServerChunkCache : ChunkSource
{
    private readonly HashSet<int> _chunksToUnload = [];
    private readonly Chunk _empty;
    private readonly ChunkSource _generator;
    private readonly ChunkStorage _storage;
    public bool forceLoad = false;
    private readonly Dictionary<int, Chunk> _chunksByPos = [];
    private readonly List<Chunk> _chunks = [];
    private readonly ServerWorld _world;

    public ServerChunkCache(ServerWorld world, ChunkStorage storage, ChunkSource generator)
    {
        _empty = new EmptyChunk(world, new byte[32768], 0, 0);
        _world = world;
        _storage = storage;
        _generator = generator;
    }


    public bool isChunkLoaded(int x, int z)
    {
        return _chunksByPos.ContainsKey(ChunkPos.hashCode(x, z));
    }

    public void isLoaded(int chunkX, int chunkZ)
    {
        Vec3i var3 = _world.getSpawnPos();
        int var4 = chunkX * 16 + 8 - var3.x;
        int var5 = chunkZ * 16 + 8 - var3.z;
        short var6 = 128;
        if (var4 < -var6 || var4 > var6 || var5 < -var6 || var5 > var6)
        {
            _chunksToUnload.Add(ChunkPos.hashCode(chunkX, chunkZ));
        }
    }


    public Chunk loadChunk(int chunkX, int chunkZ)
    {
        int var3 = ChunkPos.hashCode(chunkX, chunkZ);
        _chunksToUnload.Remove(var3);
        _chunksByPos.TryGetValue(var3, out Chunk? var4);
        if (var4 == null)
        {
            var4 = loadChunkFromStorage(chunkX, chunkZ);
            if (var4 == null)
            {
                if (_generator == null)
                {
                    var4 = _empty;
                }
                else
                {
                    var4 = _generator.getChunk(chunkX, chunkZ);
                }
            }

            _chunksByPos.Add(var3, var4);
            _chunks.Add(var4);
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
        _chunksByPos.TryGetValue(ChunkPos.hashCode(chunkX, chunkZ), out Chunk? var3);
        if (var3 == null)
        {
            return !_world.eventProcessingEnabled && !forceLoad ? _empty : loadChunk(chunkX, chunkZ);
        }
        else
        {
            return var3;
        }
    }

    private Chunk? loadChunkFromStorage(int chunkX, int chunkZ)
    {
        if (_storage == null)
        {
            return null;
        }
        else
        {
            try
            {
                Chunk var3 = _storage.loadChunk(_world, chunkX, chunkZ);
                var3?.lastSaveTime = _world.getTime();

                return var3;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return null;
            }
        }
    }

    private void saveEntities(Chunk chunk)
    {
        if (_storage != null)
        {
            try
            {
                _storage.saveEntities(_world, chunk);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }
    }

    private void saveChunk(Chunk chunk)
    {
        if (_storage != null)
        {
            try
            {
                chunk.lastSaveTime = _world.getTime();
                _storage.saveChunk(_world, chunk, null, -1);
            }
            catch (java.io.IOException ex)
            {
                ex.printStackTrace();
            }
            catch (IOException ex)
            {
                Log.Error(ex);
            }
        }
    }


    public void decorate(ChunkSource source, int x, int z)
    {
        Chunk var4 = getChunk(x, z);
        if (!var4.terrainPopulated)
        {
            var4.terrainPopulated = true;
            if (_generator != null)
            {
                _generator.decorate(source, x, z);
                var4.markDirty();
            }
        }
    }


    public bool save(bool saveEntities, LoadingDisplay display)
    {
        int var3 = 0;

        for (int var4 = 0; var4 < _chunks.Count; var4++)
        {
            Chunk var5 = _chunks[var4];
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
            if (_storage == null)
            {
                return true;
            }

            _storage.flush();
        }

        return true;
    }


    public bool tick()
    {
        if (!_world.savingDisabled)
        {
            for (int var1 = 0; var1 < 100; var1++)
            {
                if (_chunksToUnload.Count > 0)
                {
                    int var2 = _chunksToUnload.First();
                    Chunk var3 = _chunksByPos[var2];
                    var3.unload();
                    saveChunk(var3);
                    saveEntities(var3);
                    _chunksToUnload.Remove(var2);
                    _chunksByPos.Remove(var2);
                    _chunks.Remove(var3);
                }
            }

            _storage?.tick();
        }

        return _generator.tick();
    }


    public bool canSave()
    {
        return !_world.savingDisabled;
    }

    public void markChunksForUnload(int renderDistanceChunks)
    {
    }

    public string getDebugInfo()
    {
        return "NOP";
    }
}
