using BetaSharp.Blocks.Entities;
using BetaSharp.Blocks.Materials;
using BetaSharp.Entities;
using BetaSharp.Items;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds;

namespace BetaSharp.Blocks;

public class BlockDispenser : BlockWithEntity
{
    private java.util.Random random = new();

    public BlockDispenser(int id) : base(id, Material.Stone)
    {
        textureId = 45;
    }

    public override int getTickRate()
    {
        return 4;
    }

    public override int getDroppedItemId(int blockMeta, java.util.Random random)
    {
        return Block.DISPENSER.id;
    }

    public override void onPlaced(World world, int x, int y, int z)
    {
        base.onPlaced(world, x, y, z);
        updateDirection(world, x, y, z);
    }

    private void updateDirection(World world, int x, int y, int z)
    {
        if (!world.isRemote)
        {
            int blockNorth = world.getBlockId(x, y, z - 1);
            int blockSouth = world.getBlockId(x, y, z + 1);
            int blockWest = world.getBlockId(x - 1, y, z);
            int blockEast = world.getBlockId(x + 1, y, z);
            sbyte direction = 3;
            if (Block.BLOCKS_OPAQUE[blockNorth] && !Block.BLOCKS_OPAQUE[blockSouth])
            {
                direction = 3;
            }

            if (Block.BLOCKS_OPAQUE[blockSouth] && !Block.BLOCKS_OPAQUE[blockNorth])
            {
                direction = 2;
            }

            if (Block.BLOCKS_OPAQUE[blockWest] && !Block.BLOCKS_OPAQUE[blockEast])
            {
                direction = 5;
            }

            if (Block.BLOCKS_OPAQUE[blockEast] && !Block.BLOCKS_OPAQUE[blockWest])
            {
                direction = 4;
            }

            world.setBlockMeta(x, y, z, direction);
        }
    }

    public override int getTextureId(BlockView blockView, int x, int y, int z, int side)
    {
        if (side == 1)
        {
            return textureId + 17;
        }
        else if (side == 0)
        {
            return textureId + 17;
        }
        else
        {
            int meta = blockView.getBlockMeta(x, y, z);
            return side != meta ? textureId : textureId + 1;
        }
    }

    public override int getTexture(int side)
    {
        return side == 1 ? textureId + 17 : (side == 0 ? textureId + 17 : (side == 3 ? textureId + 1 : textureId));
    }

    public override bool onUse(World world, int x, int y, int z, EntityPlayer player)
    {
        if (world.isRemote)
        {
            return true;
        }
        else
        {
            BlockEntityDispenser dispenser = (BlockEntityDispenser)world.getBlockEntity(x, y, z);
            player.openDispenserScreen(dispenser);
            return true;
        }
    }

    private void dispense(World world, int x, int y, int z, java.util.Random random)
    {
        int meta = world.getBlockMeta(x, y, z);
        int dirX = 0;
        int dirZ = 0;
        if (meta == 3)
        {
            dirZ = 1;
        }
        else if (meta == 2)
        {
            dirZ = -1;
        }
        else if (meta == 5)
        {
            dirX = 1;
        }
        else
        {
            dirX = -1;
        }

        BlockEntityDispenser dispenser = (BlockEntityDispenser)world.getBlockEntity(x, y, z);
        ItemStack itemStack = dispenser.getItemToDispose();
        double spawnX = (double)x + (double)dirX * 0.6D + 0.5D;
        double spawnY = (double)y + 0.5D;
        double spawnZ = (double)z + (double)dirZ * 0.6D + 0.5D;
        if (itemStack == null)
        {
            world.worldEvent(1001, x, y, z, 0);
        }
        else
        {
            if (itemStack.itemId == Item.ARROW.id)
            {
                EntityArrow arrow = new EntityArrow(world, spawnX, spawnY, spawnZ);
                arrow.setArrowHeading((double)dirX, (double)0.1F, (double)dirZ, 1.1F, 6.0F);
                arrow.doesArrowBelongToPlayer = true;
                world.spawnEntity(arrow);
                world.worldEvent(1002, x, y, z, 0);
            }
            else if (itemStack.itemId == Item.EGG.id)
            {
                EntityEgg egg = new EntityEgg(world, spawnX, spawnY, spawnZ);
                egg.setEggHeading((double)dirX, (double)0.1F, (double)dirZ, 1.1F, 6.0F);
                world.spawnEntity(egg);
                world.worldEvent(1002, x, y, z, 0);
            }
            else if (itemStack.itemId == Item.SNOWBALL.id)
            {
                EntitySnowball snowball = new EntitySnowball(world, spawnX, spawnY, spawnZ);
                snowball.setSnowballHeading((double)dirX, (double)0.1F, (double)dirZ, 1.1F, 6.0F);
                world.spawnEntity(snowball);
                world.worldEvent(1002, x, y, z, 0);
            }
            else
            {
                EntityItem item = new EntityItem(world, spawnX, spawnY - 0.3D, spawnZ, itemStack);
                double var20 = random.nextDouble() * 0.1D + 0.2D;
                item.velocityX = (double)dirX * var20;
                item.velocityY = (double)0.2F;
                item.velocityZ = (double)dirZ * var20;
                item.velocityX += random.nextGaussian() * (double)0.0075F * 6.0D;
                item.velocityY += random.nextGaussian() * (double)0.0075F * 6.0D;
                item.velocityZ += random.nextGaussian() * (double)0.0075F * 6.0D;
                world.spawnEntity(item);
                world.worldEvent(1000, x, y, z, 0);
            }

            world.worldEvent(2000, x, y, z, dirX + 1 + (dirZ + 1) * 3);
        }

    }

