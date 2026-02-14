using BetaSharp.Blocks;
using BetaSharp.Blocks.Materials;
using BetaSharp.Util.Maths.Noise;
using BetaSharp.Worlds.Biomes;
using BetaSharp.Worlds.Chunks;
using BetaSharp.Worlds.Gen.Carvers;
using BetaSharp.Worlds.Gen.Features;

namespace BetaSharp.Worlds.Gen.Chunks;

public class OverworldChunkGenerator : ChunkSource
{

    private readonly java.util.Random random;
    private readonly OctavePerlinNoiseSampler minLimitPerlinNoise;
    private readonly OctavePerlinNoiseSampler maxLimitPerlinNoise;
    private readonly OctavePerlinNoiseSampler perlinNoise1;
    private readonly OctavePerlinNoiseSampler perlinNoise2;
    private readonly OctavePerlinNoiseSampler perlinNoise3;
    public OctavePerlinNoiseSampler floatingIslandScale;
    public OctavePerlinNoiseSampler floatingIslandNoise;
    public OctavePerlinNoiseSampler forestNoise;
    private readonly World world;
    private double[] heightMap;
    private double[] sandBuffer = new double[256];
    private double[] gravelBuffer = new double[256];
    private double[] depthBuffer = new double[256];
    private readonly Carver cave = new CaveCarver();
    private Biome[] biomes;
    double[] perlinNoiseBuffer;
    double[] minLimitPerlinNoiseBuffer;
    double[] maxLimitPerlinNoiseBuffer;
    double[] scaleNoiseBuffer;
    double[] depthNoiseBuffer;
    private double[] temperatures;

    public OverworldChunkGenerator(World world, long seed)
    {
        this.world = world;
        random = new java.util.Random(seed);
        minLimitPerlinNoise = new OctavePerlinNoiseSampler(random, 16);
        maxLimitPerlinNoise = new OctavePerlinNoiseSampler(random, 16);
        perlinNoise1 = new OctavePerlinNoiseSampler(random, 8);
        perlinNoise2 = new OctavePerlinNoiseSampler(random, 4);
        perlinNoise3 = new OctavePerlinNoiseSampler(random, 4);
        floatingIslandScale = new OctavePerlinNoiseSampler(random, 10);
        floatingIslandNoise = new OctavePerlinNoiseSampler(random, 16);
        forestNoise = new OctavePerlinNoiseSampler(random, 8);
    }

