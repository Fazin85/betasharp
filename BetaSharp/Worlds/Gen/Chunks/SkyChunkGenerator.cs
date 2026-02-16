using BetaSharp.Blocks;
using BetaSharp.Blocks.Materials;
using BetaSharp.Util.Maths.Noise;
using BetaSharp.Worlds.Biomes;
using BetaSharp.Worlds.Chunks;
using BetaSharp.Worlds.Gen.Carvers;
using BetaSharp.Worlds.Gen.Features;

namespace BetaSharp.Worlds.Gen.Chunks;

public class SkyChunkGenerator : ChunkSource
{

    private java.util.Random Random;
    private OctavePerlinNoiseSampler field_28086_k;
    private OctavePerlinNoiseSampler field_28085_l;
    private OctavePerlinNoiseSampler field_28084_m;
    private OctavePerlinNoiseSampler field_28083_n;
    private OctavePerlinNoiseSampler field_28082_o;
    public OctavePerlinNoiseSampler field_28096_a;
    public OctavePerlinNoiseSampler field_28095_b;
    public OctavePerlinNoiseSampler field_28094_c;
    private World World;
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

    public SkyChunkGenerator(World World, long Seed)
    {
        this.World = World;
        Random = new java.util.Random(Seed);
        field_28086_k = new OctavePerlinNoiseSampler(Random, 16);
        field_28085_l = new OctavePerlinNoiseSampler(Random, 16);
        field_28084_m = new OctavePerlinNoiseSampler(Random, 8);
        field_28083_n = new OctavePerlinNoiseSampler(Random, 4);
        field_28082_o = new OctavePerlinNoiseSampler(Random, 4);
        field_28096_a = new OctavePerlinNoiseSampler(Random, 10);
        field_28095_b = new OctavePerlinNoiseSampler(Random, 16);
        field_28094_c = new OctavePerlinNoiseSampler(Random, 8);
    }

    public void func_28071_a(int ChunkX, int ChunkZ, byte[] Buffer, Biome[] Biomes, double[] TemperatureMap)
    {
        byte SectionCount = 2;
        int SectionCountPlusOne = SectionCount + 1;
        byte Height = 33;
        int Depth = SectionCount + 1;
        field_28080_q = func_28073_a(field_28080_q, ChunkX * SectionCount, 0, ChunkZ * SectionCount, SectionCountPlusOne, Height, Depth);

        for (int SectionX = 0; SectionX < SectionCount; ++SectionX)
        {
            for (int SectionZ = 0; SectionZ < SectionCount; ++SectionZ)
            {
                for (int SubY = 0; SubY < 32; ++SubY)
                {
                    double InterpScale = 0.25D;
                    double a00 = field_28080_q[((SectionX + 0) * Depth + SectionZ + 0) * Height + SubY + 0];
                    double a01 = field_28080_q[((SectionX + 0) * Depth + SectionZ + 1) * Height + SubY + 0];
                    double a10 = field_28080_q[((SectionX + 1) * Depth + SectionZ + 0) * Height + SubY + 0];
                    double a11 = field_28080_q[((SectionX + 1) * Depth + SectionZ + 1) * Height + SubY + 0];
                    double da00 = (field_28080_q[((SectionX + 0) * Depth + SectionZ + 0) * Height + SubY + 1] - a00) * InterpScale;
                    double da01 = (field_28080_q[((SectionX + 0) * Depth + SectionZ + 1) * Height + SubY + 1] - a01) * InterpScale;
                    double da10 = (field_28080_q[((SectionX + 1) * Depth + SectionZ + 0) * Height + SubY + 1] - a10) * InterpScale;
                    double da11 = (field_28080_q[((SectionX + 1) * Depth + SectionZ + 1) * Height + SubY + 1] - a11) * InterpScale;

                    for (int SubSection = 0; SubSection < 4; ++SubSection)
                    {
                        double innerScale = 0.125D;
                        double val00 = a00;
                        double val01 = a01;
                        double stepX = (a10 - a00) * innerScale;
                        double stepZ = (a11 - a01) * innerScale;

                        for (int Quarter = 0; Quarter < 8; ++Quarter)
                        {
                            int index = Quarter + SectionX * 8 << 11 | 0 + SectionZ * 8 << 7 | SubY * 4 + SubSection;
                            short indexStep = 128;
                            double innerInterp = 0.125D;
                            double current = val00;
                            double delta = (val01 - val00) * innerInterp;

                            for (int i = 0; i < 8; ++i)
                            {
                                int blockId = 0;
                                if (current > 0.0D)
                                {
                                    blockId = Block.Stone.id;
                                }

                                Buffer[index] = (byte)blockId;
                                index += indexStep;
                                current += delta;
                            }

                            val00 += stepX;
                            val01 += stepZ;
                        }

                        a00 += da00;
                        a01 += da01;
                        a10 += da10;
                        a11 += da11;
                    }
                }
            }
        }

    }

