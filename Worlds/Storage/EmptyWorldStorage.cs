using betareborn.Server.Worlds;
using betareborn.Worlds.Chunks.Storage;
using betareborn.Worlds.Dimensions;
using java.util;

namespace betareborn.Worlds.Storage
{

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

        public void save(WorldProperties var1, List<object> var2)
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

}