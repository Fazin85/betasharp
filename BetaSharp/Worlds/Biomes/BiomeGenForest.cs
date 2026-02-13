using BetaSharp.Entities;
using BetaSharp.Worlds.Gen.Features;

namespace BetaSharp.Worlds.Biomes;

public class BiomeGenForest : Biome
{

    public BiomeGenForest()
    {
        spawnableCreatureList.Add(new SpawnListEntry(EntityWolf.Class, 2));
    }

    public override Feature getRandomWorldGenForTrees(java.util.Random var1)
    {
        return var1.nextInt(5) == 0 ? new BirchTreeFeature() : var1.nextInt(3) == 0 ? new LargeOakTreeFeature() : new OakTreeFeature();
    }
}