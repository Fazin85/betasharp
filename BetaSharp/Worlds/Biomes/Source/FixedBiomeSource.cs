using BetaSharp.Util.Maths;

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

        Array.Fill(map, _temperature);
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

        Array.Fill(biomes, _biome);
        Array.Fill(DownfallMap.Value, _downfall);
        Array.Fill(TemperatureMap.Value, _temperature);

        return biomes;
    }
}
