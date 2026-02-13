using BetaSharp.Blocks;

namespace BetaSharp.Worlds.Gen.Features;

public class DeadBushPatchFeature : Feature
{

    private int deadBushBlockId;

    public DeadBushPatchFeature(int var1)
    {
        deadBushBlockId = var1;
    }

    public override bool generate(World var1, java.util.Random var2, int var3, int var4, int var5)
    {
        bool var6 = false;

        while (true)
        {
            int var11 = var1.getBlockId(var3, var4, var5);
            if (var11 != 0 && var11 != Block.LEAVES.id || var4 <= 0)
            {
                for (int var7 = 0; var7 < 4; ++var7)
                {
                    int var8 = var3 + var2.nextInt(8) - var2.nextInt(8);
                    int var9 = var4 + var2.nextInt(4) - var2.nextInt(4);
                    int var10 = var5 + var2.nextInt(8) - var2.nextInt(8);
                    if (var1.isAir(var8, var9, var10) && ((BlockPlant)Block.BLOCKS[deadBushBlockId]).canGrow(var1, var8, var9, var10))
                    {
                        var1.setBlockWithoutNotifyingNeighbors(var8, var9, var10, deadBushBlockId);
                    }
                }

                return true;
            }

            --var4;
        }
    }
}