using java.lang;

namespace BetaSharp.Util.Maths;

public class Vec3i : java.lang.Object, Comparable
{
    public int x;
    public int y;
    public int z;

    public Vec3i()
    {
    }

    public Vec3i(int var1, int var2, int var3)
    {
        x = var1;
        y = var2;
        z = var3;
    }

    public Vec3i(Vec3i var1)
    {
        x = var1.x;
        y = var1.y;
        z = var1.z;
    }

    public override bool equals(object var1)
    {
        if (var1 is not Vec3i)
        {
            return false;
        }
        else
        {
            Vec3i var2 = (Vec3i)var1;
            return x == var2.x && y == var2.y && z == var2.z;
        }
    }

    public override int hashCode()
    {
        return x + z << 8 + y << 16;
    }

    public int compareChunkCoordinate(Vec3i var1)
    {
        return y == var1.y ? z == var1.z ? x - var1.x : z - var1.z : y - var1.y;
    }

    public double getSqDistanceTo(int var1, int var2, int var3)
    {
        int var4 = x - var1;
        int var5 = y - var2;
        int var6 = z - var3;
        return java.lang.Math.sqrt(var4 * var4 + var5 * var5 + var6 * var6);
    }

    public int CompareTo(object? var1)
    {
        return compareChunkCoordinate((Vec3i)var1!);
    }
}