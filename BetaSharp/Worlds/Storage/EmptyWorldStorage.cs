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

    }

    public IChunkStorage GetChunkStorage(Dimension dimension)
    {
        return null;
    }

    public void Save(WorldProperties props, List<EntityPlayer> players)
    {

    }

    public void Save(WorldProperties props)
    {

    }

    public string GetWorldPropertiesFile(string name)
    {
        return null;
    }

    public void ForceSave()
    {

    }

    public IPlayerSaveHandler GetPlayerSaveHandler()
    {
        throw new NotImplementedException("EmptyWorldStorage does not support player saving.");
    }
}
