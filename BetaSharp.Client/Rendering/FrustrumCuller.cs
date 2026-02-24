using BetaSharp.Util.Maths;

namespace BetaSharp.Client.Rendering;

public class FrustrumCuller : Culler
{

    private readonly FrustumData frustum = Frustum.Instance();
    private double _x;
    private double _y;
    private double _z;

    public void setPosition(double x, double y, double z)
    {
        _x = x;
        _y = y;
        _z = z;
    }

    public bool IsBoxInFrustum(Box box)
    {
        return frustum.IsBoxInFrustum(box.Offset(-_x, -_y, -_z));
    }

    public bool isBoundingBoxInFrustum(Box aabb)
    {
        return IsBoxInFrustum(aabb);
    }
}
