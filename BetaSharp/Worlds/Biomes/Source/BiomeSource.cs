using BetaSharp.Util.Maths;
using BetaSharp.Util.Maths.Noise;

namespace BetaSharp.Worlds.Biomes.Source;

public class BiomeSource : java.lang.Object
{
    private readonly OctaveSimplexNoiseSampler temperatureSampler;
    private readonly OctaveSimplexNoiseSampler downfallSampler;
    private readonly OctaveSimplexNoiseSampler weirdnessSampler;
    public double[] temperatureMap;
    public double[] downfallMap;
    public double[] weirdnessMap;
    public Biome[] biomes;

    protected BiomeSource()
    {
    }

    public BiomeSource(World world)
    {
        temperatureSampler = new OctaveSimplexNoiseSampler(new java.util.Random(world.getSeed() * 9871L), 4);
        downfallSampler = new OctaveSimplexNoiseSampler(new java.util.Random(world.getSeed() * 39811L), 4);
        weirdnessSampler = new OctaveSimplexNoiseSampler(new java.util.Random(world.getSeed() * 543321L), 2);
    }

    public virtual Biome getBiome(ChunkPos chunkPos)
    {
        return getBiome(chunkPos.x << 4, chunkPos.z << 4);
    }

    public virtual Biome getBiome(int x, int z)
    {
        return getBiomesInArea(x, z, 1, 1)[0];
    }

    public virtual double getTemperature(int x, int z)
    {
        temperatureMap = temperatureSampler.sample(temperatureMap, x, z, 1, 1, (double)0.025F, (double)0.025F, 0.5D);
        return temperatureMap[0];
    }

    public virtual Biome[] getBiomesInArea(int x, int z, int width, int depth)
    {
        biomes = getBiomesInArea(biomes, x, z, width, depth);
        return biomes;
    }

    public virtual double[] getTemperatures(double[] map, int x, int z, int width, int depth)
    {
        if (map == null || map.Length < width * depth)
        {
            map = new double[width * depth];
        }

        map = temperatureSampler.sample(map, x, z, width, depth, (double)0.025F, (double)0.025F, 0.25D);
        weirdnessMap = weirdnessSampler.sample(weirdnessMap, x, z, width, depth, 0.25D, 0.25D, 0.5882352941176471D);
        int var6 = 0;

        for (int var7 = 0; var7 < width; ++var7)
        {
            for (int var8 = 0; var8 < depth; ++var8)
            {
                double var9 = weirdnessMap[var6] * 1.1D + 0.5D;
                double var11 = 0.01D;
                double var13 = 1.0D - var11;
                double var15 = (map[var6] * 0.15D + 0.7D) * var13 + var9 * var11;
                var15 = 1.0D - (1.0D - var15) * (1.0D - var15);
                if (var15 < 0.0D)
                {
                    var15 = 0.0D;
                }

                if (var15 > 1.0D)
                {
                    var15 = 1.0D;
                }

                map[var6] = var15;
                ++var6;
            }
        }

        return map;
    }

    public virtual Biome[] getBiomesInArea(Biome[] biomes, int x, int z, int width, int depth)
    {
        if (biomes == null || biomes.Length < width * depth)
        {
            biomes = new Biome[width * depth];
        }

        temperatureMap = temperatureSampler.sample(temperatureMap, x, z, width, width, (double)0.025F, (double)0.025F, 0.25D);
        downfallMap = downfallSampler.sample(downfallMap, x, z, width, width, (double)0.05F, (double)0.05F, 1.0D / 3.0D);
        weirdnessMap = weirdnessSampler.sample(weirdnessMap, x, z, width, width, 0.25D, 0.25D, 0.5882352941176471D);
        int var6 = 0;

        for (int var7 = 0; var7 < width; ++var7)
        {
            for (int var8 = 0; var8 < depth; ++var8)
            {
                double var9 = weirdnessMap[var6] * 1.1D + 0.5D;
                double var11 = 0.01D;
                double var13 = 1.0D - var11;
                double var15 = (temperatureMap[var6] * 0.15D + 0.7D) * var13 + var9 * var11;
                var11 = 0.002D;
                var13 = 1.0D - var11;
                double var17 = (downfallMap[var6] * 0.15D + 0.5D) * var13 + var9 * var11;
                var15 = 1.0D - (1.0D - var15) * (1.0D - var15);
                if (var15 < 0.0D)
                {
                    var15 = 0.0D;
                }

                if (var17 < 0.0D)
                {
                    var17 = 0.0D;
                }

                if (var15 > 1.0D)
                {
                    var15 = 1.0D;
                }

                if (var17 > 1.0D)
                {
                    var17 = 1.0D;
                }

                temperatureMap[var6] = var15;
                downfallMap[var6] = var17;
                biomes[var6++] = Biome.getBiome(var15, var17);
            }
        }

        return biomes;
    }
}