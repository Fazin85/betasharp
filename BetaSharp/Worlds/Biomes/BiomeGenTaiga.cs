using BetaSharp.Entities;
using BetaSharp.Worlds.Gen.Features;

namespace BetaSharp.Worlds.Biomes;

public class BiomeGenTaiga : Biome
{

    public BiomeGenTaiga()
    {
        CreatureList.Add(new SpawnListEntry(EntityWolf.Class, 2));
    }

    public override Feature GetRandomWorldGenForTrees(java.util.Random rand)
    {
        return rand.nextInt(3) == 0 ? new PineTreeFeature() : new SpruceTreeFeature();
    }
}