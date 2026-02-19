using BetaSharp.Blocks;
using BetaSharp.Blocks.Materials;
using BetaSharp.Util.Maths;
using BetaSharp.Util.Maths.Noise;
using BetaSharp.Worlds.Biomes;
using BetaSharp.Worlds.Chunks;
using BetaSharp.Worlds.Gen.Carvers;
using BetaSharp.Worlds.Gen.Features;

namespace BetaSharp.Worlds.Gen.Chunks;

public class OverworldChunkGenerator : ChunkSource
{

    private readonly JavaRandom random;
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
        random = new JavaRandom(seed);
        minLimitPerlinNoise = new OctavePerlinNoiseSampler(random, 16);
        maxLimitPerlinNoise = new OctavePerlinNoiseSampler(random, 16);
        perlinNoise1 = new OctavePerlinNoiseSampler(random, 8);
        perlinNoise2 = new OctavePerlinNoiseSampler(random, 4);
        perlinNoise3 = new OctavePerlinNoiseSampler(random, 4);
        floatingIslandScale = new OctavePerlinNoiseSampler(random, 10);
        floatingIslandNoise = new OctavePerlinNoiseSampler(random, 16);
        forestNoise = new OctavePerlinNoiseSampler(random, 8);
    }

    /// <summary>
    /// Generate the base terrain
    /// </summary>
    /// <param name="chunkX">X-Coordinate of this chunk</param>
    /// <param name="chunkZ">Z-Coordinate of this chunk</param>
    /// <param name="blocks">1D Array of Blocks within this chunk</param>
    /// <param name="biomes">1D Array of Biome values within this chunk</param>
    /// <param name="temperatures">1D Array of Temperature values within this chunk</param>
    /// <returns>The interpolated result.</returns>
    public void buildTerrain(int chunkX, int chunkZ, byte[] blocks, Biome[] biomes, double[] temperatures)
    {
        // TODO: Replace some of these with global-constants
        //const byte vertScale = 8; // ChunkHeight / 8 = 16 (?)
        const byte horiScale = 4; // ChunkWidth / 4 = 4
        const byte halfChunkHeight = 64;
        const int  xMax = horiScale + 1; // ChunkWidth / 4 + 1
        const byte yMax = 17; // ChunkHeight / 8 + 1
        const int  zMax = horiScale + 1; // ChunkWidth / 4 + 1

	    // Generate 4x16x4 low resolution noise map
        heightMap = generateHeightMap(heightMap, chunkX * horiScale, 0, chunkZ * horiScale, xMax, yMax, zMax);

	    // Terrain noise is trilinearly interpolated and only sampled every 4 blocks
        for (int sampleX = 0; sampleX < horiScale; ++sampleX)
        {
            for (int sampleZ = 0; sampleZ < horiScale; ++sampleZ)
            {
                // Chunk Height / 8 = 16
                for (int sampleY = 0; sampleY < 16; ++sampleY)
                {
                    const double verticalLerpStep = 0.125D;
                    double corner000 = heightMap[((sampleX + 0) * zMax + sampleZ + 0) * yMax + sampleY + 0];
                    double corner010 = heightMap[((sampleX + 0) * zMax + sampleZ + 1) * yMax + sampleY + 0];
                    double corner100 = heightMap[((sampleX + 1) * zMax + sampleZ + 0) * yMax + sampleY + 0];
                    double corner110 = heightMap[((sampleX + 1) * zMax + sampleZ + 1) * yMax + sampleY + 0];
                    double corner001 = (heightMap[((sampleX + 0) * zMax + sampleZ + 0) * yMax + sampleY + 1] - corner000) * verticalLerpStep;
                    double corner011 = (heightMap[((sampleX + 0) * zMax + sampleZ + 1) * yMax + sampleY + 1] - corner010) * verticalLerpStep;
                    double corner101 = (heightMap[((sampleX + 1) * zMax + sampleZ + 0) * yMax + sampleY + 1] - corner100) * verticalLerpStep;
                    double corner111 = (heightMap[((sampleX + 1) * zMax + sampleZ + 1) * yMax + sampleY + 1] - corner110) * verticalLerpStep;

				    // Interpolate the 1/4th scale noise
                    for (int subY = 0; subY < 8; ++subY)
                    {
                        const double horizontalLerpStep = 0.25D; // 1.0 / horiScale
                        double terrainX0 = corner000;
                        double terrainX1 = corner010;
                        double terrainStepX0 = (corner100 - corner000) * horizontalLerpStep;
                        double terrainStepX1 = (corner110 - corner010) * horizontalLerpStep;

                        for (int subX = 0; subX < 4; ++subX)
                        {
                            int blockIndex = (((subX + sampleX * 4) << 11) | ((sampleZ * 4) << 7) | ((sampleY * 8) + subY));
                            const short chunkHeight = 128; // Chunk Height
                            double terrainDensity = terrainX0;
                            double densityStepZ = (terrainX1 - terrainX0) * horizontalLerpStep;

                            for (int subZ = 0; subZ < 4; ++subZ)
                            {
                                // Here the actual block is determined
                                // Default to air block
                                int blockType = 0;
                                
							    // If water is too cold, turn into ice
                                double temp = temperatures[(sampleX * 4 + subX) * 16 + sampleZ * 4 + subZ];
                                if (sampleY * 8 + subY < halfChunkHeight)
                                {
                                    if (temp < 0.5D && sampleY * 8 + subY >= halfChunkHeight - 1)
                                    {
                                        blockType = Block.Ice.id;
                                    }
                                    else
                                    {
                                        blockType = Block.Water.id;
                                    }
                                }

                                // If the terrain density is above 0.0,
                                // turn it into stone
                                if (terrainDensity > 0.0D)
                                {
                                    blockType = Block.Stone.id;
                                }

                                blocks[blockIndex] = (byte)blockType;
                                blockIndex += chunkHeight;
                                terrainDensity += densityStepZ;
                            }

                            terrainX0 += terrainStepX0;
                            terrainX1 += terrainStepX1;
                        }

                        corner000 += corner001;
                        corner010 += corner011;
                        corner100 += corner101;
                        corner110 += corner111;
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
                bool var11 = sandBuffer[var8 + var9 * 16] + random.NextDouble() * 0.2D > 0.0D;
                bool var12 = gravelBuffer[var8 + var9 * 16] + random.NextDouble() * 0.2D > 3.0D;
                int var13 = (int)(depthBuffer[var8 + var9 * 16] / 3.0D + 3.0D + random.NextDouble() * 0.25D);
                int var14 = -1;
                byte var15 = var10.TopBlockId;
                byte var16 = var10.SoilBlockId;

                for (int var17 = 127; var17 >= 0; --var17)
                {
                    int var18 = (var9 * 16 + var8) * 128 + var17;
                    if (var17 <= 0 + random.NextInt(5))
                    {
                        blocks[var18] = (byte)Block.Bedrock.id;
                    }
                    else
                    {
                        byte var19 = blocks[var18];
                        if (var19 == 0)
                        {
                            var14 = -1;
                        }
                        else if (var19 == Block.Stone.id)
                        {
                            if (var14 == -1)
                            {
                                if (var13 <= 0)
                                {
                                    var15 = 0;
                                    var16 = (byte)Block.Stone.id;
                                }
                                else if (var17 >= var5 - 4 && var17 <= var5 + 1)
                                {
                                    var15 = var10.TopBlockId;
                                    var16 = var10.SoilBlockId;
                                    if (var12)
                                    {
                                        var15 = 0;
                                    }

                                    if (var12)
                                    {
                                        var16 = (byte)Block.Gravel.id;
                                    }

                                    if (var11)
                                    {
                                        var15 = (byte)Block.Sand.id;
                                    }

                                    if (var11)
                                    {
                                        var16 = (byte)Block.Sand.id;
                                    }
                                }

                                if (var17 < var5 && var15 == 0)
                                {
                                    var15 = (byte)Block.Water.id;
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
                                if (var14 == 0 && var16 == Block.Sand.id)
                                {
                                    var14 = random.NextInt(4);
                                    var16 = (byte)Block.Sandstone.id;
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
        random.SetSeed(chunkX * 341873128712L + chunkZ * 132897987541L);
        byte[] blocks = new byte[-java.lang.Short.MIN_VALUE];
        Chunk var4 = new Chunk(world, blocks, chunkX, chunkZ);
        biomes = world.getBiomeSource().GetBiomesInArea(biomes, chunkX * 16, chunkZ * 16, 16, 16);
        double[] var5 = world.getBiomeSource().TemperatureMap;
        buildTerrain(chunkX, chunkZ, blocks, biomes, var5);
        buildSurfaces(chunkX, chunkZ, blocks, biomes);
        cave.carve(this, world, chunkX, chunkZ, blocks);
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
        double[] var12 = world.getBiomeSource().TemperatureMap;
        double[] var13 = world.getBiomeSource().DownfallMap;
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
        Biome var6 = world.getBiomeSource().GetBiome(var4 + 16, var5 + 16);
        random.SetSeed(world.getSeed());
        long var7 = random.NextLong() / 2L * 2L + 1L;
        long var9 = random.NextLong() / 2L * 2L + 1L;
        random.SetSeed(x * var7 + z * var9 ^ world.getSeed());
        double var11 = 0.25D;
        int var13;
        int var14;
        int var15;
        if (random.NextInt(4) == 0)
        {
            var13 = var4 + random.NextInt(16) + 8;
            var14 = random.NextInt(128);
            var15 = var5 + random.NextInt(16) + 8;
            new LakeFeature(Block.Water.id).Generate(world, random, var13, var14, var15);
        }

        if (random.NextInt(8) == 0)
        {
            var13 = var4 + random.NextInt(16) + 8;
            var14 = random.NextInt(random.NextInt(120) + 8);
            var15 = var5 + random.NextInt(16) + 8;
            if (var14 < 64 || random.NextInt(10) == 0)
            {
                new LakeFeature(Block.Lava.id).Generate(world, random, var13, var14, var15);
            }
        }

        int var16;
        for (var13 = 0; var13 < 8; ++var13)
        {
            var14 = var4 + random.NextInt(16) + 8;
            var15 = random.NextInt(128);
            var16 = var5 + random.NextInt(16) + 8;
            new DungeonFeature().Generate(world, random, var14, var15, var16);
        }

        for (var13 = 0; var13 < 10; ++var13)
        {
            var14 = var4 + random.NextInt(16);
            var15 = random.NextInt(128);
            var16 = var5 + random.NextInt(16);
            new ClayOreFeature(32).Generate(world, random, var14, var15, var16);
        }

        for (var13 = 0; var13 < 20; ++var13)
        {
            var14 = var4 + random.NextInt(16);
            var15 = random.NextInt(128);
            var16 = var5 + random.NextInt(16);
            new OreFeature(Block.Dirt.id, 32).Generate(world, random, var14, var15, var16);
        }

        for (var13 = 0; var13 < 10; ++var13)
        {
            var14 = var4 + random.NextInt(16);
            var15 = random.NextInt(128);
            var16 = var5 + random.NextInt(16);
            new OreFeature(Block.Gravel.id, 32).Generate(world, random, var14, var15, var16);
        }

        for (var13 = 0; var13 < 20; ++var13)
        {
            var14 = var4 + random.NextInt(16);
            var15 = random.NextInt(128);
            var16 = var5 + random.NextInt(16);
            new OreFeature(Block.CoalOre.id, 16).Generate(world, random, var14, var15, var16);
        }

        for (var13 = 0; var13 < 20; ++var13)
        {
            var14 = var4 + random.NextInt(16);
            var15 = random.NextInt(64);
            var16 = var5 + random.NextInt(16);
            new OreFeature(Block.IronOre.id, 8).Generate(world, random, var14, var15, var16);
        }

        for (var13 = 0; var13 < 2; ++var13)
        {
            var14 = var4 + random.NextInt(16);
            var15 = random.NextInt(32);
            var16 = var5 + random.NextInt(16);
            new OreFeature(Block.GoldOre.id, 8).Generate(world, random, var14, var15, var16);
        }

        for (var13 = 0; var13 < 8; ++var13)
        {
            var14 = var4 + random.NextInt(16);
            var15 = random.NextInt(16);
            var16 = var5 + random.NextInt(16);
            new OreFeature(Block.RedstoneOre.id, 7).Generate(world, random, var14, var15, var16);
        }

        for (var13 = 0; var13 < 1; ++var13)
        {
            var14 = var4 + random.NextInt(16);
            var15 = random.NextInt(16);
            var16 = var5 + random.NextInt(16);
            new OreFeature(Block.DiamondOre.id, 7).Generate(world, random, var14, var15, var16);
        }

        for (var13 = 0; var13 < 1; ++var13)
        {
            var14 = var4 + random.NextInt(16);
            var15 = random.NextInt(16) + random.NextInt(16);
            var16 = var5 + random.NextInt(16);
            new OreFeature(Block.LapisOre.id, 6).Generate(world, random, var14, var15, var16);
        }

        var11 = 0.5D;
        var13 = (int)((forestNoise.generateNoise(var4 * var11, var5 * var11) / 8.0D + random.NextDouble() * 4.0D + 4.0D) / 3.0D);
        var14 = 0;
        if (random.NextInt(10) == 0)
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
            var16 = var4 + random.NextInt(16) + 8;
            var17 = var5 + random.NextInt(16) + 8;
            Feature var18 = var6.GetRandomWorldGenForTrees(random);
            var18.prepare(1.0D, 1.0D, 1.0D);
            var18.Generate(world, random, var16, world.getTopY(var16, var17), var17);
        }

        byte var27 = 0;
        if (var6 == Biome.Forest)
        {
            var27 = 2;
        }

        if (var6 == Biome.SeasonalForest)
        {
            var27 = 4;
        }

        if (var6 == Biome.Taiga)
        {
            var27 = 2;
        }

        if (var6 == Biome.Plains)
        {
            var27 = 3;
        }

        int var19;
        int var25;
        for (var16 = 0; var16 < var27; ++var16)
        {
            var17 = var4 + random.NextInt(16) + 8;
            var25 = random.NextInt(128);
            var19 = var5 + random.NextInt(16) + 8;
            new PlantPatchFeature(Block.Dandelion.id).Generate(world, random, var17, var25, var19);
        }

        byte var28 = 0;
        if (var6 == Biome.Forest)
        {
            var28 = 2;
        }

        if (var6 == Biome.Rainforest)
        {
            var28 = 10;
        }

        if (var6 == Biome.SeasonalForest)
        {
            var28 = 2;
        }

        if (var6 == Biome.Taiga)
        {
            var28 = 1;
        }

        if (var6 == Biome.Plains)
        {
            var28 = 10;
        }

        int var20;
        int var21;
        for (var17 = 0; var17 < var28; ++var17)
        {
            byte var26 = 1;
            if (var6 == Biome.Rainforest && random.NextInt(3) != 0)
            {
                var26 = 2;
            }

            var19 = var4 + random.NextInt(16) + 8;
            var20 = random.NextInt(128);
            var21 = var5 + random.NextInt(16) + 8;
            new GrassPatchFeature(Block.Grass.id, var26).Generate(world, random, var19, var20, var21);
        }

        var28 = 0;
        if (var6 == Biome.Desert)
        {
            var28 = 2;
        }

        for (var17 = 0; var17 < var28; ++var17)
        {
            var25 = var4 + random.NextInt(16) + 8;
            var19 = random.NextInt(128);
            var20 = var5 + random.NextInt(16) + 8;
            new DeadBushPatchFeature(Block.DeadBush.id).Generate(world, random, var25, var19, var20);
        }

        if (random.NextInt(2) == 0)
        {
            var17 = var4 + random.NextInt(16) + 8;
            var25 = random.NextInt(128);
            var19 = var5 + random.NextInt(16) + 8;
            new PlantPatchFeature(Block.Rose.id).Generate(world, random, var17, var25, var19);
        }

        if (random.NextInt(4) == 0)
        {
            var17 = var4 + random.NextInt(16) + 8;
            var25 = random.NextInt(128);
            var19 = var5 + random.NextInt(16) + 8;
            new PlantPatchFeature(Block.BrownMushroom.id).Generate(world, random, var17, var25, var19);
        }

        if (random.NextInt(8) == 0)
        {
            var17 = var4 + random.NextInt(16) + 8;
            var25 = random.NextInt(128);
            var19 = var5 + random.NextInt(16) + 8;
            new PlantPatchFeature(Block.RedMushroom.id).Generate(world, random, var17, var25, var19);
        }

        for (var17 = 0; var17 < 10; ++var17)
        {
            var25 = var4 + random.NextInt(16) + 8;
            var19 = random.NextInt(128);
            var20 = var5 + random.NextInt(16) + 8;
            new SugarCanePatchFeature().Generate(world, random, var25, var19, var20);
        }

        if (random.NextInt(32) == 0)
        {
            var17 = var4 + random.NextInt(16) + 8;
            var25 = random.NextInt(128);
            var19 = var5 + random.NextInt(16) + 8;
            new PumpkinPatchFeature().Generate(world, random, var17, var25, var19);
        }

        var17 = 0;
        if (var6 == Biome.Desert)
        {
            var17 += 10;
        }

        for (var25 = 0; var25 < var17; ++var25)
        {
            var19 = var4 + random.NextInt(16) + 8;
            var20 = random.NextInt(128);
            var21 = var5 + random.NextInt(16) + 8;
            new CactusPatchFeature().Generate(world, random, var19, var20, var21);
        }

        for (var25 = 0; var25 < 50; ++var25)
        {
            var19 = var4 + random.NextInt(16) + 8;
            var20 = random.NextInt(random.NextInt(120) + 8);
            var21 = var5 + random.NextInt(16) + 8;
            new SpringFeature(Block.FlowingWater.id).Generate(world, random, var19, var20, var21);
        }

        for (var25 = 0; var25 < 20; ++var25)
        {
            var19 = var4 + random.NextInt(16) + 8;
            var20 = random.NextInt(random.NextInt(random.NextInt(112) + 8) + 8);
            var21 = var5 + random.NextInt(16) + 8;
            new SpringFeature(Block.FlowingLava.id).Generate(world, random, var19, var20, var21);
        }

        temperatures = world.getBiomeSource().GetTemperatures(temperatures, var4 + 8, var5 + 8, 16, 16);

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
                    world.setBlock(var25, var22, var19, Block.Snow.id);
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
