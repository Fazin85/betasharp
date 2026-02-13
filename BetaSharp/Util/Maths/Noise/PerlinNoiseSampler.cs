namespace BetaSharp.Util.Maths.Noise;

public class PerlinNoiseSampler : NoiseSampler
{
    private readonly int[] permutations;
    public double xCoord;
    public double yCoord;
    public double zCoord;

    public PerlinNoiseSampler() : this(new())
    {
    }

    public PerlinNoiseSampler(java.util.Random var1)
    {
        permutations = new int[512];
        xCoord = var1.nextDouble() * 256.0D;
        yCoord = var1.nextDouble() * 256.0D;
        zCoord = var1.nextDouble() * 256.0D;

        int var2;
        for (var2 = 0; var2 < 256; permutations[var2] = var2++)
        {
        }

        for (var2 = 0; var2 < 256; ++var2)
        {
            int var3 = var1.nextInt(256 - var2) + var2;
            int var4 = permutations[var2];
            permutations[var2] = permutations[var3];
            permutations[var3] = var4;
            permutations[var2 + 256] = permutations[var2];
        }

    }

    public double generateNoise(double var1, double var3, double var5)
    {
        double var7 = var1 + xCoord;
        double var9 = var3 + yCoord;
        double var11 = var5 + zCoord;
        int var13 = (int)var7;
        int var14 = (int)var9;
        int var15 = (int)var11;
        if (var7 < var13)
        {
            --var13;
        }

        if (var9 < var14)
        {
            --var14;
        }

        if (var11 < var15)
        {
            --var15;
        }

        int var16 = var13 & 255;
        int var17 = var14 & 255;
        int var18 = var15 & 255;
        var7 -= var13;
        var9 -= var14;
        var11 -= var15;
        double var19 = var7 * var7 * var7 * (var7 * (var7 * 6.0D - 15.0D) + 10.0D);
        double var21 = var9 * var9 * var9 * (var9 * (var9 * 6.0D - 15.0D) + 10.0D);
        double var23 = var11 * var11 * var11 * (var11 * (var11 * 6.0D - 15.0D) + 10.0D);
        int var25 = permutations[var16] + var17;
        int var26 = permutations[var25] + var18;
        int var27 = permutations[var25 + 1] + var18;
        int var28 = permutations[var16 + 1] + var17;
        int var29 = permutations[var28] + var18;
        int var30 = permutations[var28 + 1] + var18;
        return lerp(var23, lerp(var21, lerp(var19, grad(permutations[var26], var7, var9, var11), grad(permutations[var29], var7 - 1.0D, var9, var11)), lerp(var19, grad(permutations[var27], var7, var9 - 1.0D, var11), grad(permutations[var30], var7 - 1.0D, var9 - 1.0D, var11))), lerp(var21, lerp(var19, grad(permutations[var26 + 1], var7, var9, var11 - 1.0D), grad(permutations[var29 + 1], var7 - 1.0D, var9, var11 - 1.0D)), lerp(var19, grad(permutations[var27 + 1], var7, var9 - 1.0D, var11 - 1.0D), grad(permutations[var30 + 1], var7 - 1.0D, var9 - 1.0D, var11 - 1.0D))));
    }

    public double lerp(double var1, double var3, double var5)
    {
        return var3 + var1 * (var5 - var3);
    }

    public double func_4110_a(int var1, double var2, double var4)
    {
        int var6 = var1 & 15;
        double var7 = (1 - ((var6 & 8) >> 3)) * var2;
        double var9 = var6 < 4 ? 0.0D : var6 != 12 && var6 != 14 ? var4 : var2;
        return ((var6 & 1) == 0 ? var7 : -var7) + ((var6 & 2) == 0 ? var9 : -var9);
    }

    public double grad(int var1, double var2, double var4, double var6)
    {
        int var8 = var1 & 15;
        double var9 = var8 < 8 ? var2 : var4;
        double var11 = var8 < 4 ? var4 : var8 != 12 && var8 != 14 ? var6 : var2;
        return ((var8 & 1) == 0 ? var9 : -var9) + ((var8 & 2) == 0 ? var11 : -var11);
    }

    public double func_801_a(double var1, double var3)
    {
        return generateNoise(var1, var3, 0.0D);
    }

