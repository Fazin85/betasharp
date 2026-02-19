using System.Collections.Generic;
using BetaSharp.Entities;
using BetaSharp.Server.Worlds;
using BetaSharp.Worlds.Chunks.Storage;
using BetaSharp.Worlds.Dimensions;

namespace BetaSharp.Worlds.Storage;

public interface IWorldStorage
{
    WorldProperties LoadProperties();

    void CheckSessionLock();


    IChunkStorage GetChunkStorage(Dimension dim);

    void Save(WorldProperties props, List<EntityPlayer> players);

    void Save(WorldProperties props);

    void ForceSave();

    IPlayerSaveHandler GetPlayerSaveHandler();


    string GetWorldPropertiesFile(string name);
}
