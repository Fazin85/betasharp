using BetaSharp.Util.Hit;

namespace BetaSharp.Util.Maths;

public struct Box
{
    public double minX;
    public double minY;
    public double minZ;
    public double maxX;
    public double maxY;
    public double maxZ;
        
    public Box(double x1, double y1, double z1, double x2, double y2, double z2)
    {
        minX = x1;
        minY = y1;
        minZ = z1;
        maxX = x2;
        maxY = y2;
        maxZ = z2;
    }

    public Box stretch(double x, double y, double z)
    {
        double var7 = minX;
        double var9 = minY;
        double var11 = minZ;
        double var13 = maxX;
        double var15 = maxY;
        double var17 = maxZ;
        if (x < 0.0D)
        {
            var7 += x;
        }

        if (x > 0.0D)
        {
            var13 += x;
        }

        if (y < 0.0D)
        {
            var9 += y;
        }

        if (y > 0.0D)
        {
            var15 += y;
        }

        if (z < 0.0D)
        {
            var11 += z;
        }

        if (z > 0.0D)
        {
            var17 += z;
        }

        return new Box(var7, var9, var11, var13, var15, var17);
    }

    public Box expand(double x, double y, double z)
    {
        double var7 = minX - x;
        double var9 = minY - y;
        double var11 = minZ - z;
        double var13 = maxX + x;
        double var15 = maxY + y;
        double var17 = maxZ + z;
        return new Box(var7, var9, var11, var13, var15, var17);
    }

    public Box offset(double x, double y, double z)
    {
        return new Box(minX + x, minY + y, minZ + z, maxX + x, maxY + y, maxZ + z);
    }

    public double getXOffset(Box box, double x)
    {
        if (box.maxY > minY && box.minY < maxY)
        {
            if (box.maxZ > minZ && box.minZ < maxZ)
            {
                double var4;
                if (x > 0.0D && box.maxX <= minX)
                {
                    var4 = minX - box.maxX;
                    if (var4 < x)
                    {
                        x = var4;
                    }
                }

                if (x < 0.0D && box.minX >= maxX)
                {
                    var4 = maxX - box.minX;
                    if (var4 > x)
                    {
                        x = var4;
                    }
                }

                return x;
            }
            else
            {
                return x;
            }
        }
        else
        {
            return x;
        }
    }

    public double getYOffset(Box box, double y)
    {
        if (box.maxX > minX && box.minX < maxX)
        {
            if (box.maxZ > minZ && box.minZ < maxZ)
            {
                double var4;
                if (y > 0.0D && box.maxY <= minY)
                {
                    var4 = minY - box.maxY;
                    if (var4 < y)
                    {
                        y = var4;
                    }
                }

                if (y < 0.0D && box.minY >= maxY)
                {
                    var4 = maxY - box.minY;
                    if (var4 > y)
                    {
                        y = var4;
                    }
                }

                return y;
            }
            else
            {
                return y;
            }
        }
        else
        {
            return y;
        }
    }

    public double getZOffset(Box box, double z)
    {
        if (box.maxX > minX && box.minX < maxX)
        {
            if (box.maxY > minY && box.minY < maxY)
            {
                double var4;
                if (z > 0.0D && box.maxZ <= minZ)
                {
                    var4 = minZ - box.maxZ;
                    if (var4 < z)
                    {
                        z = var4;
                    }
                }

                if (z < 0.0D && box.minZ >= maxZ)
                {
                    var4 = maxZ - box.minZ;
                    if (var4 > z)
                    {
                        z = var4;
                    }
                }

                return z;
            }
            else
            {
                return z;
            }
        }
        else
        {
            return z;
        }
    }

    public bool intersects(Box box)
    {
        return box.maxX > minX && box.minX < maxX ? box.maxY > minY && box.minY < maxY ? box.maxZ > minZ && box.minZ < maxZ : false : false;
    }

    public Box translate(double x, double y, double z)
    {
        minX += x;
        minY += y;
        minZ += z;
        maxX += x;
        maxY += y;
        maxZ += z;
        return this;
    }

    public bool contains(Vec3D pos)
    {
        return pos.x > minX && pos.x < maxX ? pos.y > minY && pos.y < maxY ? pos.z > minZ && pos.z < maxZ : false : false;
    }

    public double getAverageSizeLength()
    {
        double var1 = maxX - minX;
        double var3 = maxY - minY;
        double var5 = maxZ - minZ;
        return (var1 + var3 + var5) / 3.0D;
    }

