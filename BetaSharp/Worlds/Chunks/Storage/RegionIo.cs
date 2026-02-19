using System;
using System.Collections.Generic;
using System.IO;
using static System.IO.Path;

namespace BetaSharp.Worlds.Chunks.Storage;

public static class RegionIo
{

    private static readonly Dictionary<string, WeakReference<RegionFile>> Cache = new();
    private static readonly object _lock = new();

    public static RegionFile GetRegionFile(string baseDir, int chunkX, int chunkZ)
    {
        lock (_lock)
        {
            string regionDirPath = Combine(baseDir, "region");

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
        return region.GetSizeDelta();
    }

    public static ChunkDataStream GetChunkInputStream(string baseDir, int chunkX, int chunkZ)
    {
        RegionFile region = GetRegionFile(baseDir, chunkX, chunkZ);
        return region.GetChunkDataInputStream(chunkX & 31, chunkZ & 31);
    }

    public static Stream GetChunkOutputStream(string baseDir, int chunkX, int chunkZ)
    {
        RegionFile region = GetRegionFile(baseDir, chunkX, chunkZ);
        return region.GetChunkDataOutputStream(chunkX & 31, chunkZ & 31);
    }
}
