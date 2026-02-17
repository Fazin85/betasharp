using BetaSharp.Blocks;
using BetaSharp.Blocks.Entities;
using BetaSharp.Blocks.Materials;
using BetaSharp.Items;

namespace BetaSharp.Worlds.Gen.Features;

public class DungeonFeature : Feature
{

    public override bool Generate(World world, java.util.Random rand, int x, int y, int z)
    {
        byte height = 3;
        int radiusX = rand.nextInt(2) + 2;
        int radiusZ = rand.nextInt(2) + 2;
        int openingsCount = 0;


        for (int cx = x - radiusX - 1; cx <= x + radiusX + 1; ++cx)
        {
            for (int cy = y - 1; cy <= y + height + 1; ++cy)
            {
                for (int cz = z - radiusZ - 1; cz <= z + radiusZ + 1; ++cz)
                {
                    Material mat = world.getMaterial(cx, cy, cz);
                    if ((cy == y - 1 || cy == y + height + 1) && !mat.IsSolid)
                    {
                        return false;
                    }

                    bool isWall = cx == x - radiusX - 1 ||
                                  cx == x + radiusX + 1 ||
                                  cz == z - radiusZ - 1 ||
                                  cz == z + radiusZ + 1;

                    if (isWall && cy == y && world.isAir(cx, cy, cz) && world.isAir(cx, cy + 1, cz))
                    {
                        ++openingsCount;
                    }
                }
            }
        }

        if (openingsCount < 1 || openingsCount > 5) return false;

        for (int cx = x - radiusX - 1; cx <= x + radiusX + 1; ++cx)
        {
            for (int cy = y + height; cy >= y - 1; --cy)
            {
                for (int cz = z - radiusZ - 1; cz <= z + radiusZ + 1; ++cz)
                {
                    bool isInside = cx != x - radiusX - 1 &&
                                    cy != y - 1 &&
                                    cz != z - radiusZ - 1 &&
                                    cx != x + radiusX + 1 &&
                                    cy != y + height + 1 &&
                                    cz != z + radiusZ + 1;
                    if (isInside)
                    {
                        world.setBlock(cx, cy, cz, 0);
                    }
                    else if (cy >= 0 && !world.getMaterial(cx, cy - 1, cz).IsSolid)
                    {
                        world.setBlock(cx, cy, cz, 0);
                    }
                    else if (world.getMaterial(cx, cy, cz).IsSolid)
                    {
                        if (cy == y - 1 && rand.nextInt(4) != 0)
                            world.setBlock(cx, cy, cz, Block.MossyCobblestone.id);
                        else
                            world.setBlock(cx, cy, cz, Block.Cobblestone.id);
                    }
                }
            }
        }


        for (int i = 0; i < 2; ++i)
        {
            for (int j = 0; j < 3; ++j)
            {
                int chestX = x + rand.nextInt(radiusX * 2 + 1) - radiusX;
                int chestZ = z + rand.nextInt(radiusZ * 2 + 1) - radiusZ;
                if (world.isAir(chestX, y, chestZ))
                {
                    int neighbors = 0;
                    if (world.getMaterial(chestX - 1, y, chestZ).IsSolid) ++neighbors;
                    if (world.getMaterial(chestX + 1, y, chestZ).IsSolid) ++neighbors;
                    if (world.getMaterial(chestX, y, chestZ - 1).IsSolid) ++neighbors;
                    if (world.getMaterial(chestX, y, chestZ + 1).IsSolid) ++neighbors;

                    if (neighbors != 1) continue;

                    world.setBlock(chestX, y, chestZ, Block.Chest.id);

                    BlockEntityChest chest = (BlockEntityChest)world.getBlockEntity(chestX, y, chestZ);
                    for (int k = 0; k < 8; ++k)
                    {
                        ItemStack? loot = PickCheckLootItem(rand);
                        if (loot != null)
                        {
                            chest.setStack(rand.nextInt(chest.size()), loot);
                        }
                    }

                }
            }
        }

        world.setBlock(x, y, z, Block.Spawner.id);
        BlockEntityMobSpawner var19 = (BlockEntityMobSpawner)world.getBlockEntity(x, y, z);
        var19.SetSpawnedEntityId(PickMobSpawner(rand));
        return true;


    }

    private ItemStack? PickCheckLootItem(java.util.Random rand)
    {
        int chance = rand.nextInt(11);

        return chance switch
        {
            0 => new ItemStack(Item.SADDLE),
            1 => new ItemStack(Item.IRON_INGOT, rand.nextInt(4) + 1),
            2 => new ItemStack(Item.BREAD),
            3 => new ItemStack(Item.WHEAT, rand.nextInt(4) + 1),
            4 => new ItemStack(Item.GUNPOWDER, rand.nextInt(4) + 1),
            5 => new ItemStack(Item.STRING, rand.nextInt(4) + 1),
            6 => new ItemStack(Item.BUCKET),
            7 => rand.nextInt(100) == 0 ? new ItemStack(Item.GOLDEN_APPLE) : null,
            8 => rand.nextInt(2) == 0 ? new ItemStack(Item.REDSTONE, rand.nextInt(4) + 1) : null,
            9 => rand.nextInt(10) == 0 ? new ItemStack(Item.ITEMS[Item.RECORD_THIRTEEN.id + rand.nextInt(2)]) : null,
            10 => new ItemStack(Item.DYE, 1, 3),
            _ => null,
        };
    }

    private string PickMobSpawner(java.util.Random rand)
    {
        return rand.nextInt(4) switch
        {
            0 => "Skeleton",
            1 => "Zombie",
            2 => "Zombie",
            3 => "Spider",
            _ => "Zombie"
        };
    }
}