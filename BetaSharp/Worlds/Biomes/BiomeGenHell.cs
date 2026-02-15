using BetaSharp.Entities;

namespace BetaSharp.Worlds.Biomes;

public class BiomeGenHell : Biome
{

    public BiomeGenHell()
    {
        MonsterList.Clear();
        CreatureList.Clear();
        WaterCreatureList.Clear();

        MonsterList.Add(new SpawnListEntry(EntityGhast.Class, 10));
        MonsterList.Add(new SpawnListEntry(EntityPigZombie.Class, 10));
    }
}