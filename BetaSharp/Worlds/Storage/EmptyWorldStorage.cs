using BetaSharp.Entities;
using BetaSharp.Server.Worlds;
using BetaSharp.Worlds.Chunks.Storage;
using BetaSharp.Worlds.Dimensions;

namespace BetaSharp.Worlds.Storage;

public class EmptyWorldStorage : WorldStorage
{

    public WorldProperties loadProperties()
    {
        return null;
    }

    public void checkSessionLock()
    {
    }

    public ChunkStorage getChunkStorage(Dimension var1)
    {
        return null;
    }

    public void save(WorldProperties var1, List<EntityPlayer> var2)
    {
    }

    public void save(WorldProperties var1)
    {
    }

    public java.io.File getWorldPropertiesFile(string var1)
    {
        return null;
    }

    public void forceSave()
    {
    }

    public PlayerSaveHandler getPlayerSaveHandler()
    {
        throw new NotImplementedException();
    }
}