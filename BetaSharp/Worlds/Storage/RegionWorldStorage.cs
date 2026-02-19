using BetaSharp.Entities;
using BetaSharp.NBT;
using BetaSharp.Server.Worlds;
using BetaSharp.Worlds.Chunks.Storage;
using BetaSharp.Worlds.Dimensions;
using System;
using System.Collections.Generic;
using System.IO;
using static System.IO.Path;

namespace BetaSharp.Worlds.Storage;

public class RegionWorldStorage : IWorldStorage, IPlayerSaveHandler
{
    private readonly string _saveDir;
    private readonly string _playersDir;
    private readonly string _dataDir;
    private readonly long _now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

    public RegionWorldStorage(string baseDir, string worldName, bool createPlayersDir)
    {
        _saveDir = Combine(baseDir, worldName);
        Directory.CreateDirectory(_saveDir);

        _playersDir = Combine(_saveDir, "players");
        _dataDir = Combine(_saveDir, "data");
        Directory.CreateDirectory(_dataDir);

        if (createPlayersDir)
        {
            Directory.CreateDirectory(_playersDir);
        }

        WriteSessionLock();
    }

    // UPDATED: Return type is now IChunkStorage
    public virtual IChunkStorage GetChunkStorage(Dimension dimension)
    {
        if (dimension is NetherDimension)
        {
            string netherPath = Combine(_saveDir, "DIM-1");
            if (!Directory.Exists(netherPath))
            {
                Directory.CreateDirectory(netherPath);
            }
            // RegionChunkStorage now expects a string path
            return new RegionChunkStorage(netherPath);
        }

        return new RegionChunkStorage(_saveDir);
    }

    private void WriteSessionLock()
    {
        try
        {
            string lockFile = Combine(_saveDir, "session.lock");
            using FileStream fs = File.Create(lockFile);

            // Session lock uses Big Endian
            byte[] bytes = BitConverter.GetBytes(_now);
            if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
            fs.Write(bytes, 0, bytes.Length);
        }
        catch (IOException ex)
        {
            Log.Error($"Failed to write session lock: {ex.Message}");
            throw new Exception("Failed to check session lock, aborting", ex);
        }
    }

    public void CheckSessionLock()
    {
        try
        {
            string lockFile = Combine(_saveDir, "session.lock");
            if (!File.Exists(lockFile)) return;

            using FileStream fs = File.OpenRead(lockFile);
            byte[] bytes = new byte[8];
            fs.Read(bytes, 0, 8);

            if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
            long diskTime = BitConverter.ToInt64(bytes, 0);

            if (diskTime != _now)
            {
                throw new Exception("The save is being accessed from another location, aborting");
            }
        }
        catch (IOException)
        {
            throw new Exception("Failed to check session lock, aborting");
        }
    }

    public virtual void Save(WorldProperties props, List<EntityPlayer> players)
    {
        props.SaveVersion = 19132;
        NBTTagCompound dataTag = props.getNBTTagCompoundWithPlayer(players);
        NBTTagCompound root = new();
        root.SetTag("Data", dataTag);
        SaveLevelDat(root);
    }

    public void Save(WorldProperties props)
    {
        NBTTagCompound dataTag = props.getNBTTagCompound();
        NBTTagCompound root = new();
        root.SetTag("Data", dataTag);
        SaveLevelDat(root);
    }

    private void SaveLevelDat(NBTTagCompound root)
    {
        try
        {
            string newFile = Combine(_saveDir, "level.dat_new");
            string oldFile = Combine(_saveDir, "level.dat_old");
            string currentFile = Combine(_saveDir, "level.dat");

            using (FileStream stream = File.Create(newFile))
            {
                NbtIo.WriteCompressed(root, stream);
            }

            if (File.Exists(oldFile)) File.Delete(oldFile);
            if (File.Exists(currentFile)) File.Move(currentFile, oldFile);
            File.Move(newFile, currentFile);
        }
        catch (Exception e)
        {
            Log.Error($"Failed to save level.dat: {e.Message}");
        }
    }

    public WorldProperties LoadProperties()
    {
        string file = Combine(_saveDir, "level.dat");
        if (!File.Exists(file)) file = Combine(_saveDir, "level.dat_old");

        if (File.Exists(file))
        {
            try
            {
                using FileStream stream = File.OpenRead(file);
                NBTTagCompound root = NbtIo.ReadCompressed(stream);
                NBTTagCompound data = root.GetCompoundTag("Data");
                return new WorldProperties(data);
            }
            catch (Exception e)
            {
                Log.Error($"Error loading properties: {e.Message}");
            }
        }
        return null;
    }

    public string GetWorldPropertiesFile(string name) => Combine(_dataDir, $"{name}.dat");

    public void SavePlayerData(EntityPlayer player)
    {
        try
        {
            NBTTagCompound tag = new();
            player.write(tag);

            string tempFile = Combine(_playersDir, "_tmp_.dat");
            string playerFile = Combine(_playersDir, $"{player.name}.dat");

            using (FileStream stream = File.Create(tempFile))
            {
                NbtIo.WriteCompressed(tag, stream);
            }

            if (File.Exists(playerFile)) File.Delete(playerFile);
            File.Move(tempFile, playerFile);
        }
        catch (Exception)
        {
            Log.Warn($"Failed to save player data for {player.name}");
        }
    }

    public void LoadPlayerData(EntityPlayer player)
    {
        NBTTagCompound tag = LoadPlayerData(player.name);
        if (tag != null) player.read(tag);
    }

    public NBTTagCompound LoadPlayerData(string playerName)
    {
        try
        {
            string playerFile = Combine(_playersDir, $"{playerName}.dat");
            if (File.Exists(playerFile))
            {
                using FileStream stream = File.OpenRead(playerFile);
                return NbtIo.ReadCompressed(stream);
            }

            string levelFile = Combine(_saveDir, "level.dat");
            if (File.Exists(levelFile))
            {
                using FileStream stream = File.OpenRead(levelFile);
                NBTTagCompound levelDat = NbtIo.ReadCompressed(stream);
                NBTTagCompound data = levelDat.GetCompoundTag("Data");

                if (data.HasKey("Player"))
                {
                    NBTTagCompound playerTag = data.GetCompoundTag("Player");
                    using (FileStream writeStream = File.Create(playerFile))
                    {
                        NbtIo.WriteCompressed(playerTag, writeStream);
                    }
                    return playerTag;
                }
            }
        }
        catch (Exception e)
        {
            Log.Warn($"Failed to load player data for {playerName}: {e.Message}");
        }
        return null;
    }

    public IPlayerSaveHandler GetPlayerSaveHandler() => this;

    public void ForceSave() { }
}
