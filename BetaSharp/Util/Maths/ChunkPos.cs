namespace BetaSharp.Util.Maths;

public readonly record struct ChunkPos(int x, int z)
{
    public readonly int X = x;
    public readonly int Z = z;

    public static int GetHashCode(int x, int z)
    {
        return (x < 0 ? int.MinValue : 0) | (x & short.MaxValue) << 16 | (z < 0 ? -short.MinValue : 0) | z & short.MaxValue;
    }

    public override int GetHashCode()
    {
        return GetHashCode(X, Z);
    }
}
