using BetaSharp.Entities;
using BetaSharp.Util.Maths;

namespace BetaSharp;

public class PathEntity
{
    private readonly PathPoint[] points;
    public readonly int pathLength;
    private int pathIndex;

    public PathEntity(PathPoint[] var1)
    {
        points = var1;
        pathLength = var1.Length;
    }

    public void incrementPathIndex()
    {
        ++pathIndex;
    }

    public bool isFinished()
    {
        return pathIndex >= points.Length;
    }

    public PathPoint func_22328_c()
    {
        return pathLength > 0 ? points[pathLength - 1] : null;
    }

    public Vec3D getPosition(Entity var1)
    {
        double var2 = (double)points[pathIndex].xCoord + (double)((int)(var1.width + 1.0F)) * 0.5D;
        double var4 = (double)points[pathIndex].yCoord;
        double var6 = (double)points[pathIndex].zCoord + (double)((int)(var1.width + 1.0F)) * 0.5D;
        return new Vec3D(var2, var4, var6);
    }
}