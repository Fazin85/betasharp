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

    public SimplexNoiseSampler(java.util.Random var1)
    {
        perm = new int[512];
        offsetX = var1.nextDouble() * 256.0D;
        offsetY = var1.nextDouble() * 256.0D;
        offsetZ = var1.nextDouble() * 256.0D;

        int var2;
        for (var2 = 0; var2 < 256; perm[var2] = var2++)
        {
        }

        for (var2 = 0; var2 < 256; ++var2)
        {
            int var3 = var1.nextInt(256 - var2) + var2;
            int var4 = perm[var2];
            perm[var2] = perm[var3];
            perm[var3] = var4;
            perm[var2 + 256] = perm[var2];
        }

    }

    private static int wrap(double var0)
    {
        return var0 > 0.0D ? (int)var0 : (int)var0 - 1;
    }

    private static double func_4156_a(int[] var0, double var1, double var3)
    {
        return var0[0] * var1 + var0[1] * var3;
    }

    public void func_4157_a(double[] var1, double var2, double var4, int var6, int var7, double var8, double var10, double var12)
    {
        int var14 = 0;

        for (int var15 = 0; var15 < var6; ++var15)
        {
            double var16 = (var2 + var15) * var8 + offsetX;

            for (int var18 = 0; var18 < var7; ++var18)
            {
                double var19 = (var4 + var18) * var10 + offsetY;
                double var27 = (var16 + var19) * F2;
                int var29 = wrap(var16 + var27);
                int var30 = wrap(var19 + var27);
                double var31 = (var29 + var30) * G2;
                double var33 = var29 - var31;
                double var35 = var30 - var31;
                double var37 = var16 - var33;
                double var39 = var19 - var35;
                byte var41;
                byte var42;
                if (var37 > var39)
                {
                    var41 = 1;
                    var42 = 0;
                }
                else
                {
                    var41 = 0;
                    var42 = 1;
                }

                double var43 = var37 - var41 + G2;
                double var45 = var39 - var42 + G2;
                double var47 = var37 - 1.0D + 2.0D * G2;
                double var49 = var39 - 1.0D + 2.0D * G2;
                int var51 = var29 & 255;
                int var52 = var30 & 255;
                int var53 = perm[var51 + perm[var52]] % 12;
                int var54 = perm[var51 + var41 + perm[var52 + var42]] % 12;
                int var55 = perm[var51 + 1 + perm[var52 + 1]] % 12;
                double var56 = 0.5D - var37 * var37 - var39 * var39;
                double var21;
                if (var56 < 0.0D)
                {
                    var21 = 0.0D;
                }
                else
                {
                    var56 *= var56;
                    var21 = var56 * var56 * func_4156_a(grads[var53], var37, var39);
                }

                double var58 = 0.5D - var43 * var43 - var45 * var45;
                double var23;
                if (var58 < 0.0D)
                {
                    var23 = 0.0D;
                }
                else
                {
                    var58 *= var58;
                    var23 = var58 * var58 * func_4156_a(grads[var54], var43, var45);
                }

                double var60 = 0.5D - var47 * var47 - var49 * var49;
                double var25;
                if (var60 < 0.0D)
                {
                    var25 = 0.0D;
                }
                else
                {
                    var60 *= var60;
                    var25 = var60 * var60 * func_4156_a(grads[var55], var47, var49);
                }

                int var10001 = var14++;
                var1[var10001] += 70.0D * (var21 + var23 + var25) * var12;
            }
        }

    }
}