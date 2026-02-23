using BetaSharp.Util.Maths;
using java.lang;

namespace BetaSharp.PathFinding;

internal class PathPoint
{
    private readonly int _hash;
    public readonly int X;
    public readonly int Y;
    public readonly int Z;
    
    public int Index = -1;
    public float TotalPathDistance;
    public float DistanceToNext;
    public float DistanceToTarget;
    public PathPoint? Previous;
    public bool IsFirst = false;

    public PathPoint(int x, int y, int z)
    {
        X = x;
        Y = y;
        Z = z;
        _hash = CalculateHash(x, y, z);
    }

    public static int CalculateHash(int x, int y, int z)
    {
        return (y & 255) | 
               ((x & short.MaxValue) << 8) | 
               ((z & short.MaxValue) << 24) | 
               (x < 0 ? int.MinValue : 0) | 
               (z < 0 ? 32768 : 0); 
    }

    public float DistanceTo(PathPoint other)
    {
        float dx = other.X - X;
        float dy = other.Y - Y;
        float dz = other.Z - Z;
        return MathHelper.Sqrt(dx * dx + dy * dy + dz * dz);
    }

    public override bool Equals(object? obj)
    {
        return obj is PathPoint other && 
               _hash == other._hash && 
               X == other.X && 
               Y == other.Y && 
               Z == other.Z;
    }

    public override int GetHashCode()
    {
        return _hash;
    }

    public bool IsAssigned()
    {
        return Index >= 0;
    }

    public override string ToString()
    {
        return $"{X}, {Y}, {Z}";
    }
}