using BetaSharp.Blocks;
using BetaSharp.Blocks.Materials;
using BetaSharp.Entities;
using BetaSharp.Util;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds;

namespace BetaSharp;

public class Pathfinder : java.lang.Object
{
    private readonly BlockView worldMap;
    private readonly Path path = new();
    private readonly Dictionary<int, PathPoint> pointMap = new();
    private readonly PathPoint[] pathOptions = new PathPoint[32];

    public Pathfinder(BlockView var1)
    {
        worldMap = var1;
    }

    public PathEntity createEntityPathTo(Entity var1, Entity var2, float var3)
    {
        return createEntityPathTo(var1, var2.x, var2.boundingBox.minY, var2.z, var3);
    }

    public PathEntity createEntityPathTo(Entity var1, int var2, int var3, int var4, float var5)
    {
        return createEntityPathTo(var1, (double)((float)var2 + 0.5F), (double)((float)var3 + 0.5F), (double)((float)var4 + 0.5F), var5);
    }

    private PathEntity createEntityPathTo(Entity var1, double var2, double var4, double var6, float var8)
    {
        path.clearPath();
        pointMap.Clear();
        PathPoint var9 = openPoint(MathHelper.Floor(var1.boundingBox.minX), MathHelper.Floor(var1.boundingBox.minY), MathHelper.Floor(var1.boundingBox.minZ));
        PathPoint var10 = openPoint(MathHelper.Floor(var2 - (double)(var1.width / 2.0F)), MathHelper.Floor(var4), MathHelper.Floor(var6 - (double)(var1.width / 2.0F)));
        PathPoint var11 = new(MathHelper.Floor(var1.width + 1.0F), MathHelper.Floor(var1.height + 1.0F), MathHelper.Floor(var1.width + 1.0F));
        PathEntity var12 = addToPath(var1, var9, var10, var11, var8);
        return var12;
    }

    private PathEntity addToPath(Entity var1, PathPoint var2, PathPoint var3, PathPoint var4, float var5)
    {
        var2.totalPathDistance = 0.0F;
        var2.distanceToNext = var2.distanceTo(var3);
        var2.distanceToTarget = var2.distanceToNext;
        path.clearPath();
        path.addPoint(var2);
        PathPoint var6 = var2;

        while (!path.isPathEmpty())
        {
            PathPoint var7 = path.dequeue();
            if (var7.equals(var3))
            {
                return createEntityPath(var2, var3);
            }

            if (var7.distanceTo(var3) < var6.distanceTo(var3))
            {
                var6 = var7;
            }

            var7.isFirst = true;
            int var8 = findPathOptions(var1, var7, var4, var3, var5);

            for (int var9 = 0; var9 < var8; ++var9)
            {
                PathPoint var10 = pathOptions[var9];
                float var11 = var7.totalPathDistance + var7.distanceTo(var10);
                if (!var10.isAssigned() || var11 < var10.totalPathDistance)
                {
                    var10.previous = var7;
                    var10.totalPathDistance = var11;
                    var10.distanceToNext = var10.distanceTo(var3);
                    if (var10.isAssigned())
                    {
                        path.changeDistance(var10, var10.totalPathDistance + var10.distanceToNext);
                    }
                    else
                    {
                        var10.distanceToTarget = var10.totalPathDistance + var10.distanceToNext;
                        path.addPoint(var10);
                    }
                }
            }
        }

        if (var6 == var2)
        {
            return null;
        }
        else
        {
            return createEntityPath(var2, var6);
        }
    }

