using BetaSharp.Blocks;
using BetaSharp.Blocks.Materials;
using BetaSharp.Util.Maths;

namespace BetaSharp.Worlds.Gen.Features;

public class ClayOreFeature : Feature
{

    private int clayBlockId = Block.Clay.id;
    private int numberOfBlocks;

    public ClayOreFeature(int numberOfBlocks)
    {
        this.numberOfBlocks = numberOfBlocks;
    }

    public override bool generate(World world, java.util.Random rand, int x, int y, int z)
    {
        if (world.getMaterial(x, y, z) != Material.Water)
        {
            return false;
        }
        else
        {
            float var6 = rand.nextFloat() * (float)Math.PI;
            double var7 = (double)(x + 8 + MathHelper.sin(var6) * numberOfBlocks / 8.0F);
            double var9 = (double)(x + 8 - MathHelper.sin(var6) * numberOfBlocks / 8.0F);
            double var11 = (double)(z + 8 + MathHelper.cos(var6) * numberOfBlocks / 8.0F);
            double var13 = (double)(z + 8 - MathHelper.cos(var6) * numberOfBlocks / 8.0F);
            double var15 = y + rand.nextInt(3) + 2;
            double var17 = y + rand.nextInt(3) + 2;

            for (int var19 = 0; var19 <= numberOfBlocks; ++var19)
            {
                double var20 = var7 + (var9 - var7) * var19 / numberOfBlocks;
                double var22 = var15 + (var17 - var15) * var19 / numberOfBlocks;
                double var24 = var11 + (var13 - var11) * var19 / numberOfBlocks;
                double var26 = rand.nextDouble() * numberOfBlocks / 16.0D;
                double var28 = (double)(MathHelper.sin(var19 * (float)Math.PI / numberOfBlocks) + 1.0F) * var26 + 1.0D;
                double var30 = (double)(MathHelper.sin(var19 * (float)Math.PI / numberOfBlocks) + 1.0F) * var26 + 1.0D;
                int var32 = MathHelper.floor_double(var20 - var28 / 2.0D);
                int var33 = MathHelper.floor_double(var20 + var28 / 2.0D);
                int var34 = MathHelper.floor_double(var22 - var30 / 2.0D);
                int var35 = MathHelper.floor_double(var22 + var30 / 2.0D);
                int var36 = MathHelper.floor_double(var24 - var28 / 2.0D);
                int var37 = MathHelper.floor_double(var24 + var28 / 2.0D);

                for (int var38 = var32; var38 <= var33; ++var38)
                {
                    for (int var39 = var34; var39 <= var35; ++var39)
                    {
                        for (int var40 = var36; var40 <= var37; ++var40)
                        {
                            double var41 = (var38 + 0.5D - var20) / (var28 / 2.0D);
                            double var43 = (var39 + 0.5D - var22) / (var30 / 2.0D);
                            double var45 = (var40 + 0.5D - var24) / (var28 / 2.0D);
                            if (var41 * var41 + var43 * var43 + var45 * var45 < 1.0D)
                            {
                                int var47 = world.getBlockId(var38, var39, var40);
                                if (var47 == Block.Sand.id)
                                {
                                    world.SetBlockWithoutNotifyingNeighbors(var38, var39, var40, clayBlockId);
                                }
                            }
                        }
                    }
                }
            }

            return true;
        }
    }
}
