using BetaSharp.Util.Maths;
using java.lang;

namespace BetaSharp;

public class PathPoint : java.lang.Object
{
    public readonly int xCoord;
    public readonly int yCoord;
    public readonly int zCoord;
    private readonly int hash;
    public int index = -1;
    public float totalPathDistance;
    public float distanceToNext;
    public float distanceToTarget;
    public PathPoint previous;
    public bool isFirst = false;

    public PathPoint(int var1, int var2, int var3)
    {
        xCoord = var1;
        yCoord = var2;
        zCoord = var3;
        hash = func_22329_a(var1, var2, var3);
    }

    public static int func_22329_a(int var0, int var1, int var2)
    {
        return var1 & 255 | (var0 & Short.MAX_VALUE) << 8 | (var2 & Short.MAX_VALUE) << 24 | (var0 < 0 ? Integer.MIN_VALUE : 0) | (var2 < 0 ? -Short.MIN_VALUE : 0);
    }

    public float distanceTo(PathPoint var1)
    {
        float var2 = (float)(var1.xCoord - xCoord);
        float var3 = (float)(var1.yCoord - yCoord);
        float var4 = (float)(var1.zCoord - zCoord);
        return MathHelper.Sqrt(var2 * var2 + var3 * var3 + var4 * var4);
    }

    public override bool equals(object var1)
    {
        if (var1 is not PathPoint)
        {
            return false;
        }
        else
        {
            PathPoint var2 = (PathPoint)var1;
            return hash == var2.hash && xCoord == var2.xCoord && yCoord == var2.yCoord && zCoord == var2.zCoord;
        }
    }

    public override int hashCode()
    {
        return hash;
    }

    public bool isAssigned()
    {
        return index >= 0;
    }

    public override string toString()
    {
        return xCoord + ", " + yCoord + ", " + zCoord;
    }
}