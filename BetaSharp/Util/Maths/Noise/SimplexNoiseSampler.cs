namespace BetaSharp.Util.Maths.Noise;

public class SimplexNoiseSampler : java.lang.Object
{
    private static readonly int[][] grads = [[1, 1, 0], [-1, 1, 0], [1, -1, 0], [-1, -1, 0], [1, 0, 1], [-1, 0, 1], [1, 0, -1], [-1, 0, -1], [0, 1, 1], [0, -1, 1], [0, 1, -1], [0, -1, -1]];
    private readonly int[] perm;
    public double offsetX;
    public double offsetY;
    public double offsetZ;
    private static readonly double F2 = 0.5D * (java.lang.Math.sqrt(3.0D) - 1.0D);
    private static readonly double G2 = (3.0D - java.lang.Math.sqrt(3.0D)) / 6.0D;

    public SimplexNoiseSampler() : this(new())
    {
    }

    public SimplexNoiseSampler(java.util.Random rand)
    {
        perm = new int[512];
        offsetX = rand.nextDouble() * 256.0D;
        offsetY = rand.nextDouble() * 256.0D;
        offsetZ = rand.nextDouble() * 256.0D;

        // Fill perm with values from 0 to 255 in random order, duplicating the first 256 values to the end of the array
        for (int i = 0; i < 256; i++)
        {
            perm[i] = i;
        }

        for (int i = 0; i < 256; ++i)
        {
            int j = rand.nextInt(256 - i) + i;
            (perm[i], perm[j]) = (perm[j], perm[i]);
            perm[i + 256] = perm[i];
        }

    }

    private static int floor(double num)
    {
        return num > 0.0D ? (int)num : (int)num - 1;
    }

    private static double dot(int[] gradient, double dx, double dy)
    {
        return gradient[0] * dx + gradient[1] * dy;
    }

    public void sample(double[] map, double x, double z, int width, int depth, double xFrequency, double zFrequency, double amplitude)
    {
        int counter = 0;

        for (int x1 = 0; x1 < width; ++x1)
        {
            double x2 = (x + x1) * xFrequency + offsetX;

            for (int z1 = 0; z1 < depth; ++z1)
            {
                double z2 = (z + z1) * zFrequency + offsetY;
                double s = (x2 + z2) * F2;
                int i = floor(x2 + s);
                int j = floor(z2 + s);
                double t = (i + j) * G2;
                double x3 = i - t;
                double z3 = j - t;
                double x4 = x2 - x3;
                double z4 = z2 - z3;
                byte i1;
                byte j1;
                if (x4 > z4)
                {
                    i1 = 1;
                    j1 = 0;
                }
                else
                {
                    i1 = 0;
                    j1 = 1;
                }

                double x5 = x4 - i1 + G2;
                double z5 = z4 - j1 + G2;
                double x6 = x4 - 1.0D + 2.0D * G2;
                double z6 = z4 - 1.0D + 2.0D * G2;
                int ii = i & 255;
                int jj = j & 255;
                int gi0 = perm[ii + perm[jj]] % 12;
                int gi1 = perm[ii + i1 + perm[jj + j1]] % 12;
                int gi2 = perm[ii + 1 + perm[jj + 1]] % 12;
                double t0 = 0.5D - x4 * x4 - z4 * z4;
                double n0;
                if (t0 < 0.0D)
                {
                    n0 = 0.0D;
                }
                else
                {
                    t0 *= t0;
                    n0 = t0 * t0 * dot(grads[gi0], x4, z4);
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
                    n1 = t1 * t1 * dot(grads[gi1], x5, z5);
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
                    n2 = t2 * t2 * dot(grads[gi2], x6, z6);
                }

                map[++counter] += 70.0D * (n0 + n1 + n2) * amplitude;
            }
        }
    }
}
