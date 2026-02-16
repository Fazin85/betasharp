using BetaSharp.Blocks;
using BetaSharp.Blocks.Entities;
using BetaSharp.Blocks.Materials;
using BetaSharp.Items;

namespace BetaSharp.Worlds.Gen.Features;

public class DungeonFeature : Feature
{

    public override bool generate(World world, java.util.Random rand, int x, int y, int z)
    {
        byte var6 = 3;
        int var7 = rand.nextInt(2) + 2;
        int var8 = rand.nextInt(2) + 2;
        int var9 = 0;

        int var10;
        int var11;
        int var12;
        for (var10 = x - var7 - 1; var10 <= x + var7 + 1; ++var10)
        {
            for (var11 = y - 1; var11 <= y + var6 + 1; ++var11)
            {
                for (var12 = z - var8 - 1; var12 <= z + var8 + 1; ++var12)
                {
                    Material var13 = world.getMaterial(var10, var11, var12);
                    if (var11 == y - 1 && !var13.IsSolid)
                    {
                        return false;
                    }

                    if (var11 == y + var6 + 1 && !var13.IsSolid)
                    {
                        return false;
                    }

                    if ((var10 == x - var7 - 1 || var10 == x + var7 + 1 || var12 == z - var8 - 1 || var12 == z + var8 + 1) && var11 == y && world.isAir(var10, var11, var12) && world.isAir(var10, var11 + 1, var12))
                    {
                        ++var9;
                    }
                }
            }
        }

        if (var9 >= 1 && var9 <= 5)
        {
            for (var10 = x - var7 - 1; var10 <= x + var7 + 1; ++var10)
            {
                for (var11 = y + var6; var11 >= y - 1; --var11)
                {
                    for (var12 = z - var8 - 1; var12 <= z + var8 + 1; ++var12)
                    {
                        if (var10 != x - var7 - 1 && var11 != y - 1 && var12 != z - var8 - 1 && var10 != x + var7 + 1 && var11 != y + var6 + 1 && var12 != z + var8 + 1)
                        {
                            world.setBlock(var10, var11, var12, 0);
                        }
                        else if (var11 >= 0 && !world.getMaterial(var10, var11 - 1, var12).IsSolid)
                        {
                            world.setBlock(var10, var11, var12, 0);
                        }
                        else if (world.getMaterial(var10, var11, var12).IsSolid)
                        {
                            if (var11 == y - 1 && rand.nextInt(4) != 0)
                            {
                                world.setBlock(var10, var11, var12, Block.MossyCobblestone.id);
                            }
                            else
                            {
                                world.setBlock(var10, var11, var12, Block.Cobblestone.id);
                            }
                        }
                    }
                }
            }

            for (var10 = 0; var10 < 2; ++var10)
            {
                for (var11 = 0; var11 < 3; ++var11)
                {
                    var12 = x + rand.nextInt(var7 * 2 + 1) - var7;
                    int var14 = z + rand.nextInt(var8 * 2 + 1) - var8;
                    if (world.isAir(var12, y, var14))
                    {
                        int var15 = 0;
                        if (world.getMaterial(var12 - 1, y, var14).IsSolid)
                        {
                            ++var15;
                        }
                        if (world.getMaterial(var12 + 1, y, var14).IsSolid)
                        {
                            ++var15;
                        }
                        if (world.getMaterial(var12, y, var14 - 1).IsSolid)
                        {
                            ++var15;
                        }
                        if (world.getMaterial(var12, y, var14 + 1).IsSolid)
                        {
                            ++var15;
                        }
                        if (var15 == 1)
                        {
                            world.setBlock(var12, y, var14, Block.Chest.id);
                            BlockEntityChest var16 = (BlockEntityChest)world.getBlockEntity(var12, y, var14);

                            for (int var17 = 0; var17 < 8; ++var17)
                            {
                                ItemStack var18 = pickCheckLootItem(rand);
                                if (var18 != null)
                                {
                                    var16.setStack(rand.nextInt(var16.size()), var18);
                                }
                            }
                        }
                    }
                }
            }

            world.setBlock(x, y, z, Block.Spawner.id);
            BlockEntityMobSpawner var19 = (BlockEntityMobSpawner)world.getBlockEntity(x, y, z);
            var19.setSpawnedEntityId(pickMobSpawner(rand));
            return true;
        }
        else
        {
            return false;
        }
    }

    private ItemStack pickCheckLootItem(java.util.Random var1)
    {
        int var2 = var1.nextInt(11);
        return var2 == 0 ? new ItemStack(Item.SADDLE) : var2 == 1 ? new ItemStack(Item.IRON_INGOT, var1.nextInt(4) + 1) : var2 == 2 ? new ItemStack(Item.BREAD) : var2 == 3 ? new ItemStack(Item.WHEAT, var1.nextInt(4) + 1) : var2 == 4 ? new ItemStack(Item.GUNPOWDER, var1.nextInt(4) + 1) : var2 == 5 ? new ItemStack(Item.STRING, var1.nextInt(4) + 1) : var2 == 6 ? new ItemStack(Item.BUCKET) : var2 == 7 && var1.nextInt(100) == 0 ? new ItemStack(Item.GOLDEN_APPLE) : var2 == 8 && var1.nextInt(2) == 0 ? new ItemStack(Item.REDSTONE, var1.nextInt(4) + 1) : var2 == 9 && var1.nextInt(10) == 0 ? new ItemStack(Item.ITEMS[Item.RECORD_THIRTEEN.id + var1.nextInt(2)]) : var2 == 10 ? new ItemStack(Item.DYE, 1, 3) : null;
    }

    private string pickMobSpawner(java.util.Random var1)
    {
        int var2 = var1.nextInt(4);
        return var2 == 0 ? "Skeleton" : var2 == 1 ? "Zombie" : var2 == 2 ? "Zombie" : var2 == 3 ? "Spider" : "";
    }
}