using BetaSharp.Blocks;

namespace BetaSharp.Worlds.Gen.Features;

public class CactusPatchFeature : Feature
{

    public override bool Generate(World world, java.util.Random rand, int x, int y, int z)
    {
        for (int i = 0; i < 10; ++i)
        {
            int genX = x + rand.nextInt(8) - rand.nextInt(8);
            int genY = y + rand.nextInt(4) - rand.nextInt(4);
            int genZ = z + rand.nextInt(8) - rand.nextInt(8);
            if (world.isAir(genX, genY, genZ))
            {
                int height = 1 + rand.nextInt(rand.nextInt(3) + 1);

                for (int h = 0; h < height; ++h)
                {
                    if (Block.Cactus.canGrow(world, genX, genY + h, genZ))
                    {
                        world.SetBlockWithoutNotifyingNeighbors(genX, genY + h, genZ, Block.Cactus.id);
                    }
                }
            }
        }

        return true;
    }
}
