using BetaSharp.Blocks;
using BetaSharp.Blocks.Materials;
using BetaSharp.Util.Maths;
using BetaSharp.Util.Maths.Noise;
using BetaSharp.Worlds.Biomes;
using BetaSharp.Worlds.Chunks;
using BetaSharp.Worlds.Gen.Carvers;
using BetaSharp.Worlds.Gen.Features;

namespace BetaSharp.Worlds.Gen.Chunks;

public class SkyChunkGenerator : ChunkSource
{

    private JavaRandom rand;
    private OctavePerlinNoiseSampler field_28086_k;
    private OctavePerlinNoiseSampler field_28085_l;
    private OctavePerlinNoiseSampler field_28084_m;
    private OctavePerlinNoiseSampler field_28083_n;
    private OctavePerlinNoiseSampler field_28082_o;
    public OctavePerlinNoiseSampler field_28096_a;
    public OctavePerlinNoiseSampler field_28095_b;
    public OctavePerlinNoiseSampler field_28094_c;
    private World world;
    private double[] field_28080_q;
    private double[] field_28079_r = new double[256];
    private double[] field_28078_s = new double[256];
    private double[] field_28077_t = new double[256];
    private Carver field_28076_u = new CaveCarver();
    private Biome[] field_28075_v;
    double[] field_28093_d;
    double[] field_28092_e;
    double[] field_28091_f;
    double[] field_28090_g;
    double[] field_28089_h;
    private double[] field_28074_w;

    public SkyChunkGenerator(World world, long seed)
    {
        this.world = world;
        rand = new (seed);
        field_28086_k = new OctavePerlinNoiseSampler(rand, 16);
        field_28085_l = new OctavePerlinNoiseSampler(rand, 16);
        field_28084_m = new OctavePerlinNoiseSampler(rand, 8);
        field_28083_n = new OctavePerlinNoiseSampler(rand, 4);
        field_28082_o = new OctavePerlinNoiseSampler(rand, 4);
        field_28096_a = new OctavePerlinNoiseSampler(rand, 10);
        field_28095_b = new OctavePerlinNoiseSampler(rand, 16);
        field_28094_c = new OctavePerlinNoiseSampler(rand, 8);
    }

    public void func_28071_a(int var1, int var2, byte[] var3, Biome[] var4, double[] var5)
    {
        byte var6 = 2;
        int var7 = var6 + 1;
        byte var8 = 33;
        int var9 = var6 + 1;
        field_28080_q = func_28073_a(field_28080_q, var1 * var6, 0, var2 * var6, var7, var8, var9);

        for (int var10 = 0; var10 < var6; ++var10)
        {
            for (int var11 = 0; var11 < var6; ++var11)
            {
                for (int var12 = 0; var12 < 32; ++var12)
                {
                    double var13 = 0.25D;
                    double var15 = field_28080_q[((var10 + 0) * var9 + var11 + 0) * var8 + var12 + 0];
                    double var17 = field_28080_q[((var10 + 0) * var9 + var11 + 1) * var8 + var12 + 0];
                    double var19 = field_28080_q[((var10 + 1) * var9 + var11 + 0) * var8 + var12 + 0];
                    double var21 = field_28080_q[((var10 + 1) * var9 + var11 + 1) * var8 + var12 + 0];
                    double var23 = (field_28080_q[((var10 + 0) * var9 + var11 + 0) * var8 + var12 + 1] - var15) * var13;
                    double var25 = (field_28080_q[((var10 + 0) * var9 + var11 + 1) * var8 + var12 + 1] - var17) * var13;
                    double var27 = (field_28080_q[((var10 + 1) * var9 + var11 + 0) * var8 + var12 + 1] - var19) * var13;
                    double var29 = (field_28080_q[((var10 + 1) * var9 + var11 + 1) * var8 + var12 + 1] - var21) * var13;

                    for (int var31 = 0; var31 < 4; ++var31)
                    {
                        double var32 = 0.125D;
                        double var34 = var15;
                        double var36 = var17;
                        double var38 = (var19 - var15) * var32;
                        double var40 = (var21 - var17) * var32;

                        for (int var42 = 0; var42 < 8; ++var42)
                        {
                            int var43 = var42 + var10 * 8 << 11 | 0 + var11 * 8 << 7 | var12 * 4 + var31;
                            short var44 = 128;
                            double var45 = 0.125D;
                            double var47 = var34;
                            double var49 = (var36 - var34) * var45;

                            for (int var51 = 0; var51 < 8; ++var51)
                            {
                                int var52 = 0;
                                if (var47 > 0.0D)
                                {
                                    var52 = Block.Stone.id;
                                }

                                var3[var43] = (byte)var52;
                                var43 += var44;
                                var47 += var49;
                            }

                            var34 += var38;
                            var36 += var40;
                        }

                        var15 += var23;
                        var17 += var25;
                        var19 += var27;
                        var21 += var29;
                    }
                }
            }
        }

    }

