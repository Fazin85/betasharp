using BetaSharp.Blocks;

namespace BetaSharp.Worlds.Gen.Features;

public class NetherFirePatchFeature : Feature
{

    public override bool generate(World var1, java.util.Random var2, int var3, int var4, int var5)
    {
        for (int var6 = 0; var6 < 64; ++var6)
        {
            int var7 = var3 + var2.nextInt(8) - var2.nextInt(8);
            int var8 = var4 + var2.nextInt(4) - var2.nextInt(4);
            int var9 = var5 + var2.nextInt(8) - var2.nextInt(8);
            if (var1.isAir(var7, var8, var9) && var1.getBlockId(var7, var8 - 1, var9) == Block.NETHERRACK.id)
            {
                var1.setBlock(var7, var8, var9, Block.FIRE.id);
            }
        }

        return true;
    }
}