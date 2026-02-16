using BetaSharp.Blocks;

namespace BetaSharp.Worlds.Gen.Features;

public class PineTreeFeature : Feature
{

    public override bool generate(World var1, java.util.Random var2, int var3, int var4, int var5)
    {
        int var6 = var2.nextInt(5) + 7;
        int var7 = var6 - var2.nextInt(2) - 3;
        int var8 = var6 - var7;
        int var9 = 1 + var2.nextInt(var8 + 1);
        bool var10 = true;
        if (var4 >= 1 && var4 + var6 + 1 <= 128)
        {
            int var11;
            int var13;
            int var14;
            int var15;
            int var18;
            for (var11 = var4; var11 <= var4 + 1 + var6 && var10; ++var11)
            {
                bool var12 = true;
                if (var11 - var4 < var7)
                {
                    var18 = 0;
                }
                else
                {
                    var18 = var9;
                }

                for (var13 = var3 - var18; var13 <= var3 + var18 && var10; ++var13)
                {
                    for (var14 = var5 - var18; var14 <= var5 + var18 && var10; ++var14)
                    {
                        if (var11 >= 0 && var11 < 128)
                        {
                            var15 = var1.getBlockId(var13, var11, var14);
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
                var11 = var1.getBlockId(var3, var4 - 1, var5);
                if ((var11 == Block.GrassBlock.id || var11 == Block.Dirt.id) && var4 < 128 - var6 - 1)
                {
                    var1.SetBlockWithoutNotifyingNeighbors(var3, var4 - 1, var5, Block.Dirt.id);
                    var18 = 0;

                    for (var13 = var4 + var6; var13 >= var4 + var7; --var13)
                    {
                        for (var14 = var3 - var18; var14 <= var3 + var18; ++var14)
                        {
                            var15 = var14 - var3;

                            for (int var16 = var5 - var18; var16 <= var5 + var18; ++var16)
                            {
                                int var17 = var16 - var5;
                                if ((java.lang.Math.abs(var15) != var18 || java.lang.Math.abs(var17) != var18 || var18 <= 0) && !Block.BlocksOpaque[var1.getBlockId(var14, var13, var16)])
                                {
                                    var1.SetBlockWithoutNotifyingNeighbors(var14, var13, var16, Block.Leaves.id, 1);
                                }
                            }
                        }

                        if (var18 >= 1 && var13 == var4 + var7 + 1)
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
                        var14 = var1.getBlockId(var3, var4 + var13, var5);
                        if (var14 == 0 || var14 == Block.Leaves.id)
                        {
                            var1.SetBlockWithoutNotifyingNeighbors(var3, var4 + var13, var5, Block.Log.id, 1);
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