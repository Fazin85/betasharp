using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace BetaSharp.Util.Maths.Noise;

internal class SimplexNoiseSampler : NoiseSampler
{
    private static readonly double[] s_gradX256 = new double[256];
    private static readonly double[] s_gradZ256 = new double[256];

    static SimplexNoiseSampler()
    {
        int[] rawGradX = [1, -1, 1, -1, 1, -1, 1, -1, 0, 0, 0, 0];
        int[] rawGradZ = [1, 1, -1, -1, 0, 0, 0, 0, 1, -1, 1, -1];

        for (int i = 0; i < 256; i++)
        {
            s_gradX256[i] = rawGradX[i % 12];
            s_gradZ256[i] = rawGradZ[i % 12];
        }
    }

    private readonly int[] _permutations;
    private readonly double _x;
    private readonly double _y;

    private static readonly double s_f2 = 0.5D * (Math.Sqrt(3.0D) - 1.0D);
    private static readonly double s_g2 = (3.0D - Math.Sqrt(3.0D)) / 6.0D;

    public SimplexNoiseSampler() : this(new JavaRandom())
    {
    }

    public SimplexNoiseSampler(JavaRandom rand)
    {
        _permutations = new int[512];
        _x = rand.NextDouble() * 256.0D;
        _y = rand.NextDouble() * 256.0D;
        _ = rand.NextDouble() * 256.0D;

        for (int i = 0; i < 256; i++)
        {
            _permutations[i] = i;
        }

        for (int i = 0; i < 256; ++i)
        {
            int j = rand.NextInt(256 - i) + i;
            (_permutations[i], _permutations[j]) = (_permutations[j], _permutations[i]);
            _permutations[i + 256] = _permutations[i];
        }
    }


    public void Sample(double[] buffer, double x, double z, int width, int depth, double xFrequency, double zFrequency, double amplitude)
    {
        int counter = 0;

        // Grab raw memory references to bypass all array bounds checking in the loop
        ref double bufRef = ref MemoryMarshal.GetArrayDataReference(buffer);
        ref int permRef = ref MemoryMarshal.GetArrayDataReference(_permutations);
        ref double gradXRef = ref MemoryMarshal.GetArrayDataReference(s_gradX256);
        ref double gradZRef = ref MemoryMarshal.GetArrayDataReference(s_gradZ256);

        for (int x1 = 0; x1 < width; ++x1)
        {
            double x2 = (x + x1) * xFrequency + _x;

            for (int z1 = 0; z1 < depth; ++z1)
            {
                double z2 = (z + z1) * zFrequency + _y;
                double s = (x2 + z2) * s_f2;
                int i = Floor(x2 + s);
                int j = Floor(z2 + s);
                double t = (i + j) * s_g2;
                double x3 = i - t;
                double z3 = j - t;
                double x4 = x2 - x3;
                double z4 = z2 - z3;

                // Branchless logic
                byte i1 = (byte)(x4 > z4 ? 1 : 0);
                byte j1 = (byte)(i1 ^ 1);

                double x5 = x4 - i1 + s_g2;
                double z5 = z4 - j1 + s_g2;
                double x6 = x4 - 1.0D + 2.0D * s_g2;
                double z6 = z4 - 1.0D + 2.0D * s_g2;

                int ii = i & 255;
                int jj = j & 255;

                // Lookups bypassing bounds checks. NO modulo needed!
                int permJ = Unsafe.Add(ref permRef, jj);
                int permJ1 = Unsafe.Add(ref permRef, jj + j1);
                int permJ2 = Unsafe.Add(ref permRef, jj + 1);

                int gi0 = Unsafe.Add(ref permRef, ii + permJ);
                int gi1 = Unsafe.Add(ref permRef, ii + i1 + permJ1);
                int gi2 = Unsafe.Add(ref permRef, ii + 1 + permJ2);

                double t0 = 0.5D - x4 * x4 - z4 * z4;
                double n0;
                if (t0 < 0.0D)
                {
                    n0 = 0.0D;
                }
                else
                {
                    t0 *= t0;
                    n0 = t0 * t0 * (Unsafe.Add(ref gradXRef, gi0) * x4 + Unsafe.Add(ref gradZRef, gi0) * z4);
                }

                double t1 = 0.5D - x5 * x5 - z5 * z5;
                double n1;
                if (t1 < 0.0D)
                {
                    n1 = 0.0D;
                }
                else
                {
                    t1 *= t1;
                    n1 = t1 * t1 * (Unsafe.Add(ref gradXRef, gi1) * x5 + Unsafe.Add(ref gradZRef, gi1) * z5);
                }

                double t2 = 0.5D - x6 * x6 - z6 * z6;
                double n2;
                if (t2 < 0.0D)
                {
                    n2 = 0.0D;
                }
                else
                {
                    t2 *= t2;
                    n2 = t2 * t2 * (Unsafe.Add(ref gradXRef, gi2) * x6 + Unsafe.Add(ref gradZRef, gi2) * z6);
                }

                Unsafe.Add(ref bufRef, counter++) += 70.0D * (n0 + n1 + n2) * amplitude;
            }
        }
    }
}
