using BetaSharp.Util.Maths;

namespace BetaSharp.Client.Rendering;

public class FrustumData
{
    public float[] Frustum = new float[24];
    public float[] ProjectionMatrix = new float[16];
    public float[] ModelviewMatrix = new float[16];
    public float[] ClippingMatrix = new float[16];
    public bool IsBoxInFrustum(Box box)
    {
        float fMinX = (float)box.minX, fMinY = (float)box.minY, fMinZ = (float)box.minZ;
        float fMaxX = (float)box.maxX, fMaxY = (float)box.maxY, fMaxZ = (float)box.maxZ;

        for (int i = 0; i < 6; i++)
        {
            int offset = i * 4;
            float a = Frustum[offset];
            float b = Frustum[offset + 1];
            float c = Frustum[offset + 2];
            float d = Frustum[offset + 3];

            if (a * fMinX + b * fMinY + c * fMinZ + d <= 0.0f &&
                a * fMaxX + b * fMinY + c * fMinZ + d <= 0.0f &&
                a * fMinX + b * fMaxY + c * fMinZ + d <= 0.0f &&
                a * fMaxX + b * fMaxY + c * fMinZ + d <= 0.0f &&
                a * fMinX + b * fMinY + c * fMaxZ + d <= 0.0f &&
                a * fMaxX + b * fMinY + c * fMaxZ + d <= 0.0f &&
                a * fMinX + b * fMaxY + c * fMaxZ + d <= 0.0f &&
                a * fMaxX + b * fMaxY + c * fMaxZ + d <= 0.0f)
            {
                return false;
            }
        }

        return true;
    }
}