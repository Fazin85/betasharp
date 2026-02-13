using java.lang;

namespace BetaSharp.Worlds.Storage;

public class WorldSaveInfo : java.lang.Object, Comparable
{
    private readonly string fileName;
    private readonly string displayName;
    private readonly long lastPlayed;
    private readonly long size;
    private readonly bool isUnsupported;

    public WorldSaveInfo(string var1, string var2, long var3, long var5, bool var7)
    {
        fileName = var1;
        displayName = var2;
        lastPlayed = var3;
        size = var5;
        isUnsupported = var7;
    }

    public string getFileName()
    {
        return fileName;
    }

    public string getDisplayName()
    {
        return displayName;
    }

    public long getSize()
    {
        return size;
    }

    public bool getIsUnsupported()
    {
        return isUnsupported;
    }

    public long getLastPlayed()
    {
        return lastPlayed;
    }

    public int func_22160_a(WorldSaveInfo var1)
    {
        return lastPlayed < var1.lastPlayed ? 1 : lastPlayed > var1.lastPlayed ? -1 : fileName.CompareTo(var1.fileName);
    }

    public int CompareTo(object? var1)
    {
        return func_22160_a((WorldSaveInfo)var1!);
    }
}