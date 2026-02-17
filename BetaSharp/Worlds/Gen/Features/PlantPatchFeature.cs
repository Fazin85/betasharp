using BetaSharp.Blocks;

namespace BetaSharp.Worlds.Gen.Features;

public class PlantPatchFeature : Feature
{

    private int plantBlockId;

    public PlantPatchFeature(int plantBlockId)
    {
        this.plantBlockId = plantBlockId;
    }

    public override bool Generate(World world, java.util.Random rand, int x, int y, int z)
    {
        for (int i = 0; i < 64; ++i)
        {
            int genX = x + rand.nextInt(8) - rand.nextInt(8);
            int genY = y + rand.nextInt(4) - rand.nextInt(4);
            int genZ = z + rand.nextInt(8) - rand.nextInt(8);
            if (world.isAir(genX, genY, genZ) && ((BlockPlant)Block.Blocks[plantBlockId]).canGrow(world, genX, genY, genZ))
            {
                world.SetBlockWithoutNotifyingNeighbors(genX, genY, genZ, plantBlockId);
            }
        }

        return true;
    }
}
