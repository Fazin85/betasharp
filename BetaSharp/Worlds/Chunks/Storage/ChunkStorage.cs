using System;
using BetaSharp.Worlds; // Ensure the correct namespace for World/Chunk is included

namespace BetaSharp.Worlds.Chunks.Storage;

public interface IChunkStorage
{
    /// <summary>
    /// Loads a chunk from storage.
    /// </summary>
    Chunk LoadChunk(World world, int chunkX, int chunkZ);

    /// <summary>
    /// Saves a specific chunk to storage.
    /// </summary>
    void SaveChunk(World world, Chunk chunk, Action onSave, long sequence);

    /// <summary>
    /// Saves only the entities within a chunk.
    /// </summary>
    void SaveEntities(World world, Chunk chunk);

    /// <summary>
    /// Performs periodic maintenance or cleanup.
    /// </summary>
    void Tick();

    /// <summary>
    /// Clears any in-memory buffers.
    /// </summary>
    void Flush();

    /// <summary>
    /// Ensures all pending data is physically written to the storage medium.
    /// </summary>
    void FlushToDisk();
}
