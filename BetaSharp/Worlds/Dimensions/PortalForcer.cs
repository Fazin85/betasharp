using BetaSharp.Blocks;
using BetaSharp.Entities;
using BetaSharp.Util.Maths;

namespace BetaSharp.Worlds.Dimensions;

public class PortalForcer
{
    private JavaRandom random = new();

    public void moveToPortal(World world, Entity entity)
    {
        if (!teleportToValidPort(world, entity))
        {
            createPortal(world, entity);
            teleportToValidPort(world, entity);
        }
    }

    public bool teleportToValidPort(World world, Entity entity)
    {
        short var3 = 128;
        double var4 = -1.0D;
        int var6 = 0;
        int var7 = 0;
        int var8 = 0;
        int var9 = MathHelper.Floor(entity.x);
        int var10 = MathHelper.Floor(entity.z);

        double var18;
        for (int var11 = var9 - var3; var11 <= var9 + var3; ++var11)
        {
            double var12 = var11 + 0.5D - entity.x;

            for (int var14 = var10 - var3; var14 <= var10 + var3; ++var14)
            {
                double var15 = var14 + 0.5D - entity.z;

                for (int var17 = 127; var17 >= 0; --var17)
                {
                    if (world.getBlockId(var11, var17, var14) == Block.NetherPortal.id)
                    {
                        while (world.getBlockId(var11, var17 - 1, var14) == Block.NetherPortal.id)
                        {
                            --var17;
                        }

                        var18 = var17 + 0.5D - entity.y;
                        double var20 = var12 * var12 + var18 * var18 + var15 * var15;
                        if (var4 < 0.0D || var20 < var4)
                        {
                            var4 = var20;
                            var6 = var11;
                            var7 = var17;
                            var8 = var14;
                        }
                    }
                }
            }
        }

        if (var4 >= 0.0D)
        {
            double var22 = var6 + 0.5D;
            double var16 = var7 + 0.5D;
            var18 = var8 + 0.5D;
            if (world.getBlockId(var6 - 1, var7, var8) == Block.NetherPortal.id)
            {
                var22 -= 0.5D;
            }

            if (world.getBlockId(var6 + 1, var7, var8) == Block.NetherPortal.id)
            {
                var22 += 0.5D;
            }

            if (world.getBlockId(var6, var7, var8 - 1) == Block.NetherPortal.id)
            {
                var18 -= 0.5D;
            }

            if (world.getBlockId(var6, var7, var8 + 1) == Block.NetherPortal.id)
            {
                var18 += 0.5D;
            }

            entity.setPositionAndAnglesKeepPrevAngles(var22, var16, var18, entity.yaw, 0.0F);
            entity.velocityX = entity.velocityY = entity.velocityZ = 0.0D;
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool createPortal(World world, Entity entity)
    {
        byte var3 = 16;
        double var4 = -1.0D;
        int var6 = MathHelper.Floor(entity.x);
        int var7 = MathHelper.Floor(entity.y);
        int var8 = MathHelper.Floor(entity.z);
        int var9 = var6;
        int var10 = var7;
        int var11 = var8;
        int var12 = 0;
        int var13 = random.NextInt(4);

        int var14;
        double var15;
        int var17;
        double var18;
        int var20;
        int var21;
        int var22;
        int var23;
        int var24;
        int var25;
        int var26;
        int var27;
        int var28;
        double var32;
        double var33;
        for (var14 = var6 - var3; var14 <= var6 + var3; ++var14)
        {
            var15 = var14 + 0.5D - entity.x;

            for (var17 = var8 - var3; var17 <= var8 + var3; ++var17)
            {
                var18 = var17 + 0.5D - entity.z;

                for (var20 = 127; var20 >= 0; --var20)
                {
                    if (world.isAir(var14, var20, var17))
                    {
                        while (var20 > 0 && world.isAir(var14, var20 - 1, var17))
                        {
                            --var20;
                        }

                        for (var21 = var13; var21 < var13 + 4; ++var21)
                        {
                            var22 = var21 % 2;
                            var23 = 1 - var22;
                            if (var21 % 4 >= 2)
                            {
                                var22 = -var22;
                                var23 = -var23;
                            }

                            bool validLocation = true;
                            for (var24 = 0; var24 < 3 && validLocation; ++var24)
                            {
                                for (var25 = 0; var25 < 4 && validLocation; ++var25)
                                {
                                    for (var26 = -1; var26 < 4 && validLocation; ++var26)
                                    {
                                        var27 = var14 + (var25 - 1) * var22 + var24 * var23;
                                        var28 = var20 + var26;
                                        int var29 = var17 + (var25 - 1) * var23 - var24 * var22;
                                        if (var26 < 0 && !world.getMaterial(var27, var28, var29).IsSolid || var26 >= 0 && !world.isAir(var27, var28, var29))
                                        {
                                            validLocation = false;
                                        }
                                    }
                                }
                            }

                            if (validLocation)
                            {
                                var32 = var20 + 0.5D - entity.y;
                                var33 = var15 * var15 + var32 * var32 + var18 * var18;
                                if (var4 < 0.0D || var33 < var4)
                                {
                                    var4 = var33;
                                    var9 = var14;
                                    var10 = var20;
                                    var11 = var17;
                                    var12 = var21 % 4;
                                }
                            }
                        }
                    }
                }
            }
        }

        if (var4 < 0.0D)
        {
            for (var14 = var6 - var3; var14 <= var6 + var3; ++var14)
            {
                var15 = var14 + 0.5D - entity.x;

                for (var17 = var8 - var3; var17 <= var8 + var3; ++var17)
                {
                    var18 = var17 + 0.5D - entity.z;

                    for (var20 = 127; var20 >= 0; --var20)
                    {
                        if (world.isAir(var14, var20, var17))
                        {
                            while (world.isAir(var14, var20 - 1, var17))
                            {
                                --var20;
                            }

                            for (var21 = var13; var21 < var13 + 2; ++var21)
                            {
                                var22 = var21 % 2;
                                var23 = 1 - var22;

                                bool validLocation = true;
                                for (var24 = 0; var24 < 4 && validLocation; ++var24)
                                {
                                    for (var25 = -1; var25 < 4 && validLocation; ++var25)
                                    {
                                        var26 = var14 + (var24 - 1) * var22;
                                        var27 = var20 + var25;
                                        var28 = var17 + (var24 - 1) * var23;
                                        if (var25 < 0 && !world.getMaterial(var26, var27, var28).IsSolid || var25 >= 0 && !world.isAir(var26, var27, var28))
                                        {
                                            validLocation = false;
                                        }
                                    }
                                }

                                if (validLocation)
                                {
                                    var32 = var20 + 0.5D - entity.y;
                                    var33 = var15 * var15 + var32 * var32 + var18 * var18;
                                    if (var4 < 0.0D || var33 < var4)
                                    {
                                        var4 = var33;
                                        var9 = var14;
                                        var10 = var20;
                                        var11 = var17;
                                        var12 = var21 % 2;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        int var30 = var9;
        int var16 = var10;
        var17 = var11;
        int var31 = var12 % 2;
        int var19 = 1 - var31;
        if (var12 % 4 >= 2)
        {
            var31 = -var31;
            var19 = -var19;
        }

        bool var34;
        if (var4 < 0.0D)
        {
            if (var10 < 70)
            {
                var10 = 70;
            }

            if (var10 > 118)
            {
                var10 = 118;
            }

            var16 = var10;

            for (var20 = -1; var20 <= 1; ++var20)
            {
                for (var21 = 1; var21 < 3; ++var21)
                {
                    for (var22 = -1; var22 < 3; ++var22)
                    {
                        var23 = var30 + (var21 - 1) * var31 + var20 * var19;
                        var24 = var16 + var22;
                        var25 = var17 + (var21 - 1) * var19 - var20 * var31;
                        var34 = var22 < 0;
                        world.setBlock(var23, var24, var25, var34 ? Block.Obsidian.id : 0);
                    }
                }
            }
        }

        for (var20 = 0; var20 < 4; ++var20)
        {
            world.pauseTicking = true;

            for (var21 = 0; var21 < 4; ++var21)
            {
                for (var22 = -1; var22 < 4; ++var22)
                {
                    var23 = var30 + (var21 - 1) * var31;
                    var24 = var16 + var22;
                    var25 = var17 + (var21 - 1) * var19;
                    var34 = var21 == 0 || var21 == 3 || var22 == -1 || var22 == 3;
                    world.setBlock(var23, var24, var25, var34 ? Block.Obsidian.id : Block.NetherPortal.id);
                }
            }

            world.pauseTicking = false;

            for (var21 = 0; var21 < 4; ++var21)
            {
                for (var22 = -1; var22 < 4; ++var22)
                {
                    var23 = var30 + (var21 - 1) * var31;
                    var24 = var16 + var22;
                    var25 = var17 + (var21 - 1) * var19;
                    world.notifyNeighbors(var23, var24, var25, world.getBlockId(var23, var24, var25));
                }
            }
        }

        return true;
    }
}