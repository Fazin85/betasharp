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

    public override bool generate(World world, java.util.Random rand, int x, int y, int z)
    {
        x -= 8;

        for (z -= 8; y > 0 && world.isAir(x, y, z); --y)
        {
        }

        y -= 4;
        bool[] var6 = new bool[2048];
        int var7 = rand.nextInt(4) + 4;

        int var8;
        for (var8 = 0; var8 < var7; ++var8)
        {
            double var9 = rand.nextDouble() * 6.0D + 3.0D;
            double var11 = rand.nextDouble() * 4.0D + 2.0D;
            double var13 = rand.nextDouble() * 6.0D + 3.0D;
            double var15 = rand.nextDouble() * (16.0D - var9 - 2.0D) + 1.0D + var9 / 2.0D;
            double var17 = rand.nextDouble() * (8.0D - var11 - 4.0D) + 2.0D + var11 / 2.0D;
            double var19 = rand.nextDouble() * (16.0D - var13 - 2.0D) + 1.0D + var13 / 2.0D;

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
                        Material var12 = world.getMaterial(x + var8, y + var10, z + var32);
                        if (var10 >= 4 && var12.IsFluid)
                        {
                            return false;
                        }

                        if (var10 < 4 && !var12.IsSolid && world.getBlockId(x + var8, y + var10, z + var32) != waterBlockId)
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
                        world.setBlockWithoutNotifyingNeighbors(x + var8, y + var10, z + var32, var10 >= 4 ? 0 : waterBlockId);
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
                    if (var6[(var8 * 16 + var32) * 8 + var10] && world.getBlockId(x + var8, y + var10 - 1, z + var32) == Block.Dirt.id && world.getBrightness(LightType.Sky, x + var8, y + var10, z + var32) > 0)
                    {
                        world.setBlockWithoutNotifyingNeighbors(x + var8, y + var10 - 1, z + var32, Block.GrassBlock.id);
                    }
                }
            }
        }

        if (Block.Blocks[waterBlockId].material == Material.Lava)
        {
            for (var8 = 0; var8 < 16; ++var8)
            {
                for (var32 = 0; var32 < 16; ++var32)
                {
                    for (var10 = 0; var10 < 8; ++var10)
                    {
                        var33 = !var6[(var8 * 16 + var32) * 8 + var10] && (var8 < 15 && var6[((var8 + 1) * 16 + var32) * 8 + var10] || var8 > 0 && var6[((var8 - 1) * 16 + var32) * 8 + var10] || var32 < 15 && var6[(var8 * 16 + var32 + 1) * 8 + var10] || var32 > 0 && var6[(var8 * 16 + (var32 - 1)) * 8 + var10] || var10 < 7 && var6[(var8 * 16 + var32) * 8 + var10 + 1] || var10 > 0 && var6[(var8 * 16 + var32) * 8 + (var10 - 1)]);
                        if (var33 && (var10 < 4 || rand.nextInt(2) != 0) && world.getMaterial(x + var8, y + var10, z + var32).IsSolid)
                        {
                            world.setBlockWithoutNotifyingNeighbors(x + var8, y + var10, z + var32, Block.Stone.id);
                        }
                    }
                }
            }
        }

        return true;
    }
}