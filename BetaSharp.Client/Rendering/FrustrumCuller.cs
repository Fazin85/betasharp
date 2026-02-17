using BetaSharp.Util.Maths;

namespace BetaSharp.Client.Rendering;

public class FrustrumCuller : Culler
{

    private readonly FrustumData frustum = Frustum.getInstance();
    private double xPosition;
    private double yPosition;
    private double zPosition;

    public void setPosition(double x, double y, double z)
    {
        xPosition = x;
        yPosition = y;
        zPosition = z;
    }

    public bool isBoxInFrustum(double minX, double minY, double minZ, double maxX, double maxY, double maxZ)
    {
        return frustum.isBoxInFrustum(minX - xPosition, minY - yPosition, minZ - zPosition, maxX - xPosition, maxY - yPosition, maxZ - zPosition);
    }

    public bool isBoundingBoxInFrustum(Box aabb)
    {
        return isBoxInFrustum(aabb.minX, aabb.minY, aabb.minZ, aabb.maxX, aabb.maxY, aabb.maxZ);
    }
}
