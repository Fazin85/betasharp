using betareborn.Blocks;
using betareborn.Worlds;

namespace betareborn.Chunks
{
    public class ChunkProviderHell : IChunkProvider
    {

        private java.util.Random hellRNG;
        private NoiseGeneratorOctaves field_4169_i;
        private NoiseGeneratorOctaves field_4168_j;
        private NoiseGeneratorOctaves field_4167_k;
        private NoiseGeneratorOctaves field_4166_l;
        private NoiseGeneratorOctaves field_4165_m;
        public NoiseGeneratorOctaves field_4177_a;
        public NoiseGeneratorOctaves field_4176_b;
        private World worldObj;
        private double[] field_4163_o;
        private double[] field_4162_p = new double[256];
        private double[] field_4161_q = new double[256];
        private double[] field_4160_r = new double[256];
        private MapGenBase field_4159_s = new MapGenCavesHell();
        double[] field_4175_c;
        double[] field_4174_d;
        double[] field_4173_e;
        double[] field_4172_f;
        double[] field_4171_g;

        public ChunkProviderHell(World var1, long var2)
        {
            worldObj = var1;
            hellRNG = new(var2);
            field_4169_i = new NoiseGeneratorOctaves(hellRNG, 16);
            field_4168_j = new NoiseGeneratorOctaves(hellRNG, 16);
            field_4167_k = new NoiseGeneratorOctaves(hellRNG, 8);
            field_4166_l = new NoiseGeneratorOctaves(hellRNG, 4);
            field_4165_m = new NoiseGeneratorOctaves(hellRNG, 4);
            field_4177_a = new NoiseGeneratorOctaves(hellRNG, 10);
            field_4176_b = new NoiseGeneratorOctaves(hellRNG, 16);
        }

        public void func_4059_a(int var1, int var2, byte[] var3)
        {
            byte var4 = 4;
            byte var5 = 32;
            int var6 = var4 + 1;
            byte var7 = 17;
            int var8 = var4 + 1;
            field_4163_o = func_4057_a(field_4163_o, var1 * var4, 0, var2 * var4, var6, var7, var8);

            for (int var9 = 0; var9 < var4; ++var9)
            {
                for (int var10 = 0; var10 < var4; ++var10)
                {
                    for (int var11 = 0; var11 < 16; ++var11)
                    {
                        double var12 = 0.125D;
                        double var14 = field_4163_o[((var9 + 0) * var8 + var10 + 0) * var7 + var11 + 0];
                        double var16 = field_4163_o[((var9 + 0) * var8 + var10 + 1) * var7 + var11 + 0];
                        double var18 = field_4163_o[((var9 + 1) * var8 + var10 + 0) * var7 + var11 + 0];
                        double var20 = field_4163_o[((var9 + 1) * var8 + var10 + 1) * var7 + var11 + 0];
                        double var22 = (field_4163_o[((var9 + 0) * var8 + var10 + 0) * var7 + var11 + 1] - var14) * var12;
                        double var24 = (field_4163_o[((var9 + 0) * var8 + var10 + 1) * var7 + var11 + 1] - var16) * var12;
                        double var26 = (field_4163_o[((var9 + 1) * var8 + var10 + 0) * var7 + var11 + 1] - var18) * var12;
                        double var28 = (field_4163_o[((var9 + 1) * var8 + var10 + 1) * var7 + var11 + 1] - var20) * var12;

                        for (int var30 = 0; var30 < 8; ++var30)
                        {
                            double var31 = 0.25D;
                            double var33 = var14;
                            double var35 = var16;
                            double var37 = (var18 - var14) * var31;
                            double var39 = (var20 - var16) * var31;

                            for (int var41 = 0; var41 < 4; ++var41)
                            {
                                int var42 = var41 + var9 * 4 << 11 | 0 + var10 * 4 << 7 | var11 * 8 + var30;
                                short var43 = 128;
                                double var44 = 0.25D;
                                double var46 = var33;
                                double var48 = (var35 - var33) * var44;

                                for (int var50 = 0; var50 < 4; ++var50)
                                {
                                    int var51 = 0;
                                    if (var11 * 8 + var30 < var5)
                                    {
                                        var51 = Block.LAVA.id;
                                    }

                                    if (var46 > 0.0D)
                                    {
                                        var51 = Block.NETHERRACK.id;
                                    }

                                    var3[var42] = (byte)var51;
                                    var42 += var43;
                                    var46 += var48;
                                }

                                var33 += var37;
                                var35 += var39;
                            }

                            var14 += var22;
                            var16 += var24;
                            var18 += var26;
                            var20 += var28;
                        }
                    }
                }
            }

        }

