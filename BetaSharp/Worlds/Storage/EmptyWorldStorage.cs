using BetaSharp.Entities;
using BetaSharp.Server.Worlds;
using BetaSharp.Worlds.Chunks.Storage;
using BetaSharp.Worlds.Dimensions;

namespace BetaSharp.Worlds.Storage;

public class EmptyWorldStorage : IWorldStorage
{
    public WorldProperties? LoadProperties() => null;

    public void CheckSessionLock() { }

    public IChunkStorage? GetChunkStorage(Dimension dimension) => null;

    public void Save(WorldProperties properties, List<EntityPlayer> players) { }

    public void Save(WorldProperties properties) { }

    public FileInfo? GetWorldPropertiesFile(string name) => null;

    public void ForceSave() { }

    public IPlayerStorage? GetPlayerStorage()
    {
        throw new NotImplementedException();
    }
}