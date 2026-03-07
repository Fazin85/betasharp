using System.Runtime.CompilerServices;

namespace BetaSharp.Util.Maths.Noise;

public abstract class NoiseSampler
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected static int Floor(double num)
    {
        return num > 0.0D ? (int)num : (int)num - 1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected static double Lerp(double t, double a, double b)
    {
        return a + t * (b - a);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected static double Grad(int hash, double x, double y)
    {
        int h = hash & 15;
        double u = (1 - ((h & 8) >> 3)) * x;
        double v = h < 4 ? 0.0D : h != 12 && h != 14 ? y : x;
        return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected static double Grad(int hash, double x, double y, double z)
    {
        int h = hash & 15;
        double u = h < 8 ? x : y;
        double v = h < 4 ? y : h != 12 && h != 14 ? z : x;
        return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v);
    }
}