    public Box contract(double x, double y, double z)
    {
        double var7 = minX + x;
        double var9 = minY + y;
        double var11 = minZ + z;
        double var13 = maxX - x;
        double var15 = maxY - y;
        double var17 = maxZ - z;
        return new Box(var7, var9, var11, var13, var15, var17);
    }

    public HitResult raycast(Vec3D startPos, Vec3D endPos)
    {
        Vec3D? hitMinX = startPos.getIntermediateWithXValue(endPos, minX);
        Vec3D? hitMaxX = startPos.getIntermediateWithXValue(endPos, maxX);
        Vec3D? hitMinY = startPos.getIntermediateWithYValue(endPos, minY);
        Vec3D? hitMaxY = startPos.getIntermediateWithYValue(endPos, maxY);
        Vec3D? hitMinZ = startPos.getIntermediateWithZValue(endPos, minZ);
        Vec3D? hitMaxZ = startPos.getIntermediateWithZValue(endPos, maxZ);
        if (hitMinX != null && !isVecInsideYZBounds(hitMinX.Value))
        {
            hitMinX = null;
        }

        if (hitMaxX != null && !isVecInsideYZBounds(hitMaxX.Value))
        {
            hitMaxX = null;
        }

        if (hitMinY != null && !isVecInsideXZBounds(hitMinY.Value))
        {
            hitMinY = null;
        }

        if (hitMaxY != null && !isVecInsideXZBounds(hitMaxY.Value))
        {
            hitMaxY = null;
        }

        if (hitMinZ != null && !isVecInsideXYBounds(hitMinZ.Value))
        {
            hitMinZ = null;
        }

        if (hitMaxZ != null && !isVecInsideXYBounds(hitMaxZ.Value))
        {
            hitMaxZ = null;
        }

        Vec3D? hitPos = null;
        if (hitMinX != null && (hitPos == null || startPos.distanceTo(hitMinX.Value) < startPos.distanceTo(hitPos.Value)))
        {
            hitPos = hitMinX;
        }

        if (hitMaxX != null && (hitPos == null || startPos.distanceTo(hitMaxX.Value) < startPos.distanceTo(hitPos.Value)))
        {
            hitPos = hitMaxX;
        }

        if (hitMinY != null && (hitPos == null || startPos.distanceTo(hitMinY.Value) < startPos.distanceTo(hitPos.Value)))
        {
            hitPos = hitMinY;
        }

        if (hitMaxY != null && (hitPos == null || startPos.distanceTo(hitMaxY.Value) < startPos.distanceTo(hitPos.Value)))
        {
            hitPos = hitMaxY;
        }

        if (hitMinZ != null && (hitPos == null || startPos.distanceTo(hitMinZ.Value) < startPos.distanceTo(hitPos.Value)))
        {
            hitPos = hitMinZ;
        }

        if (hitMaxZ != null && (hitPos == null || startPos.distanceTo(hitMaxZ.Value) < startPos.distanceTo(hitPos.Value)))
        {
            hitPos = hitMaxZ;
        }

        if (hitPos == null)
        {
            return new HitResult(HitResultType.MISS);
        }
        else
        {
            int side = -1;
            if (hitPos == hitMinX)
            {
                side = 4;
            }

            if (hitPos == hitMaxX)
            {
                side = 5;
            }

            if (hitPos == hitMinY)
            {
                side = 0;
            }

            if (hitPos == hitMaxY)
            {
                side = 1;
            }

            if (hitPos == hitMinZ)
            {
                side = 2;
            }

            if (hitPos == hitMaxZ)
            {
                side = 3;
            }

            return new HitResult(0, 0, 0, side, hitPos.Value, HitResultType.TILE);
        }
    }

    private bool isVecInsideYZBounds(Vec3D pos)
    {
        return pos.y >= minY && pos.y <= maxY && pos.z >= minZ && pos.z <= maxZ;
    }

    private bool isVecInsideXZBounds(Vec3D pos)
    {
        return pos.x >= minX && pos.x <= maxX && pos.z >= minZ && pos.z <= maxZ;
    }

    private bool isVecInsideXYBounds(Vec3D pos)
    {
        return pos.x >= minX && pos.x <= maxX && pos.y >= minY && pos.y <= maxY;
    }

    public override string ToString()
    {
        return "box[" + minX + ", " + minY + ", " + minZ + " -> " + maxX + ", " + maxY + ", " + maxZ + "]";
    }
}