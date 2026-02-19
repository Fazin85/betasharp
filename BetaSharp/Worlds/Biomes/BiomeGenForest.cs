using BetaSharp.Entities;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds.Gen.Features;

namespace BetaSharp.Worlds.Biomes;

public class BiomeGenForest : Biome
{

    public BiomeGenForest()
    {
        CreatureList.Add(new SpawnListEntry(EntityWolf.Class, 2));
    }

    public override Feature GetRandomWorldGenForTrees(JavaRandom rand)
    {
        return rand.NextInt(5) == 0 ?
            new BirchTreeFeature() :
            rand.NextInt(3) == 0 ?
                new LargeOakTreeFeature() :
                new OakTreeFeature();
    }
}