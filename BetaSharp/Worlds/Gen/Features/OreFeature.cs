using BetaSharp.Blocks;
using BetaSharp.Util.Maths;

namespace BetaSharp.Worlds.Gen.Features;

public class OreFeature : Feature
{

    private int minableBlockId;
    private int numberOfBlocks;

    public OreFeature(int var1, int var2)
    {
        minableBlockId = var1;
        numberOfBlocks = var2;
    }

    public override bool generate(World var1, java.util.Random var2, int var3, int var4, int var5)
    {
        float var6 = var2.nextFloat() * (float)Math.PI;
        double var7 = (double)(var3 + 8 + MathHelper.sin(var6) * numberOfBlocks / 8.0F);
        double var9 = (double)(var3 + 8 - MathHelper.sin(var6) * numberOfBlocks / 8.0F);
        double var11 = (double)(var5 + 8 + MathHelper.cos(var6) * numberOfBlocks / 8.0F);
        double var13 = (double)(var5 + 8 - MathHelper.cos(var6) * numberOfBlocks / 8.0F);
        double var15 = var4 + var2.nextInt(3) + 2;
        double var17 = var4 + var2.nextInt(3) + 2;

        for (int var19 = 0; var19 <= numberOfBlocks; ++var19)
        {
            double var20 = var7 + (var9 - var7) * var19 / numberOfBlocks;
            double var22 = var15 + (var17 - var15) * var19 / numberOfBlocks;
            double var24 = var11 + (var13 - var11) * var19 / numberOfBlocks;
            double var26 = var2.nextDouble() * numberOfBlocks / 16.0D;
            double var28 = (double)(MathHelper.sin(var19 * (float)Math.PI / numberOfBlocks) + 1.0F) * var26 + 1.0D;
            double var30 = (double)(MathHelper.sin(var19 * (float)Math.PI / numberOfBlocks) + 1.0F) * var26 + 1.0D;
            int var32 = MathHelper.floor_double(var20 - var28 / 2.0D);
            int var33 = MathHelper.floor_double(var22 - var30 / 2.0D);
            int var34 = MathHelper.floor_double(var24 - var28 / 2.0D);
            int var35 = MathHelper.floor_double(var20 + var28 / 2.0D);
            int var36 = MathHelper.floor_double(var22 + var30 / 2.0D);
            int var37 = MathHelper.floor_double(var24 + var28 / 2.0D);

            for (int var38 = var32; var38 <= var35; ++var38)
            {
                double var39 = (var38 + 0.5D - var20) / (var28 / 2.0D);
                if (var39 * var39 < 1.0D)
                {
                    for (int var41 = var33; var41 <= var36; ++var41)
                    {
                        double var42 = (var41 + 0.5D - var22) / (var30 / 2.0D);
                        if (var39 * var39 + var42 * var42 < 1.0D)
                        {
                            for (int var44 = var34; var44 <= var37; ++var44)
                            {
                                double var45 = (var44 + 0.5D - var24) / (var28 / 2.0D);
                                if (var39 * var39 + var42 * var42 + var45 * var45 < 1.0D && var1.getBlockId(var38, var41, var44) == Block.STONE.id)
                                {
                                    var1.setBlockWithoutNotifyingNeighbors(var38, var41, var44, minableBlockId);
                                }
                            }
                        }
                    }
                }
            }
        }

        return true;
    }
}