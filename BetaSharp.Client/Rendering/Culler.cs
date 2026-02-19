using BetaSharp.Util.Maths;

namespace BetaSharp.Client.Rendering;

public interface Culler
{
    bool isBoundingBoxInFrustum(Box aabb);

    void setPosition(double x, double y, double z);
}
