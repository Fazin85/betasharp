namespace BetaSharp.Util.Maths.Noise;

public class OctavePerlinNoiseSampler : NoiseSampler
{

    private readonly PerlinNoiseSampler[] _octaves;
    private readonly int _octaveCount;

    public OctavePerlinNoiseSampler(java.util.Random rand, int octaveCount)
    {
        _octaveCount = octaveCount;
        _octaves = new PerlinNoiseSampler[octaveCount];

        for (int i = 0; i < octaveCount; ++i)
        {
            _octaves[i] = new PerlinNoiseSampler(rand);
        }
    }

    public double func_806_a(double var1, double var3)
    {
        double var5 = 0.0D;
        double var7 = 1.0D;

        for (int i = 0; i < _octaveCount; ++i)
        {
            var5 += _octaves[i].func_801_a(var1 * var7, var3 * var7) / var7;
            var7 /= 2.0D;
        }

        return var5;
    }

    public double[] create(double[] map, double var2, double var4, double var6, int var8, int var9, int var10, double var11, double var13, double var15)
    {
        if (map == null)
        {
            map = new double[var8 * var9 * var10];
        }
        else
        {
            Array.Fill(map, 0);
        }

        double var20 = 1.0D;

        for (int i = 0; i < _octaveCount; ++i)
        {
            _octaves[i].func_805_a(map, var2, var4, var6, var8, var9, var10, var11 * var20, var13 * var20, var15 * var20, var20);
            var20 /= 2.0D;
        }

        return map;
    }

    public double[] create(double[] map, int var2, int var3, int var4, int var5, double var6, double var8, double var10)
    {
        return create(map, var2, 10.0D, var3, var4, 1, var5, var6, 1.0D, var8);
    }
}
