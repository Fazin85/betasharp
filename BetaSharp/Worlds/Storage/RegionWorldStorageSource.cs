using System;
using System.Collections.Generic;
using System.IO;
using BetaSharp.NBT;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds.Chunks.Storage;
using SysPath = System.IO.Path; // Create an alias

namespace BetaSharp.Worlds.Storage;

public class RegionWorldStorageSource
{
    private readonly DirectoryInfo _baseDir;

    public RegionWorldStorageSource(string directory)
    {
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        _baseDir = new DirectoryInfo(directory);
    }

    public virtual string GetName() => "Scaevolus' McRegion";

    /// <summary>
    /// Scans the base directory for valid world folders and returns their metadata.
    /// </summary>
    public virtual List<WorldSaveInfo> GetAllSaves()
    {
        var saves = new List<WorldSaveInfo>();

        foreach (var subDir in _baseDir.GetDirectories())
        {
            var props = GetProperties(subDir.Name);
            if (props != null)
            {
                bool requiresConversion = props.SaveVersion != 19132;
                string displayName = props.LevelName;

                if (string.IsNullOrWhiteSpace(displayName))
                {
                    displayName = subDir.Name;
                }

                saves.Add(new WorldSaveInfo(
                    subDir.Name,
                    displayName,
                    props.LastTimePlayed,
                    props.SizeOnDisk,
                    requiresConversion));
            }
        }

        return saves;
    }

    public virtual void Flush()
    {
        RegionIo.Flush();
    }

    public virtual IWorldStorage GetStorage(string worldName, bool createPlayersDir)
    {
        return new RegionWorldStorage(_baseDir.FullName, worldName, createPlayersDir);
    }

    private static long GetFolderSize(DirectoryInfo d)
    {
        long size = 0;
        // Add size of files
        foreach (FileInfo fi in d.GetFiles())
        {
            size += fi.Length;
        }
        // Recurring into subdirectories
        foreach (DirectoryInfo di in d.GetDirectories())
        {
            size += GetFolderSize(di);
        }
        return size;
    }

    public virtual WorldProperties GetProperties(string worldFolderName)
    {
        string worldPath = SysPath.Combine(_baseDir.FullName, worldFolderName);
        if (!Directory.Exists(worldPath)) return null;

        string levelFile = SysPath.Combine(worldPath, "level.dat");
        if (!File.Exists(levelFile))
        {
            levelFile = SysPath.Combine(worldPath, "level.dat_old");
        }

        if (File.Exists(levelFile))
        {
            try
            {
                using var stream = File.OpenRead(levelFile);
                NBTTagCompound root = NbtIo.ReadCompressed(stream);
                NBTTagCompound data = root.GetCompoundTag("Data");

                var properties = new WorldProperties(data);
                properties.SizeOnDisk = GetFolderSize(new DirectoryInfo(worldPath));
                return properties;
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to read properties for {worldFolderName}: {ex.Message}");
            }
        }

        return null;
    }

    public void RenameWorld(string worldFolderName, string newDisplayName)
    {
        string levelFile = SysPath.Combine(_baseDir.FullName, worldFolderName, "level.dat");

        if (File.Exists(levelFile))
        {
            try
            {
                NBTTagCompound root;
                using (var readStream = File.OpenRead(levelFile))
                {
                    root = NbtIo.ReadCompressed(readStream);
                }

                NBTTagCompound data = root.GetCompoundTag("Data");
                data.SetString("LevelName", newDisplayName);

                using (var writeStream = File.Create(levelFile))
                {
                    NbtIo.WriteCompressed(root, writeStream);
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to rename world {worldFolderName}: {ex.Message}");
            }
        }
    }

    public void DeleteWorld(string worldFolderName)
    {
        string worldPath = SysPath.Combine(_baseDir.FullName, worldFolderName);
        if (Directory.Exists(worldPath))
        {
            try
            {
                // true = recursive delete
                Directory.Delete(worldPath, true);
            }
            catch (IOException ex)
            {
                Log.Error($"Failed to delete world {worldFolderName}: {ex.Message}");
            }
        }
    }
}
