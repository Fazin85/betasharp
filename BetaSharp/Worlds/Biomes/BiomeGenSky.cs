using BetaSharp.Entities;

namespace BetaSharp.Worlds.Biomes;

public class BiomeGenSky : Biome
{

    public BiomeGenSky()
    {
        MonsterList.Clear();
        CreatureList.Clear();
        WaterCreatureList.Clear();

        CreatureList.Add(new SpawnListEntry(EntityChicken.Class, 10));
    }

    public override int GetSkyColorByTemp(float rand)
    {
        return 0xC0C0FF;
    }
}