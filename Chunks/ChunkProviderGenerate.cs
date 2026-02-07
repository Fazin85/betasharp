using betareborn.Biomes;
using betareborn.Blocks;
using betareborn.Materials;
using betareborn.Worlds;

namespace betareborn.Chunks
{
    public class ChunkProviderGenerate : IChunkProvider
    {

        private java.util.Random rand;
        private NoiseGeneratorOctaves field_912_k;
        private NoiseGeneratorOctaves field_911_l;
        private NoiseGeneratorOctaves field_910_m;
        private NoiseGeneratorOctaves field_909_n;
        private NoiseGeneratorOctaves field_908_o;
        public NoiseGeneratorOctaves field_922_a;
        public NoiseGeneratorOctaves field_921_b;
        public NoiseGeneratorOctaves mobSpawnerNoise;
        private World worldObj;
        private double[] field_4180_q;
        private double[] sandNoise = new double[256];
        private double[] gravelNoise = new double[256];
        private double[] stoneNoise = new double[256];
        private MapGenBase field_902_u = new MapGenCaves();
        private Biome[] biomesForGeneration;
        double[] field_4185_d;
        double[] field_4184_e;
        double[] field_4183_f;
        double[] field_4182_g;
        double[] field_4181_h;
        private double[] generatedTemperatures;

        public ChunkProviderGenerate(World var1, long var2)
        {
            worldObj = var1;
            rand = new java.util.Random(var2);
            field_912_k = new NoiseGeneratorOctaves(rand, 16);
            field_911_l = new NoiseGeneratorOctaves(rand, 16);
            field_910_m = new NoiseGeneratorOctaves(rand, 8);
            field_909_n = new NoiseGeneratorOctaves(rand, 4);
            field_908_o = new NoiseGeneratorOctaves(rand, 4);
            field_922_a = new NoiseGeneratorOctaves(rand, 10);
            field_921_b = new NoiseGeneratorOctaves(rand, 16);
            mobSpawnerNoise = new NoiseGeneratorOctaves(rand, 8);
        }

        public void generateTerrain(int var1, int var2, byte[] var3, Biome[] var4, double[] var5)
        {
            byte var6 = 4;
            byte var7 = 64;
            int var8 = var6 + 1;
            byte var9 = 17;
            int var10 = var6 + 1;
            field_4180_q = func_4061_a(field_4180_q, var1 * var6, 0, var2 * var6, var8, var9, var10);

            for (int var11 = 0; var11 < var6; ++var11)
            {
                for (int var12 = 0; var12 < var6; ++var12)
                {
                    for (int var13 = 0; var13 < 16; ++var13)
                    {
                        double var14 = 0.125D;
                        double var16 = field_4180_q[((var11 + 0) * var10 + var12 + 0) * var9 + var13 + 0];
                        double var18 = field_4180_q[((var11 + 0) * var10 + var12 + 1) * var9 + var13 + 0];
                        double var20 = field_4180_q[((var11 + 1) * var10 + var12 + 0) * var9 + var13 + 0];
                        double var22 = field_4180_q[((var11 + 1) * var10 + var12 + 1) * var9 + var13 + 0];
                        double var24 = (field_4180_q[((var11 + 0) * var10 + var12 + 0) * var9 + var13 + 1] - var16) * var14;
                        double var26 = (field_4180_q[((var11 + 0) * var10 + var12 + 1) * var9 + var13 + 1] - var18) * var14;
                        double var28 = (field_4180_q[((var11 + 1) * var10 + var12 + 0) * var9 + var13 + 1] - var20) * var14;
                        double var30 = (field_4180_q[((var11 + 1) * var10 + var12 + 1) * var9 + var13 + 1] - var22) * var14;

                        for (int var32 = 0; var32 < 8; ++var32)
                        {
                            double var33 = 0.25D;
                            double var35 = var16;
                            double var37 = var18;
                            double var39 = (var20 - var16) * var33;
                            double var41 = (var22 - var18) * var33;

                            for (int var43 = 0; var43 < 4; ++var43)
                            {
                                int var44 = var43 + var11 * 4 << 11 | 0 + var12 * 4 << 7 | var13 * 8 + var32;
                                short var45 = 128;
                                double var46 = 0.25D;
                                double var48 = var35;
                                double var50 = (var37 - var35) * var46;

                                for (int var52 = 0; var52 < 4; ++var52)
                                {
                                    double var53 = var5[(var11 * 4 + var43) * 16 + var12 * 4 + var52];
                                    int var55 = 0;
                                    if (var13 * 8 + var32 < var7)
                                    {
                                        if (var53 < 0.5D && var13 * 8 + var32 >= var7 - 1)
                                        {
                                            var55 = Block.ICE.id;
                                        }
                                        else
                                        {
                                            var55 = Block.WATER.id;
                                        }
                                    }

                                    if (var48 > 0.0D)
                                    {
                                        var55 = Block.STONE.id;
                                    }

                                    var3[var44] = (byte)var55;
                                    var44 += var45;
                                    var48 += var50;
                                }

                                var35 += var39;
                                var37 += var41;
                            }

                            var16 += var24;
                            var18 += var26;
                            var20 += var28;
                            var22 += var30;
                        }
                    }
                }
            }

        }

