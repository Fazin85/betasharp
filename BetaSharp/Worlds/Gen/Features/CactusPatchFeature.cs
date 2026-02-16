using BetaSharp.Blocks;

namespace BetaSharp.Worlds.Gen.Features;

public class CactusPatchFeature : Feature
{

    public override bool generate(World world, java.util.Random rand, int x, int y, int z)
    {
        for (int var6 = 0; var6 < 10; ++var6)
        {
            int var7 = x + rand.nextInt(8) - rand.nextInt(8);
            int var8 = y + rand.nextInt(4) - rand.nextInt(4);
            int var9 = z + rand.nextInt(8) - rand.nextInt(8);
            if (world.isAir(var7, var8, var9))
            {
                int var10 = 1 + rand.nextInt(rand.nextInt(3) + 1);

                for (int var11 = 0; var11 < var10; ++var11)
                {
                    if (Block.Cactus.canGrow(world, var7, var8 + var11, var9))
                    {
                        world.setBlockWithoutNotifyingNeighbors(var7, var8 + var11, var9, Block.Cactus.id);
                    }
                }
            }
        }

        return true;
    }
}