using BetaSharp.Blocks;

namespace BetaSharp.Worlds.Gen.Features;

public class BirchTreeFeature : Feature
{

    public override bool generate(World world, java.util.Random rand, int x, int y, int z)
    {
        int var6 = rand.nextInt(3) + 5;
        bool var7 = true;
        if (y >= 1 && y + var6 + 1 <= 128)
        {
            int var8;
            int var10;
            int var11;
            int var12;
            for (var8 = y; var8 <= y + 1 + var6; ++var8)
            {
                byte var9 = 1;
                if (var8 == y)
                {
                    var9 = 0;
                }

                if (var8 >= y + 1 + var6 - 2)
                {
                    var9 = 2;
                }

                for (var10 = x - var9; var10 <= x + var9 && var7; ++var10)
                {
                    for (var11 = z - var9; var11 <= z + var9 && var7; ++var11)
                    {
                        if (var8 >= 0 && var8 < 128)
                        {
                            var12 = world.getBlockId(var10, var8, var11);
                            if (var12 != 0 && var12 != Block.Leaves.id)
                            {
                                var7 = false;
                            }
                        }
                        else
                        {
                            var7 = false;
                        }
                    }
                }
            }

            if (!var7)
            {
                return false;
            }
            else
            {
                var8 = world.getBlockId(x, y - 1, z);
                if ((var8 == Block.GrassBlock.id || var8 == Block.Dirt.id) && y < 128 - var6 - 1)
                {
                    world.setBlockWithoutNotifyingNeighbors(x, y - 1, z, Block.Dirt.id);

                    int var16;
                    for (var16 = y - 3 + var6; var16 <= y + var6; ++var16)
                    {
                        var10 = var16 - (y + var6);
                        var11 = 1 - var10 / 2;

                        for (var12 = x - var11; var12 <= x + var11; ++var12)
                        {
                            int var13 = var12 - x;

                            for (int var14 = z - var11; var14 <= z + var11; ++var14)
                            {
                                int var15 = var14 - z;
                                if ((java.lang.Math.abs(var13) != var11 || java.lang.Math.abs(var15) != var11 || rand.nextInt(2) != 0 && var10 != 0) && !Block.BlocksOpaque[world.getBlockId(var12, var16, var14)])
                                {
                                    world.setBlockWithoutNotifyingNeighbors(var12, var16, var14, Block.Leaves.id, 2);
                                }
                            }
                        }
                    }

                    for (var16 = 0; var16 < var6; ++var16)
                    {
                        var10 = world.getBlockId(x, y + var16, z);
                        if (var10 == 0 || var10 == Block.Leaves.id)
                        {
                            world.setBlockWithoutNotifyingNeighbors(x, y + var16, z, Block.Log.id, 2);
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