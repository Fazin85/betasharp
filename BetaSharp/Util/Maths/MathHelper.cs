using System.Runtime.CompilerServices;

namespace BetaSharp.Util.Maths;

public static class MathHelper
{
    private static readonly float[] SinTable = new float[65536];
    private const float FastMathFactor = 65536.0f / (float)(Math.PI * 2.0);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Sin(float value)
    {
        return SinTable[(int)(value * FastMathFactor) & 0xFFFF];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Cos(float value)
    {
        return SinTable[(int)(value * FastMathFactor + 16384.0F) &  0xFFFF];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Sqrt(float value)
    {
        return (float)Math.Sqrt((double)value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Sqrt(double value)
    {
        return (float)Math.Sqrt(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Floor(float value)
    {
        int var1 = (int)value;
        return value < var1 ? var1 - 1 : var1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Floor(double value)
    {
        int i = (int)value;
        return value < i ? i - 1 : i;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Abs(float value)
    {
        return value >= 0.0F ? value : -value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int FloorDiv(int a, int b)
    {
        return a < 0 ? -((-a - 1) / b) - 1 : a / b;
    }

    static MathHelper()
    {
        for (int i = 0; i < 65536; ++i)
        {
            SinTable[i] = (float)Math.Sin(i * Math.PI * 2.0D / 65536.0D);
        }
    }
}
