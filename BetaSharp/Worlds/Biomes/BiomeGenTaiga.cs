using BetaSharp.Entities;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds.Gen.Features;

namespace BetaSharp.Worlds.Biomes;

public class BiomeGenTaiga : Biome
{

    public BiomeGenTaiga()
    {
        CreatureList.Add(new SpawnListEntry(EntityWolf.Class, 2));
    }

    public override Feature GetRandomWorldGenForTrees(JavaRandom rand)
    {
        return rand.NextInt(3) == 0 ? new PineTreeFeature() : new SpruceTreeFeature();
    }
}