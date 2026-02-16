namespace BetaSharp.Worlds.Chunks;

public interface ChunkSource
{
    bool isChunkLoaded(int ChunkX, int ChunkZ);

    Chunk getChunk(int ChunkX, int ChunkZ);

    Chunk loadChunk(int ChunkX, int ChunkZ);

    void decorate(ChunkSource SourceChunk, int ChunkX, int ChunkZ);

    bool save(bool Flush, LoadingDisplay LoadingDisplay);

    bool tick();
    void markChunksForUnload(int renderDistanceChunks);

    bool canSave();

    string getDebugInfo();
}