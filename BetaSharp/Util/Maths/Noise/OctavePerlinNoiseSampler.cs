namespace BetaSharp.Util.Maths.Noise;

public class OctavePerlinNoiseSampler : NoiseSampler
{

    private PerlinNoiseSampler[] generatorCollection;
    private int field_1191_b;

    public OctavePerlinNoiseSampler(java.util.Random var1, int var2)
    {
        field_1191_b = var2;
        generatorCollection = new PerlinNoiseSampler[var2];

        for (int var3 = 0; var3 < var2; ++var3)
        {
            generatorCollection[var3] = new PerlinNoiseSampler(var1);
        }

    }

    public double func_806_a(double var1, double var3)
    {
        double var5 = 0.0D;
        double var7 = 1.0D;

        for (int var9 = 0; var9 < field_1191_b; ++var9)
        {
            var5 += generatorCollection[var9].func_801_a(var1 * var7, var3 * var7) / var7;
            var7 /= 2.0D;
        }

        return var5;
    }

    public double[] create(double[] var1, double var2, double var4, double var6, int var8, int var9, int var10, double var11, double var13, double var15)
    {
        if (var1 == null)
        {
            var1 = new double[var8 * var9 * var10];
        }
        else
        {
            for (int var17 = 0; var17 < var1.Length; ++var17)
            {
                var1[var17] = 0.0D;
            }
        }

        double var20 = 1.0D;

        for (int var19 = 0; var19 < field_1191_b; ++var19)
        {
            generatorCollection[var19].func_805_a(var1, var2, var4, var6, var8, var9, var10, var11 * var20, var13 * var20, var15 * var20, var20);
            var20 /= 2.0D;
        }

        return var1;
    }

    public double[] create(double[] var1, int var2, int var3, int var4, int var5, double var6, double var8, double var10)
    {
        return create(var1, var2, 10.0D, var3, var4, 1, var5, var6, 1.0D, var8);
    }
}