        public void func_4058_b(int var1, int var2, byte[] var3)
        {
            byte var4 = 64;
            double var5 = 1.0D / 32.0D;
            field_4162_p = field_4166_l.generateNoiseOctaves(field_4162_p, (double)(var1 * 16), (double)(var2 * 16), 0.0D, 16, 16, 1, var5, var5, 1.0D);
            field_4161_q = field_4166_l.generateNoiseOctaves(field_4161_q, (double)(var1 * 16), 109.0134D, (double)(var2 * 16), 16, 1, 16, var5, 1.0D, var5);
            field_4160_r = field_4165_m.generateNoiseOctaves(field_4160_r, (double)(var1 * 16), (double)(var2 * 16), 0.0D, 16, 16, 1, var5 * 2.0D, var5 * 2.0D, var5 * 2.0D);

            for (int var7 = 0; var7 < 16; ++var7)
            {
                for (int var8 = 0; var8 < 16; ++var8)
                {
                    bool var9 = field_4162_p[var7 + var8 * 16] + hellRNG.nextDouble() * 0.2D > 0.0D;
                    bool var10 = field_4161_q[var7 + var8 * 16] + hellRNG.nextDouble() * 0.2D > 0.0D;
                    int var11 = (int)(field_4160_r[var7 + var8 * 16] / 3.0D + 3.0D + hellRNG.nextDouble() * 0.25D);
                    int var12 = -1;
                    byte var13 = (byte)Block.NETHERRACK.id;
                    byte var14 = (byte)Block.NETHERRACK.id;

                    for (int var15 = 127; var15 >= 0; --var15)
                    {
                        int var16 = (var8 * 16 + var7) * 128 + var15;
                        if (var15 >= 127 - hellRNG.nextInt(5))
                        {
                            var3[var16] = (byte)Block.BEDROCK.id;
                        }
                        else if (var15 <= 0 + hellRNG.nextInt(5))
                        {
                            var3[var16] = (byte)Block.BEDROCK.id;
                        }
                        else
                        {
                            byte var17 = var3[var16];
                            if (var17 == 0)
                            {
                                var12 = -1;
                            }
                            else if (var17 == Block.NETHERRACK.id)
                            {
                                if (var12 == -1)
                                {
                                    if (var11 <= 0)
                                    {
                                        var13 = 0;
                                        var14 = (byte)Block.NETHERRACK.id;
                                    }
                                    else if (var15 >= var4 - 4 && var15 <= var4 + 1)
                                    {
                                        var13 = (byte)Block.NETHERRACK.id;
                                        var14 = (byte)Block.NETHERRACK.id;
                                        if (var10)
                                        {
                                            var13 = (byte)Block.GRAVEL.id;
                                        }

                                        if (var10)
                                        {
                                            var14 = (byte)Block.NETHERRACK.id;
                                        }

                                        if (var9)
                                        {
                                            var13 = (byte)Block.SOUL_SAND.id;
                                        }

                                        if (var9)
                                        {
                                            var14 = (byte)Block.SOUL_SAND.id;
                                        }
                                    }

                                    if (var15 < var4 && var13 == 0)
                                    {
                                        var13 = (byte)Block.LAVA.id;
                                    }

                                    var12 = var11;
                                    if (var15 >= var4 - 1)
                                    {
                                        var3[var16] = var13;
                                    }
                                    else
                                    {
                                        var3[var16] = var14;
                                    }
                                }
                                else if (var12 > 0)
                                {
                                    --var12;
                                    var3[var16] = var14;
                                }
                            }
                        }
                    }
                }
            }

        }

