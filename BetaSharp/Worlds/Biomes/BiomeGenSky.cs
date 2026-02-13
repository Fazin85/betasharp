using BetaSharp.Entities;

namespace BetaSharp.Worlds.Biomes;

public class BiomeGenSky : Biome
{

    public BiomeGenSky()
    {
        spawnableMonsterList.Clear();
        spawnableCreatureList.Clear();
        spawnableWaterCreatureList.Clear();
        spawnableCreatureList.Add(new SpawnListEntry(EntityChicken.Class, 10));
    }

    public override int getSkyColorByTemp(float var1)
    {
        return 12632319;
    }
}