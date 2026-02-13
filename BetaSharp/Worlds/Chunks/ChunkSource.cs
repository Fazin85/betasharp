namespace BetaSharp.Worlds.Chunks;

public interface ChunkSource
{
    bool isChunkLoaded(int var1, int var2);

    Chunk getChunk(int var1, int var2);

    Chunk loadChunk(int var1, int var2);

    void decorate(ChunkSource var1, int var2, int var3);

    bool save(bool var1, LoadingDisplay var2);

    bool tick();
    void markChunksForUnload(int renderDistanceChunks);

    bool canSave();

    string getDebugInfo();
}