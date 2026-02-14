using BetaSharp.Blocks;
using BetaSharp.Blocks.Materials;

namespace BetaSharp.Worlds.Gen.Features;

public class SugarCanePatchFeature : Feature
{

    public override bool generate(World var1, java.util.Random var2, int var3, int var4, int var5)
    {
        for (int var6 = 0; var6 < 20; ++var6)
        {
            int var7 = var3 + var2.nextInt(4) - var2.nextInt(4);
            int var8 = var4;
            int var9 = var5 + var2.nextInt(4) - var2.nextInt(4);
            if (var1.isAir(var7, var4, var9) && (var1.getMaterial(var7 - 1, var4 - 1, var9) == Material.Water || var1.getMaterial(var7 + 1, var4 - 1, var9) == Material.Water || var1.getMaterial(var7, var4 - 1, var9 - 1) == Material.Water || var1.getMaterial(var7, var4 - 1, var9 + 1) == Material.Water))
            {
                int var10 = 2 + var2.nextInt(var2.nextInt(3) + 1);

                for (int var11 = 0; var11 < var10; ++var11)
                {
                    if (Block.SUGAR_CANE.canGrow(var1, var7, var8 + var11, var9))
                    {
                        var1.setBlockWithoutNotifyingNeighbors(var7, var8 + var11, var9, Block.SUGAR_CANE.id);
                    }
                }
            }
        }

        return true;
    }
}