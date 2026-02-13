using BetaSharp.Worlds.Gen.Features;

namespace BetaSharp.Worlds.Biomes;

public class BiomeGenRainforest : Biome
{

    public override Feature getRandomWorldGenForTrees(java.util.Random var1)
    {
        return var1.nextInt(3) == 0 ? new LargeOakTreeFeature() : new OakTreeFeature();
    }
}