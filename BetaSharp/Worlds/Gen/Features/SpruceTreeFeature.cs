using BetaSharp.Blocks;

namespace BetaSharp.Worlds.Gen.Features;

public class SpruceTreeFeature : Feature
{

    public override bool generate(World world, java.util.Random rand, int x, int y, int z)
    {
        int var6 = rand.nextInt(4) + 6;
        int var7 = 1 + rand.nextInt(2);
        int var8 = var6 - var7;
        int var9 = 2 + rand.nextInt(2);
        bool var10 = true;
        if (y >= 1 && y + var6 + 1 <= 128)
        {
            int var11;
            int var13;
            int var15;
            int var21;
            for (var11 = y; var11 <= y + 1 + var6 && var10; ++var11)
            {
                bool var12 = true;
                if (var11 - y < var7)
                {
                    var21 = 0;
                }
                else
                {
                    var21 = var9;
                }

                for (var13 = x - var21; var13 <= x + var21 && var10; ++var13)
                {
                    for (int var14 = z - var21; var14 <= z + var21 && var10; ++var14)
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
                    world.SetBlockWithoutNotifyingNeighbors(x, y - 1, z, Block.Dirt.id);
                    var21 = rand.nextInt(2);
                    var13 = 1;
                    byte var22 = 0;

                    int var16;
                    int var17;
                    for (var15 = 0; var15 <= var8; ++var15)
                    {
                        var16 = y + var6 - var15;

                        for (var17 = x - var21; var17 <= x + var21; ++var17)
                        {
                            int var18 = var17 - x;

                            for (int var19 = z - var21; var19 <= z + var21; ++var19)
                            {
                                int var20 = var19 - z;
                                if ((java.lang.Math.abs(var18) != var21 || java.lang.Math.abs(var20) != var21 || var21 <= 0) && !Block.BlocksOpaque[world.getBlockId(var17, var16, var19)])
                                {
                                    world.SetBlockWithoutNotifyingNeighbors(var17, var16, var19, Block.Leaves.id, 1);
                                }
                            }
                        }

                        if (var21 >= var13)
                        {
                            var21 = var22;
                            var22 = 1;
                            ++var13;
                            if (var13 > var9)
                            {
                                var13 = var9;
                            }
                        }
                        else
                        {
                            ++var21;
                        }
                    }

                    var15 = rand.nextInt(3);

                    for (var16 = 0; var16 < var6 - var15; ++var16)
                    {
                        var17 = world.getBlockId(x, y + var16, z);
                        if (var17 == 0 || var17 == Block.Leaves.id)
                        {
                            world.SetBlockWithoutNotifyingNeighbors(x, y + var16, z, Block.Log.id, 1);
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
