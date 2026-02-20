namespace BetaSharp.Util.Maths;

public static class MathHelper
{
    private static readonly float[] SIN_TABLE = new float[65536];

    public static float sin(float var0)
    {
        return SIN_TABLE[(int)(var0 * 10430.378F) & '\uffff'];
    }

    public static float cos(float var0)
    {
        return SIN_TABLE[(int)(var0 * 10430.378F + 16384.0F) & '\uffff'];
    }

    public static float sqrt_float(float var0)
    {
        return (float)java.lang.Math.sqrt((double)var0);
    }

    public static int floor(double value)
    {
        int var2 = (int)value;
        return value < var2 ? var2 - 1 : var2;
    }

    public static float sqrt_double(double var0)
    {
        return (float)java.lang.Math.sqrt(var0);
    }

    public static int floor_float(float var0)
    {
        int var1 = (int)var0;
        return var0 < var1 ? var1 - 1 : var1;
    }

    public static int floor_double(double var0)
    {
        int var2 = (int)var0;
        return var0 < var2 ? var2 - 1 : var2;
    }

    public static float abs(float var0)
    {
        return var0 >= 0.0F ? var0 : -var0;
    }

    public static int floorDiv(int a, int b)
    {
        return a < 0 ? -((-a - 1) / b) - 1 : a / b;
    }

    static MathHelper()
    {
        for (int var0 = 0; var0 < 65536; ++var0)
        {
            SIN_TABLE[var0] = (float)java.lang.Math.sin(var0 * Math.PI * 2.0D / 65536.0D);
        }
    }
}