    public void func_28072_a(int ChunkX, int ChunkZ, byte[] Buffer, Biome[] Biomes)
    {
        double NoiseScale = 1.0D / 32.0D;
        field_28079_r = field_28083_n.create(field_28079_r, ChunkX * 16, ChunkZ * 16, 0.0D, 16, 16, 1, NoiseScale, NoiseScale, 1.0D);
        field_28078_s = field_28083_n.create(field_28078_s, ChunkX * 16, 109.0134D, ChunkZ * 16, 16, 1, 16, NoiseScale, 1.0D, NoiseScale);
        field_28077_t = field_28082_o.create(field_28077_t, ChunkX * 16, ChunkZ * 16, 0.0D, 16, 16, 1, NoiseScale * 2.0D, NoiseScale * 2.0D, NoiseScale * 2.0D);

        for (int LocalX = 0; LocalX < 16; ++LocalX)
        {
            for (int LocalZ = 0; LocalZ < 16; ++LocalZ)
            {
                Biome biome = Biomes[LocalX + LocalZ * 16];
                int surfaceDepth = (int)(field_28077_t[LocalX + LocalZ * 16] / 3.0D + 3.0D + Random.nextDouble() * 0.25D);
                int depthCounter = -1;
                byte topBlock = biome.TopBlockId;
                byte soilBlock = biome.SoilBlockId;

                for (int y = 127; y >= 0; --y)
                {
                    int idx = (LocalZ * 16 + LocalX) * 128 + y;
                    byte blockId = Buffer[idx];
                    if (blockId == 0)
                    {
                        depthCounter = -1;
                    }
                    else if (blockId == Block.Stone.id)
                    {
                        if (depthCounter == -1)
                        {
                            if (surfaceDepth <= 0)
                            {
                                topBlock = 0;
                                soilBlock = (byte)Block.Stone.id;
                            }

                            depthCounter = surfaceDepth;
                            if (y >= 0)
                            {
                                Buffer[idx] = topBlock;
                            }
                            else
                            {
                                Buffer[idx] = soilBlock;
                            }
                        }
                        else if (depthCounter > 0)
                        {
                            --depthCounter;
                            Buffer[idx] = soilBlock;
                            if (depthCounter == 0 && soilBlock == Block.Sand.id)
                            {
                                depthCounter = Random.nextInt(4);
                                soilBlock = (byte)Block.Sandstone.id;
                            }
                        }
                    }
                }
            }
        }

    }

    public Chunk loadChunk(int ChunkX, int ChunkZ)
    {
        return getChunk(ChunkX, ChunkZ);
    }

    public Chunk getChunk(int ChunkX, int ChunkZ)
    {
        Random.setSeed(ChunkX * 341873128712L + ChunkZ * 132897987541L);
        byte[] chunkData = new byte[-java.lang.Short.MIN_VALUE];
        Chunk chunk = new Chunk(World, chunkData, ChunkX, ChunkZ);
        field_28075_v = World.getBiomeSource().GetBiomesInArea(field_28075_v, ChunkX * 16, ChunkZ * 16, 16, 16);
        double[] tempMap = World.getBiomeSource().TemperatureMap;
        func_28071_a(ChunkX, ChunkZ, chunkData, field_28075_v, tempMap);
        func_28072_a(ChunkX, ChunkZ, chunkData, field_28075_v);
        field_28076_u.carve(this, World, ChunkX, ChunkZ, chunkData);
        chunk.populateHeightMap();
        return chunk;
    }

