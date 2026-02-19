using java.lang;

namespace BetaSharp;

public class BlockEvent : java.lang.Object, Comparable
{
    private static long nextTickEntryID;
    public int x;
    public int y;
    public int z;
    public int blockId;
    public long ticks;
    private readonly long tickEntryID = nextTickEntryID++;

    public BlockEvent(int var1, int var2, int var3, int var4)
    {
        x = var1;
        y = var2;
        z = var3;
        blockId = var4;
    }

    public override bool equals(object var1)
    {
        if (var1 is not BlockEvent)
        {
            return false;
        }
        else
        {
            BlockEvent var2 = (BlockEvent)var1;
            return x == var2.x && y == var2.y && z == var2.z && blockId == var2.blockId;
        }
    }

    public override int hashCode()
    {
        return (x * 128 * 1024 + z * 128 + y) * 256 + blockId;
    }

    public BlockEvent setScheduledTime(long var1)
    {
        ticks = var1;
        return this;
    }

    public int comparer(BlockEvent var1)
    {
        return ticks < var1.ticks ? -1 : (ticks > var1.ticks ? 1 : (tickEntryID < var1.tickEntryID ? -1 : (tickEntryID > var1.tickEntryID ? 1 : 0)));
    }

    public int CompareTo(object? var1)
    {
        return comparer((BlockEvent)var1!);
    }
}