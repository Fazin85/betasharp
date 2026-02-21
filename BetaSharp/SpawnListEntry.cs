namespace BetaSharp;

public class SpawnListEntry(Type type, int spawnRarityRate)
{
	public readonly Type Type = type;
    public readonly int SpawnRarityRate = spawnRarityRate;
}