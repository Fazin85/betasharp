using BetaSharp.Util.Maths;
using BetaSharp.Worlds;
using BetaSharp.Worlds.Chunks;
using java.lang;
using java.util;

namespace BetaSharp.Client.Chunks;

public class MultiplayerChunkCache : ChunkSource
{

    private readonly Chunk empty;
    private readonly Dictionary<ChunkPos, Chunk> chunkByPos = [];
    private readonly World world;

    public MultiplayerChunkCache(World world)
    {
        empty = new EmptyChunk(world, new byte[-Short.MIN_VALUE], 0, 0);
        this.world = world;
    }

    public bool isChunkLoaded(int x, int y)
    {
        if (this != null)
        {
            return true;
        }
        else
        {
            ChunkPos key = new ChunkPos(x, y);
            return chunkByPos.ContainsKey(key);
        }
    }

    public void unloadChunk(int x, int z)
    {
        Chunk chunk = getChunk(x, z);
        if (!chunk.isEmpty())
        {
            chunk.unload();
        }

        chunkByPos.Remove(new ChunkPos(x, z));
    }

    public Chunk loadChunk(int x, int z)
    {
        ChunkPos key = new ChunkPos(x, z);
        byte[] blocks = new byte[-Short.MIN_VALUE];
        Chunk chunk = new Chunk(world, blocks, x, z);
        Arrays.fill(chunk.skyLight.bytes, 255);

        if (chunkByPos.ContainsKey(key))
        {
            chunkByPos[key] = chunk;
        }
        else
        {
            chunkByPos.Add(key, chunk);
        }

        chunk.loaded = true;
        return chunk;
    }

    public Chunk getChunk(int x, int z)
    {
        ChunkPos key = new ChunkPos(x, z);
        chunkByPos.TryGetValue(key, out Chunk? chunk);
        return chunk == null ? empty : chunk;
    }

    public bool save(bool bl, LoadingDisplay display)
    {
        return true;
    }

    public bool tick()
    {
        return false;
    }

    public bool canSave()
    {
        return false;
    }

    public void decorate(ChunkSource source, int x, int y)
    {
    }

    public void markChunksForUnload(int _)
    {
    }

    public string getDebugInfo()
    {
        return "MultiplayerChunkCache: " + chunkByPos.Count;
    }
}