    public void buildTerrain(int chunkX, int chunkZ, byte[] blocks, Biome[] biomes, double[] temperatures)
    {
        byte var6 = 4;
        byte var7 = 64;
        int var8 = var6 + 1;
        byte var9 = 17;
        int var10 = var6 + 1;
        heightMap = generateHeightMap(heightMap, chunkX * var6, 0, chunkZ * var6, var8, var9, var10);

        for (int var11 = 0; var11 < var6; ++var11)
        {
            for (int var12 = 0; var12 < var6; ++var12)
            {
                for (int var13 = 0; var13 < 16; ++var13)
                {
                    double var14 = 0.125D;
                    double var16 = heightMap[((var11 + 0) * var10 + var12 + 0) * var9 + var13 + 0];
                    double var18 = heightMap[((var11 + 0) * var10 + var12 + 1) * var9 + var13 + 0];
                    double var20 = heightMap[((var11 + 1) * var10 + var12 + 0) * var9 + var13 + 0];
                    double var22 = heightMap[((var11 + 1) * var10 + var12 + 1) * var9 + var13 + 0];
                    double var24 = (heightMap[((var11 + 0) * var10 + var12 + 0) * var9 + var13 + 1] - var16) * var14;
                    double var26 = (heightMap[((var11 + 0) * var10 + var12 + 1) * var9 + var13 + 1] - var18) * var14;
                    double var28 = (heightMap[((var11 + 1) * var10 + var12 + 0) * var9 + var13 + 1] - var20) * var14;
                    double var30 = (heightMap[((var11 + 1) * var10 + var12 + 1) * var9 + var13 + 1] - var22) * var14;

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
                                double var53 = temperatures[(var11 * 4 + var43) * 16 + var12 * 4 + var52];
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

                                blocks[var44] = (byte)var55;
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

    public void buildSurfaces(int chunkX, int chunkZ, byte[] blocks, Biome[] biomes)
    {
        byte var5 = 64;
        double var6 = 1.0D / 32.0D;
        sandBuffer = perlinNoise2.create(sandBuffer, chunkX * 16, chunkZ * 16, 0.0D, 16, 16, 1, var6, var6, 1.0D);
        gravelBuffer = perlinNoise2.create(gravelBuffer, chunkX * 16, 109.0134D, chunkZ * 16, 16, 1, 16, var6, 1.0D, var6);
        depthBuffer = perlinNoise3.create(depthBuffer, chunkX * 16, chunkZ * 16, 0.0D, 16, 16, 1, var6 * 2.0D, var6 * 2.0D, var6 * 2.0D);

        for (int var8 = 0; var8 < 16; ++var8)
        {
            for (int var9 = 0; var9 < 16; ++var9)
            {
                Biome var10 = biomes[var8 + var9 * 16];
                bool var11 = sandBuffer[var8 + var9 * 16] + random.nextDouble() * 0.2D > 0.0D;
                bool var12 = gravelBuffer[var8 + var9 * 16] + random.nextDouble() * 0.2D > 3.0D;
                int var13 = (int)(depthBuffer[var8 + var9 * 16] / 3.0D + 3.0D + random.nextDouble() * 0.25D);
                int var14 = -1;
                byte var15 = var10.topBlockId;
                byte var16 = var10.soilBlockId;

                for (int var17 = 127; var17 >= 0; --var17)
                {
                    int var18 = (var9 * 16 + var8) * 128 + var17;
                    if (var17 <= 0 + random.nextInt(5))
                    {
                        blocks[var18] = (byte)Block.BEDROCK.id;
                    }
                    else
                    {
                        byte var19 = blocks[var18];
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
                                    var15 = var10.topBlockId;
                                    var16 = var10.soilBlockId;
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
                                    blocks[var18] = var15;
                                }
                                else
                                {
                                    blocks[var18] = var16;
                                }
                            }
                            else if (var14 > 0)
                            {
                                --var14;
                                blocks[var18] = var16;
                                if (var14 == 0 && var16 == Block.SAND.id)
                                {
                                    var14 = random.nextInt(4);
                                    var16 = (byte)Block.SANDSTONE.id;
                                }
                            }
                        }
                    }
                }
            }
        }

    }

    public Chunk loadChunk(int chunkX, int chunkZ)
    {
        return getChunk(chunkX, chunkZ);
    }

    public Chunk getChunk(int chunkX, int chunkZ)
    {
        random.setSeed(chunkX * 341873128712L + chunkZ * 132897987541L);
        byte[] var3 = new byte[-java.lang.Short.MIN_VALUE];
        Chunk var4 = new Chunk(world, var3, chunkX, chunkZ);
        biomes = world.getBiomeSource().getBiomesInArea(biomes, chunkX * 16, chunkZ * 16, 16, 16);
        double[] var5 = world.getBiomeSource().temperatureMap;
        buildTerrain(chunkX, chunkZ, var3, biomes, var5);
        buildSurfaces(chunkX, chunkZ, var3, biomes);
        cave.carve(this, world, chunkX, chunkZ, var3);
        var4.populateHeightMap();
        return var4;
    }

    private double[] generateHeightMap(double[] heightMap, int x, int y, int z, int sizeX, int sizeY, int sizeZ)
    {
        if (heightMap == null)
        {
            heightMap = new double[sizeX * sizeY * sizeZ];
        }

        double var8 = 684.412D;
        double var10 = 684.412D;
        double[] var12 = world.getBiomeSource().temperatureMap;
        double[] var13 = world.getBiomeSource().downfallMap;
        scaleNoiseBuffer = floatingIslandScale.create(scaleNoiseBuffer, x, z, sizeX, sizeZ, 1.121D, 1.121D, 0.5D);
        depthNoiseBuffer = floatingIslandNoise.create(depthNoiseBuffer, x, z, sizeX, sizeZ, 200.0D, 200.0D, 0.5D);
        perlinNoiseBuffer = perlinNoise1.create(perlinNoiseBuffer, x, y, z, sizeX, sizeY, sizeZ, var8 / 80.0D, var10 / 160.0D, var8 / 80.0D);
        minLimitPerlinNoiseBuffer = minLimitPerlinNoise.create(minLimitPerlinNoiseBuffer, x, y, z, sizeX, sizeY, sizeZ, var8, var10, var8);
        maxLimitPerlinNoiseBuffer = maxLimitPerlinNoise.create(maxLimitPerlinNoiseBuffer, x, y, z, sizeX, sizeY, sizeZ, var8, var10, var8);
        int var14 = 0;
        int var15 = 0;
        int var16 = 16 / sizeX;

        for (int var17 = 0; var17 < sizeX; ++var17)
        {
            int var18 = var17 * var16 + var16 / 2;

            for (int var19 = 0; var19 < sizeZ; ++var19)
            {
                int var20 = var19 * var16 + var16 / 2;
                double var21 = var12[var18 * 16 + var20];
                double var23 = var13[var18 * 16 + var20] * var21;
                double var25 = 1.0D - var23;
                var25 *= var25;
                var25 *= var25;
                var25 = 1.0D - var25;
                double var27 = (scaleNoiseBuffer[var15] + 256.0D) / 512.0D;
                var27 *= var25;
                if (var27 > 1.0D)
                {
                    var27 = 1.0D;
                }

                double var29 = depthNoiseBuffer[var15] / 8000.0D;
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
                var29 = var29 * sizeY / 16.0D;
                double var31 = sizeY / 2.0D + var29 * 4.0D;
                ++var15;

                for (int var33 = 0; var33 < sizeY; ++var33)
                {
                    double var34 = 0.0D;
                    double var36 = (var33 - var31) * 12.0D / var27;
                    if (var36 < 0.0D)
                    {
                        var36 *= 4.0D;
                    }

                    double var38 = minLimitPerlinNoiseBuffer[var14] / 512.0D;
                    double var40 = maxLimitPerlinNoiseBuffer[var14] / 512.0D;
                    double var42 = (perlinNoiseBuffer[var14] / 10.0D + 1.0D) / 2.0D;
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
                    if (var33 > sizeY - 4)
                    {
                        double var44 = (double)((var33 - (sizeY - 4)) / 3.0F);
                        var34 = var34 * (1.0D - var44) + -10.0D * var44;
                    }

                    heightMap[var14] = var34;
                    ++var14;
                }
            }
        }

        return heightMap;
    }

    public bool isChunkLoaded(int x, int z)
    {
        return true;
    }

    public void decorate(ChunkSource source, int x, int z)
    {
        BlockSand.fallInstantly = true;
        int var4 = x * 16;
        int var5 = z * 16;
        Biome var6 = world.getBiomeSource().getBiome(var4 + 16, var5 + 16);
        random.setSeed(world.getSeed());
        long var7 = random.nextLong() / 2L * 2L + 1L;
        long var9 = random.nextLong() / 2L * 2L + 1L;
        random.setSeed(x * var7 + z * var9 ^ world.getSeed());
        double var11 = 0.25D;
        int var13;
        int var14;
        int var15;
        if (random.nextInt(4) == 0)
        {
            var13 = var4 + random.nextInt(16) + 8;
            var14 = random.nextInt(128);
            var15 = var5 + random.nextInt(16) + 8;
            new LakeFeature(Block.WATER.id).generate(world, random, var13, var14, var15);
        }

        if (random.nextInt(8) == 0)
        {
            var13 = var4 + random.nextInt(16) + 8;
            var14 = random.nextInt(random.nextInt(120) + 8);
            var15 = var5 + random.nextInt(16) + 8;
            if (var14 < 64 || random.nextInt(10) == 0)
            {
                new LakeFeature(Block.LAVA.id).generate(world, random, var13, var14, var15);
            }
        }

        int var16;
        for (var13 = 0; var13 < 8; ++var13)
        {
            var14 = var4 + random.nextInt(16) + 8;
            var15 = random.nextInt(128);
            var16 = var5 + random.nextInt(16) + 8;
            new DungeonFeature().generate(world, random, var14, var15, var16);
        }

        for (var13 = 0; var13 < 10; ++var13)
        {
            var14 = var4 + random.nextInt(16);
            var15 = random.nextInt(128);
            var16 = var5 + random.nextInt(16);
            new ClayOreFeature(32).generate(world, random, var14, var15, var16);
        }

        for (var13 = 0; var13 < 20; ++var13)
        {
            var14 = var4 + random.nextInt(16);
            var15 = random.nextInt(128);
            var16 = var5 + random.nextInt(16);
            new OreFeature(Block.DIRT.id, 32).generate(world, random, var14, var15, var16);
        }

        for (var13 = 0; var13 < 10; ++var13)
        {
            var14 = var4 + random.nextInt(16);
            var15 = random.nextInt(128);
            var16 = var5 + random.nextInt(16);
            new OreFeature(Block.GRAVEL.id, 32).generate(world, random, var14, var15, var16);
        }

        for (var13 = 0; var13 < 20; ++var13)
        {
            var14 = var4 + random.nextInt(16);
            var15 = random.nextInt(128);
            var16 = var5 + random.nextInt(16);
            new OreFeature(Block.COAL_ORE.id, 16).generate(world, random, var14, var15, var16);
        }

        for (var13 = 0; var13 < 20; ++var13)
        {
            var14 = var4 + random.nextInt(16);
            var15 = random.nextInt(64);
            var16 = var5 + random.nextInt(16);
            new OreFeature(Block.IRON_ORE.id, 8).generate(world, random, var14, var15, var16);
        }

        for (var13 = 0; var13 < 2; ++var13)
        {
            var14 = var4 + random.nextInt(16);
            var15 = random.nextInt(32);
            var16 = var5 + random.nextInt(16);
            new OreFeature(Block.GOLD_ORE.id, 8).generate(world, random, var14, var15, var16);
        }

        for (var13 = 0; var13 < 8; ++var13)
        {
            var14 = var4 + random.nextInt(16);
            var15 = random.nextInt(16);
            var16 = var5 + random.nextInt(16);
            new OreFeature(Block.REDSTONE_ORE.id, 7).generate(world, random, var14, var15, var16);
        }

        for (var13 = 0; var13 < 1; ++var13)
        {
            var14 = var4 + random.nextInt(16);
            var15 = random.nextInt(16);
            var16 = var5 + random.nextInt(16);
            new OreFeature(Block.DIAMOND_ORE.id, 7).generate(world, random, var14, var15, var16);
        }

        for (var13 = 0; var13 < 1; ++var13)
        {
            var14 = var4 + random.nextInt(16);
            var15 = random.nextInt(16) + random.nextInt(16);
            var16 = var5 + random.nextInt(16);
            new OreFeature(Block.LAPIS_ORE.id, 6).generate(world, random, var14, var15, var16);
        }

        var11 = 0.5D;
        var13 = (int)((forestNoise.func_806_a(var4 * var11, var5 * var11) / 8.0D + random.nextDouble() * 4.0D + 4.0D) / 3.0D);
        var14 = 0;
        if (random.nextInt(10) == 0)
        {
            ++var14;
        }

        if (var6 == Biome.FOREST)
        {
            var14 += var13 + 5;
        }

        if (var6 == Biome.RAINFOREST)
        {
            var14 += var13 + 5;
        }

        if (var6 == Biome.SEASONAL_FOREST)
        {
            var14 += var13 + 2;
        }

        if (var6 == Biome.TAIGA)
        {
            var14 += var13 + 5;
        }

        if (var6 == Biome.DESERT)
        {
            var14 -= 20;
        }

        if (var6 == Biome.TUNDRA)
        {
            var14 -= 20;
        }

        if (var6 == Biome.PLAINS)
        {
            var14 -= 20;
        }

        int var17;
        for (var15 = 0; var15 < var14; ++var15)
        {
            var16 = var4 + random.nextInt(16) + 8;
            var17 = var5 + random.nextInt(16) + 8;
            Feature var18 = var6.getRandomWorldGenForTrees(random);
            var18.prepare(1.0D, 1.0D, 1.0D);
            var18.generate(world, random, var16, world.getTopY(var16, var17), var17);
        }

        byte var27 = 0;
        if (var6 == Biome.FOREST)
        {
            var27 = 2;
        }

        if (var6 == Biome.SEASONAL_FOREST)
        {
            var27 = 4;
        }

        if (var6 == Biome.TAIGA)
        {
            var27 = 2;
        }

        if (var6 == Biome.PLAINS)
        {
            var27 = 3;
        }

        int var19;
        int var25;
        for (var16 = 0; var16 < var27; ++var16)
        {
            var17 = var4 + random.nextInt(16) + 8;
            var25 = random.nextInt(128);
            var19 = var5 + random.nextInt(16) + 8;
            new PlantPatchFeature(Block.DANDELION.id).generate(world, random, var17, var25, var19);
        }

        byte var28 = 0;
        if (var6 == Biome.FOREST)
        {
            var28 = 2;
        }

        if (var6 == Biome.RAINFOREST)
        {
            var28 = 10;
        }

        if (var6 == Biome.SEASONAL_FOREST)
        {
            var28 = 2;
        }

        if (var6 == Biome.TAIGA)
        {
            var28 = 1;
        }

        if (var6 == Biome.PLAINS)
        {
            var28 = 10;
        }

        int var20;
        int var21;
        for (var17 = 0; var17 < var28; ++var17)
        {
            byte var26 = 1;
            if (var6 == Biome.RAINFOREST && random.nextInt(3) != 0)
            {
                var26 = 2;
            }

            var19 = var4 + random.nextInt(16) + 8;
            var20 = random.nextInt(128);
            var21 = var5 + random.nextInt(16) + 8;
            new GrassPatchFeature(Block.GRASS.id, var26).generate(world, random, var19, var20, var21);
        }

        var28 = 0;
        if (var6 == Biome.DESERT)
        {
            var28 = 2;
        }

        for (var17 = 0; var17 < var28; ++var17)
        {
            var25 = var4 + random.nextInt(16) + 8;
            var19 = random.nextInt(128);
            var20 = var5 + random.nextInt(16) + 8;
            new DeadBushPatchFeature(Block.DEAD_BUSH.id).generate(world, random, var25, var19, var20);
        }

        if (random.nextInt(2) == 0)
        {
            var17 = var4 + random.nextInt(16) + 8;
            var25 = random.nextInt(128);
            var19 = var5 + random.nextInt(16) + 8;
            new PlantPatchFeature(Block.ROSE.id).generate(world, random, var17, var25, var19);
        }

        if (random.nextInt(4) == 0)
        {
            var17 = var4 + random.nextInt(16) + 8;
            var25 = random.nextInt(128);
            var19 = var5 + random.nextInt(16) + 8;
            new PlantPatchFeature(Block.BROWN_MUSHROOM.id).generate(world, random, var17, var25, var19);
        }

        if (random.nextInt(8) == 0)
        {
            var17 = var4 + random.nextInt(16) + 8;
            var25 = random.nextInt(128);
            var19 = var5 + random.nextInt(16) + 8;
            new PlantPatchFeature(Block.RED_MUSHROOM.id).generate(world, random, var17, var25, var19);
        }

        for (var17 = 0; var17 < 10; ++var17)
        {
            var25 = var4 + random.nextInt(16) + 8;
            var19 = random.nextInt(128);
            var20 = var5 + random.nextInt(16) + 8;
            new SugarCanePatchFeature().generate(world, random, var25, var19, var20);
        }

        if (random.nextInt(32) == 0)
        {
            var17 = var4 + random.nextInt(16) + 8;
            var25 = random.nextInt(128);
            var19 = var5 + random.nextInt(16) + 8;
            new PumpkinPatchFeature().generate(world, random, var17, var25, var19);
        }

        var17 = 0;
        if (var6 == Biome.DESERT)
        {
            var17 += 10;
        }

        for (var25 = 0; var25 < var17; ++var25)
        {
            var19 = var4 + random.nextInt(16) + 8;
            var20 = random.nextInt(128);
            var21 = var5 + random.nextInt(16) + 8;
            new CactusPatchFeature().generate(world, random, var19, var20, var21);
        }

        for (var25 = 0; var25 < 50; ++var25)
        {
            var19 = var4 + random.nextInt(16) + 8;
            var20 = random.nextInt(random.nextInt(120) + 8);
            var21 = var5 + random.nextInt(16) + 8;
            new SpringFeature(Block.FLOWING_WATER.id).generate(world, random, var19, var20, var21);
        }

        for (var25 = 0; var25 < 20; ++var25)
        {
            var19 = var4 + random.nextInt(16) + 8;
            var20 = random.nextInt(random.nextInt(random.nextInt(112) + 8) + 8);
            var21 = var5 + random.nextInt(16) + 8;
            new SpringFeature(Block.FLOWING_LAVA.id).generate(world, random, var19, var20, var21);
        }

        temperatures = world.getBiomeSource().getTemperatures(temperatures, var4 + 8, var5 + 8, 16, 16);

        for (var25 = var4 + 8; var25 < var4 + 8 + 16; ++var25)
        {
            for (var19 = var5 + 8; var19 < var5 + 8 + 16; ++var19)
            {
                var20 = var25 - (var4 + 8);
                var21 = var19 - (var5 + 8);
                int var22 = world.getTopSolidBlockY(var25, var19);
                double var23 = temperatures[var20 * 16 + var21] - (var22 - 64) / 64.0D * 0.3D;
                if (var23 < 0.5D && var22 > 0 && var22 < 128 && world.isAir(var25, var22, var19) && world.getMaterial(var25, var22 - 1, var19).BlocksMovement && world.getMaterial(var25, var22 - 1, var19) != Material.Ice)
                {
                    world.setBlock(var25, var22, var19, Block.SNOW.id);
                }
            }
        }

        BlockSand.fallInstantly = false;
    }

    public bool save(bool saveEntities, LoadingDisplay display)
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