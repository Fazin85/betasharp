using BetaSharp.Blocks;

namespace BetaSharp.Worlds.Gen.Features;

public class PineTreeFeature : Feature
{

    public override bool generate(World world, java.util.Random rand, int x, int y, int z)
    {
        int var6 = rand.nextInt(5) + 7;
        int var7 = var6 - rand.nextInt(2) - 3;
        int var8 = var6 - var7;
        int var9 = 1 + rand.nextInt(var8 + 1);
        bool var10 = true;
        if (y >= 1 && y + var6 + 1 <= 128)
        {
            int var11;
            int var13;
            int var14;
            int var15;
            int var18;
            for (var11 = y; var11 <= y + 1 + var6 && var10; ++var11)
            {
                bool var12 = true;
                if (var11 - y < var7)
                {
                    var18 = 0;
                }
                else
                {
                    var18 = var9;
                }

                for (var13 = x - var18; var13 <= x + var18 && var10; ++var13)
                {
                    for (var14 = z - var18; var14 <= z + var18 && var10; ++var14)
                    {
                        if (var11 >= 0 && var11 < 128)
                        {
                            var15 = world.getBlockId(var13, var11, var14);
                            if (var15 != 0 && var15 != Block.Leaves.id)
                            {
                                var10 = false;
                            }
                        }
                        else
                        {
                            var10 = false;
                        }
                    }
                }
            }

            if (!var10)
            {
                return false;
            }
            else
            {
                var11 = world.getBlockId(x, y - 1, z);
                if ((var11 == Block.GrassBlock.id || var11 == Block.Dirt.id) && y < 128 - var6 - 1)
                {
                    world.setBlockWithoutNotifyingNeighbors(x, y - 1, z, Block.Dirt.id);
                    var18 = 0;

                    for (var13 = y + var6; var13 >= y + var7; --var13)
                    {
                        for (var14 = x - var18; var14 <= x + var18; ++var14)
                        {
                            var15 = var14 - x;

                            for (int var16 = z - var18; var16 <= z + var18; ++var16)
                            {
                                int var17 = var16 - z;
                                if ((java.lang.Math.abs(var15) != var18 || java.lang.Math.abs(var17) != var18 || var18 <= 0) && !Block.BlocksOpaque[world.getBlockId(var14, var13, var16)])
                                {
                                    world.setBlockWithoutNotifyingNeighbors(var14, var13, var16, Block.Leaves.id, 1);
                                }
                            }
                        }

                        if (var18 >= 1 && var13 == y + var7 + 1)
                        {
                            --var18;
                        }
                        else if (var18 < var9)
                        {
                            ++var18;
                        }
                    }

                    for (var13 = 0; var13 < var6 - 1; ++var13)
                    {
                        var14 = world.getBlockId(x, y + var13, z);
                        if (var14 == 0 || var14 == Block.Leaves.id)
                        {
                            world.setBlockWithoutNotifyingNeighbors(x, y + var13, z, Block.Log.id, 1);
                        }
                    }

                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        else
        {
            return false;
        }
    }
}