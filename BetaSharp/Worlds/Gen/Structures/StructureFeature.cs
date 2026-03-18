using BetaSharp;
using BetaSharp.Worlds;
using BetaSharp.Worlds.Gen.Structures;
using BetaSharp.NBT;
using BetaSharp.Util.Maths;
public class StructureFeature
{
    private string structureName;
    private float probability; // 0.0 à 1.0

    public StructureFeature(string structureName, float probability)
    {
        this.structureName = structureName;
        this.probability = probability;
    }

    public void generate(World world, Random random, int chunkX, int chunkZ)
    {
        if (random.Next() > probability) return;

        NbtStructure structure = StructureManager.get(structureName);
        if (structure == null) return;

        // Position aléatoire dans le chunk
        int x = chunkX * 16 + random.Next(16);
        int z = chunkZ * 16 + random.Next(16);
        // Trouve le sol
        int y = findGroundLevel(world, x, z);
        if (y < 0) return;

        structure.place(world, x, y, z, random);
    }

    private int findGroundLevel(World world, int x, int z)
    {
        for (int y = 128; y > 0; y--)
        {
            if (world.getBlockId(x, y, z) != 0 && world.getBlockId(x, y + 1, z) == 0)
                return y + 1;
        }
        return -1;
    }
}
