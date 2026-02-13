namespace BetaSharp;

public class SpawnListEntry : java.lang.Object
{
    public java.lang.Class entityClass;
    public int spawnRarityRate;

    public SpawnListEntry(java.lang.Class var1, int var2)
    {
        entityClass = var1;
        spawnRarityRate = var2;
    }
}