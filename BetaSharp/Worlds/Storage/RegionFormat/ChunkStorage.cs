using BetaSharp.Worlds.Chunks;
using BetaSharp.Worlds.Core;

namespace BetaSharp.Worlds.Storage.RegionFormat;

public interface IChunkStorage
{
    Chunk LoadChunk(World world, int chunkX, int chunkZ);

    void SaveChunk(World world, Chunk chunk, Action onSave, long sequence);

    void SaveEntities(World world, Chunk chunk);

    void Tick();

    void Flush();

    void FlushToDisk();
}
