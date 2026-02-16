namespace BetaSharp.Client.Rendering;

public class FrustumData : java.lang.Object
{
    public float[][] frustum = new float[16][];
    public float[] projectionMatrix = new float[16];
    public float[] modelviewMatrix = new float[16];
    public float[] clippingMatrix = new float[16];

    public FrustumData()
    {
        for (int i = 0; i < 16; i++)
        {
            frustum[i] = new float[16];
        }
    }

    public bool isBoxInFrustum(double var1, double var3, double var5, double var7, double var9, double var11)
    {
        for (int var13 = 0; var13 < 6; ++var13)
        {
            if (frustum[var13][0] * var1 + frustum[var13][1] * var3 + frustum[var13][2] * var5 + frustum[var13][3] <= 0.0D && frustum[var13][0] * var7 + frustum[var13][1] * var3 + frustum[var13][2] * var5 + frustum[var13][3] <= 0.0D && frustum[var13][0] * var1 + frustum[var13][1] * var9 + frustum[var13][2] * var5 + frustum[var13][3] <= 0.0D && frustum[var13][0] * var7 + frustum[var13][1] * var9 + frustum[var13][2] * var5 + frustum[var13][3] <= 0.0D && frustum[var13][0] * var1 + frustum[var13][1] * var3 + frustum[var13][2] * var11 + frustum[var13][3] <= 0.0D && frustum[var13][0] * var7 + frustum[var13][1] * var3 + frustum[var13][2] * var11 + frustum[var13][3] <= 0.0D && frustum[var13][0] * var1 + frustum[var13][1] * var9 + frustum[var13][2] * var11 + frustum[var13][3] <= 0.0D && frustum[var13][0] * var7 + frustum[var13][1] * var9 + frustum[var13][2] * var11 + frustum[var13][3] <= 0.0D)
            {
                return false;
            }
        }

        return true;
    }
}