    public void func_28072_a(int var1, int var2, byte[] var3, Biome[] var4)
    {
        double var5 = 1.0D / 32.0D;
        field_28079_r = field_28083_n.create(field_28079_r, var1 * 16, var2 * 16, 0.0D, 16, 16, 1, var5, var5, 1.0D);
        field_28078_s = field_28083_n.create(field_28078_s, var1 * 16, 109.0134D, var2 * 16, 16, 1, 16, var5, 1.0D, var5);
        field_28077_t = field_28082_o.create(field_28077_t, var1 * 16, var2 * 16, 0.0D, 16, 16, 1, var5 * 2.0D, var5 * 2.0D, var5 * 2.0D);

        for (int var7 = 0; var7 < 16; ++var7)
        {
            for (int var8 = 0; var8 < 16; ++var8)
            {
                Biome var9 = var4[var7 + var8 * 16];
                int var10 = (int)(field_28077_t[var7 + var8 * 16] / 3.0D + 3.0D + rand.NextDouble() * 0.25D);
                int var11 = -1;
                byte var12 = var9.TopBlockId;
                byte var13 = var9.SoilBlockId;

                for (int var14 = 127; var14 >= 0; --var14)
                {
                    int var15 = (var8 * 16 + var7) * 128 + var14;
                    byte var16 = var3[var15];
                    if (var16 == 0)
                    {
                        var11 = -1;
                    }
                    else if (var16 == Block.Stone.id)
                    {
                        if (var11 == -1)
                        {
                            if (var10 <= 0)
                            {
                                var12 = 0;
                                var13 = (byte)Block.Stone.id;
                            }

                            var11 = var10;
                            if (var14 >= 0)
                            {
                                var3[var15] = var12;
                            }
                            else
                            {
                                var3[var15] = var13;
                            }
                        }
                        else if (var11 > 0)
                        {
                            --var11;
                            var3[var15] = var13;
                            if (var11 == 0 && var13 == Block.Sand.id)
                            {
                                var11 = rand.NextInt(4);
                                var13 = (byte)Block.Sandstone.id;
                            }
                        }
                    }
                }
            }
        }

    }

    public Chunk loadChunk(int var1, int var2)
    {
        return getChunk(var1, var2);
    }

    public Chunk getChunk(int var1, int var2)
    {
        rand.SetSeed(var1 * 341873128712L + var2 * 132897987541L);
        byte[] var3 = new byte[-java.lang.Short.MIN_VALUE];
        Chunk var4 = new Chunk(world, var3, var1, var2);
        field_28075_v = world.getBiomeSource().GetBiomesInArea(field_28075_v, var1 * 16, var2 * 16, 16, 16);
        double[] var5 = world.getBiomeSource().TemperatureMap;
        func_28071_a(var1, var2, var3, field_28075_v, var5);
        func_28072_a(var1, var2, var3, field_28075_v);
        field_28076_u.carve(this, world, var1, var2, var3);
        var4.populateHeightMap();
        return var4;
    }