    public void func_805_a(double[] var1, double var2, double var4, double var6, int var8, int var9, int var10, double var11, double var13, double var15, double var17)
    {
        int var10001;
        int var19;
        int var22;
        double var31;
        double var35;
        int var37;
        double var38;
        int var40;
        int var41;
        double var42;
        int var75;
        if (var9 == 1)
        {
            bool var64 = false;
            bool var65 = false;
            bool var21 = false;
            bool var68 = false;
            double var70 = 0.0D;
            double var73 = 0.0D;
            var75 = 0;
            double var77 = 1.0D / var17;

            for (int var30 = 0; var30 < var8; ++var30)
            {
                var31 = (var2 + var30) * var11 + xCoord;
                int var78 = (int)var31;
                if (var31 < var78)
                {
                    --var78;
                }

                int var34 = var78 & 255;
                var31 -= var78;
                var35 = var31 * var31 * var31 * (var31 * (var31 * 6.0D - 15.0D) + 10.0D);

                for (var37 = 0; var37 < var10; ++var37)
                {
                    var38 = (var6 + var37) * var15 + zCoord;
                    var40 = (int)var38;
                    if (var38 < var40)
                    {
                        --var40;
                    }

                    var41 = var40 & 255;
                    var38 -= var40;
                    var42 = var38 * var38 * var38 * (var38 * (var38 * 6.0D - 15.0D) + 10.0D);
                    var19 = permutations[var34] + 0;
                    int var66 = permutations[var19] + var41;
                    int var67 = permutations[var34 + 1] + 0;
                    var22 = permutations[var67] + var41;
                    var70 = lerp(var35, func_4110_a(permutations[var66], var31, var38), grad(permutations[var22], var31 - 1.0D, 0.0D, var38));
                    var73 = lerp(var35, grad(permutations[var66 + 1], var31, 0.0D, var38 - 1.0D), grad(permutations[var22 + 1], var31 - 1.0D, 0.0D, var38 - 1.0D));
                    double var79 = lerp(var42, var70, var73);
                    var10001 = var75++;
                    var1[var10001] += var79 * var77;
                }
            }

        }
        else
        {
            var19 = 0;
            double var20 = 1.0D / var17;
            var22 = -1;
            bool var23 = false;
            bool var24 = false;
            bool var25 = false;
            bool var26 = false;
            bool var27 = false;
            bool var28 = false;
            double var29 = 0.0D;
            var31 = 0.0D;
            double var33 = 0.0D;
            var35 = 0.0D;

            for (var37 = 0; var37 < var8; ++var37)
            {
                var38 = (var2 + var37) * var11 + xCoord;
                var40 = (int)var38;
                if (var38 < var40)
                {
                    --var40;
                }

                var41 = var40 & 255;
                var38 -= var40;
                var42 = var38 * var38 * var38 * (var38 * (var38 * 6.0D - 15.0D) + 10.0D);

                for (int var44 = 0; var44 < var10; ++var44)
                {
                    double var45 = (var6 + var44) * var15 + zCoord;
                    int var47 = (int)var45;
                    if (var45 < var47)
                    {
                        --var47;
                    }

                    int var48 = var47 & 255;
                    var45 -= var47;
                    double var49 = var45 * var45 * var45 * (var45 * (var45 * 6.0D - 15.0D) + 10.0D);

                    for (int var51 = 0; var51 < var9; ++var51)
                    {
                        double var52 = (var4 + var51) * var13 + yCoord;
                        int var54 = (int)var52;
                        if (var52 < var54)
                        {
                            --var54;
                        }

                        int var55 = var54 & 255;
                        var52 -= var54;
                        double var56 = var52 * var52 * var52 * (var52 * (var52 * 6.0D - 15.0D) + 10.0D);
                        if (var51 == 0 || var55 != var22)
                        {
                            var22 = var55;
                            int var69 = permutations[var41] + var55;
                            int var71 = permutations[var69] + var48;
                            int var72 = permutations[var69 + 1] + var48;
                            int var74 = permutations[var41 + 1] + var55;
                            var75 = permutations[var74] + var48;
                            int var76 = permutations[var74 + 1] + var48;
                            var29 = lerp(var42, grad(permutations[var71], var38, var52, var45), grad(permutations[var75], var38 - 1.0D, var52, var45));
                            var31 = lerp(var42, grad(permutations[var72], var38, var52 - 1.0D, var45), grad(permutations[var76], var38 - 1.0D, var52 - 1.0D, var45));
                            var33 = lerp(var42, grad(permutations[var71 + 1], var38, var52, var45 - 1.0D), grad(permutations[var75 + 1], var38 - 1.0D, var52, var45 - 1.0D));
                            var35 = lerp(var42, grad(permutations[var72 + 1], var38, var52 - 1.0D, var45 - 1.0D), grad(permutations[var76 + 1], var38 - 1.0D, var52 - 1.0D, var45 - 1.0D));
                        }

                        double var58 = lerp(var56, var29, var31);
                        double var60 = lerp(var56, var33, var35);
                        double var62 = lerp(var49, var58, var60);
                        var10001 = var19++;
                        var1[var10001] += var62 * var20;
                    }
                }
            }

        }
    }
}