        public Chunk prepareChunk(int var1, int var2)
        {
            return provideChunk(var1, var2);
        }

        public Chunk provideChunk(int var1, int var2)
        {
            hellRNG.setSeed((long)var1 * 341873128712L + (long)var2 * 132897987541L);
            byte[] var3 = new byte[-java.lang.Short.MIN_VALUE];
            func_4059_a(var1, var2, var3);
            func_4058_b(var1, var2, var3);
            field_4159_s.func_867_a(this, worldObj, var1, var2, var3);
            Chunk var4 = new Chunk(worldObj, var3, var1, var2);
            return var4;
        }

        private double[] func_4057_a(double[] var1, int var2, int var3, int var4, int var5, int var6, int var7)
        {
            if (var1 == null)
            {
                var1 = new double[var5 * var6 * var7];
            }

            double var8 = 684.412D;
            double var10 = 2053.236D;
            field_4172_f = field_4177_a.generateNoiseOctaves(field_4172_f, (double)var2, (double)var3, (double)var4, var5, 1, var7, 1.0D, 0.0D, 1.0D);
            field_4171_g = field_4176_b.generateNoiseOctaves(field_4171_g, (double)var2, (double)var3, (double)var4, var5, 1, var7, 100.0D, 0.0D, 100.0D);
            field_4175_c = field_4167_k.generateNoiseOctaves(field_4175_c, (double)var2, (double)var3, (double)var4, var5, var6, var7, var8 / 80.0D, var10 / 60.0D, var8 / 80.0D);
            field_4174_d = field_4169_i.generateNoiseOctaves(field_4174_d, (double)var2, (double)var3, (double)var4, var5, var6, var7, var8, var10, var8);
            field_4173_e = field_4168_j.generateNoiseOctaves(field_4173_e, (double)var2, (double)var3, (double)var4, var5, var6, var7, var8, var10, var8);
            int var12 = 0;
            int var13 = 0;
            double[] var14 = new double[var6];

            int var15;
            for (var15 = 0; var15 < var6; ++var15)
            {
                var14[var15] = java.lang.Math.cos((double)var15 * Math.PI * 6.0D / (double)var6) * 2.0D;
                double var16 = (double)var15;
                if (var15 > var6 / 2)
                {
                    var16 = (double)(var6 - 1 - var15);
                }

                if (var16 < 4.0D)
                {
                    var16 = 4.0D - var16;
                    var14[var15] -= var16 * var16 * var16 * 10.0D;
                }
            }

            for (var15 = 0; var15 < var5; ++var15)
            {
                for (int var36 = 0; var36 < var7; ++var36)
                {
                    double var17 = (field_4172_f[var13] + 256.0D) / 512.0D;
                    if (var17 > 1.0D)
                    {
                        var17 = 1.0D;
                    }

                    double var19 = 0.0D;
                    double var21 = field_4171_g[var13] / 8000.0D;
                    if (var21 < 0.0D)
                    {
                        var21 = -var21;
                    }

                    var21 = var21 * 3.0D - 3.0D;
                    if (var21 < 0.0D)
                    {
                        var21 /= 2.0D;
                        if (var21 < -1.0D)
                        {
                            var21 = -1.0D;
                        }

                        var21 /= 1.4D;
                        var21 /= 2.0D;
                        var17 = 0.0D;
                    }
                    else
                    {
                        if (var21 > 1.0D)
                        {
                            var21 = 1.0D;
                        }

                        var21 /= 6.0D;
                    }

                    var17 += 0.5D;
                    var21 = var21 * (double)var6 / 16.0D;
                    ++var13;

                    for (int var23 = 0; var23 < var6; ++var23)
                    {
                        double var24 = 0.0D;
                        double var26 = var14[var23];
                        double var28 = field_4174_d[var12] / 512.0D;
                        double var30 = field_4173_e[var12] / 512.0D;
                        double var32 = (field_4175_c[var12] / 10.0D + 1.0D) / 2.0D;
                        if (var32 < 0.0D)
                        {
                            var24 = var28;
                        }
                        else if (var32 > 1.0D)
                        {
                            var24 = var30;
                        }
                        else
                        {
                            var24 = var28 + (var30 - var28) * var32;
                        }

                        var24 -= var26;
                        double var34;
                        if (var23 > var6 - 4)
                        {
                            var34 = (double)((float)(var23 - (var6 - 4)) / 3.0F);
                            var24 = var24 * (1.0D - var34) + -10.0D * var34;
                        }

                        if ((double)var23 < var19)
                        {
                            var34 = (var19 - (double)var23) / 4.0D;
                            if (var34 < 0.0D)
                            {
                                var34 = 0.0D;
                            }

                            if (var34 > 1.0D)
                            {
                                var34 = 1.0D;
                            }

                            var24 = var24 * (1.0D - var34) + -10.0D * var34;
                        }

                        var1[var12] = var24;
                        ++var12;
                    }
                }
            }

            return var1;
        }

