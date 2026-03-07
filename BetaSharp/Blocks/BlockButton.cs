using BetaSharp.Blocks.Materials;
using BetaSharp.Entities;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds.Core;

namespace BetaSharp.Blocks;

internal class BlockButton : Block
{
    public BlockButton(int id, int textureId) : base(id, textureId, Material.PistonBreakable)
    {
        setTickRandomly(true);
    }

    public override Box? getCollisionShape(IBlockReader world, int x, int y, int z)
    {
        return null;
    }

    public override int getTickRate()
    {
        return 20;
    }

    public override bool isOpaque()
    {
        return false;
    }

    public override bool isFullCube()
    {
        return false;
    }

    public override bool canPlaceAt(WorldBlockView world, int x, int y, int z, int side)
    {
        return side == 2 && world.shouldSuffocate(x, y, z + 1) ? true : (side == 3 && world.shouldSuffocate(x, y, z - 1) ? true : (side == 4 && world.shouldSuffocate(x + 1, y, z) ? true : side == 5 && world.shouldSuffocate(x - 1, y, z)));
    }

    public override bool canPlaceAt(WorldBlockView world, int x, int y, int z)
    {
        return world.shouldSuffocate(x - 1, y, z) ? true : (world.shouldSuffocate(x + 1, y, z) ? true : (world.shouldSuffocate(x, y, z - 1) ? true : world.shouldSuffocate(x, y, z + 1)));
    }

    public override void onPlaced(World world, int x, int y, int z, int direction)
    {
        int facing = world.getBlockMeta(x, y, z);
        int pressedBit = facing & 8;
        facing &= 7;
        if (direction == 2 && world.shouldSuffocate(x, y, z + 1))
        {
            facing = 4;
        }
        else if (direction == 3 && world.shouldSuffocate(x, y, z - 1))
        {
            facing = 3;
        }
        else if (direction == 4 && world.shouldSuffocate(x + 1, y, z))
        {
            facing = 2;
        }
        else if (direction == 5 && world.shouldSuffocate(x - 1, y, z))
        {
            facing = 1;
        }
        else
        {
            facing = getPlacementSide(world, x, y, z);
        }

        world.setBlockMeta(x, y, z, facing + pressedBit);
    }

    private int getPlacementSide(World world, int x, int y, int z)
    {
        return world.shouldSuffocate(x - 1, y, z) ? 1 : (world.shouldSuffocate(x + 1, y, z) ? 2 : (world.shouldSuffocate(x, y, z - 1) ? 3 : (world.shouldSuffocate(x, y, z + 1) ? 4 : 1)));
    }

    public override void neighborUpdate(WorldBlockView world, int x, int y, int z, int id)
    {
        if (breakIfCannotPlaceAt(world, x, y, z))
        {
            int facing = world.getBlockMeta(x, y, z) & 7;
            bool shouldBreak = false;
            if (!world.shouldSuffocate(x - 1, y, z) && facing == 1)
            {
                shouldBreak = true;
            }

            if (!world.shouldSuffocate(x + 1, y, z) && facing == 2)
            {
                shouldBreak = true;
            }

            if (!world.shouldSuffocate(x, y, z - 1) && facing == 3)
            {
                shouldBreak = true;
            }

            if (!world.shouldSuffocate(x, y, z + 1) && facing == 4)
            {
                shouldBreak = true;
            }

            if (shouldBreak)
            {
                dropStacks(world, x, y, z, world.getBlockMeta(x, y, z));
                world.setBlock(x, y, z, 0);
            }
        }

    }

    private bool breakIfCannotPlaceAt(WorldBlockView world, int x, int y, int z)
    {
        if (!canPlaceAt(world, x, y, z))
        {
            dropStacks(world, x, y, z, world.getBlockMeta(x, y, z));
            world.setBlock(x, y, z, 0);
            return false;
        }
        else
        {
            return true;
        }
    }

    public override void updateBoundingBox(IBlockReader iBlockReader, int x, int y, int z)
    {
        int meta = iBlockReader.getBlockMeta(x, y, z);
        int facing = meta & 7;
        bool isPressed = (meta & 8) > 0;
        float minY = 6.0F / 16.0F;
        float maxY = 10.0F / 16.0F;
        float halfWidth = 3.0F / 16.0F;
        float thickness = 2.0F / 16.0F;
        if (isPressed)
        {
            thickness = 1.0F / 16.0F;
        }

        if (facing == 1)
        {
            setBoundingBox(0.0F, minY, 0.5F - halfWidth, thickness, maxY, 0.5F + halfWidth);
        }
        else if (facing == 2)
        {
            setBoundingBox(1.0F - thickness, minY, 0.5F - halfWidth, 1.0F, maxY, 0.5F + halfWidth);
        }
        else if (facing == 3)
        {
            setBoundingBox(0.5F - halfWidth, minY, 0.0F, 0.5F + halfWidth, maxY, thickness);
        }
        else if (facing == 4)
        {
            setBoundingBox(0.5F - halfWidth, minY, 1.0F - thickness, 0.5F + halfWidth, maxY, 1.0F);
        }

    }

