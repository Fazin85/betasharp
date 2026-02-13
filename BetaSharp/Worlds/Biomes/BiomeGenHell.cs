using BetaSharp.Entities;

namespace BetaSharp.Worlds.Biomes;

public class BiomeGenHell : Biome
{

    public BiomeGenHell()
    {
        spawnableMonsterList.Clear();
        spawnableCreatureList.Clear();
        spawnableWaterCreatureList.Clear();
        spawnableMonsterList.Add(new SpawnListEntry(EntityGhast.Class, 10));

        spawnableMonsterList.Add(new SpawnListEntry(EntityPigZombie.Class, 10));
    }
}