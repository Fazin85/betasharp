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
    private readonly OctavePerlinNoiseSampler selectorNoise;
    private readonly OctavePerlinNoiseSampler sandGravelNoise;
    private readonly OctavePerlinNoiseSampler depthNoise;
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
    double[] selectorNoiseBuffer;
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
        selectorNoise = new OctavePerlinNoiseSampler(random, 8);
        sandGravelNoise = new OctavePerlinNoiseSampler(random, 4);
        depthNoise = new OctavePerlinNoiseSampler(random, 4);
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
    public void BuildTerrain(int chunkX, int chunkZ, byte[] blocks, Biome[] biomes, double[] temperatures)
    {
        // TODO: Replace some of these with global-constants
        //const byte vertScale = 8; // ChunkHeight / 8 = 16 (?)
        const byte horiScale = 4; // ChunkWidth / 4 = 4
        const byte halfChunkHeight = 64;
        const int  xMax = horiScale + 1; // ChunkWidth / 4 + 1
        const byte yMax = 17; // ChunkHeight / 8 + 1
        const int  zMax = horiScale + 1; // ChunkWidth / 4 + 1

	    // Generate 4x16x4 low resolution noise map
        heightMap = GenerateHeightMap(heightMap, chunkX * horiScale, 0, chunkZ * horiScale, xMax, yMax, zMax);

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

    /// <summary>
    /// Generate the base terrain
    /// </summary>
    /// <param name="chunkX">X-Coordinate of this chunk</param>
    /// <param name="chunkZ">Z-Coordinate of this chunk</param>
    /// <param name="blocks">1D Array of Blocks within this chunk</param>
    /// <param name="biomes">1D Array of Biome values within this chunk</param>
    /// <returns>The interpolated result.</returns>
    public void BuildSurfaces(int chunkX, int chunkZ, byte[] blocks, Biome[] biomes)
    {
        byte blockZ = 64;
        double chunkBiome = 1.0D / 32.0D;
        sandBuffer = sandGravelNoise.create(sandBuffer, chunkX * 16, chunkZ * 16, 0.0D, 16, 16, 1, chunkBiome, chunkBiome, 1.0D);
        gravelBuffer = sandGravelNoise.create(gravelBuffer, chunkX * 16, 109.0134D, chunkZ * 16, 16, 1, 16, chunkBiome, 1.0D, chunkBiome);
        depthBuffer = depthNoise.create(depthBuffer, chunkX * 16, chunkZ * 16, 0.0D, 16, 16, 1, chunkBiome * 2.0D, chunkBiome * 2.0D, chunkBiome * 2.0D);

        for (int var8 = 0; var8 < 16; ++var8)
        {
            for (int zOffset = 0; zOffset < 16; ++zOffset)
            {
                Biome var10 = biomes[var8 + zOffset * 16];
                bool fraction = sandBuffer[var8 + zOffset * 16] + random.NextDouble() * 0.2D > 0.0D;
                bool var12 = gravelBuffer[var8 + zOffset * 16] + random.NextDouble() * 0.2D > 3.0D;
                int featureX = (int)(depthBuffer[var8 + zOffset * 16] / 3.0D + 3.0D + random.NextDouble() * 0.25D);
                int featureY = -1;
                byte featureZ = var10.TopBlockId;
                byte var16 = var10.SoilBlockId;

                for (int var17 = 127; var17 >= 0; --var17)
                {
                    int treeFeature = (zOffset * 16 + var8) * 128 + var17;
                    if (var17 <= 0 + random.NextInt(5))
                    {
                        blocks[treeFeature] = (byte)Block.Bedrock.id;
                    }
                    else
                    {
                        byte z = blocks[treeFeature];
                        if (z == 0)
                        {
                            featureY = -1;
                        }
                        else if (z == Block.Stone.id)
                        {
                            if (featureY == -1)
                            {
                                if (featureX <= 0)
                                {
                                    featureZ = 0;
                                    var16 = (byte)Block.Stone.id;
                                }
                                else if (var17 >= blockZ - 4 && var17 <= blockZ + 1)
                                {
                                    featureZ = var10.TopBlockId;
                                    var16 = var10.SoilBlockId;
                                    if (var12)
                                    {
                                        featureZ = 0;
                                    }

                                    if (var12)
                                    {
                                        var16 = (byte)Block.Gravel.id;
                                    }

                                    if (fraction)
                                    {
                                        featureZ = (byte)Block.Sand.id;
                                    }

                                    if (fraction)
                                    {
                                        var16 = (byte)Block.Sand.id;
                                    }
                                }

                                if (var17 < blockZ && featureZ == 0)
                                {
                                    featureZ = (byte)Block.Water.id;
                                }

                                featureY = featureX;
                                if (var17 >= blockZ - 1)
                                {
                                    blocks[treeFeature] = featureZ;
                                }
                                else
                                {
                                    blocks[treeFeature] = var16;
                                }
                            }
                            else if (featureY > 0)
                            {
                                --featureY;
                                blocks[treeFeature] = var16;
                                if (featureY == 0 && var16 == Block.Sand.id)
                                {
                                    featureY = random.NextInt(4);
                                    var16 = (byte)Block.Sandstone.id;
                                }
                            }
                        }
                    }
                }
            }
        }

    }

    public Chunk LoadChunk(int chunkX, int chunkZ)
    {
        return GetChunk(chunkX, chunkZ);
    }

    public Chunk GetChunk(int chunkX, int chunkZ)
    {
        random.SetSeed(chunkX * 341873128712L + chunkZ * 132897987541L);
        byte[] blocks = new byte[-java.lang.Short.MIN_VALUE];
        Chunk chunk = new Chunk(world, blocks, chunkX, chunkZ);
        biomes = world.getBiomeSource().GetBiomesInArea(biomes, chunkX * 16, chunkZ * 16, 16, 16);
        double[] temperatureMap = world.getBiomeSource().TemperatureMap;
        BuildTerrain(chunkX, chunkZ, blocks, biomes, temperatureMap);
        BuildSurfaces(chunkX, chunkZ, blocks, biomes);
        cave.carve(this, world, chunkX, chunkZ, blocks);
        chunk.populateHeightMap();
        return chunk;
    }

    private double[] GenerateHeightMap(double[] heightMap, int x, int y, int z, int sizeX, int sizeY, int sizeZ)
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
        selectorNoiseBuffer = selectorNoise.create(selectorNoiseBuffer, x, y, z, sizeX, sizeY, sizeZ, var8 / 80.0D, var10 / 160.0D, var8 / 80.0D);
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
                    double var42 = (selectorNoiseBuffer[var14] / 10.0D + 1.0D) / 2.0D;
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

    public bool IsChunkLoaded(int x, int z)
    {
        return true;
    }

    public void DecorateTerrain(ChunkSource source, int chunkX, int chunkZ)
    {
        BlockSand.fallInstantly = true;
        int blockX = chunkX * 16;
        int blockZ = chunkZ * 16;
        Biome chunkBiome = world.getBiomeSource().GetBiome(blockX + 16, blockZ + 16);
        random.SetSeed(world.getSeed());
        long xOffset = random.NextLong() / 2L * 2L + 1L;
        long zOffset = random.NextLong() / 2L * 2L + 1L;
        random.SetSeed(chunkX * xOffset + chunkZ * zOffset ^ world.getSeed());
        double fraction = 0.25D;
        int featureX;
        int featureY;
        int featureZ;

	    // Generate lakes
        if (random.NextInt(4) == 0)
        {
            featureX = blockX + random.NextInt(16) + 8;
            featureY = random.NextInt(128);
            featureZ = blockZ + random.NextInt(16) + 8;
            new LakeFeature(Block.Water.id).Generate(world, random, featureX, featureY, featureZ);
        }

	    // Generate lava lakes
        if (random.NextInt(8) == 0)
        {
            featureX = blockX + random.NextInt(16) + 8;
            featureY = random.NextInt(random.NextInt(120) + 8);
            featureZ = blockZ + random.NextInt(16) + 8;
            if (featureY < 64 || random.NextInt(10) == 0)
            {
                new LakeFeature(Block.Lava.id).Generate(world, random, featureX, featureY, featureZ);
            }
        }

	    // Generate Dungeons
        for (int i = 0; i < 8; ++i)
        {
            featureX = blockX + random.NextInt(16) + 8;
            featureY = random.NextInt(128);
            featureZ = blockZ + random.NextInt(16) + 8;
            new DungeonFeature().Generate(world, random, featureX, featureY, featureZ);
        }

	    // Generate Clay patches
        for (int i = 0; i < 10; ++i)
        {
            featureX = blockX + random.NextInt(16);
            featureY = random.NextInt(128);
            featureZ = blockZ + random.NextInt(16);
            new ClayOreFeature(32).Generate(world, random, featureX, featureY, featureZ);
        }

	    // Generate Dirt blobs
        for (int i = 0; i < 20; ++i)
        {
            featureX = blockX + random.NextInt(16);
            featureY = random.NextInt(128);
            featureZ = blockZ + random.NextInt(16);
            new OreFeature(Block.Dirt.id, 32).Generate(world, random, featureX, featureY, featureZ);
        }

	    // Generate Gravel blobs
        for (int i = 0; i < 10; ++i)
        {
            featureX = blockX + random.NextInt(16);
            featureY = random.NextInt(128);
            featureZ = blockZ + random.NextInt(16);
            new OreFeature(Block.Gravel.id, 32).Generate(world, random, featureX, featureY, featureZ);
        }

	    // Generate Coal Ore Veins
        for (int i = 0; i < 20; ++i)
        {
            featureX = blockX + random.NextInt(16);
            featureY = random.NextInt(128);
            featureZ = blockZ + random.NextInt(16);
            new OreFeature(Block.CoalOre.id, 16).Generate(world, random, featureX, featureY, featureZ);
        }

	    // Generate Iron Ore Veins
        for (int i = 0; i < 20; ++i)
        {
            featureX = blockX + random.NextInt(16);
            featureY = random.NextInt(64);
            featureZ = blockZ + random.NextInt(16);
            new OreFeature(Block.IronOre.id, 8).Generate(world, random, featureX, featureY, featureZ);
        }

	    // Generate Gold Ore Veins
        for (int i = 0; i < 2; ++i)
        {
            featureX = blockX + random.NextInt(16);
            featureY = random.NextInt(32);
            featureZ = blockZ + random.NextInt(16);
            new OreFeature(Block.GoldOre.id, 8).Generate(world, random, featureX, featureY, featureZ);
        }

	    // Generate Redstone Ore Veins
        for (int i = 0; i < 8; ++i)
        {
            featureX = blockX + random.NextInt(16);
            featureY = random.NextInt(16);
            featureZ = blockZ + random.NextInt(16);
            new OreFeature(Block.RedstoneOre.id, 7).Generate(world, random, featureX, featureY, featureZ);
        }

	    // Generate Diamond Ore Veins
        for (int i = 0; i < 1; ++i)
        {
            featureX = blockX + random.NextInt(16);
            featureY = random.NextInt(16);
            featureZ = blockZ + random.NextInt(16);
            new OreFeature(Block.DiamondOre.id, 7).Generate(world, random, featureX, featureY, featureZ);
        }

	    // Generate Lapis Lazuli Ore Veins
        for (int i = 0; i < 1; ++i)
        {
            featureX = blockX + random.NextInt(16);
            featureY = random.NextInt(16);
            featureZ = blockZ + random.NextInt(16);
            new OreFeature(Block.LapisOre.id, 6).Generate(world, random, featureX, featureY, featureZ);
        }

	    // Determine the number and type of trees that should be generated
        fraction = 0.5D;
        int treeDensitySample = (int)((forestNoise.generateNoise(blockX * fraction, blockZ * fraction) / 8.0D + random.NextDouble() * 4.0D + 4.0D) / 3.0D);
        int numberOfTrees = 0;
        if (random.NextInt(10) == 0)
        {
            ++numberOfTrees;
        }

        if (chunkBiome == Biome.Forest)
        {
            numberOfTrees += treeDensitySample + 5;
        }

        if (chunkBiome == Biome.Rainforest)
        {
            numberOfTrees += treeDensitySample + 5;
        }

        if (chunkBiome == Biome.SeasonalForest)
        {
            numberOfTrees += treeDensitySample + 2;
        }

        if (chunkBiome == Biome.Taiga)
        {
            numberOfTrees += treeDensitySample + 5;
        }

        if (chunkBiome == Biome.Desert)
        {
            numberOfTrees -= 20;
        }

        if (chunkBiome == Biome.Tundra)
        {
            numberOfTrees -= 20;
        }

        if (chunkBiome == Biome.Plains)
        {
            numberOfTrees -= 20;
        }

        for (int i = 0; i < numberOfTrees; ++i)
        {
            featureX = blockX + random.NextInt(16) + 8;
            featureZ = blockZ + random.NextInt(16) + 8;
            Feature treeFeature = chunkBiome.GetRandomWorldGenForTrees(random);
            treeFeature.prepare(1.0D, 1.0D, 1.0D);
            treeFeature.Generate(world, random, featureX, world.getTopY(featureX, featureZ), featureZ);
        }

	    // Choose an appropriate amount of Dandelions
        byte amountOfDandelions = 0;
        if (chunkBiome == Biome.Forest)
        {
            amountOfDandelions = 2;
        }

        if (chunkBiome == Biome.SeasonalForest)
        {
            amountOfDandelions = 4;
        }

        if (chunkBiome == Biome.Taiga)
        {
            amountOfDandelions = 2;
        }

        if (chunkBiome == Biome.Plains)
        {
            amountOfDandelions = 3;
        }


	    // Generate Dandelions
        for (byte i = 0; i < amountOfDandelions; ++i)
        {
            featureX = blockX + random.NextInt(16) + 8;
            featureY = random.NextInt(128);
            featureZ = blockZ + random.NextInt(16) + 8;
            new PlantPatchFeature(Block.Dandelion.id).Generate(world, random, featureX, featureY, featureZ);
        }

        byte amountOfTallgrass = 0;
        if (chunkBiome == Biome.Forest)
        {
            amountOfTallgrass = 2;
        }

        if (chunkBiome == Biome.Rainforest)
        {
            amountOfTallgrass = 10;
        }

        if (chunkBiome == Biome.SeasonalForest)
        {
            amountOfTallgrass = 2;
        }

        if (chunkBiome == Biome.Taiga)
        {
            amountOfTallgrass = 1;
        }

        if (chunkBiome == Biome.Plains)
        {
            amountOfTallgrass = 10;
        }

	    // Generate Tallgrass and Ferns
        for (byte i = 0; i < amountOfTallgrass; ++i)
        {
            byte grassMeta = 1;
            if (chunkBiome == Biome.Rainforest && random.NextInt(3) != 0)
            {
                // Fern
                grassMeta = 2;
            }

            featureX = blockX + random.NextInt(16) + 8;
            featureY = random.NextInt(128);
            featureZ = blockZ + random.NextInt(16) + 8;
            new GrassPatchFeature(Block.Grass.id, grassMeta).Generate(world, random, featureX, featureY, featureZ);
        }

	    // Generate Deadbushes
        byte amountOfDeadBushes = 0;
        if (chunkBiome == Biome.Desert)
        {
            amountOfDeadBushes = 2;
        }

        for (byte i = 0; i < amountOfDeadBushes; ++i)
        {
            featureX = blockX + random.NextInt(16) + 8;
            featureY = random.NextInt(128);
            featureZ = blockZ + random.NextInt(16) + 8;
            new DeadBushPatchFeature(Block.DeadBush.id).Generate(world, random, featureX, featureY, featureZ);
        }

        // Generate Roses
        if (random.NextInt(2) == 0)
        {
            featureX = blockX + random.NextInt(16) + 8;
            featureY = random.NextInt(128);
            featureZ = blockZ + random.NextInt(16) + 8;
            new PlantPatchFeature(Block.Rose.id).Generate(world, random, featureX, featureY, featureZ);
        }

	    // Generate Brown Mushrooms
        if (random.NextInt(4) == 0)
        {
            featureX = blockX + random.NextInt(16) + 8;
            featureY = random.NextInt(128);
            featureZ = blockZ + random.NextInt(16) + 8;
            new PlantPatchFeature(Block.BrownMushroom.id).Generate(world, random, featureX, featureY, featureZ);
        }

	    // Generate Red Mushrooms
        if (random.NextInt(8) == 0)
        {
            featureX = blockX + random.NextInt(16) + 8;
            featureY = random.NextInt(128);
            featureZ = blockZ + random.NextInt(16) + 8;
            new PlantPatchFeature(Block.RedMushroom.id).Generate(world, random, featureX, featureY, featureZ);
        }

	    // Generate Sugarcane
        for (int i = 0; i < 10; ++i)
        {
            featureX = blockX + random.NextInt(16) + 8;
            featureY = random.NextInt(128);
            featureZ = blockZ + random.NextInt(16) + 8;
            new SugarCanePatchFeature().Generate(world, random, featureX, featureY, featureZ);
        }

	    // Generate Pumpkin Patches
        if (random.NextInt(32) == 0)
        {
            featureX = blockX + random.NextInt(16) + 8;
            featureY = random.NextInt(128);
            featureZ = blockZ + random.NextInt(16) + 8;
            new PumpkinPatchFeature().Generate(world, random, featureX, featureY, featureZ);
        }

	    // Generate Cacti
        byte amountOfCacti = 0;
        if (chunkBiome == Biome.Desert)
        {
            amountOfCacti += 10;
        }

        for (int i = 0; i < amountOfCacti; ++i)
        {
            featureX = blockX + random.NextInt(16) + 8;
            featureY = random.NextInt(128);
            featureZ = blockZ + random.NextInt(16) + 8;
            new CactusPatchFeature().Generate(world, random, featureX, featureY, featureZ);
        }

        // Generate one-block water sources
        for (int i = 0; i < 50; ++i)
        {
            featureX = blockX + random.NextInt(16) + 8;
            featureY = random.NextInt(random.NextInt(120) + 8);
            featureZ = blockZ + random.NextInt(16) + 8;
            new SpringFeature(Block.FlowingWater.id).Generate(world, random, featureX, featureY, featureZ);
        }

        // Generate one-block lava sources
        for (int x = 0; x < 20; ++x)
        {
            featureX = blockX + random.NextInt(16) + 8;
            featureY = random.NextInt(random.NextInt(random.NextInt(112) + 8) + 8);
            featureZ = blockZ + random.NextInt(16) + 8;
            new SpringFeature(Block.FlowingLava.id).Generate(world, random, featureX, featureY, featureZ);
        }

        // Place Snow in cold regions
        temperatures = world.getBiomeSource().GetTemperatures(temperatures, blockX + 8, blockZ + 8, 16, 16);

        for (int x = blockX + 8; x < blockX + 8 + 16; ++x)
        {
            for (int z = blockZ + 8; z < blockZ + 8 + 16; ++z)
            {
                int offsetX = x - (blockX + 8);
                int offsetZ = z - (blockZ + 8);
                int var22 = world.getTopSolidBlockY(x, z);
                double temperatureSample = temperatures[offsetX * 16 + offsetZ] - (var22 - 64) / 64.0D * 0.3D;
                if (temperatureSample < 0.5D && var22 > 0 && var22 < 128 && world.isAir(x, var22, z) && world.getMaterial(x, var22 - 1, z).BlocksMovement && world.getMaterial(x, var22 - 1, z) != Material.Ice)
                {
                    world.setBlock(x, var22, z, Block.Snow.id);
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
