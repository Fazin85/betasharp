using BetaSharp.Entities;
using BetaSharp.Server.Worlds;
using BetaSharp.Worlds.Chunks.Storage;
using BetaSharp.Worlds.Dimensions;

namespace BetaSharp.Worlds.Storage;

public interface WorldStorage
{
    WorldProperties loadProperties();

    void checkSessionLock();

    ChunkStorage getChunkStorage(Dimension dim);

    void save(WorldProperties var1, List<EntityPlayer> var2);

    void save(WorldProperties var1);
    void forceSave();

    PlayerSaveHandler getPlayerSaveHandler();

    java.io.File getWorldPropertiesFile(string name);
}