    private double[] func_28073_a(double[] NoiseArray, int X, int Y, int Z, int SizeX, int SizeY, int SizeZ)
    {
        if (NoiseArray == null)
        {
            NoiseArray = new double[SizeX * SizeY * SizeZ];
        }

        double scaleA = 684.412D;
        double scaleB = 684.412D;
        double[] tempMap = World.getBiomeSource().TemperatureMap;
        double[] downfallMap = World.getBiomeSource().DownfallMap;
        field_28090_g = field_28096_a.create(field_28090_g, X, Z, SizeX, SizeZ, 1.121D, 1.121D, 0.5D);
        field_28089_h = field_28095_b.create(field_28089_h, X, Z, SizeX, SizeZ, 200.0D, 200.0D, 0.5D);
        scaleA *= 2.0D;
        field_28093_d = field_28084_m.create(field_28093_d, X, Y, Z, SizeX, SizeY, SizeZ, scaleA / 80.0D, scaleB / 160.0D, scaleA / 80.0D);
        field_28092_e = field_28086_k.create(field_28092_e, X, Y, Z, SizeX, SizeY, SizeZ, scaleA, scaleB, scaleA);
        field_28091_f = field_28085_l.create(field_28091_f, X, Y, Z, SizeX, SizeY, SizeZ, scaleA, scaleB, scaleA);
        int index = 0;
        int noiseIndex = 0;
        int step = 16 / SizeX;

        for (int sx = 0; sx < SizeX; ++sx)
        {
            int sampleX = sx * step + step / 2;

            for (int sz = 0; sz < SizeZ; ++sz)
            {
                int sampleZ = sz * step + step / 2;
                double temp = tempMap[sampleX * 16 + sampleZ];
                double down = downfallMap[sampleX * 16 + sampleZ] * temp;
                double blendMask = 1.0D - down;
                blendMask *= blendMask;
                blendMask *= blendMask;
                blendMask = 1.0D - blendMask;
                double noiseMix = (field_28090_g[noiseIndex] + 256.0D) / 512.0D;
                noiseMix *= blendMask;
                if (noiseMix > 1.0D)
                {
                    noiseMix = 1.0D;
                }

                double heightAdj = field_28089_h[noiseIndex] / 8000.0D;
                if (heightAdj < 0.0D)
                {
                    heightAdj = -heightAdj * 0.3D;
                }

                heightAdj = heightAdj * SizeY / 8.0D;
                heightAdj = 0.0D;
                if (noiseMix < 0.0D)
                {
                    noiseMix = 0.0D;
                }

                noiseMix += 0.5D;
                heightAdj = heightAdj * SizeY / 16.0D;
                ++noiseIndex;
                double halfY = SizeY / 2.0D;

                for (int sy = 0; sy < SizeY; ++sy)
                {
                    double value = 0.0D;
                    double absDist = (sy - halfY) * 8.0D / noiseMix;
                    if (absDist < 0.0D)
                    {
                        absDist *= -1.0D;
                    }

                    double sampleA = field_28092_e[index] / 512.0D;
                    double sampleB = field_28091_f[index] / 512.0D;
                    double blend = (field_28093_d[index] / 10.0D + 1.0D) / 2.0D;
                    if (blend < 0.0D)
                    {
                        value = sampleA;
                    }
                    else if (blend > 1.0D)
                    {
                        value = sampleB;
                    }
                    else
                    {
                        value = sampleA + (sampleB - sampleA) * blend;
                    }

                    value -= 8.0D;
                    byte top = 32;
                    double edgeBlend;
                    if (sy > SizeY - top)
                    {
                        edgeBlend = (double)((sy - (SizeY - top)) / (top - 1.0F));
                        value = value * (1.0D - edgeBlend) + -30.0D * edgeBlend;
                    }

                    top = 8;
                    if (sy < top)
                    {
                        edgeBlend = (double)((top - sy) / (top - 1.0F));
                        value = value * (1.0D - edgeBlend) + -30.0D * edgeBlend;
                    }

                    NoiseArray[index] = value;
                    ++index;
                }
            }
        }

        return NoiseArray;
    }

    public bool isChunkLoaded(int ChunkX, int ChunkZ)
    {
        return true;
    }

