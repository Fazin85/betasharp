using java.lang;

namespace BetaSharp;

public class Path : java.lang.Object
{
    private PathPoint[] pathPoints = new PathPoint[1024];
    private int count;

    public PathPoint addPoint(PathPoint var1)
    {
        if (var1.index >= 0)
        {
            throw new IllegalStateException("OW KNOWS!");
        }
        else
        {
            if (count == pathPoints.Length)
            {
                PathPoint[] var2 = new PathPoint[count << 1];
                java.lang.System.arraycopy(pathPoints, 0, var2, 0, count);
                pathPoints = var2;
            }

            pathPoints[count] = var1;
            var1.index = count;
            sortBack(count++);
            return var1;
        }
    }

    public void clearPath()
    {
        count = 0;
    }

    public PathPoint dequeue()
    {
        PathPoint var1 = pathPoints[0];
        pathPoints[0] = pathPoints[--count];
        pathPoints[count] = null;
        if (count > 0)
        {
            sortForward(0);
        }

        var1.index = -1;
        return var1;
    }

    public void changeDistance(PathPoint var1, float var2)
    {
        float var3 = var1.distanceToTarget;
        var1.distanceToTarget = var2;
        if (var2 < var3)
        {
            sortBack(var1.index);
        }
        else
        {
            sortForward(var1.index);
        }

    }

    private void sortBack(int var1)
    {
        PathPoint var2 = pathPoints[var1];

        int var4;
        for (float var3 = var2.distanceToTarget; var1 > 0; var1 = var4)
        {
            var4 = var1 - 1 >> 1;
            PathPoint var5 = pathPoints[var4];
            if (var3 >= var5.distanceToTarget)
            {
                break;
            }

            pathPoints[var1] = var5;
            var5.index = var1;
        }

        pathPoints[var1] = var2;
        var2.index = var1;
    }

    private void sortForward(int var1)
    {
        PathPoint var2 = pathPoints[var1];
        float var3 = var2.distanceToTarget;

        while (true)
        {
            int var4 = 1 + (var1 << 1);
            int var5 = var4 + 1;
            if (var4 >= count)
            {
                break;
            }

            PathPoint var6 = pathPoints[var4];
            float var7 = var6.distanceToTarget;
            PathPoint var8;
            float var9;
            if (var5 >= count)
            {
                var8 = null;
                var9 = Float.POSITIVE_INFINITY;
            }
            else
            {
                var8 = pathPoints[var5];
                var9 = var8.distanceToTarget;
            }

            if (var7 < var9)
            {
                if (var7 >= var3)
                {
                    break;
                }

                pathPoints[var1] = var6;
                var6.index = var1;
                var1 = var4;
            }
            else
            {
                if (var9 >= var3)
                {
                    break;
                }

                pathPoints[var1] = var8;
                var8.index = var1;
                var1 = var5;
            }
        }

        pathPoints[var1] = var2;
        var2.index = var1;
    }

    public bool isPathEmpty()
    {
        return count == 0;
    }
}