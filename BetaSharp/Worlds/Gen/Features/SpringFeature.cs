using BetaSharp.Blocks;

namespace BetaSharp.Worlds.Gen.Features;

public class SpringFeature : Feature
{

    private int liquidBlockId;

    public SpringFeature(int var1)
    {
        liquidBlockId = var1;
    }

    public override bool generate(World world, java.util.Random rand, int x, int y, int z)
    {
        if (world.getBlockId(x, y + 1, z) != Block.Stone.id)
        {
            return false;
        }
        else if (world.getBlockId(x, y - 1, z) != Block.Stone.id)
        {
            return false;
        }
        else if (world.getBlockId(x, y, z) != 0 && world.getBlockId(x, y, z) != Block.Stone.id)
        {
            return false;
        }
        else
        {
            int var6 = 0;
            if (world.getBlockId(x - 1, y, z) == Block.Stone.id)
            {
                ++var6;
            }

            if (world.getBlockId(x + 1, y, z) == Block.Stone.id)
            {
                ++var6;
            }

            if (world.getBlockId(x, y, z - 1) == Block.Stone.id)
            {
                ++var6;
            }

            if (world.getBlockId(x, y, z + 1) == Block.Stone.id)
            {
                ++var6;
            }

            int var7 = 0;
            if (world.isAir(x - 1, y, z))
            {
                ++var7;
            }

            if (world.isAir(x + 1, y, z))
            {
                ++var7;
            }

            if (world.isAir(x, y, z - 1))
            {
                ++var7;
            }

            if (world.isAir(x, y, z + 1))
            {
                ++var7;
            }

            if (var6 == 3 && var7 == 1)
            {
                world.setBlock(x, y, z, liquidBlockId);
                world.instantBlockUpdateEnabled = true;
                Block.Blocks[liquidBlockId].onTick(world, x, y, z, rand);
                world.instantBlockUpdateEnabled = false;
            }

            return true;
        }
    }
}