    private double[] func_28073_a(double[] var1, int var2, int var3, int var4, int var5, int var6, int var7)
    {
        if (var1 == null)
        {
            var1 = new double[var5 * var6 * var7];
        }

        double var8 = 684.412D;
        double var10 = 684.412D;
        double[] var12 = world.getBiomeSource().TemperatureMap;
        double[] var13 = world.getBiomeSource().DownfallMap;
        field_28090_g = field_28096_a.create(field_28090_g, var2, var4, var5, var7, 1.121D, 1.121D, 0.5D);
        field_28089_h = field_28095_b.create(field_28089_h, var2, var4, var5, var7, 200.0D, 200.0D, 0.5D);
        var8 *= 2.0D;
        field_28093_d = field_28084_m.create(field_28093_d, var2, var3, var4, var5, var6, var7, var8 / 80.0D, var10 / 160.0D, var8 / 80.0D);
        field_28092_e = field_28086_k.create(field_28092_e, var2, var3, var4, var5, var6, var7, var8, var10, var8);
        field_28091_f = field_28085_l.create(field_28091_f, var2, var3, var4, var5, var6, var7, var8, var10, var8);
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
                double var27 = (field_28090_g[var15] + 256.0D) / 512.0D;
                var27 *= var25;
                if (var27 > 1.0D)
                {
                    var27 = 1.0D;
                }

                double var29 = field_28089_h[var15] / 8000.0D;
                if (var29 < 0.0D)
                {
                    var29 = -var29 * 0.3D;
                }

                var29 = var29 * 3.0D - 2.0D;
                if (var29 > 1.0D)
                {
                    var29 = 1.0D;
                }

                var29 /= 8.0D;
                var29 = 0.0D;
                if (var27 < 0.0D)
                {
                    var27 = 0.0D;
                }

                var27 += 0.5D;
                var29 = var29 * var6 / 16.0D;
                ++var15;
                double var31 = var6 / 2.0D;

                for (int var33 = 0; var33 < var6; ++var33)
                {
                    double var34 = 0.0D;
                    double var36 = (var33 - var31) * 8.0D / var27;
                    if (var36 < 0.0D)
                    {
                        var36 *= -1.0D;
                    }

                    double var38 = field_28092_e[var14] / 512.0D;
                    double var40 = field_28091_f[var14] / 512.0D;
                    double var42 = (field_28093_d[var14] / 10.0D + 1.0D) / 2.0D;
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

                    var34 -= 8.0D;
                    byte var44 = 32;
                    double var45;
                    if (var33 > var6 - var44)
                    {
                        var45 = (double)((var33 - (var6 - var44)) / (var44 - 1.0F));
                        var34 = var34 * (1.0D - var45) + -30.0D * var45;
                    }

                    var44 = 8;
                    if (var33 < var44)
                    {
                        var45 = (double)((var44 - var33) / (var44 - 1.0F));
                        var34 = var34 * (1.0D - var45) + -30.0D * var45;
                    }

                    var1[var14] = var34;
                    ++var14;
                }
            }
        }

        return var1;
    }

    public bool isChunkLoaded(int var1, int var2)
    {
        return true;
    }

    public void decorate(ChunkSource var1, int var2, int var3)
    {
        BlockSand.fallInstantly = true;
        int var4 = var2 * 16;
        int var5 = var3 * 16;
        Biome var6 = world.getBiomeSource().GetBiome(var4 + 16, var5 + 16);
        rand.SetSeed(world.getSeed());
        long var7 = rand.NextLong() / 2L * 2L + 1L;
        long var9 = rand.NextLong() / 2L * 2L + 1L;
        rand.SetSeed(var2 * var7 + var3 * var9 ^ world.getSeed());
        double var11 = 0.25D;
        int var13;
        int var14;
        int var15;
        if (rand.NextInt(4) == 0)
        {
            var13 = var4 + rand.NextInt(16) + 8;
            var14 = rand.NextInt(128);
            var15 = var5 + rand.NextInt(16) + 8;
            new LakeFeature(Block.Water.id).Generate(world, rand, var13, var14, var15);
        }

        if (rand.NextInt(8) == 0)
        {
            var13 = var4 + rand.NextInt(16) + 8;
            var14 = rand.NextInt(rand.NextInt(120) + 8);
            var15 = var5 + rand.NextInt(16) + 8;
            if (var14 < 64 || rand.NextInt(10) == 0)
            {
                new LakeFeature(Block.Lava.id).Generate(world, rand, var13, var14, var15);
            }
        }

        int var16;
        for (var13 = 0; var13 < 8; ++var13)
        {
            var14 = var4 + rand.NextInt(16) + 8;
            var15 = rand.NextInt(128);
            var16 = var5 + rand.NextInt(16) + 8;
            new DungeonFeature().Generate(world, rand, var14, var15, var16);
        }

        for (var13 = 0; var13 < 10; ++var13)
        {
            var14 = var4 + rand.NextInt(16);
            var15 = rand.NextInt(128);
            var16 = var5 + rand.NextInt(16);
            new ClayOreFeature(32).Generate(world, rand, var14, var15, var16);
        }

        for (var13 = 0; var13 < 20; ++var13)
        {
            var14 = var4 + rand.NextInt(16);
            var15 = rand.NextInt(128);
            var16 = var5 + rand.NextInt(16);
            new OreFeature(Block.Dirt.id, 32).Generate(world, rand, var14, var15, var16);
        }

        for (var13 = 0; var13 < 10; ++var13)
        {
            var14 = var4 + rand.NextInt(16);
            var15 = rand.NextInt(128);
            var16 = var5 + rand.NextInt(16);
            new OreFeature(Block.Gravel.id, 32).Generate(world, rand, var14, var15, var16);
        }

        for (var13 = 0; var13 < 20; ++var13)
        {
            var14 = var4 + rand.NextInt(16);
            var15 = rand.NextInt(128);
            var16 = var5 + rand.NextInt(16);
            new OreFeature(Block.CoalOre.id, 16).Generate(world, rand, var14, var15, var16);
        }

        for (var13 = 0; var13 < 20; ++var13)
        {
            var14 = var4 + rand.NextInt(16);
            var15 = rand.NextInt(64);
            var16 = var5 + rand.NextInt(16);
            new OreFeature(Block.IronOre.id, 8).Generate(world, rand, var14, var15, var16);
        }

        for (var13 = 0; var13 < 2; ++var13)
        {
            var14 = var4 + rand.NextInt(16);
            var15 = rand.NextInt(32);
            var16 = var5 + rand.NextInt(16);
            new OreFeature(Block.GoldOre.id, 8).Generate(world, rand, var14, var15, var16);
        }

        for (var13 = 0; var13 < 8; ++var13)
        {
            var14 = var4 + rand.NextInt(16);
            var15 = rand.NextInt(16);
            var16 = var5 + rand.NextInt(16);
            new OreFeature(Block.RedstoneOre.id, 7).Generate(world, rand, var14, var15, var16);
        }

        for (var13 = 0; var13 < 1; ++var13)
        {
            var14 = var4 + rand.NextInt(16);
            var15 = rand.NextInt(16);
            var16 = var5 + rand.NextInt(16);
            new OreFeature(Block.DiamondOre.id, 7).Generate(world, rand, var14, var15, var16);
        }

        for (var13 = 0; var13 < 1; ++var13)
        {
            var14 = var4 + rand.NextInt(16);
            var15 = rand.NextInt(16) + rand.NextInt(16);
            var16 = var5 + rand.NextInt(16);
            new OreFeature(Block.LapisOre.id, 6).Generate(world, rand, var14, var15, var16);
        }

        var11 = 0.5D;
        var13 = (int)((field_28094_c.generateNoise(var4 * var11, var5 * var11) / 8.0D + rand.NextDouble() * 4.0D + 4.0D) / 3.0D);
        var14 = 0;
        if (rand.NextInt(10) == 0)
        {
            ++var14;
        }

        if (var6 == Biome.Forest)
        {
            var14 += var13 + 5;
        }

        if (var6 == Biome.Rainforest)
        {
            var14 += var13 + 5;
        }

        if (var6 == Biome.SeasonalForest)
        {
            var14 += var13 + 2;
        }

        if (var6 == Biome.Taiga)
        {
            var14 += var13 + 5;
        }

        if (var6 == Biome.Desert)
        {
            var14 -= 20;
        }

        if (var6 == Biome.Tundra)
        {
            var14 -= 20;
        }

        if (var6 == Biome.Plains)
        {
            var14 -= 20;
        }

        int var17;
        for (var15 = 0; var15 < var14; ++var15)
        {
            var16 = var4 + rand.NextInt(16) + 8;
            var17 = var5 + rand.NextInt(16) + 8;
            Feature var18 = var6.GetRandomWorldGenForTrees(rand);
            var18.prepare(1.0D, 1.0D, 1.0D);
            var18.Generate(world, rand, var16, world.getTopY(var16, var17), var17);
        }

        int var23;
        for (var15 = 0; var15 < 2; ++var15)
        {
            var16 = var4 + rand.NextInt(16) + 8;
            var17 = rand.NextInt(128);
            var23 = var5 + rand.NextInt(16) + 8;
            new PlantPatchFeature(Block.Dandelion.id).Generate(world, rand, var16, var17, var23);
        }

        if (rand.NextInt(2) == 0)
        {
            var15 = var4 + rand.NextInt(16) + 8;
            var16 = rand.NextInt(128);
            var17 = var5 + rand.NextInt(16) + 8;
            new PlantPatchFeature(Block.Rose.id).Generate(world, rand, var15, var16, var17);
        }

        if (rand.NextInt(4) == 0)
        {
            var15 = var4 + rand.NextInt(16) + 8;
            var16 = rand.NextInt(128);
            var17 = var5 + rand.NextInt(16) + 8;
            new PlantPatchFeature(Block.BrownMushroom.id).Generate(world, rand, var15, var16, var17);
        }

        if (rand.NextInt(8) == 0)
        {
            var15 = var4 + rand.NextInt(16) + 8;
            var16 = rand.NextInt(128);
            var17 = var5 + rand.NextInt(16) + 8;
            new PlantPatchFeature(Block.RedMushroom.id).Generate(world, rand, var15, var16, var17);
        }

        for (var15 = 0; var15 < 10; ++var15)
        {
            var16 = var4 + rand.NextInt(16) + 8;
            var17 = rand.NextInt(128);
            var23 = var5 + rand.NextInt(16) + 8;
            new SugarCanePatchFeature().Generate(world, rand, var16, var17, var23);
        }

        if (rand.NextInt(32) == 0)
        {
            var15 = var4 + rand.NextInt(16) + 8;
            var16 = rand.NextInt(128);
            var17 = var5 + rand.NextInt(16) + 8;
            new PumpkinPatchFeature().Generate(world, rand, var15, var16, var17);
        }

        var15 = 0;
        if (var6 == Biome.Desert)
        {
            var15 += 10;
        }

        int var19;
        for (var16 = 0; var16 < var15; ++var16)
        {
            var17 = var4 + rand.NextInt(16) + 8;
            var23 = rand.NextInt(128);
            var19 = var5 + rand.NextInt(16) + 8;
            new CactusPatchFeature().Generate(world, rand, var17, var23, var19);
        }

        for (var16 = 0; var16 < 50; ++var16)
        {
            var17 = var4 + rand.NextInt(16) + 8;
            var23 = rand.NextInt(rand.NextInt(120) + 8);
            var19 = var5 + rand.NextInt(16) + 8;
            new SpringFeature(Block.FlowingWater.id).Generate(world, rand, var17, var23, var19);
        }

        for (var16 = 0; var16 < 20; ++var16)
        {
            var17 = var4 + rand.NextInt(16) + 8;
            var23 = rand.NextInt(rand.NextInt(rand.NextInt(112) + 8) + 8);
            var19 = var5 + rand.NextInt(16) + 8;
            new SpringFeature(Block.FlowingLava.id).Generate(world, rand, var17, var23, var19);
        }

        field_28074_w = world.getBiomeSource().GetTemperatures(field_28074_w, var4 + 8, var5 + 8, 16, 16);

        for (var16 = var4 + 8; var16 < var4 + 8 + 16; ++var16)
        {
            for (var17 = var5 + 8; var17 < var5 + 8 + 16; ++var17)
            {
                var23 = var16 - (var4 + 8);
                var19 = var17 - (var5 + 8);
                int var20 = world.getTopSolidBlockY(var16, var17);
                double var21 = field_28074_w[var23 * 16 + var19] - (var20 - 64) / 64.0D * 0.3D;
                if (var21 < 0.5D && var20 > 0 && var20 < 128 && world.isAir(var16, var20, var17) && world.getMaterial(var16, var20 - 1, var17).BlocksMovement && world.getMaterial(var16, var20 - 1, var17) != Material.Ice)
                {
                    world.setBlock(var16, var20, var17, Block.Snow.id);
                }
            }
        }

        BlockSand.fallInstantly = false;
    }

    public bool save(bool var1, LoadingDisplay var2)
    {
        return true;
    }

    public bool tick()
    {
        return false;
    }

    public bool canSave()
    {
        return true;
    }

    public string getDebugInfo()
    {
        return "RandomLevelSource";
    }

    public void markChunksForUnload(int _)
    {
    }
}
