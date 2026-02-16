using BetaSharp.Util.Maths;

namespace BetaSharp.Client.Rendering;

public class FrustrumCuller : Culler
{

    private readonly FrustumData frustum = Frustum.getInstance();
    private double xPosition;
    private double yPosition;
    private double zPosition;

    public void setPosition(double var1, double var3, double var5)
    {
        xPosition = var1;
        yPosition = var3;
        zPosition = var5;
    }

    public bool isBoxInFrustum(double var1, double var3, double var5, double var7, double var9, double var11)
    {
        return frustum.isBoxInFrustum(var1 - xPosition, var3 - yPosition, var5 - zPosition, var7 - xPosition, var9 - yPosition, var11 - zPosition);
    }

    public bool isBoundingBoxInFrustum(Box var1)
    {
        return isBoxInFrustum(var1.minX, var1.minY, var1.minZ, var1.maxX, var1.maxY, var1.maxZ);
    }
}