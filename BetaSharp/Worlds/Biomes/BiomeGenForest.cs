using BetaSharp.Entities;
using BetaSharp.Worlds.Gen.Features;

namespace BetaSharp.Worlds.Biomes;

public class BiomeGenForest : Biome
{

    public BiomeGenForest()
    {
        CreatureList.Add(new SpawnListEntry(EntityWolf.Class, 2));
    }

    public override Feature GetRandomWorldGenForTrees(java.util.Random rand)
    {
        return rand.nextInt(5) == 0 ?
            new BirchTreeFeature() :
            rand.nextInt(3) == 0 ?
                new LargeOakTreeFeature() :
                new OakTreeFeature();
    }
}