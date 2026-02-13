using java.lang;

namespace BetaSharp.Util.Maths;

public readonly record struct ChunkPos
{
    public readonly int x;
    public readonly int z;

    public ChunkPos(int var1, int var2)
    {
        x = var1;
        z = var2;
    }

    public static int hashCode(int var0, int var1)
    {
        return (var0 < 0 ? Integer.MIN_VALUE : 0) | (var0 & Short.MAX_VALUE) << 16 | (var1 < 0 ? -Short.MIN_VALUE : 0) | var1 & Short.MAX_VALUE;
    }

    public override int GetHashCode()
    {
        return hashCode(x, z);
    }
}