        public bool chunkExists(int var1, int var2)
        {
            return true;
        }

        public void populate(IChunkProvider var1, int var2, int var3)
        {
            BlockSand.fallInstantly = true;
            int var4 = var2 * 16;
            int var5 = var3 * 16;

            int var6;
            int var7;
            int var8;
            int var9;
            for (var6 = 0; var6 < 8; ++var6)
            {
                var7 = var4 + hellRNG.nextInt(16) + 8;
                var8 = hellRNG.nextInt(120) + 4;
                var9 = var5 + hellRNG.nextInt(16) + 8;
                (new WorldGenHellLava(Block.FLOWING_LAVA.id)).generate(worldObj, hellRNG, var7, var8, var9);
            }

            var6 = hellRNG.nextInt(hellRNG.nextInt(10) + 1) + 1;

            int var10;
            for (var7 = 0; var7 < var6; ++var7)
            {
                var8 = var4 + hellRNG.nextInt(16) + 8;
                var9 = hellRNG.nextInt(120) + 4;
                var10 = var5 + hellRNG.nextInt(16) + 8;
                (new WorldGenFire()).generate(worldObj, hellRNG, var8, var9, var10);
            }

            var6 = hellRNG.nextInt(hellRNG.nextInt(10) + 1);

            for (var7 = 0; var7 < var6; ++var7)
            {
                var8 = var4 + hellRNG.nextInt(16) + 8;
                var9 = hellRNG.nextInt(120) + 4;
                var10 = var5 + hellRNG.nextInt(16) + 8;
                (new WorldGenGlowStone1()).generate(worldObj, hellRNG, var8, var9, var10);
            }

            for (var7 = 0; var7 < 10; ++var7)
            {
                var8 = var4 + hellRNG.nextInt(16) + 8;
                var9 = hellRNG.nextInt(128);
                var10 = var5 + hellRNG.nextInt(16) + 8;
                (new WorldGenGlowStone2()).generate(worldObj, hellRNG, var8, var9, var10);
            }

            if (hellRNG.nextInt(1) == 0)
            {
                var7 = var4 + hellRNG.nextInt(16) + 8;
                var8 = hellRNG.nextInt(128);
                var9 = var5 + hellRNG.nextInt(16) + 8;
                (new WorldGenFlowers(Block.BROWN_MUSHROOM.id)).generate(worldObj, hellRNG, var7, var8, var9);
            }

            if (hellRNG.nextInt(1) == 0)
            {
                var7 = var4 + hellRNG.nextInt(16) + 8;
                var8 = hellRNG.nextInt(128);
                var9 = var5 + hellRNG.nextInt(16) + 8;
                (new WorldGenFlowers(Block.RED_MUSHROOM.id)).generate(worldObj, hellRNG, var7, var8, var9);
            }

            BlockSand.fallInstantly = false;
        }

        public bool saveChunks(bool var1, IProgressUpdate var2)
        {
            return true;
        }

        public bool unload100OldestChunks()
        {
            return false;
        }

        public bool canSave()
        {
            return true;
        }

        public String makeString()
        {
            return "HellRandomLevelSource";
        }

        public void markChunksForUnload(int _)
        {
        }
    }

}