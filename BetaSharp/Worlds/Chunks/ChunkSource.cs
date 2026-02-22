namespace BetaSharp.Worlds.Chunks;

public interface ChunkSource
{
    bool IsChunkLoaded(int x, int z);

    Chunk GetChunk(int x, int z);

    Chunk LoadChunk(int x, int z);

    void DecorateTerrain(ChunkSource source, int x, int z);

    bool Save(bool saveEntities, LoadingDisplay display);

    bool Tick();

    bool CanSave();

    string GetDebugInfo();
}
