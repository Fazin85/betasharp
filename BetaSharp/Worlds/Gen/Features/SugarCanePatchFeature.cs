using BetaSharp.Blocks;
using BetaSharp.Blocks.Materials;

namespace BetaSharp.Worlds.Gen.Features;

public class SugarCanePatchFeature : Feature
{

    public override bool generate(World world, java.util.Random rand, int x, int y, int z)
    {
        for (int var6 = 0; var6 < 20; ++var6)
        {
            int var7 = x + rand.nextInt(4) - rand.nextInt(4);
            int var8 = y;
            int var9 = z + rand.nextInt(4) - rand.nextInt(4);
            if (world.isAir(var7, y, var9) && (world.getMaterial(var7 - 1, y - 1, var9) == Material.Water || world.getMaterial(var7 + 1, y - 1, var9) == Material.Water || world.getMaterial(var7, y - 1, var9 - 1) == Material.Water || world.getMaterial(var7, y - 1, var9 + 1) == Material.Water))
            {
                int var10 = 2 + rand.nextInt(rand.nextInt(3) + 1);

                for (int var11 = 0; var11 < var10; ++var11)
                {
                    if (Block.SugarCane.canGrow(world, var7, var8 + var11, var9))
                    {
                        world.SetBlockWithoutNotifyingNeighbors(var7, var8 + var11, var9, Block.SugarCane.id);
                    }
                }
            }
        }

        return true;
    }
}
