using BetaSharp.Entities;
using BetaSharp.NBT;
using BetaSharp.Server.Worlds;
using BetaSharp.Worlds.Chunks.Storage;
using BetaSharp.Worlds.Dimensions;
using java.io;
using java.util.logging;
using File = System.IO.File;

namespace BetaSharp.Worlds.Storage;

public class RegionWorldStorage : WorldStorage, PlayerSaveHandler
{
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
            java.io.File var1 = new(saveDirectory, "session.lock");
            DataOutputStream var2 = new(new FileOutputStream(var1));

            try
            {
                var2.writeLong(now);
            }
            finally
            {
                var2.close();
            }

        }
        catch (java.io.IOException ex)
        {
            Log.Error($"Failed to check session lock, aborting: {ex}");
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
            java.io.File var1 = new(saveDirectory, "session.lock");
            DataInputStream var2 = new(new FileInputStream(var1));

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
        catch (java.io.IOException)
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

    public virtual void save(WorldProperties var1, List<EntityPlayer> var2)
    {
        var1.SaveVersion = 19132;

        NBTTagCompound var3 = var1.getNBTTagCompoundWithPlayer(var2);
        NBTTagCompound tag = new();
        tag.SetTag("Data", var3);

        try
        {
            java.io.File levelDatNew = new(saveDirectory, "level.dat_new");
            java.io.File levelDatOld = new(saveDirectory, "level.dat_old");
            java.io.File levelDat = new(saveDirectory, "level.dat");

            using FileStream stream = File.OpenWrite(levelDatNew.getAbsolutePath());
            NbtIo.WriteCompressed(tag, stream);

            if (levelDatOld.exists())
            {
                levelDatOld.delete();
            }

            levelDat.renameTo(levelDatOld);
            if (levelDat.exists())
            {
                levelDat.delete();
            }

            levelDatNew.renameTo(levelDat);
            if (levelDatNew.exists())
            {
                levelDatNew.delete();
            }
        }
        catch (System.Exception e)
        {
            Log.Error(e);
        }
    }

    public WorldProperties loadProperties()
    {
        java.io.File file = new(saveDirectory, "level.dat");
        NBTTagCompound var2;
        NBTTagCompound var3;
        if (file.exists())
        {
            try
            {
                using FileStream stream = File.OpenRead(file.getAbsolutePath());
                var2 = NbtIo.ReadCompressed(stream);
                var3 = var2.GetCompoundTag("Data");
                WorldProperties wInfo = new(var3);
                return wInfo;
            }
            catch (java.lang.Exception ex)
            {
                ex.printStackTrace();
            }
        }

        file = new java.io.File(saveDirectory, "level.dat_old");
        if (file.exists())
        {
            try
            {
                using FileStream stream = File.OpenRead(file.getAbsolutePath());
                var2 = NbtIo.ReadCompressed(stream);
                var3 = var2.GetCompoundTag("Data");
                WorldProperties wInfo = new(var3);
                return wInfo;
            }
            catch (java.lang.Exception ex)
            {
                Log.Error(ex);
            }
        }

        return null;
    }

    public void save(WorldProperties var1)
    {
        NBTTagCompound var2 = var1.getNBTTagCompound();
        NBTTagCompound tag = new();
        tag.SetTag("Data", var2);

        try
        {
            java.io.File file = new(saveDirectory, "level.dat_new");
            java.io.File var5 = new(saveDirectory, "level.dat_old");
            java.io.File var6 = new(saveDirectory, "level.dat");

            using FileStream stream = File.OpenWrite(file.getAbsolutePath());
            NbtIo.WriteCompressed(tag, stream);

            if (var5.exists())
            {
                var5.delete();
            }

            var6.renameTo(var5);
            if (var6.exists())
            {
                var6.delete();
            }

            file.renameTo(var6);
            if (file.exists())
            {
                file.delete();
            }
        }
        catch (java.lang.Exception ex)
        {
            Log.Error(ex);
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
            NBTTagCompound tag = new();
            player.write(tag);
            java.io.File file = new(playersDirectory, "_tmp_.dat");
            java.io.File var4 = new(playersDirectory, player.name + ".dat");

            using FileStream stream = File.OpenWrite(file.getAbsolutePath());
            NbtIo.WriteCompressed(tag, stream);

            if (var4.exists())
            {
                var4.delete();
            }

            file.renameTo(var4);
        }
        catch (Exception var5)
        {
            Log.Warn($"Failed to save player data for {player.name}");
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

    public NBTTagCompound loadPlayerData(string playerName)
    {
        try
        {
            java.io.File file = new(playersDirectory, playerName + ".dat");
            if (file.exists())
            {
                using FileStream stream = File.OpenRead(file.getAbsolutePath());
                return NbtIo.ReadCompressed(stream);
            }

            java.io.File levelFile = new(saveDirectory, "level.dat");
            if (levelFile.exists())
            {
                try
                {
                    using FileStream stream = File.OpenRead(levelFile.getAbsolutePath());
                    NBTTagCompound levelDat = NbtIo.ReadCompressed(stream);
                    NBTTagCompound data = levelDat.GetCompoundTag("Data");
                    if (data.HasKey("Player"))
                    {
                        NBTTagCompound playerTag = data.GetCompoundTag("Player");

                        using FileStream writeStream = File.OpenWrite(file.getAbsolutePath());
                        NbtIo.WriteCompressed(playerTag, writeStream);

                        Log.Info($"Migrated singleplayer player data from level.dat to {file.getName()}");
                        return playerTag;
                    }
                }
                catch (Exception e)
                {
                    Log.Warn($"Failed to migrate player data from level.dat: {e}");
                }
            }
        }
        catch (Exception var3)
        {
            Log.Warn($"Failed to load player data for {playerName}");
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
