using betareborn.Server.Worlds;
using betareborn.Worlds.Chunks.Storage;
using betareborn.Worlds.Dimensions;
using System.Collections.Generic;

namespace betareborn.Worlds.Storage
{
    public interface WorldStorage
    {
        WorldProperties loadProperties();

        void checkSessionLock();

        ChunkStorage getChunkStorage(Dimension dim);

        void save(WorldProperties var1, List<object> var2);

        void save(WorldProperties var1);
        void forceSave();

        PlayerSaveHandler getPlayerSaveHandler();

        java.io.File getWorldPropertiesFile(string name);
    }

}