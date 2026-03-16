using BetaSharp.Util.Maths;
using BetaSharp.Util.Maths.Noise;

namespace BetaSharp.Worlds.Biomes.Source;

public class BiomeSource : IDisposable
{
    private readonly OctaveSimplexNoiseSampler _temperatureSampler;
    private readonly OctaveSimplexNoiseSampler _downfallSampler;
    private readonly OctaveSimplexNoiseSampler _weirdnessSampler;
    public readonly ThreadLocal<double[]> TemperatureMap = new(() => new double[256]);
    public readonly ThreadLocal<double[]> DownfallMap = new(() => new double[256]);
    public readonly ThreadLocal<double[]> WeirdnessMap = new(() => new double[256]);
    public readonly ThreadLocal<Biome[]> Biomes = new(() => new Biome[256]);

    protected BiomeSource()
    {
    }

    public BiomeSource(World world)
    {
        _temperatureSampler = new OctaveSimplexNoiseSampler(new JavaRandom(world.getSeed() * 9871L), 4);
        _downfallSampler = new OctaveSimplexNoiseSampler(new JavaRandom(world.getSeed() * 39811L), 4);
        _weirdnessSampler = new OctaveSimplexNoiseSampler(new JavaRandom(world.getSeed() * 543321L), 2);
    }

    public BiomeSource(BiomeSource other)
    {
        _temperatureSampler = other._temperatureSampler;
        _downfallSampler = other._downfallSampler;
        _weirdnessSampler = other._weirdnessSampler;
    }

    public Biome GetBiome(ChunkPos chunkPos)
    {
        return GetBiome(chunkPos.X << 4, chunkPos.Z << 4);
    }

    public virtual Biome GetBiome(int x, int z)
    {
        return GetBiomesInArea(x, z, 1, 1)[0];
    }

    public virtual double GetSkyTemperature(int x, int z) // frequency scaler is different from normal temperature
    {
        TemperatureMap.Value = _temperatureSampler.sample(TemperatureMap.Value, x, z, 1, 1, (double)0.025F, (double)0.025F, 0.5D);
        return TemperatureMap.Value[0];
    }

    public Biome[] GetBiomesInArea(int x, int z, int width, int depth)
    {
        Biomes.Value = GetBiomesInArea(Biomes.Value, x, z, width, depth);
        return Biomes.Value;
    }

    public virtual double[] GetWeirdTemperatures(double[] map, int x, int z, int width, int depth) // incorporates some weirdness into the result
    {
        int size = width * depth;
        if (map == null || map.Length < size)
        {
            map = new double[size];
        }

        map = _temperatureSampler.sample(map, x, z, width, depth, (double)0.025F, (double)0.025F, 0.25D);
        WeirdnessMap.Value = _weirdnessSampler.sample(WeirdnessMap.Value, x, z, width, depth, 0.25D, 0.25D, 10 / 17d);
        int index = 0;

        for (int i = 0; i < width; ++i)
        {
            for (int j = 0; j < depth; ++j)
            {
                double weirdness = WeirdnessMap.Value[index] * 1.1D + 0.5D;
                double weight = 0.01D;
                double oneMinusWeight = 1.0D - weight;
                double temperature = (map[index] * 0.15D + 0.7D) * oneMinusWeight + weirdness * weight;
                temperature = 1.0D - (1.0D - temperature) * (1.0D - temperature);
                if (temperature < 0.0D)
                {
                    temperature = 0.0D;
                }

                if (temperature > 1.0D)
                {
                    temperature = 1.0D;
                }

                map[index] = temperature;
                ++index;
            }
        }

        return map;
    }

    public virtual Biome[] GetBiomesInArea(Biome[] biomes, int x, int z, int width, int depth)
    {
        int size = width * depth;
        if (biomes == null || biomes.Length < size)
        {
            biomes = new Biome[size];
        }

        TemperatureMap.Value = _temperatureSampler.sample(TemperatureMap.Value, x, z, width, width, (double)0.025F, (double)0.025F, 0.25D);
        DownfallMap.Value = _downfallSampler.sample(DownfallMap.Value, x, z, width, width, (double)0.05F, (double)0.05F, 1.0D / 3.0D);
        WeirdnessMap.Value = _weirdnessSampler.sample(WeirdnessMap.Value, x, z, width, width, 0.25D, 0.25D, 0.5882352941176471D);
        int index = 0;

        for (int i = 0; i < width; ++i)
        {
            for (int j = 0; j < depth; ++j)
            {
                double weirdness = WeirdnessMap.Value[index] * 1.1D + 0.5D;
                double weight = 0.01D;
                double oneMinusWeight = 1.0D - weight;
                double temperature = (TemperatureMap.Value[index] * 0.15D + 0.7D) * oneMinusWeight + weirdness * weight;
                weight = 0.002D;
                oneMinusWeight = 1.0D - weight;
                double downfall = (DownfallMap.Value[index] * 0.15D + 0.5D) * oneMinusWeight + weirdness * weight;
                temperature = 1.0D - (1.0D - temperature) * (1.0D - temperature);
                if (temperature < 0.0D)
                {
                    temperature = 0.0D;
                }

                if (downfall < 0.0D)
                {
                    downfall = 0.0D;
                }

                if (temperature > 1.0D)
                {
                    temperature = 1.0D;
                }

                if (downfall > 1.0D)
                {
                    downfall = 1.0D;
                }

                TemperatureMap.Value[index] = temperature;
                DownfallMap.Value[index] = downfall;
                biomes[index++] = Biome.GetBiome(temperature, downfall);
            }
        }

        return biomes;
    }

    public void Dispose()
    {
        TemperatureMap.Dispose();
        DownfallMap.Dispose();
        WeirdnessMap.Dispose();
        Biomes.Dispose();
    }

    ~BiomeSource()
    {
        Dispose();
    }
}