    public override void onBlockBreakStart(World world, int x, int y, int z, EntityPlayer player)
    {
        onUse(world, x, y, z, player);
    }

    public override bool onUse(World world, int x, int y, int z, EntityPlayer player)
    {
        int meta = world.getBlockMeta(x, y, z);
        int facing = meta & 7;
        int pressToggle = 8 - (meta & 8);
        if (pressToggle == 0)
        {
            return true;
        }
        else
        {
            world.setBlockMeta(x, y, z, facing + pressToggle);
            world.setBlocksDirty(x, y, z, x, y, z);
            world.playSound((double)x + 0.5D, (double)y + 0.5D, (double)z + 0.5D, "random.click", 0.3F, 0.6F);
            world.notifyNeighbors(x, y, z, id);
            if (facing == 1)
            {
                world.notifyNeighbors(x - 1, y, z, id);
            }
            else if (facing == 2)
            {
                world.notifyNeighbors(x + 1, y, z, id);
            }
            else if (facing == 3)
            {
                world.notifyNeighbors(x, y, z - 1, id);
            }
            else if (facing == 4)
            {
                world.notifyNeighbors(x, y, z + 1, id);
            }
            else
            {
                world.notifyNeighbors(x, y - 1, z, id);
            }

            world.ScheduleBlockUpdate(x, y, z, id, getTickRate());
            return true;
        }
    }

    public override void onBreak(World world, int x, int y, int z)
    {
        int meta = world.getBlockMeta(x, y, z);
        if ((meta & 8) > 0)
        {
            world.notifyNeighbors(x, y, z, id);
            int facing = meta & 7;
            if (facing == 1)
            {
                world.notifyNeighbors(x - 1, y, z, id);
            }
            else if (facing == 2)
            {
                world.notifyNeighbors(x + 1, y, z, id);
            }
            else if (facing == 3)
            {
                world.notifyNeighbors(x, y, z - 1, id);
            }
            else if (facing == 4)
            {
                world.notifyNeighbors(x, y, z + 1, id);
            }
            else
            {
                world.notifyNeighbors(x, y - 1, z, id);
            }
        }

        base.onBreak(world, x, y, z);
    }

    public override bool isPoweringSide(IBlockReader iBlockReader, int x, int y, int z, int side)
    {
        return (iBlockReader.getBlockMeta(x, y, z) & 8) > 0;
    }

    public override bool isStrongPoweringSide(IBlockReader world, int x, int y, int z, int side)
    {
        int meta = world.getBlockMeta(x, y, z);
        if ((meta & 8) == 0)
        {
            return false;
        }
        else
        {
            int facing = meta & 7;
            return facing == 5 && side == 1 ? true : (facing == 4 && side == 2 ? true : (facing == 3 && side == 3 ? true : (facing == 2 && side == 4 ? true : facing == 1 && side == 5)));
        }
    }

    public override bool canEmitRedstonePower()
    {
        return true;
    }

    public override void onTick(WorldBlockView worldView,int x,int y,int z, JavaRandom random,WorldEventBroadcaster broadcaster, bool isRemote)
    {
        if (!isRemote)
        {
            int meta = worldView.getBlockMeta(x, y, z);
            if ((meta & 8) != 0)
            {
                worldView.setBlockMeta(x, y, z, meta & 7);
                worldView.NotifyNeighbors(x, y, z, id);
                int facing = meta & 7;
                if (facing == 1)
                {
                    worldView.NotifyNeighbors(x - 1, y, z, id);
                }
                else if (facing == 2)
                {
                    worldView.NotifyNeighbors(x + 1, y, z, id);
                }
                else if (facing == 3)
                {
                    worldView.NotifyNeighbors(x, y, z - 1, id);
                }
                else if (facing == 4)
                {
                    worldView.NotifyNeighbors(x, y, z + 1, id);
                }
                else
                {
                    worldView.NotifyNeighbors(x, y - 1, z, id);
                }

                broadcaster.PlaySoundAtPos(x + 0.5D, y + 0.5D, z + 0.5D, "random.click", 0.3F, 0.5F);
                broadcaster.setBlocksDirty(x, y, z, x, y, z);
            }
        }
    }

    public override void setupRenderBoundingBox()
    {
        float halfWidth = 3.0F / 16.0F;
        float halfHeight = 2.0F / 16.0F;
        float halfDepth = 2.0F / 16.0F;
        setBoundingBox(0.5F - halfWidth, 0.5F - halfHeight, 0.5F - halfDepth, 0.5F + halfWidth, 0.5F + halfHeight, 0.5F + halfDepth);
    }
}
