using System;
using System.Collections.Generic;
using System.IO;
using static System.IO.Path;

namespace BetaSharp.Worlds.Chunks.Storage;

public static class RegionIo
{
    // Dictionary replaces HashMap. WeakReference replaces SoftReference.
    private static readonly Dictionary<string, WeakReference<RegionFile>> Cache = new();
    private static readonly object _lock = new();

    public static RegionFile GetRegionFile(string baseDir, int chunkX, int chunkZ)
    {
        lock (_lock)
        {
            string regionDirPath = Combine(baseDir, "region");
            // Minecraft region files are named based on chunk coordinates divided by 32
            string fileName = $"r.{chunkX >> 5}.{chunkZ >> 5}.mcr";
            string filePath = Combine(regionDirPath, fileName);

            if (Cache.TryGetValue(filePath, out var weakRef))
            {
                if (weakRef.TryGetTarget(out RegionFile cachedFile))
                {
                    return cachedFile;
                }
            }

            if (!Directory.Exists(regionDirPath))
            {
                Directory.CreateDirectory(regionDirPath);
            }

            if (Cache.Count >= 256)
            {
                Flush();
            }

            // You'll need to update RegionFile's constructor to take a string path
            RegionFile newRegionFile = new RegionFile(filePath);
            Cache[filePath] = new WeakReference<RegionFile>(newRegionFile);

            return newRegionFile;
        }
    }

    public static void Flush()
    {
        lock (_lock)
        {
            foreach (var weakRef in Cache.Values)
            {
                if (weakRef.TryGetTarget(out RegionFile region))
                {
                    try
                    {
                        // Assuming func_22196_b is the "close" or "sync" method
                        region.Close();
                    }
                    catch (IOException ex)
                    {
                        Log.Error($"Error flushing region file: {ex.Message}");
                    }
                }
            }
            Cache.Clear();
        }
    }

    public static int GetSizeDelta(string baseDir, int chunkX, int chunkZ)
    {
        RegionFile region = GetRegionFile(baseDir, chunkX, chunkZ);
        // Assuming func_22209_a is GetSizeDelta
        return region.GetSizeDelta();
    }

    public static ChunkDataStream GetChunkInputStream(string baseDir, int chunkX, int chunkZ)
    {
        RegionFile region = GetRegionFile(baseDir, chunkX, chunkZ);
        // Chunks inside a region file are indexed 0-31
        return region.GetChunkDataInputStream(chunkX & 31, chunkZ & 31);
    }

    public static Stream GetChunkOutputStream(string baseDir, int chunkX, int chunkZ)
    {
        RegionFile region = GetRegionFile(baseDir, chunkX, chunkZ);
        return region.GetChunkDataOutputStream(chunkX & 31, chunkZ & 31);
    }
}
