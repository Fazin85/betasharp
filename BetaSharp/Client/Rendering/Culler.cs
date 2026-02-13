using BetaSharp.Util.Maths;

namespace BetaSharp.Client.Rendering;

public interface Culler
{
    bool isBoundingBoxInFrustum(Box var1);

    void setPosition(double var1, double var3, double var5);
}