        public void replaceBlocksForBiome(int var1, int var2, byte[] var3, Biome[] var4)
        {
            byte var5 = 64;
            double var6 = 1.0D / 32.0D;
            sandNoise = field_909_n.generateNoiseOctaves(sandNoise, (double)(var1 * 16), (double)(var2 * 16), 0.0D, 16, 16, 1, var6, var6, 1.0D);
            gravelNoise = field_909_n.generateNoiseOctaves(gravelNoise, (double)(var1 * 16), 109.0134D, (double)(var2 * 16), 16, 1, 16, var6, 1.0D, var6);
            stoneNoise = field_908_o.generateNoiseOctaves(stoneNoise, (double)(var1 * 16), (double)(var2 * 16), 0.0D, 16, 16, 1, var6 * 2.0D, var6 * 2.0D, var6 * 2.0D);

            for (int var8 = 0; var8 < 16; ++var8)
            {
                for (int var9 = 0; var9 < 16; ++var9)
                {
                    Biome var10 = var4[var8 + var9 * 16];
                    bool var11 = sandNoise[var8 + var9 * 16] + rand.nextDouble() * 0.2D > 0.0D;
                    bool var12 = gravelNoise[var8 + var9 * 16] + rand.nextDouble() * 0.2D > 3.0D;
                    int var13 = (int)(stoneNoise[var8 + var9 * 16] / 3.0D + 3.0D + rand.nextDouble() * 0.25D);
                    int var14 = -1;
                    byte var15 = var10.topBlock;
                    byte var16 = var10.fillerBlock;

                    for (int var17 = 127; var17 >= 0; --var17)
                    {
                        int var18 = (var9 * 16 + var8) * 128 + var17;
                        if (var17 <= 0 + rand.nextInt(5))
                        {
                            var3[var18] = (byte)Block.BEDROCK.id;
                        }
                        else
                        {
                            byte var19 = var3[var18];
                            if (var19 == 0)
                            {
                                var14 = -1;
                            }
                            else if (var19 == Block.STONE.id)
                            {
                                if (var14 == -1)
                                {
                                    if (var13 <= 0)
                                    {
                                        var15 = 0;
                                        var16 = (byte)Block.STONE.id;
                                    }
                                    else if (var17 >= var5 - 4 && var17 <= var5 + 1)
                                    {
                                        var15 = var10.topBlock;
                                        var16 = var10.fillerBlock;
                                        if (var12)
                                        {
                                            var15 = 0;
                                        }

                                        if (var12)
                                        {
                                            var16 = (byte)Block.GRAVEL.id;
                                        }

                                        if (var11)
                                        {
                                            var15 = (byte)Block.SAND.id;
                                        }

                                        if (var11)
                                        {
                                            var16 = (byte)Block.SAND.id;
                                        }
                                    }

                                    if (var17 < var5 && var15 == 0)
                                    {
                                        var15 = (byte)Block.WATER.id;
                                    }

                                    var14 = var13;
                                    if (var17 >= var5 - 1)
                                    {
                                        var3[var18] = var15;
                                    }
                                    else
                                    {
                                        var3[var18] = var16;
                                    }
                                }
                                else if (var14 > 0)
                                {
                                    --var14;
                                    var3[var18] = var16;
                                    if (var14 == 0 && var16 == Block.SAND.id)
                                    {
                                        var14 = rand.nextInt(4);
                                        var16 = (byte)Block.SANDSTONE.id;
                                    }
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
            rand.setSeed((long)var1 * 341873128712L + (long)var2 * 132897987541L);
            byte[] var3 = new byte[-java.lang.Short.MIN_VALUE];
            Chunk var4 = new Chunk(worldObj, var3, var1, var2);
            biomesForGeneration = worldObj.getBiomeSource().getBiomesInArea(biomesForGeneration, var1 * 16, var2 * 16, 16, 16);
            double[] var5 = worldObj.getBiomeSource().temperatureMap;
            generateTerrain(var1, var2, var3, biomesForGeneration, var5);
            replaceBlocksForBiome(var1, var2, var3, biomesForGeneration);
            field_902_u.func_867_a(this, worldObj, var1, var2, var3);
            var4.func_1024_c();
            return var4;
        }

        private double[] func_4061_a(double[] var1, int var2, int var3, int var4, int var5, int var6, int var7)
        {
            if (var1 == null)
            {
                var1 = new double[var5 * var6 * var7];
            }

            double var8 = 684.412D;
            double var10 = 684.412D;
            double[] var12 = worldObj.getBiomeSource().temperatureMap;
            double[] var13 = worldObj.getBiomeSource().downfallMap;
            field_4182_g = field_922_a.func_4109_a(field_4182_g, var2, var4, var5, var7, 1.121D, 1.121D, 0.5D);
            field_4181_h = field_921_b.func_4109_a(field_4181_h, var2, var4, var5, var7, 200.0D, 200.0D, 0.5D);
            field_4185_d = field_910_m.generateNoiseOctaves(field_4185_d, (double)var2, (double)var3, (double)var4, var5, var6, var7, var8 / 80.0D, var10 / 160.0D, var8 / 80.0D);
            field_4184_e = field_912_k.generateNoiseOctaves(field_4184_e, (double)var2, (double)var3, (double)var4, var5, var6, var7, var8, var10, var8);
            field_4183_f = field_911_l.generateNoiseOctaves(field_4183_f, (double)var2, (double)var3, (double)var4, var5, var6, var7, var8, var10, var8);
            int var14 = 0;
            int var15 = 0;
            int var16 = 16 / var5;

            for (int var17 = 0; var17 < var5; ++var17)
            {
                int var18 = var17 * var16 + var16 / 2;

                for (int var19 = 0; var19 < var7; ++var19)
                {
                    int var20 = var19 * var16 + var16 / 2;
                    double var21 = var12[var18 * 16 + var20];
                    double var23 = var13[var18 * 16 + var20] * var21;
                    double var25 = 1.0D - var23;
                    var25 *= var25;
                    var25 *= var25;
                    var25 = 1.0D - var25;
                    double var27 = (field_4182_g[var15] + 256.0D) / 512.0D;
                    var27 *= var25;
                    if (var27 > 1.0D)
                    {
                        var27 = 1.0D;
                    }

                    double var29 = field_4181_h[var15] / 8000.0D;
                    if (var29 < 0.0D)
                    {
                        var29 = -var29 * 0.3D;
                    }

                    var29 = var29 * 3.0D - 2.0D;
                    if (var29 < 0.0D)
                    {
                        var29 /= 2.0D;
                        if (var29 < -1.0D)
                        {
                            var29 = -1.0D;
                        }

                        var29 /= 1.4D;
                        var29 /= 2.0D;
                        var27 = 0.0D;
                    }
                    else
                    {
                        if (var29 > 1.0D)
                        {
                            var29 = 1.0D;
                        }

                        var29 /= 8.0D;
                    }

                    if (var27 < 0.0D)
                    {
                        var27 = 0.0D;
                    }

                    var27 += 0.5D;
                    var29 = var29 * (double)var6 / 16.0D;
                    double var31 = (double)var6 / 2.0D + var29 * 4.0D;
                    ++var15;

                    for (int var33 = 0; var33 < var6; ++var33)
                    {
                        double var34 = 0.0D;
                        double var36 = ((double)var33 - var31) * 12.0D / var27;
                        if (var36 < 0.0D)
                        {
                            var36 *= 4.0D;
                        }

                        double var38 = field_4184_e[var14] / 512.0D;
                        double var40 = field_4183_f[var14] / 512.0D;
                        double var42 = (field_4185_d[var14] / 10.0D + 1.0D) / 2.0D;
                        if (var42 < 0.0D)
                        {
                            var34 = var38;
                        }
                        else if (var42 > 1.0D)
                        {
                            var34 = var40;
                        }
                        else
                        {
                            var34 = var38 + (var40 - var38) * var42;
                        }

                        var34 -= var36;
                        if (var33 > var6 - 4)
                        {
                            double var44 = (double)((float)(var33 - (var6 - 4)) / 3.0F);
                            var34 = var34 * (1.0D - var44) + -10.0D * var44;
                        }

                        var1[var14] = var34;
                        ++var14;
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
            Biome var6 = worldObj.getBiomeSource().getBiome(var4 + 16, var5 + 16);
            rand.setSeed(worldObj.getRandomSeed());
            long var7 = rand.nextLong() / 2L * 2L + 1L;
            long var9 = rand.nextLong() / 2L * 2L + 1L;
            rand.setSeed((long)var2 * var7 + (long)var3 * var9 ^ worldObj.getRandomSeed());
            double var11 = 0.25D;
            int var13;
            int var14;
            int var15;
            if (rand.nextInt(4) == 0)
            {
                var13 = var4 + rand.nextInt(16) + 8;
                var14 = rand.nextInt(128);
                var15 = var5 + rand.nextInt(16) + 8;
                (new WorldGenLakes(Block.WATER.id)).generate(worldObj, rand, var13, var14, var15);
            }

            if (rand.nextInt(8) == 0)
            {
                var13 = var4 + rand.nextInt(16) + 8;
                var14 = rand.nextInt(rand.nextInt(120) + 8);
                var15 = var5 + rand.nextInt(16) + 8;
                if (var14 < 64 || rand.nextInt(10) == 0)
                {
                    (new WorldGenLakes(Block.LAVA.id)).generate(worldObj, rand, var13, var14, var15);
                }
            }

            int var16;
            for (var13 = 0; var13 < 8; ++var13)
            {
                var14 = var4 + rand.nextInt(16) + 8;
                var15 = rand.nextInt(128);
                var16 = var5 + rand.nextInt(16) + 8;
                (new WorldGenDungeons()).generate(worldObj, rand, var14, var15, var16);
            }

            for (var13 = 0; var13 < 10; ++var13)
            {
                var14 = var4 + rand.nextInt(16);
                var15 = rand.nextInt(128);
                var16 = var5 + rand.nextInt(16);
                (new WorldGenClay(32)).generate(worldObj, rand, var14, var15, var16);
            }

            for (var13 = 0; var13 < 20; ++var13)
            {
                var14 = var4 + rand.nextInt(16);
                var15 = rand.nextInt(128);
                var16 = var5 + rand.nextInt(16);
                (new WorldGenMinable(Block.DIRT.id, 32)).generate(worldObj, rand, var14, var15, var16);
            }

            for (var13 = 0; var13 < 10; ++var13)
            {
                var14 = var4 + rand.nextInt(16);
                var15 = rand.nextInt(128);
                var16 = var5 + rand.nextInt(16);
                (new WorldGenMinable(Block.GRAVEL.id, 32)).generate(worldObj, rand, var14, var15, var16);
            }

            for (var13 = 0; var13 < 20; ++var13)
            {
                var14 = var4 + rand.nextInt(16);
                var15 = rand.nextInt(128);
                var16 = var5 + rand.nextInt(16);
                (new WorldGenMinable(Block.COAL_ORE.id, 16)).generate(worldObj, rand, var14, var15, var16);
            }

            for (var13 = 0; var13 < 20; ++var13)
            {
                var14 = var4 + rand.nextInt(16);
                var15 = rand.nextInt(64);
                var16 = var5 + rand.nextInt(16);
                (new WorldGenMinable(Block.IRON_ORE.id, 8)).generate(worldObj, rand, var14, var15, var16);
            }

            for (var13 = 0; var13 < 2; ++var13)
            {
                var14 = var4 + rand.nextInt(16);
                var15 = rand.nextInt(32);
                var16 = var5 + rand.nextInt(16);
                (new WorldGenMinable(Block.GOLD_ORE.id, 8)).generate(worldObj, rand, var14, var15, var16);
            }

            for (var13 = 0; var13 < 8; ++var13)
            {
                var14 = var4 + rand.nextInt(16);
                var15 = rand.nextInt(16);
                var16 = var5 + rand.nextInt(16);
                (new WorldGenMinable(Block.REDSTONE_ORE.id, 7)).generate(worldObj, rand, var14, var15, var16);
            }

            for (var13 = 0; var13 < 1; ++var13)
            {
                var14 = var4 + rand.nextInt(16);
                var15 = rand.nextInt(16);
                var16 = var5 + rand.nextInt(16);
                (new WorldGenMinable(Block.DIAMOND_ORE.id, 7)).generate(worldObj, rand, var14, var15, var16);
            }

            for (var13 = 0; var13 < 1; ++var13)
            {
                var14 = var4 + rand.nextInt(16);
                var15 = rand.nextInt(16) + rand.nextInt(16);
                var16 = var5 + rand.nextInt(16);
                (new WorldGenMinable(Block.LAPIS_ORE.id, 6)).generate(worldObj, rand, var14, var15, var16);
            }

            var11 = 0.5D;
            var13 = (int)((mobSpawnerNoise.func_806_a((double)var4 * var11, (double)var5 * var11) / 8.0D + rand.nextDouble() * 4.0D + 4.0D) / 3.0D);
            var14 = 0;
            if (rand.nextInt(10) == 0)
            {
                ++var14;
            }

            if (var6 == Biome.forest)
            {
                var14 += var13 + 5;
            }

            if (var6 == Biome.rainforest)
            {
                var14 += var13 + 5;
            }

            if (var6 == Biome.seasonalForest)
            {
                var14 += var13 + 2;
            }

            if (var6 == Biome.taiga)
            {
                var14 += var13 + 5;
            }

            if (var6 == Biome.desert)
            {
                var14 -= 20;
            }

            if (var6 == Biome.tundra)
            {
                var14 -= 20;
            }

            if (var6 == Biome.plains)
            {
                var14 -= 20;
            }

            int var17;
            for (var15 = 0; var15 < var14; ++var15)
            {
                var16 = var4 + rand.nextInt(16) + 8;
                var17 = var5 + rand.nextInt(16) + 8;
                WorldGenerator var18 = var6.getRandomWorldGenForTrees(rand);
                var18.func_517_a(1.0D, 1.0D, 1.0D);
                var18.generate(worldObj, rand, var16, worldObj.getHeightValue(var16, var17), var17);
            }

            byte var27 = 0;
            if (var6 == Biome.forest)
            {
                var27 = 2;
            }

            if (var6 == Biome.seasonalForest)
            {
                var27 = 4;
            }

            if (var6 == Biome.taiga)
            {
                var27 = 2;
            }

            if (var6 == Biome.plains)
            {
                var27 = 3;
            }

            int var19;
            int var25;
            for (var16 = 0; var16 < var27; ++var16)
            {
                var17 = var4 + rand.nextInt(16) + 8;
                var25 = rand.nextInt(128);
                var19 = var5 + rand.nextInt(16) + 8;
                (new WorldGenFlowers(Block.DANDELION.id)).generate(worldObj, rand, var17, var25, var19);
            }

            byte var28 = 0;
            if (var6 == Biome.forest)
            {
                var28 = 2;
            }

            if (var6 == Biome.rainforest)
            {
                var28 = 10;
            }

            if (var6 == Biome.seasonalForest)
            {
                var28 = 2;
            }

            if (var6 == Biome.taiga)
            {
                var28 = 1;
            }

            if (var6 == Biome.plains)
            {
                var28 = 10;
            }

            int var20;
            int var21;
            for (var17 = 0; var17 < var28; ++var17)
            {
                byte var26 = 1;
                if (var6 == Biome.rainforest && rand.nextInt(3) != 0)
                {
                    var26 = 2;
                }

                var19 = var4 + rand.nextInt(16) + 8;
                var20 = rand.nextInt(128);
                var21 = var5 + rand.nextInt(16) + 8;
                (new WorldGenTallGrass(Block.GRASS.id, var26)).generate(worldObj, rand, var19, var20, var21);
            }

            var28 = 0;
            if (var6 == Biome.desert)
            {
                var28 = 2;
            }

            for (var17 = 0; var17 < var28; ++var17)
            {
                var25 = var4 + rand.nextInt(16) + 8;
                var19 = rand.nextInt(128);
                var20 = var5 + rand.nextInt(16) + 8;
                (new WorldGenDeadBush(Block.DEAD_BUSH.id)).generate(worldObj, rand, var25, var19, var20);
            }

            if (rand.nextInt(2) == 0)
            {
                var17 = var4 + rand.nextInt(16) + 8;
                var25 = rand.nextInt(128);
                var19 = var5 + rand.nextInt(16) + 8;
                (new WorldGenFlowers(Block.ROSE.id)).generate(worldObj, rand, var17, var25, var19);
            }

            if (rand.nextInt(4) == 0)
            {
                var17 = var4 + rand.nextInt(16) + 8;
                var25 = rand.nextInt(128);
                var19 = var5 + rand.nextInt(16) + 8;
                (new WorldGenFlowers(Block.BROWN_MUSHROOM.id)).generate(worldObj, rand, var17, var25, var19);
            }

            if (rand.nextInt(8) == 0)
            {
                var17 = var4 + rand.nextInt(16) + 8;
                var25 = rand.nextInt(128);
                var19 = var5 + rand.nextInt(16) + 8;
                (new WorldGenFlowers(Block.RED_MUSHROOM.id)).generate(worldObj, rand, var17, var25, var19);
            }

            for (var17 = 0; var17 < 10; ++var17)
            {
                var25 = var4 + rand.nextInt(16) + 8;
                var19 = rand.nextInt(128);
                var20 = var5 + rand.nextInt(16) + 8;
                (new WorldGenReed()).generate(worldObj, rand, var25, var19, var20);
            }

            if (rand.nextInt(32) == 0)
            {
                var17 = var4 + rand.nextInt(16) + 8;
                var25 = rand.nextInt(128);
                var19 = var5 + rand.nextInt(16) + 8;
                (new WorldGenPumpkin()).generate(worldObj, rand, var17, var25, var19);
            }

            var17 = 0;
            if (var6 == Biome.desert)
            {
                var17 += 10;
            }

            for (var25 = 0; var25 < var17; ++var25)
            {
                var19 = var4 + rand.nextInt(16) + 8;
                var20 = rand.nextInt(128);
                var21 = var5 + rand.nextInt(16) + 8;
                (new WorldGenCactus()).generate(worldObj, rand, var19, var20, var21);
            }

            for (var25 = 0; var25 < 50; ++var25)
            {
                var19 = var4 + rand.nextInt(16) + 8;
                var20 = rand.nextInt(rand.nextInt(120) + 8);
                var21 = var5 + rand.nextInt(16) + 8;
                (new WorldGenLiquids(Block.FLOWING_WATER.id)).generate(worldObj, rand, var19, var20, var21);
            }

            for (var25 = 0; var25 < 20; ++var25)
            {
                var19 = var4 + rand.nextInt(16) + 8;
                var20 = rand.nextInt(rand.nextInt(rand.nextInt(112) + 8) + 8);
                var21 = var5 + rand.nextInt(16) + 8;
                (new WorldGenLiquids(Block.FLOWING_LAVA.id)).generate(worldObj, rand, var19, var20, var21);
            }

            generatedTemperatures = worldObj.getBiomeSource().getTemperatures(generatedTemperatures, var4 + 8, var5 + 8, 16, 16);

            for (var25 = var4 + 8; var25 < var4 + 8 + 16; ++var25)
            {
                for (var19 = var5 + 8; var19 < var5 + 8 + 16; ++var19)
                {
                    var20 = var25 - (var4 + 8);
                    var21 = var19 - (var5 + 8);
                    int var22 = worldObj.findTopSolidBlock(var25, var19);
                    double var23 = generatedTemperatures[var20 * 16 + var21] - (double)(var22 - 64) / 64.0D * 0.3D;
                    if (var23 < 0.5D && var22 > 0 && var22 < 128 && worldObj.isAir(var25, var22, var19) && worldObj.getMaterial(var25, var22 - 1, var19).blocksMovement() && worldObj.getMaterial(var25, var22 - 1, var19) != Material.ICE)
                    {
                        worldObj.setBlockWithNotify(var25, var22, var19, Block.SNOW.id);
                    }
                }
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
            return "RandomLevelSource";
        }

        public void markChunksForUnload(int _)
        {
        }
    }

}