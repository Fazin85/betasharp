namespace BetaSharp.Worlds.Chunks;

public interface ChunkSource
{
    bool IsChunkLoaded(int var1, int var2);

    Chunk GetChunk(int var1, int var2);

    Chunk LoadChunk(int var1, int var2);

    void DecorateTerrain(ChunkSource var1, int var2, int var3);

    bool save(bool var1, LoadingDisplay var2);

    bool tick();

    bool canSave();

    string getDebugInfo();
}
