using BetaSharp.Blocks.Materials;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds.Core;

namespace BetaSharp.Blocks;

internal class BlockStationary : BlockFluid
{
    public BlockStationary(int id, Material material) : base(id, material)
    {
        setTickRandomly(false);
        if (material == Material.Lava)
        {
            setTickRandomly(true);
        }

    }

    public override void neighborUpdate(WorldBlockView world, int x, int y, int z, int id)
    {
        base.neighborUpdate(world, x, y, z, id);
        if (world.getBlockId(x, y, z) == base.id)
        {
            convertToFlowing(world, x, y, z);
        }

    }

    private void convertToFlowing(World world, int x, int y, int z)
    {
        int meta = world.getBlockMeta(x, y, z);
        world.pauseTicking = true;
        world.setBlockWithoutNotifyingNeighbors(x, y, z, id - 1, meta);
        world.setBlocksDirty(x, y, z, x, y, z);
        world.ScheduleBlockUpdate(x, y, z, id - 1, getTickRate());
        world.pauseTicking = false;
    }

    public override void onTick(WorldBlockView worldView, int x, int y, int z, JavaRandom random, WorldEventBroadcaster broadcaster, bool isRemote)
    {
        if (material == Material.Lava)
        {
            int attempts = random.NextInt(3);

            for (int attempt = 0; attempt < attempts; ++attempt)
            {
                x += random.NextInt(3) - 1;
                ++y;
                z += random.NextInt(3) - 1;
                int neighborBlockId = worldView.getBlockId(x, y, z);
                if (neighborBlockId == 0)
                {
                    if (isFlammable(worldView, x - 1, y, z) || isFlammable(worldView, x + 1, y, z) || isFlammable(worldView, x, y, z - 1) || isFlammable(worldView, x, y, z + 1) || isFlammable(worldView, x, y - 1, z) || isFlammable(worldView, x, y + 1, z))
                    {
                        worldView.setBlock(x, y, z, Block.Fire.id);
                        return;
                    }
                }
                else if (Block.Blocks[neighborBlockId].material.BlocksMovement)
                {
                    return;
                }
            }
        }

    }

    private bool isFlammable(World world, int x, int y, int z)
    {
        return world.getMaterial(x, y, z).IsBurnable;
    }
}
