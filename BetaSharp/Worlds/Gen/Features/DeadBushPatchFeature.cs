using BetaSharp.Blocks;

namespace BetaSharp.Worlds.Gen.Features;

public class DeadBushPatchFeature : Feature
{

    private int deadBushBlockId;

    public DeadBushPatchFeature(int deadBushBlockId)
    {
        this.deadBushBlockId = deadBushBlockId;
    }

    public override bool generate(World world, java.util.Random rand, int x, int y, int z)
    {
        bool var6 = false;

        while (true)
        {
            int var11 = world.getBlockId(x, y, z);
            if (var11 != 0 && var11 != Block.Leaves.id || y <= 0)
            {
                for (int var7 = 0; var7 < 4; ++var7)
                {
                    int var8 = x + rand.nextInt(8) - rand.nextInt(8);
                    int var9 = y + rand.nextInt(4) - rand.nextInt(4);
                    int var10 = z + rand.nextInt(8) - rand.nextInt(8);
                    if (world.isAir(var8, var9, var10) && ((BlockPlant)Block.Blocks[deadBushBlockId]).canGrow(world, var8, var9, var10))
                    {
                        world.SetBlockWithoutNotifyingNeighbors(var8, var9, var10, deadBushBlockId);
                    }
                }

                return true;
            }

            --y;
        }
    }
}
