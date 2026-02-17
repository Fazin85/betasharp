using BetaSharp.Blocks;
using BetaSharp.Blocks.Materials;

namespace BetaSharp.Worlds.Gen.Features;

public class SugarCanePatchFeature : Feature
{

    public override bool Generate(World world, java.util.Random rand, int x, int y, int z)
    {
        for (int i = 0; i < 20; ++i)
        {
            int genX = x + rand.nextInt(4) - rand.nextInt(4);
            int genZ = z + rand.nextInt(4) - rand.nextInt(4);

            if (!world.isAir(genX, y, genZ)) continue;

            bool hasWaterNearby = world.getMaterial(genX - 1, y - 1, genZ) == Material.Water ||
                world.getMaterial(genX + 1, y - 1, genZ) == Material.Water ||
                world.getMaterial(genX, y - 1, genZ - 1) == Material.Water ||
                world.getMaterial(genX, y - 1, genZ + 1) == Material.Water;

            if (hasWaterNearby)
            {
                int height = 2 + rand.nextInt(rand.nextInt(3) + 1);

                for (int h = 0; h < height; ++h)
                {
                    if (Block.SugarCane.canGrow(world, genX, y + h, genZ))
                    {
                        world.SetBlockWithoutNotifyingNeighbors(genX, y + h, genZ, Block.SugarCane.id);
                    }
                }
            }
        }

        return true;
    }
}
