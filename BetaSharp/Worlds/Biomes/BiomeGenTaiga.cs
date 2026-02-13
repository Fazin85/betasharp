using BetaSharp.Entities;
using BetaSharp.Worlds.Gen.Features;

namespace BetaSharp.Worlds.Biomes;

public class BiomeGenTaiga : Biome
{

    public BiomeGenTaiga()
    {
        spawnableCreatureList.Add(new SpawnListEntry(EntityWolf.Class, 2));
    }

    public override Feature getRandomWorldGenForTrees(java.util.Random var1)
    {
        return var1.nextInt(3) == 0 ? new PineTreeFeature() : new SpruceTreeFeature();
    }
}