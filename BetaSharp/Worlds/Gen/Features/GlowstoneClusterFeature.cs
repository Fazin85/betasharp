using BetaSharp.Blocks;

namespace BetaSharp.Worlds.Gen.Features;

public class GlowstoneClusterFeature : Feature
{

    public override bool generate(World world, java.util.Random rand, int x, int y, int z)
    {
        if (!world.isAir(x, y, z))
        {
            return false;
        }
        else if (world.getBlockId(x, y + 1, z) != Block.Netherrack.id)
        {
            return false;
        }
        else
        {
            world.setBlock(x, y, z, Block.Glowstone.id);

            for (int var6 = 0; var6 < 1500; ++var6)
            {
                int var7 = x + rand.nextInt(8) - rand.nextInt(8);
                int var8 = y - rand.nextInt(12);
                int var9 = z + rand.nextInt(8) - rand.nextInt(8);
                if (world.getBlockId(var7, var8, var9) == 0)
                {
                    int var10 = 0;

                    for (int var11 = 0; var11 < 6; ++var11)
                    {
                        int var12 = 0;
                        if (var11 == 0)
                        {
                            var12 = world.getBlockId(var7 - 1, var8, var9);
                        }

                        if (var11 == 1)
                        {
                            var12 = world.getBlockId(var7 + 1, var8, var9);
                        }

                        if (var11 == 2)
                        {
                            var12 = world.getBlockId(var7, var8 - 1, var9);
                        }

                        if (var11 == 3)
                        {
                            var12 = world.getBlockId(var7, var8 + 1, var9);
                        }

                        if (var11 == 4)
                        {
                            var12 = world.getBlockId(var7, var8, var9 - 1);
                        }

                        if (var11 == 5)
                        {
                            var12 = world.getBlockId(var7, var8, var9 + 1);
                        }

                        if (var12 == Block.Glowstone.id)
                        {
                            ++var10;
                        }
                    }

                    if (var10 == 1)
                    {
                        world.setBlock(var7, var8, var9, Block.Glowstone.id);
                    }
                }
            }

            return true;
        }
    }
}