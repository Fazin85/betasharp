using BetaSharp.Util.Maths;
using java.util;

namespace BetaSharp.Worlds.Biomes.Source;

internal class FixedBiomeSource : BiomeSource
{

    private Biome _biome;
    private double _temperature;
    private double _downfall;

    public FixedBiomeSource(Biome biome, double temperature, double downfall)
    {
        _biome = biome;
        _temperature = temperature;
        _downfall = downfall;
    }

    public override Biome GetBiome(int x, int y) => _biome;

    public override double GetSkyTemperature(int x, int y) => _temperature;

    public override double[] GetWeirdTemperatures(double[] map, int x, int y, int width, int depth)
    {
        int size = width * depth;
        if (map == null || map.Length < size)
        {
            map = new double[size];
        }

        Arrays.fill(map, 0, size, _temperature);
        return map;
    }

    public override Biome[] GetBiomesInArea(Biome[] biomes, int x, int y, int width, int depth)
    {
        int size = width * depth;
        if (biomes == null || biomes.Length < size)
        {
            biomes = new Biome[size];
        }

        if (TemperatureMap.Value == null || TemperatureMap.Value.Length < size)
        {
            TemperatureMap.Value = new double[size];
            DownfallMap.Value = new double[size];
        }

        Arrays.fill(biomes, 0, size, _biome);
        Arrays.fill(DownfallMap.Value, 0, size, _downfall);
        Arrays.fill(TemperatureMap.Value, 0, size, _temperature);

        return biomes;
    }
}
