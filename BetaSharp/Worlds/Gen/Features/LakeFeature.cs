using BetaSharp.Blocks;
using BetaSharp.Blocks.Materials;

namespace BetaSharp.Worlds.Gen.Features;

public class LakeFeature : Feature
{

    private int waterBlockId;

    public LakeFeature(int var1)
    {
        waterBlockId = var1;
    }

    public override bool generate(World var1, java.util.Random var2, int var3, int var4, int var5)
    {
        var3 -= 8;

        for (var5 -= 8; var4 > 0 && var1.isAir(var3, var4, var5); --var4)
        {
        }

        var4 -= 4;
        bool[] var6 = new bool[2048];
        int var7 = var2.nextInt(4) + 4;

        int var8;
        for (var8 = 0; var8 < var7; ++var8)
        {
            double var9 = var2.nextDouble() * 6.0D + 3.0D;
            double var11 = var2.nextDouble() * 4.0D + 2.0D;
            double var13 = var2.nextDouble() * 6.0D + 3.0D;
            double var15 = var2.nextDouble() * (16.0D - var9 - 2.0D) + 1.0D + var9 / 2.0D;
            double var17 = var2.nextDouble() * (8.0D - var11 - 4.0D) + 2.0D + var11 / 2.0D;
            double var19 = var2.nextDouble() * (16.0D - var13 - 2.0D) + 1.0D + var13 / 2.0D;

            for (int var21 = 1; var21 < 15; ++var21)
            {
                for (int var22 = 1; var22 < 15; ++var22)
                {
                    for (int var23 = 1; var23 < 7; ++var23)
                    {
                        double var24 = (var21 - var15) / (var9 / 2.0D);
                        double var26 = (var23 - var17) / (var11 / 2.0D);
                        double var28 = (var22 - var19) / (var13 / 2.0D);
                        double var30 = var24 * var24 + var26 * var26 + var28 * var28;
                        if (var30 < 1.0D)
                        {
                            var6[(var21 * 16 + var22) * 8 + var23] = true;
                        }
                    }
                }
            }
        }

        int var10;
        int var32;
        bool var33;
        for (var8 = 0; var8 < 16; ++var8)
        {
            for (var32 = 0; var32 < 16; ++var32)
            {
                for (var10 = 0; var10 < 8; ++var10)
                {
                    var33 = !var6[(var8 * 16 + var32) * 8 + var10] && (var8 < 15 && var6[((var8 + 1) * 16 + var32) * 8 + var10] || var8 > 0 && var6[((var8 - 1) * 16 + var32) * 8 + var10] || var32 < 15 && var6[(var8 * 16 + var32 + 1) * 8 + var10] || var32 > 0 && var6[(var8 * 16 + (var32 - 1)) * 8 + var10] || var10 < 7 && var6[(var8 * 16 + var32) * 8 + var10 + 1] || var10 > 0 && var6[(var8 * 16 + var32) * 8 + (var10 - 1)]);
                    if (var33)
                    {
                        Material var12 = var1.getMaterial(var3 + var8, var4 + var10, var5 + var32);
                        if (var10 >= 4 && var12.IsFluid)
                        {
                            return false;
                        }

                        if (var10 < 4 && !var12.IsSolid && var1.getBlockId(var3 + var8, var4 + var10, var5 + var32) != waterBlockId)
                        {
                            return false;
                        }
                    }
                }
            }
        }

        for (var8 = 0; var8 < 16; ++var8)
        {
            for (var32 = 0; var32 < 16; ++var32)
            {
                for (var10 = 0; var10 < 8; ++var10)
                {
                    if (var6[(var8 * 16 + var32) * 8 + var10])
                    {
                        var1.setBlockWithoutNotifyingNeighbors(var3 + var8, var4 + var10, var5 + var32, var10 >= 4 ? 0 : waterBlockId);
                    }
                }
            }
        }

        for (var8 = 0; var8 < 16; ++var8)
        {
            for (var32 = 0; var32 < 16; ++var32)
            {
                for (var10 = 4; var10 < 8; ++var10)
                {
                    if (var6[(var8 * 16 + var32) * 8 + var10] && var1.getBlockId(var3 + var8, var4 + var10 - 1, var5 + var32) == Block.DIRT.id && var1.getBrightness(LightType.Sky, var3 + var8, var4 + var10, var5 + var32) > 0)
                    {
                        var1.setBlockWithoutNotifyingNeighbors(var3 + var8, var4 + var10 - 1, var5 + var32, Block.GRASS_BLOCK.id);
                    }
                }
            }
        }

        if (Block.BLOCKS[waterBlockId].material == Material.Lava)
        {
            for (var8 = 0; var8 < 16; ++var8)
            {
                for (var32 = 0; var32 < 16; ++var32)
                {
                    for (var10 = 0; var10 < 8; ++var10)
                    {
                        var33 = !var6[(var8 * 16 + var32) * 8 + var10] && (var8 < 15 && var6[((var8 + 1) * 16 + var32) * 8 + var10] || var8 > 0 && var6[((var8 - 1) * 16 + var32) * 8 + var10] || var32 < 15 && var6[(var8 * 16 + var32 + 1) * 8 + var10] || var32 > 0 && var6[(var8 * 16 + (var32 - 1)) * 8 + var10] || var10 < 7 && var6[(var8 * 16 + var32) * 8 + var10 + 1] || var10 > 0 && var6[(var8 * 16 + var32) * 8 + (var10 - 1)]);
                        if (var33 && (var10 < 4 || var2.nextInt(2) != 0) && var1.getMaterial(var3 + var8, var4 + var10, var5 + var32).IsSolid)
                        {
                            var1.setBlockWithoutNotifyingNeighbors(var3 + var8, var4 + var10, var5 + var32, Block.STONE.id);
                        }
                    }
                }
            }
        }

        return true;
    }
}