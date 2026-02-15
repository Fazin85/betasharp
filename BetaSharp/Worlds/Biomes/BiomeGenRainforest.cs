using BetaSharp.Worlds.Gen.Features;

namespace BetaSharp.Worlds.Biomes;

public class BiomeGenRainforest : Biome
{

    public override Feature GetRandomWorldGenForTrees(java.util.Random rand)
    {
        return rand.nextInt(3) == 0 ? new LargeOakTreeFeature() : new OakTreeFeature();
    }
}