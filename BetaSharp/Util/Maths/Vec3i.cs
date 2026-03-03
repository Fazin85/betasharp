namespace BetaSharp.Util.Maths;

public record struct Vec3i(int X, int Y, int Z) : IComparable<Vec3i>
{
    public static readonly Vec3i Zero = new Vec3i(0, 0, 0);
    public Vec3i(Vec3i other) : this(other.X, other.Y, other.Z)
    {
    }
    public int CompareTo(Vec3i other)
    {
        if (Y != other.Y) return Y.CompareTo(other.Y);
        if (Z != other.Z) return Z.CompareTo(other.Z);
        return X.CompareTo(other.X);
    }

    public int SquaredDistanceTo(Vec3i other)
    {
        return Y == other.Y ? Z == other.Z ? X - other.X : Z - other.Z : Y - other.Y;
    }

    public double DistanceTo(Vec3i other)
    {
        int dx = X - other.X;
        int dy = Y - other.Y;
        int dz = Z - other.Z;
        return System.Math.Sqrt(dx * dx + dy * dy + dz * dz);
    }

    public static explicit operator Vec3D(Vec3i v) => new Vec3D(v.X, v.Y, v.Z);
}