    public override void neighborUpdate(World world, int x, int y, int z, int id)
    {
        if (id > 0 && Block.BLOCKS[id].canEmitRedstonePower())
        {
            bool isPowered = world.isPowered(x, y, z) || world.isPowered(x, y + 1, z);
            if (isPowered)
            {
                world.scheduleBlockUpdate(x, y, z, base.id, getTickRate());
            }
        }

    }

    public override void onTick(World world, int x, int y, int z, java.util.Random random)
    {
        if (world.isPowered(x, y, z) || world.isPowered(x, y + 1, z))
        {
            dispense(world, x, y, z, random);
        }

    }

    protected override BlockEntity getBlockEntity()
    {
        return new BlockEntityDispenser();
    }

    public override void onPlaced(World world, int x, int y, int z, EntityLiving placer)
    {
        int direction = MathHelper.floor_double((double)(placer.yaw * 4.0F / 360.0F) + 0.5D) & 3;
        if (direction == 0)
        {
            world.setBlockMeta(x, y, z, 2);
        }

        if (direction == 1)
        {
            world.setBlockMeta(x, y, z, 5);
        }

        if (direction == 2)
        {
            world.setBlockMeta(x, y, z, 3);
        }

        if (direction == 3)
        {
            world.setBlockMeta(x, y, z, 4);
        }

    }

    public override void onBreak(World world, int x, int y, int z)
    {
        BlockEntityDispenser dispenser = (BlockEntityDispenser)world.getBlockEntity(x, y, z);

        for (int slotIndex = 0; slotIndex < dispenser.size(); ++slotIndex)
        {
            ItemStack stack = dispenser.getStack(slotIndex);
            if (stack != null)
            {
                float offsetX = random.nextFloat() * 0.8F + 0.1F;
                float offsetY = random.nextFloat() * 0.8F + 0.1F;
                float offsetZ = random.nextFloat() * 0.8F + 0.1F;

                while (stack.count > 0)
                {
                    int amount = random.nextInt(21) + 10;
                    if (amount > stack.count)
                    {
                        amount = stack.count;
                    }

                    stack.count -= amount;
                    EntityItem entityItem = new EntityItem(world, (double)((float)x + offsetX), (double)((float)y + offsetY), (double)((float)z + offsetZ), new ItemStack(stack.itemId, amount, stack.getDamage()));
                    float var13 = 0.05F;
                    entityItem.velocityX = (double)((float)random.nextGaussian() * var13);
                    entityItem.velocityY = (double)((float)random.nextGaussian() * var13 + 0.2F);
                    entityItem.velocityZ = (double)((float)random.nextGaussian() * var13);
                    world.spawnEntity(entityItem);
                }
            }
        }

        base.onBreak(world, x, y, z);
    }
}