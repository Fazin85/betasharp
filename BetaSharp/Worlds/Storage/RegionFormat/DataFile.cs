using java.lang;
using File = java.io.File;

namespace BetaSharp.Worlds.Storage.RegionFormat;

internal class DataFile : Comparable
{
    private readonly int chunkX;
    private readonly int chunkZ;
    private readonly File file;

    public DataFile(File file)
    {
        this.file = file;
        var match = DataFilenameFilter.ChunkFilePattern().Match(file.getName());
        if (match.Success)
        {
            chunkX = Integer.parseInt(match.Groups[1].Value, 36);
            chunkZ = Integer.parseInt(match.Groups[2].Value, 36);
        }
        else
        {
            chunkX = 0;
            chunkZ = 0;
        }
    }

    public int comp(DataFile file)
    {
        int regionX = chunkX >> 5;
        int otherRegionX = file.chunkX >> 5;
        if (regionX == otherRegionX)
        {
            int regionZ = chunkZ >> 5;
            int otherRegionZ = file.chunkZ >> 5;
            return regionZ - otherRegionZ;
        }
        else
        {
            return regionX - otherRegionX;
        }
    }

    public File getFile() => file;

    public int GetChunkX() => chunkX;

    public int GetChunkZ() => chunkZ;

    public int CompareTo(object? file) => comp((DataFile)file!);
}