    public void decorate(ChunkSource Source, int ChunkX, int ChunkZ)
    {
        BlockSand.fallInstantly = true;
        int baseX = ChunkX * 16;
        int baseZ = ChunkZ * 16;
        Biome biome = World.getBiomeSource().GetBiome(baseX + 16, baseZ + 16);
        Random.setSeed(World.getSeed());
        long randA = Random.nextLong() / 2L * 2L + 1L;
        long randB = Random.nextLong() / 2L * 2L + 1L;
        Random.setSeed(ChunkX * randA + ChunkZ * randB ^ World.getSeed());
        double TreeNoiseScale = 0.5D;
        int randX;
        int randY;
        int randZ;
        if (Random.nextInt(4) == 0)
        {
            randX = baseX + Random.nextInt(16) + 8;
            randY = Random.nextInt(128);
            randZ = baseZ + Random.nextInt(16) + 8;
            new LakeFeature(Block.Water.id).generate(World, Random, randX, randY, randZ);
        }

        if (Random.nextInt(8) == 0)
        {
            randX = baseX + Random.nextInt(16) + 8;
            randY = Random.nextInt(Random.nextInt(120) + 8);
            randZ = baseZ + Random.nextInt(16) + 8;
            if (randY < 64 || Random.nextInt(10) == 0)
            {
                new LakeFeature(Block.Lava.id).generate(World, Random, randX, randY, randZ);
            }
        }

        int tempVar;
        for (randX = 0; randX < 8; ++randX)
        {
            randY = baseX + Random.nextInt(16) + 8;
            randZ = Random.nextInt(128);
            tempVar = baseZ + Random.nextInt(16) + 8;
            new DungeonFeature().generate(World, Random, randY, randZ, tempVar);
        }

        for (randX = 0; randX < 10; ++randX)
        {
            randY = baseX + Random.nextInt(16);
            randZ = Random.nextInt(128);
            tempVar = baseZ + Random.nextInt(16);
            new ClayOreFeature(32).generate(World, Random, randY, randZ, tempVar);
        }

        for (randX = 0; randX < 20; ++randX)
        {
            randY = baseX + Random.nextInt(16);
            randZ = Random.nextInt(128);
            tempVar = baseZ + Random.nextInt(16);
            new OreFeature(Block.Dirt.id, 32).generate(World, Random, randY, randZ, tempVar);
        }

        for (randX = 0; randX < 10; ++randX)
        {
            randY = baseX + Random.nextInt(16);
            randZ = Random.nextInt(128);
            tempVar = baseZ + Random.nextInt(16);
            new OreFeature(Block.Gravel.id, 32).generate(World, Random, randY, randZ, tempVar);
        }

        for (randX = 0; randX < 20; ++randX)
        {
            randY = baseX + Random.nextInt(16);
            randZ = Random.nextInt(128);
            tempVar = baseZ + Random.nextInt(16);
            new OreFeature(Block.CoalOre.id, 16).generate(World, Random, randY, randZ, tempVar);
        }

        for (randX = 0; randX < 20; ++randX)
        {
            randY = baseX + Random.nextInt(16);
            randZ = Random.nextInt(64);
            tempVar = baseZ + Random.nextInt(16);
            new OreFeature(Block.IronOre.id, 8).generate(World, Random, randY, randZ, tempVar);
        }

        for (randX = 0; randX < 2; ++randX)
        {
            randY = baseX + Random.nextInt(16);
            randZ = Random.nextInt(32);
            tempVar = baseZ + Random.nextInt(16);
            new OreFeature(Block.GoldOre.id, 8).generate(World, Random, randY, randZ, tempVar);
        }

        for (randX = 0; randX < 8; ++randX)
        {
            randY = baseX + Random.nextInt(16);
            randZ = Random.nextInt(16);
            tempVar = baseZ + Random.nextInt(16);
            new OreFeature(Block.RedstoneOre.id, 7).generate(World, Random, randY, randZ, tempVar);
        }

        for (randX = 0; randX < 1; ++randX)
        {
            randY = baseX + Random.nextInt(16);
            randZ = Random.nextInt(16);
            tempVar = baseZ + Random.nextInt(16);
            new OreFeature(Block.DiamondOre.id, 7).generate(World, Random, randY, randZ, tempVar);
        }

        for (randX = 0; randX < 1; ++randX)
        {
            randY = baseX + Random.nextInt(16);
            randZ = Random.nextInt(16) + Random.nextInt(16);
            tempVar = baseZ + Random.nextInt(16);
            new OreFeature(Block.LapisOre.id, 6).generate(World, Random, randY, randZ, tempVar);
        }

        TreeNoiseScale = 0.5D;
        int treeCount = (int)((field_28094_c.func_806_a(baseX * TreeNoiseScale, baseZ * TreeNoiseScale) / 8.0D + Random.nextDouble() * 4.0D + 4.0D) / 3.0D);
        int treesToGenerate = 0;
        if (Random.nextInt(10) == 0)
        {
            ++treesToGenerate;
        }

        if (biome == Biome.Forest)
        {
            treesToGenerate += treeCount + 5;
        }

        if (biome == Biome.Rainforest)
        {
            treesToGenerate += treeCount + 5;
        }

        if (biome == Biome.SeasonalForest)
        {
            treesToGenerate += treeCount + 2;
        }

        if (biome == Biome.Taiga)
        {
            treesToGenerate += treeCount + 5;
        }

        if (biome == Biome.Desert)
        {
            treesToGenerate -= 20;
        }

        if (biome == Biome.Tundra)
        {
            treesToGenerate -= 20;
        }

        if (biome == Biome.Plains)
        {
            treesToGenerate -= 20;
        }

        int treeX;
        for (randZ = 0; randZ < treesToGenerate; ++randZ)
        {
            tempVar = baseX + Random.nextInt(16) + 8;
            treeX = baseZ + Random.nextInt(16) + 8;
            Feature treeFeature = biome.GetRandomWorldGenForTrees(Random);
            treeFeature.prepare(1.0D, 1.0D, 1.0D);
            treeFeature.generate(World, Random, tempVar, World.getTopY(tempVar, treeX), treeX);
        }

        int tempZ;
        int rx;
        int ry;
        int rz;
        for (rx = 0; rx < 2; ++rx)
        {
            ry = baseX + Random.nextInt(16) + 8;
            rz = Random.nextInt(128);
            tempZ = baseZ + Random.nextInt(16) + 8;
            new PlantPatchFeature(Block.Dandelion.id).generate(World, Random, ry, rz, tempZ);
        }

        if (Random.nextInt(2) == 0)
        {
            rx = baseX + Random.nextInt(16) + 8;
            ry = Random.nextInt(128);
            rz = baseZ + Random.nextInt(16) + 8;
            new PlantPatchFeature(Block.Rose.id).generate(World, Random, rx, ry, rz);
        }

        if (Random.nextInt(4) == 0)
        {
            rx = baseX + Random.nextInt(16) + 8;
            ry = Random.nextInt(128);
            rz = baseZ + Random.nextInt(16) + 8;
            new PlantPatchFeature(Block.BrownMushroom.id).generate(World, Random, rx, ry, rz);
        }

        if (Random.nextInt(8) == 0)
        {
            rx = baseX + Random.nextInt(16) + 8;
            ry = Random.nextInt(128);
            rz = baseZ + Random.nextInt(16) + 8;
            new PlantPatchFeature(Block.RedMushroom.id).generate(World, Random, rx, ry, rz);
        }

        for (rx = 0; rx < 10; ++rx)
        {
            ry = baseX + Random.nextInt(16) + 8;
            rz = Random.nextInt(128);
            tempZ = baseZ + Random.nextInt(16) + 8;
            new SugarCanePatchFeature().generate(World, Random, ry, rz, tempZ);
        }

        if (Random.nextInt(32) == 0)
        {
            rx = baseX + Random.nextInt(16) + 8;
            ry = Random.nextInt(128);
            rz = baseZ + Random.nextInt(16) + 8;
            new PumpkinPatchFeature().generate(World, Random, rx, ry, rz);
        }

        int cactusAttempts = 0;
        if (biome == Biome.Desert)
        {
            cactusAttempts += 10;
        }

        int yPos;
        for (int i = 0; i < cactusAttempts; ++i)
        {
            ry = baseX + Random.nextInt(16) + 8;
            yPos = Random.nextInt(128);
            rz = baseZ + Random.nextInt(16) + 8;
            new CactusPatchFeature().generate(World, Random, ry, yPos, rz);
        }

        for (int i = 0; i < 50; ++i)
        {
            ry = baseX + Random.nextInt(16) + 8;
            tempZ = Random.nextInt(Random.nextInt(120) + 8);
            rz = baseZ + Random.nextInt(16) + 8;
            new SpringFeature(Block.FlowingWater.id).generate(World, Random, ry, tempZ, rz);
        }

        for (int i = 0; i < 20; ++i)
        {
            ry = baseX + Random.nextInt(16) + 8;
            tempZ = Random.nextInt(Random.nextInt(Random.nextInt(112) + 8) + 8);
            rz = baseZ + Random.nextInt(16) + 8;
            new SpringFeature(Block.FlowingLava.id).generate(World, Random, ry, tempZ, rz);
        }

        field_28074_w = World.getBiomeSource().GetTemperatures(field_28074_w, baseX + 8, baseZ + 8, 16, 16);

        for (int gx = baseX + 8; gx < baseX + 8 + 16; ++gx)
        {
            for (int gz = baseZ + 8; gz < baseZ + 8 + 16; ++gz)
            {
                tempZ = gx - (baseX + 8);
                int relZ = gz - (baseZ + 8);
                int topY = World.getTopSolidBlockY(gx, gz);
                double tempVal = field_28074_w[tempZ * 16 + relZ] - (topY - 64) / 64.0D * 0.3D;
                if (tempVal < 0.5D && topY > 0 && topY < 128 && World.isAir(gx, topY, gz) && World.getMaterial(gx, topY - 1, gz).BlocksMovement && World.getMaterial(gx, topY - 1, gz) != Material.Ice)
                {
                    World.setBlock(gx, topY, gz, Block.Snow.id);
                }
            }
        }

        BlockSand.fallInstantly = false;
    }

    public bool save(bool SaveAll, LoadingDisplay Display)
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