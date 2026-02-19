using BetaSharp.Blocks.Materials;
using BetaSharp.Entities;
using BetaSharp.Worlds;
using BetaSharp.Util.Maths;

namespace BetaSharp.Blocks;

public class BlockSand : Block
{
    private static readonly ThreadLocal<bool> s_fallInstantly = new(() => false);

    public static bool fallInstantly
    {
        get => s_fallInstantly.Value;
        set => s_fallInstantly.Value = value;
    }

    public BlockSand(int id, int textureId) : base(id, textureId, Material.Sand)
    {
    }

    public override void onPlaced(World world, int x, int y, int z)
    {
        world.ScheduleBlockUpdate(x, y, z, id, getTickRate());
    }

    public override void neighborUpdate(World world, int x, int y, int z, int id)
    {
        world.ScheduleBlockUpdate(x, y, z, base.id, getTickRate());
    }

    public override void onTick(World world, int x, int y, int z, JavaRandom random)
    {
        processFall(world, x, y, z);
    }

    private void processFall(World world, int x, int y, int z)
    {
        if (canFallThrough(world, x, y - 1, z) && y >= 0)
        {
            sbyte checkRadius = 32;
            if (!fallInstantly && world.isRegionLoaded(x - checkRadius, y - checkRadius, z - checkRadius, x + checkRadius, y + checkRadius, z + checkRadius))
            {
                EntityFallingSand fallingSand = new EntityFallingSand(world, (double)((float)x + 0.5F), (double)((float)y + 0.5F), (double)((float)z + 0.5F), id);
                world.SpawnEntity(fallingSand);
            }
            else
            {
                world.setBlock(x, y, z, 0);

                while (canFallThrough(world, x, y - 1, z) && y > 0)
                {
                    --y;
                }

                if (y > 0)
                {
                    world.setBlock(x, y, z, id);
                }
            }
        }

    }

    public override int getTickRate()
    {
        return 3;
    }

    public static bool canFallThrough(World world, int x, int y, int z)
    {
        int blockId = world.getBlockId(x, y, z);
        if (blockId == 0)
        {
            return true;
        }
        else if (blockId == Block.Fire.id)
        {
            return true;
        }
        else
        {
            Material material = Block.Blocks[blockId].material;
            return material == Material.Water || material == Material.Lava;
        }
    }
}
