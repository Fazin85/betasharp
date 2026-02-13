using BetaSharp.Worlds.Storage;
using java.lang;
using java.util.regex;

namespace BetaSharp.Worlds.Chunks.Storage;

public class DataFile : java.lang.Object, Comparable
{
    private readonly java.io.File file;
    private readonly int chunkX;
    private readonly int chunkZ;

    public DataFile(java.io.File var1)
    {
        file = var1;
        Matcher var2 = DataFilenameFilter.PATTERN.matcher(var1.getName());
        if (var2.matches())
        {
            chunkX = Integer.parseInt(var2.group(1), 36);
            chunkZ = Integer.parseInt(var2.group(2), 36);
        }
        else
        {
            chunkX = 0;
            chunkZ = 0;
        }

    }

    public int comp(DataFile var1)
    {
        int var2 = chunkX >> 5;
        int var3 = var1.chunkX >> 5;
        if (var2 == var3)
        {
            int var4 = chunkZ >> 5;
            int var5 = var1.chunkZ >> 5;
            return var4 - var5;
        }
        else
        {
            return var2 - var3;
        }
    }

    public java.io.File getFile()
    {
        return file;
    }

    public int getChunkX()
    {
        return chunkX;
    }

    public int getChunkZ()
    {
        return chunkZ;
    }

    public int CompareTo(object? var1)
    {
        return comp((DataFile)var1!);
    }
}