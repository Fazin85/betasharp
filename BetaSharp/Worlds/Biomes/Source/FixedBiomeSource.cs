using BetaSharp.Util.Maths;
using java.util;

namespace BetaSharp.Worlds.Biomes.Source;

public class FixedBiomeSource : BiomeSource
{

    private Biome field_4201_e;
    private double field_4200_f;
    private double field_4199_g;

    public FixedBiomeSource(Biome var1, double var2, double var4)
    {
        field_4201_e = var1;
        field_4200_f = var2;
        field_4199_g = var4;
    }

    public override Biome getBiome(ChunkPos var1)
    {
        return field_4201_e;
    }

    public override Biome getBiome(int var1, int var2)
    {
        return field_4201_e;
    }

    public override double getTemperature(int var1, int var2)
    {
        return field_4200_f;
    }

    public override Biome[] getBiomesInArea(int var1, int var2, int var3, int var4)
    {
        biomes = getBiomesInArea(biomes, var1, var2, var3, var4);
        return biomes;
    }

    public override double[] getTemperatures(double[] var1, int var2, int var3, int var4, int var5)
    {
        if (var1 == null || var1.Length < var4 * var5)
        {
            var1 = new double[var4 * var5];
        }

        Arrays.fill(var1, 0, var4 * var5, field_4200_f);
        return var1;
    }

    public override Biome[] getBiomesInArea(Biome[] var1, int var2, int var3, int var4, int var5)
    {
        if (var1 == null || var1.Length < var4 * var5)
        {
            var1 = new Biome[var4 * var5];
        }

        if (temperatureMap == null || temperatureMap.Length < var4 * var5)
        {
            temperatureMap = new double[var4 * var5];
            downfallMap = new double[var4 * var5];
        }

        Arrays.fill(var1, 0, var4 * var5, field_4201_e);
        Arrays.fill(downfallMap, 0, var4 * var5, field_4199_g);
        Arrays.fill(temperatureMap, 0, var4 * var5, field_4200_f);
        return var1;
    }
}