using BetaSharp.Blocks.Materials;
using BetaSharp.Worlds;

namespace BetaSharp.Blocks;

public class BlockStationary : BlockFluid
{
    public BlockStationary(int id, Material material) : base(id, material)
    {
        setTickRandomly(false);
        if (material == Material.Lava)
        {
            setTickRandomly(true);
        }

    }

    public override void neighborUpdate(World world, int x, int y, int z, int id)
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
        world.scheduleBlockUpdate(x, y, z, id - 1, getTickRate());
        world.pauseTicking = false;
    }

    public override void onTick(World world, int x, int y, int z, java.util.Random random)
    {
        if (material == Material.Lava)
        {
            int attempts = random.nextInt(3);

            for (int attempt = 0; attempt < attempts; ++attempt)
            {
                x += random.nextInt(3) - 1;
                ++y;
                z += random.nextInt(3) - 1;
                int neighborBlockId = world.getBlockId(x, y, z);
                if (neighborBlockId == 0)
                {
                    if (isFlammable(world, x - 1, y, z) || isFlammable(world, x + 1, y, z) || isFlammable(world, x, y, z - 1) || isFlammable(world, x, y, z + 1) || isFlammable(world, x, y - 1, z) || isFlammable(world, x, y + 1, z))
                    {
                        world.setBlock(x, y, z, Block.FIRE.id);
                        return;
                    }
                }
                else if (Block.BLOCKS[neighborBlockId].material.BlocksMovement)
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