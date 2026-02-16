using BetaSharp.Blocks;

namespace BetaSharp.Worlds.Gen.Features;

public class GrassPatchFeature : Feature
{

    private int tallGrassBlockId;
    private int tallGrassBlockMeta;

    public GrassPatchFeature(int var1, int var2)
    {
        tallGrassBlockId = var1;
        tallGrassBlockMeta = var2;
    }

    public override bool generate(World world, java.util.Random rand, int x, int y, int z)
    {
        bool var6 = false;

        while (true)
        {
            int var11 = world.getBlockId(x, y, z);
            if (var11 != 0 && var11 != Block.Leaves.id || y <= 0)
            {
                for (int var7 = 0; var7 < 128; ++var7)
                {
                    int var8 = x + rand.nextInt(8) - rand.nextInt(8);
                    int var9 = y + rand.nextInt(4) - rand.nextInt(4);
                    int var10 = z + rand.nextInt(8) - rand.nextInt(8);
                    if (world.isAir(var8, var9, var10) && ((BlockPlant)Block.Blocks[tallGrassBlockId]).canGrow(world, var8, var9, var10))
                    {
                        world.setBlockWithoutNotifyingNeighbors(var8, var9, var10, tallGrassBlockId, tallGrassBlockMeta);
                    }
                }

                return true;
            }

            --y;
        }
    }
}