using BetaSharp.Blocks;

namespace BetaSharp.Worlds.Gen.Features;

public class PumpkinPatchFeature : Feature
{

    public override bool generate(World world, java.util.Random rand, int x, int y, int z)
    {
        for (int var6 = 0; var6 < 64; ++var6)
        {
            int var7 = x + rand.nextInt(8) - rand.nextInt(8);
            int var8 = y + rand.nextInt(4) - rand.nextInt(4);
            int var9 = z + rand.nextInt(8) - rand.nextInt(8);
            if (world.isAir(var7, var8, var9) && world.getBlockId(var7, var8 - 1, var9) == Block.GrassBlock.id && Block.Pumpkin.canPlaceAt(world, var7, var8, var9))
            {
                world.setBlockWithoutNotifyingNeighbors(var7, var8, var9, Block.Pumpkin.id, rand.nextInt(4));
            }
        }

        return true;
    }
}