using betareborn.Entities;
using betareborn.NBT;
using betareborn.Server.Worlds;
using betareborn.Worlds.Chunks.Storage;
using betareborn.Worlds.Dimensions;
using java.io;
using java.util;
using java.util.logging;

namespace betareborn.Worlds.Storage
{

    public class RegionWorldStorage : WorldStorage, PlayerSaveHandler
    {
        private static readonly Logger LOGGER = Logger.getLogger("Minecraft");

        private readonly java.io.File saveDirectory;
        private readonly java.io.File playersDirectory;
        private readonly java.io.File dataDir;
        private readonly long now = java.lang.System.currentTimeMillis();

        public RegionWorldStorage(java.io.File var1, string var2, bool var3)
        {
            saveDirectory = new java.io.File(var1, var2);
            saveDirectory.mkdirs();
            playersDirectory = new java.io.File(saveDirectory, "players");
            dataDir = new java.io.File(saveDirectory, "data");
            dataDir.mkdirs();
            if (var3)
            {
                playersDirectory.mkdirs();
            }

            writeSessionLock();
        }

        private void writeSessionLock()
        {
            try
            {
                java.io.File var1 = new java.io.File(saveDirectory, "session.lock");
                DataOutputStream var2 = new DataOutputStream(new FileOutputStream(var1));

                try
                {
                    var2.writeLong(now);
                }
                finally
                {
                    var2.close();
                }

            }
            catch (java.io.IOException var7)
            {
                var7.printStackTrace();
                throw new java.lang.RuntimeException("Failed to check session lock, aborting");
            }
        }

        protected java.io.File getDirectory()
        {
            return saveDirectory;
        }

        public void checkSessionLock()
        {
            try
            {
                java.io.File var1 = new java.io.File(saveDirectory, "session.lock");
                DataInputStream var2 = new DataInputStream(new FileInputStream(var1));

                try
                {
                    if (var2.readLong() != now)
                    {
                        throw new MinecraftException("The save is being accessed from another location, aborting");
                    }
                }
                finally
                {
                    var2.close();
                }

            }
            catch (java.io.IOException var7)
            {
                throw new MinecraftException("Failed to check session lock, aborting");
            }
        }

        public virtual ChunkStorage getChunkStorage(Dimension var1)
        {
            java.io.File var2 = getDirectory();
            if (var1 is NetherDimension)
            {
                java.io.File var3 = new(var2, "DIM-1");
                var3.mkdirs();

                return new RegionChunkStorage(var3);
            }
            else
            {
                return new RegionChunkStorage(var2);
            }
        }

        public virtual void save(WorldProperties var1, List var2)
        {
            var1.setSaveVersion(19132);

            NBTTagCompound var3 = var1.getNBTTagCompoundWithPlayer(var2);
            NBTTagCompound var4 = new();
            var4.setTag("Data", var3);

            try
            {
                var saveTask = Task.Run(() =>
                {
                    java.io.File var5 = new java.io.File(saveDirectory, "level.dat_new");
                    java.io.File var6 = new java.io.File(saveDirectory, "level.dat_old");
                    java.io.File var7 = new java.io.File(saveDirectory, "level.dat");
                    NbtIo.writeGzippedCompoundToOutputStream(var4, new FileOutputStream(var5));
                    if (var6.exists())
                    {
                        var6.delete();
                    }

                    var7.renameTo(var6);
                    if (var7.exists())
                    {
                        var7.delete();
                    }

                    var5.renameTo(var7);
                    if (var5.exists())
                    {
                        var5.delete();
                    }
                });

                AsyncIO.addTask(saveTask);
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine(e);
            }
        }

        public WorldProperties loadProperties()
        {
            java.io.File var1 = new java.io.File(saveDirectory, "level.dat");
            NBTTagCompound var2;
            NBTTagCompound var3;
            if (var1.exists())
            {
                try
                {
                    var2 = NbtIo.read(new FileInputStream(var1));
                    var3 = var2.getCompoundTag("Data");
                    WorldProperties wInfo = new(var3);
                    return wInfo;
                }
                catch (java.lang.Exception var5)
                {
                    var5.printStackTrace();
                }
            }

            var1 = new java.io.File(saveDirectory, "level.dat_old");
            if (var1.exists())
            {
                try
                {
                    var2 = NbtIo.read(new FileInputStream(var1));
                    var3 = var2.getCompoundTag("Data");
                    WorldProperties wInfo = new(var3);
                    return wInfo;
                }
                catch (java.lang.Exception var4)
                {
                    var4.printStackTrace();
                }
            }

            return null;
        }

        public void save(WorldProperties var1)
        {
            NBTTagCompound var2 = var1.getNBTTagCompound();
            NBTTagCompound var3 = new NBTTagCompound();
            var3.setTag("Data", var2);

            try
            {
                java.io.File var4 = new java.io.File(saveDirectory, "level.dat_new");
                java.io.File var5 = new java.io.File(saveDirectory, "level.dat_old");
                java.io.File var6 = new java.io.File(saveDirectory, "level.dat");
                NbtIo.writeGzippedCompoundToOutputStream(var3, new FileOutputStream(var4));
                if (var5.exists())
                {
                    var5.delete();
                }

                var6.renameTo(var5);
                if (var6.exists())
                {
                    var6.delete();
                }

                var4.renameTo(var6);
                if (var4.exists())
                {
                    var4.delete();
                }
            }
            catch (java.lang.Exception var7)
            {
                var7.printStackTrace();
            }

        }

        public java.io.File getWorldPropertiesFile(string var1)
        {
            return new java.io.File(dataDir, var1 + ".dat");
        }

        public void savePlayerData(EntityPlayer player)
        {
            try
            {
                NBTTagCompound var2 = new NBTTagCompound();
                player.write(var2);
                java.io.File var3 = new java.io.File(playersDirectory, "_tmp_.dat");
                java.io.File var4 = new java.io.File(playersDirectory, player.name + ".dat");
                NbtIo.writeGzippedCompoundToOutputStream(var2, new FileOutputStream(var3));
                if (var4.exists())
                {
                    var4.delete();
                }

                var3.renameTo(var4);
            }
            catch (Exception var5)
            {
                LOGGER.warning("Failed to save player data for " + player.name);
            }
        }

        public void loadPlayerData(EntityPlayer player)
        {
            NBTTagCompound var2 = loadPlayerData(player.name);
            if (var2 != null)
            {
                player.read(var2);
            }
        }

        public NBTTagCompound loadPlayerData(String playerName)
        {
            try
            {
                java.io.File var2 = new java.io.File(playersDirectory, playerName + ".dat");
                if (var2.exists())
                {
                    return NbtIo.read(new FileInputStream(var2));
                }
            }
            catch (Exception var3)
            {
                LOGGER.warning("Failed to load player data for " + playerName);
            }

            return null;
        }

        public PlayerSaveHandler getPlayerSaveHandler()
        {
            return this;
        }

        public void forceSave()
        {
        }
    }

}