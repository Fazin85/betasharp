namespace BetaSharp.Worlds.Chunks.Storage;

public interface ChunkStorage
{
    Chunk loadChunk(World world, int chunkX, int chunkZ);

    void saveChunk(World world, Chunk chunk, Action onSave, long sequence);

    void saveEntities(World world, Chunk chunk);

    void tick();

    void flush();

    void flushToDisk();
}