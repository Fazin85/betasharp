using BetaSharp.Util.Maths;
using BetaSharp.Worlds.Gen.Features;

namespace BetaSharp.Worlds.Biomes;

internal class BiomeGenRainforest : Biome
{

    public override Feature GetRandomWorldGenForTrees(JavaRandom rand)
    {
        return rand.NextInt(3) == 0 ? new LargeOakTreeFeature() : new OakTreeFeature();
    }
}
