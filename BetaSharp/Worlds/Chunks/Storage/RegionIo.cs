namespace BetaSharp.Worlds.Chunks.Storage;

internal static class RegionIo
{
    private static readonly Dictionary<string, WeakReference<RegionFile>> cache = new(StringComparer.Ordinal);
    private static readonly object gate = new();

    public static RegionFile CreateRegionFile(string worldDir, int chunkX, int chunkZ)
    {
        if (worldDir is null)
        {
            throw new ArgumentNullException(nameof(worldDir));
        }

        lock (gate)
        {
            string regionDir = Path.Combine(worldDir, "region");
            string regionFileName = $"r.{chunkX >> 5}.{chunkZ >> 5}.mcr";
            string regionPath = Path.Combine(regionDir, regionFileName);

            if (cache.TryGetValue(regionPath, out WeakReference<RegionFile>? weakRef))
            {
                if (weakRef.TryGetTarget(out RegionFile? existing))
                {
                    return existing;
                }
            }

            if (!Directory.Exists(regionDir))
            {
                Directory.CreateDirectory(regionDir);
            }

            if (cache.Count >= 256)
            {
                Flush();
            }

            RegionFile created = new(regionPath);
            cache[regionPath] = new WeakReference<RegionFile>(created);
            return created;
        }
    }

    public static void Flush()
    {
        lock (gate)
        {
            foreach (WeakReference<RegionFile> weakRef in cache.Values)
            {
                if (weakRef.TryGetTarget(out RegionFile? regionFile))
                {
                    regionFile.Close();
                }
            }

            cache.Clear();
        }
    }

    public static int GetSizeDelta(string worldDir, int chunkX, int chunkZ)
    {
        RegionFile regionFile = CreateRegionFile(worldDir, chunkX, chunkZ);
        return regionFile.func_22209_a();
    }

    public static ChunkDataStream? GetChunkInputStream(string worldDir, int chunkX, int chunkZ)
    {
        RegionFile regionFile = CreateRegionFile(worldDir, chunkX, chunkZ);
        return regionFile.GetChunkDataInputStream(chunkX & 31, chunkZ & 31);
    }

    public static Stream? GetChunkOutputStream(string worldDir, int chunkX, int chunkZ)
    {
        RegionFile regionFile = CreateRegionFile(worldDir, chunkX, chunkZ);
        return regionFile.GetChunkDataOutputStream(chunkX & 31, chunkZ & 31);
    }
}