    private int findPathOptions(Entity var1, PathPoint var2, PathPoint var3, PathPoint var4, float var5)
    {
        int var6 = 0;
        byte var7 = 0;
        if (getVerticalOffset(var1, var2.xCoord, var2.yCoord + 1, var2.zCoord, var3) == 1)
        {
            var7 = 1;
        }

        PathPoint var8 = getSafePoint(var1, var2.xCoord, var2.yCoord, var2.zCoord + 1, var3, var7);
        PathPoint var9 = getSafePoint(var1, var2.xCoord - 1, var2.yCoord, var2.zCoord, var3, var7);
        PathPoint var10 = getSafePoint(var1, var2.xCoord + 1, var2.yCoord, var2.zCoord, var3, var7);
        PathPoint var11 = getSafePoint(var1, var2.xCoord, var2.yCoord, var2.zCoord - 1, var3, var7);
        if (var8 != null && !var8.isFirst && var8.distanceTo(var4) < var5)
        {
            pathOptions[var6++] = var8;
        }

        if (var9 != null && !var9.isFirst && var9.distanceTo(var4) < var5)
        {
            pathOptions[var6++] = var9;
        }

        if (var10 != null && !var10.isFirst && var10.distanceTo(var4) < var5)
        {
            pathOptions[var6++] = var10;
        }

        if (var11 != null && !var11.isFirst && var11.distanceTo(var4) < var5)
        {
            pathOptions[var6++] = var11;
        }

        return var6;
    }

    private PathPoint getSafePoint(Entity var1, int var2, int var3, int var4, PathPoint var5, int var6)
    {
        PathPoint var7 = null;
        if (getVerticalOffset(var1, var2, var3, var4, var5) == 1)
        {
            var7 = openPoint(var2, var3, var4);
        }

        if (var7 == null && var6 > 0 && getVerticalOffset(var1, var2, var3 + var6, var4, var5) == 1)
        {
            var7 = openPoint(var2, var3 + var6, var4);
            var3 += var6;
        }

        if (var7 != null)
        {
            int var8 = 0;
            int var9 = 0;

            while (var3 > 0)
            {
                var9 = getVerticalOffset(var1, var2, var3 - 1, var4, var5);
                if (var9 != 1)
                {
                    break;
                }

                ++var8;
                if (var8 >= 4)
                {
                    return null;
                }

                --var3;
                if (var3 > 0)
                {
                    var7 = openPoint(var2, var3, var4);
                }
            }

            if (var9 == -2)
            {
                return null;
            }
        }

        return var7;
    }

    private PathPoint openPoint(int var1, int var2, int var3)
    {
        int var4 = PathPoint.func_22329_a(var1, var2, var3);
        PathPoint var5 = pointMap.GetValueOrDefault(var4);
        if (var5 == null)
        {
            var5 = new PathPoint(var1, var2, var3);
            pointMap[var4] = var5;
        }

        return var5;
    }

    private int getVerticalOffset(Entity var1, int var2, int var3, int var4, PathPoint var5)
    {
        for (int var6 = var2; var6 < var2 + var5.xCoord; ++var6)
        {
            for (int var7 = var3; var7 < var3 + var5.yCoord; ++var7)
            {
                for (int var8 = var4; var8 < var4 + var5.zCoord; ++var8)
                {
                    int var9 = worldMap.getBlockId(var6, var7, var8);
                    if (var9 > 0)
                    {
                        if (var9 != Block.IronDoor.id && var9 != Block.Door.id)
                        {
                            Material var11 = Block.Blocks[var9].material;
                            if (var11.BlocksMovement)
                            {
                                return 0;
                            }

                            if (var11 == Material.Water)
                            {
                                return -1;
                            }

                            if (var11 == Material.Lava)
                            {
                                return -2;
                            }
                        }
                        else
                        {
                            int var10 = worldMap.getBlockMeta(var6, var7, var8);
                            if (!BlockDoor.isOpen(var10))
                            {
                                return 0;
                            }
                        }
                    }
                }
            }
        }

        return 1;
    }

    private PathEntity createEntityPath(PathPoint var1, PathPoint var2)
    {
        int var3 = 1;

        PathPoint var4;
        for (var4 = var2; var4.previous != null; var4 = var4.previous)
        {
            ++var3;
        }

        PathPoint[] var5 = new PathPoint[var3];
        var4 = var2;
        --var3;

        for (var5[var3] = var2; var4.previous != null; var5[var3] = var4)
        {
            var4 = var4.previous;
            --var3;
        }

        return new PathEntity(var5);
    }
}
