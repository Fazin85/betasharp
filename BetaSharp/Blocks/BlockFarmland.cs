using BetaSharp.Blocks.Materials;
using BetaSharp.Entities;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds.Core;

namespace BetaSharp.Blocks;

internal class BlockFarmland : Block
{

    public BlockFarmland(int id) : base(id, Material.Soil)
    {
        textureId = 87;
        setTickRandomly(true);
        setBoundingBox(0.0F, 0.0F, 0.0F, 1.0F, 15.0F / 16.0F, 1.0F);
        setOpacity(255);
    }

    public override Box? getCollisionShape(IBlockReader world, int x, int y, int z)
    {
        return new Box((double)(x + 0), (double)(y + 0), (double)(z + 0), (double)(x + 1), (double)(y + 1), (double)(z + 1));
    }

    public override bool isOpaque()
    {
        return false;
    }

    public override bool isFullCube()
    {
        return false;
    }

    public override int getTexture(int side, int meta)
    {
        return side == 1 && meta > 0 ? textureId - 1 : (side == 1 ? textureId : 2);
    }

    public override void onTick(WorldBlockView worldView, int x, int y, int z, JavaRandom random, WorldEventBroadcaster broadcaster, bool isRemote)
    {
        if (random.NextInt(5) == 0)
        {
            if (!isWaterNearby(worldView, x, y, z) && !worldView.isRaining(x, y + 1, z))
            {
                int meta = worldView.getBlockMeta(x, y, z);
                if (meta > 0)
                {
                    worldView.setBlockMeta(x, y, z, meta - 1);
                }
                else if (!hasCrop(worldView, x, y, z))
                {
                    worldView.setBlock(x, y, z, Block.Dirt.id);
                }
            }
            else
            {
                worldView.setBlockMeta(x, y, z, 7);
            }
        }

    }

    public override void onSteppedOn(World world, int x, int y, int z, Entity entity)
    {
        if (world.random.NextInt(4) == 0)
        {
            world.setBlock(x, y, z, Block.Dirt.id);
        }

    }

    private static bool hasCrop(IBlockReader world, int x, int y, int z)
    {
        sbyte cropRadius = 0;

        for (int var6 = x - cropRadius; var6 <= x + cropRadius; ++var6)
        {
            for (int var7 = z - cropRadius; var7 <= z + cropRadius; ++var7)
            {
                if (world.getBlockId(var6, y + 1, var7) == Block.Wheat.id)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private static bool isWaterNearby(IBlockReader world, int x, int y, int z)
    {
        for (int checkX = x - 4; checkX <= x + 4; ++checkX)
        {
            for (int checkY = y; checkY <= y + 1; ++checkY)
            {
                for (int checkZ = z - 4; checkZ <= z + 4; ++checkZ)
                {
                    if (world.getMaterial(checkX, checkY, checkZ) == Material.Water)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    public override void neighborUpdate(WorldBlockView world, int x, int y, int z, int id)
    {
        base.neighborUpdate(world, x, y, z, id);
        Material material = world.getMaterial(x, y + 1, z);
        if (material.IsSolid)
        {
            world.setBlock(x, y, z, Block.Dirt.id);
        }

    }

    public override int getDroppedItemId(int blockMeta, JavaRandom random)
    {
        return Block.Dirt.getDroppedItemId(0, random);
    }
}
