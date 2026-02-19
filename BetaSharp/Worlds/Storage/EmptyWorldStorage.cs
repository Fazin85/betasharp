using System;
using System.Collections.Generic;
using BetaSharp.Entities;
using BetaSharp.Server.Worlds;
using BetaSharp.Worlds.Chunks.Storage;
using BetaSharp.Worlds.Dimensions;

namespace BetaSharp.Worlds.Storage;

/// <summary>
/// A dummy implementation of IWorldStorage used for worlds that don't persist to disk 
/// (e.g., in-memory test worlds or specific server-side instances).
/// </summary>
public class EmptyWorldStorage : IWorldStorage
{
    public WorldProperties LoadProperties()
    {
        return null;
    }

    public void CheckSessionLock()
    {
        // No-op: No disk access means no session lock required
    }

    public IChunkStorage GetChunkStorage(Dimension dimension)
    {
        return null;
    }

    public void Save(WorldProperties props, List<EntityPlayer> players)
    {
        // No-op
    }

    public void Save(WorldProperties props)
    {
        // No-op
    }

    // UPDATED: Now returns string instead of java.io.File
    public string GetWorldPropertiesFile(string name)
    {
        return null;
    }

    public void ForceSave()
    {
        // No-op
    }

    public IPlayerSaveHandler GetPlayerSaveHandler()
    {
        // Usually, if the world isn't saving, we shouldn't be trying to handle player saves
        throw new NotImplementedException("EmptyWorldStorage does not support player saving.");
    }
}
