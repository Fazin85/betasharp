using BetaSharp.Blocks;
using BetaSharp.Util.Maths;

namespace BetaSharp.Worlds.Gen.Carvers;

public class NetherCaveCarver : Carver
{

    protected void func_4129_a(int chunkX, int chunkZ, byte[] blocks, double x, double y, double z)
    {
        func_4128_a(chunkX, chunkZ, blocks, x, y, z, 1.0F + rand.NextFloat() * 6.0F, 0.0F, 0.0F, -1, -1, 0.5D);
    }

    protected void func_4128_a(int chunkX, int chunkZ, byte[] blocks, double x, double y, double z, float var10, float var11, float var12, int var13, int var14, double var15)
    {
        double var17 = chunkX * 16 + 8;
        double var19 = chunkZ * 16 + 8;
        float var21 = 0.0F;
        float var22 = 0.0F;
        JavaRandom var23 = new(rand.NextLong());
        if (var14 <= 0)
        {
            int var24 = radius * 16 - 16;
            var14 = var24 - var23.NextInt(var24 / 4);
        }

        bool var51 = false;
        if (var13 == -1)
        {
            var13 = var14 / 2;
            var51 = true;
        }

        int var25 = var23.NextInt(var14 / 2) + var14 / 4;

        for (bool var26 = var23.NextInt(6) == 0; var13 < var14; ++var13)
        {
            double var27 = 1.5D + (double)(MathHelper.Sin(var13 * (float)Math.PI / var14) * var10 * 1.0F);
            double var29 = var27 * var15;
            float var31 = MathHelper.Cos(var12);
            float var32 = MathHelper.Sin(var12);
            x += (double)(MathHelper.Cos(var11) * var31);
            y += (double)var32;
            z += (double)(MathHelper.Sin(var11) * var31);
            if (var26)
            {
                var12 *= 0.92F;
            }
            else
            {
                var12 *= 0.7F;
            }

            var12 += var22 * 0.1F;
            var11 += var21 * 0.1F;
            var22 *= 0.9F;
            var21 *= 12.0F / 16.0F;
            var22 += (var23.NextFloat() - var23.NextFloat()) * var23.NextFloat() * 2.0F;
            var21 += (var23.NextFloat() - var23.NextFloat()) * var23.NextFloat() * 4.0F;
            if (!var51 && var13 == var25 && var10 > 1.0F)
            {
                func_4128_a(chunkX, chunkZ, blocks, x, y, z, var23.NextFloat() * 0.5F + 0.5F, var11 - (float)Math.PI * 0.5F, var12 / 3.0F, var13, var14, 1.0D);
                func_4128_a(chunkX, chunkZ, blocks, x, y, z, var23.NextFloat() * 0.5F + 0.5F, var11 + (float)Math.PI * 0.5F, var12 / 3.0F, var13, var14, 1.0D);
                return;
            }

            if (var51 || var23.NextInt(4) != 0)
            {
                double var33 = x - var17;
                double var35 = z - var19;
                double var37 = var14 - var13;
                double var39 = (double)(var10 + 2.0F + 16.0F);
                if (var33 * var33 + var35 * var35 - var37 * var37 > var39 * var39)
                {
                    return;
                }

                if (x >= var17 - 16.0D - var27 * 2.0D && z >= var19 - 16.0D - var27 * 2.0D && x <= var17 + 16.0D + var27 * 2.0D && z <= var19 + 16.0D + var27 * 2.0D)
                {
                    int var52 = MathHelper.Floor(x - var27) - chunkX * 16 - 1;
                    int var34 = MathHelper.Floor(x + var27) - chunkX * 16 + 1;
                    int var53 = MathHelper.Floor(y - var29) - 1;
                    int var36 = MathHelper.Floor(y + var29) + 1;
                    int var54 = MathHelper.Floor(z - var27) - chunkZ * 16 - 1;
                    int var38 = MathHelper.Floor(z + var27) - chunkZ * 16 + 1;
                    if (var52 < 0)
                    {
                        var52 = 0;
                    }

                    if (var34 > 16)
                    {
                        var34 = 16;
                    }

                    if (var53 < 1)
                    {
                        var53 = 1;
                    }

                    if (var36 > 120)
                    {
                        var36 = 120;
                    }

                    if (var54 < 0)
                    {
                        var54 = 0;
                    }

                    if (var38 > 16)
                    {
                        var38 = 16;
                    }

                    bool var55 = false;

                    int var40;
                    int var43;
                    for (var40 = var52; !var55 && var40 < var34; ++var40)
                    {
                        for (int var41 = var54; !var55 && var41 < var38; ++var41)
                        {
                            for (int var42 = var36 + 1; !var55 && var42 >= var53 - 1; --var42)
                            {
                                var43 = (var40 * 16 + var41) * 128 + var42;
                                if (var42 >= 0 && var42 < 128)
                                {
                                    if (blocks[var43] == Block.FlowingLava.id || blocks[var43] == Block.Lava.id)
                                    {
                                        var55 = true;
                                    }

                                    if (var42 != var53 - 1 && var40 != var52 && var40 != var34 - 1 && var41 != var54 && var41 != var38 - 1)
                                    {
                                        var42 = var53;
                                    }
                                }
                            }
                        }
                    }

                    if (!var55)
                    {
                        for (var40 = var52; var40 < var34; ++var40)
                        {
                            double var56 = (var40 + chunkX * 16 + 0.5D - x) / var27;

                            for (var43 = var54; var43 < var38; ++var43)
                            {
                                double var44 = (var43 + chunkZ * 16 + 0.5D - z) / var27;
                                int var46 = (var40 * 16 + var43) * 128 + var36;

                                for (int var47 = var36 - 1; var47 >= var53; --var47)
                                {
                                    double var48 = (var47 + 0.5D - y) / var29;
                                    if (var48 > -0.7D && var56 * var56 + var48 * var48 + var44 * var44 < 1.0D)
                                    {
                                        byte var50 = blocks[var46];
                                        if (var50 == Block.Netherrack.id || var50 == Block.Dirt.id || var50 == Block.GrassBlock.id)
                                        {
                                            blocks[var46] = 0;
                                        }
                                    }

                                    --var46;
                                }
                            }
                        }

                        if (var51)
                        {
                            break;
                        }
                    }
                }
            }
        }

    }

    protected override void func_868_a(World world, int chunkX, int chunkZ, int centerChunkX, int centerChunkZ, byte[] blocks)
    {
        int var7 = rand.NextInt(rand.NextInt(rand.NextInt(10) + 1) + 1);
        if (rand.NextInt(5) != 0)
        {
            var7 = 0;
        }

        for (int var8 = 0; var8 < var7; ++var8)
        {
            double randX = chunkX * 16 + rand.NextInt(16);
            double randY = rand.NextInt(128);
            double randZ = chunkZ * 16 + rand.NextInt(16);
            int var15 = 1;
            if (rand.NextInt(4) == 0)
            {
                func_4129_a(centerChunkX, centerChunkZ, blocks, randX, randY, randZ);
                var15 += rand.NextInt(4);
            }

            for (int var16 = 0; var16 < var15; ++var16)
            {
                float var17 = rand.NextFloat() * (float)Math.PI * 2.0F;
                float var18 = (rand.NextFloat() - 0.5F) * 2.0F / 8.0F;
                float var19 = rand.NextFloat() * 2.0F + rand.NextFloat();
                func_4128_a(centerChunkX, centerChunkZ, blocks, randX, randY, randZ, var19 * 2.0F, var17, var18, 0, 0, 0.5D);
            }
        }

    }
}
