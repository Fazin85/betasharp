using BetaSharp.Worlds;

namespace BetaSharp.Blocks;

public class BlockRedstoneTorch : BlockTorch
{

    private bool lit = false;
    private static List<RedstoneUpdateInfo> torchUpdates = [];

    public override int getTexture(int side, int meta)
    {
        return side == 1 ? Block.RedstoneWire.getTexture(side, meta) : base.getTexture(side, meta);
    }

    private bool isBurnedOut(World world, int x, int y, int z, bool recordUpdate)
    {
        if (recordUpdate)
        {
            torchUpdates.Add(new RedstoneUpdateInfo(x, y, z, world.getTime()));
        }

        int updateCount = 0;

        for (int i = 0; i < torchUpdates.Capacity; ++i)
        {
            RedstoneUpdateInfo updateInfo = torchUpdates[i];
            if (updateInfo.x == x && updateInfo.y == y && updateInfo.z == z)
            {
                ++updateCount;
                if (updateCount >= 8)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public BlockRedstoneTorch(int id, int textureId, bool lit) : base(id, textureId)
    {
        this.lit = lit;
        setTickRandomly(true);
    }

    public override int getTickRate()
    {
        return 2;
    }

    public override void onPlaced(World world, int x, int y, int z)
    {
        if (world.getBlockMeta(x, y, z) == 0)
        {
            base.onPlaced(world, x, y, z);
        }

        if (lit)
        {
            world.notifyNeighbors(x, y - 1, z, id);
            world.notifyNeighbors(x, y + 1, z, id);
            world.notifyNeighbors(x - 1, y, z, id);
            world.notifyNeighbors(x + 1, y, z, id);
            world.notifyNeighbors(x, y, z - 1, id);
            world.notifyNeighbors(x, y, z + 1, id);
        }

    }

    public override void onBreak(World world, int x, int y, int z)
    {
        if (lit)
        {
            world.notifyNeighbors(x, y - 1, z, id);
            world.notifyNeighbors(x, y + 1, z, id);
            world.notifyNeighbors(x - 1, y, z, id);
            world.notifyNeighbors(x + 1, y, z, id);
            world.notifyNeighbors(x, y, z - 1, id);
            world.notifyNeighbors(x, y, z + 1, id);
        }

    }

    public override bool isPoweringSide(BlockView blockView, int x, int y, int z, int side)
    {
        if (!lit)
        {
            return false;
        }
        else
        {
            int meta = blockView.getBlockMeta(x, y, z);
            return meta == 5 && side == 1 ? false : (meta == 3 && side == 3 ? false : (meta == 4 && side == 2 ? false : (meta == 1 && side == 5 ? false : meta != 2 || side != 4)));
        }
    }

    private bool shouldUnpower(World world, int x, int y, int z)
    {
        int meta = world.getBlockMeta(x, y, z);
        return meta == 5 && world.isPoweringSide(x, y - 1, z, 0) ? true : (meta == 3 && world.isPoweringSide(x, y, z - 1, 2) ? true : (meta == 4 && world.isPoweringSide(x, y, z + 1, 3) ? true : (meta == 1 && world.isPoweringSide(x - 1, y, z, 4) ? true : meta == 2 && world.isPoweringSide(x + 1, y, z, 5))));
    }

    public override void onTick(World world, int x, int y, int z, java.util.Random random)
    {
        bool shouldTurnOff = shouldUnpower(world, x, y, z);

        while (torchUpdates.Count > 0 && world.getTime() - torchUpdates[0].updateTime > 100L)
        {
            torchUpdates.RemoveAt(0);
        }

        if (lit)
        {
            if (shouldTurnOff)
            {
                world.setBlock(x, y, z, Block.RedstoneTorch.id, world.getBlockMeta(x, y, z));
                if (isBurnedOut(world, x, y, z, true))
                {
                    world.playSound((double)((float)x + 0.5F), (double)((float)y + 0.5F), (double)((float)z + 0.5F), "random.fizz", 0.5F, 2.6F + (world.random.nextFloat() - world.random.nextFloat()) * 0.8F);

                    for (int particleIndex = 0; particleIndex < 5; ++particleIndex)
                    {
                        double particleX = (double)x + random.nextDouble() * 0.6D + 0.2D;
                        double particleY = (double)y + random.nextDouble() * 0.6D + 0.2D;
                        double particleZ = (double)z + random.nextDouble() * 0.6D + 0.2D;
                        world.addParticle("smoke", particleX, particleY, particleZ, 0.0D, 0.0D, 0.0D);
                    }
                }
            }
        }
        else if (!shouldTurnOff && !isBurnedOut(world, x, y, z, false))
        {
            world.setBlock(x, y, z, Block.LitRedstoneTorch.id, world.getBlockMeta(x, y, z));
        }

    }

    public override void neighborUpdate(World world, int x, int y, int z, int id)
    {
        base.neighborUpdate(world, x, y, z, id);
        world.ScheduleBlockUpdate(x, y, z, base.id, getTickRate());
    }

    public override bool isStrongPoweringSide(World world, int x, int y, int z, int side)
    {
        return side == 0 ? isPoweringSide(world, x, y, z, side) : false;
    }

    public override int getDroppedItemId(int blockMeta, java.util.Random random)
    {
        return Block.LitRedstoneTorch.id;
    }

    public override bool canEmitRedstonePower()
    {
        return true;
    }

    public override void randomDisplayTick(World world, int x, int y, int z, java.util.Random random)
    {
        if (lit)
        {
            int meta = world.getBlockMeta(x, y, z);
            double particleX = (double)((float)x + 0.5F) + (double)(random.nextFloat() - 0.5F) * 0.2D;
            double particleY = (double)((float)y + 0.7F) + (double)(random.nextFloat() - 0.5F) * 0.2D;
            double particleZ = (double)((float)z + 0.5F) + (double)(random.nextFloat() - 0.5F) * 0.2D;
            double verticalOffset = (double)0.22F;
            double horizontalOffset = (double)0.27F;
            if (meta == 1)
            {
                world.addParticle("reddust", particleX - horizontalOffset, particleY + verticalOffset, particleZ, 0.0D, 0.0D, 0.0D);
            }
            else if (meta == 2)
            {
                world.addParticle("reddust", particleX + horizontalOffset, particleY + verticalOffset, particleZ, 0.0D, 0.0D, 0.0D);
            }
            else if (meta == 3)
            {
                world.addParticle("reddust", particleX, particleY + verticalOffset, particleZ - horizontalOffset, 0.0D, 0.0D, 0.0D);
            }
            else if (meta == 4)
            {
                world.addParticle("reddust", particleX, particleY + verticalOffset, particleZ + horizontalOffset, 0.0D, 0.0D, 0.0D);
            }
            else
            {
                world.addParticle("reddust", particleX, particleY, particleZ, 0.0D, 0.0D, 0.0D);
            }

        }
    }
}