using BetaSharp.Blocks.Entities;
using BetaSharp.Blocks.Materials;
using BetaSharp.Entities;
using BetaSharp.Items;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds;

namespace BetaSharp.Blocks;

public class BlockFurnace : BlockWithEntity
{

    private java.util.Random random = new();
    private readonly bool lit;
    private static bool ignoreBlockRemoval = false;

    public BlockFurnace(int id, bool lit) : base(id, Material.Stone)
    {
        this.lit = lit;
        textureId = 45;
    }

    public override int getDroppedItemId(int blockMeta, java.util.Random random)
    {
        return Block.FURNACE.id;
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
            int westBlockId = world.getBlockId(x - 1, y, z);
            int eastBlockId = world.getBlockId(x + 1, y, z);
            sbyte direction = 3;
            if (Block.BLOCKS_OPAQUE[blockNorth] && !Block.BLOCKS_OPAQUE[blockSouth])
            {
                direction = 3;
            }

            if (Block.BLOCKS_OPAQUE[blockSouth] && !Block.BLOCKS_OPAQUE[blockNorth])
            {
                direction = 2;
            }

            if (Block.BLOCKS_OPAQUE[westBlockId] && !Block.BLOCKS_OPAQUE[eastBlockId])
            {
                direction = 5;
            }

            if (Block.BLOCKS_OPAQUE[eastBlockId] && !Block.BLOCKS_OPAQUE[westBlockId])
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
            return side != meta ? textureId : (lit ? textureId + 16 : textureId - 1);
        }
    }

    public override void randomDisplayTick(World world, int x, int y, int z, java.util.Random random)
    {
        if (lit)
        {
            int var6 = world.getBlockMeta(x, y, z);
            float particleX = (float)x + 0.5F;
            float particleY = (float)y + 0.0F + random.nextFloat() * 6.0F / 16.0F;
            float particleZ = (float)z + 0.5F;
            float flameOffset = 0.52F;
            float randomOffset = random.nextFloat() * 0.6F - 0.3F;
            if (var6 == 4)
            {
                world.addParticle("smoke", (double)(particleX - flameOffset), (double)particleY, (double)(particleZ + randomOffset), 0.0D, 0.0D, 0.0D);
                world.addParticle("flame", (double)(particleX - flameOffset), (double)particleY, (double)(particleZ + randomOffset), 0.0D, 0.0D, 0.0D);
            }
            else if (var6 == 5)
            {
                world.addParticle("smoke", (double)(particleX + flameOffset), (double)particleY, (double)(particleZ + randomOffset), 0.0D, 0.0D, 0.0D);
                world.addParticle("flame", (double)(particleX + flameOffset), (double)particleY, (double)(particleZ + randomOffset), 0.0D, 0.0D, 0.0D);
            }
            else if (var6 == 2)
            {
                world.addParticle("smoke", (double)(particleX + randomOffset), (double)particleY, (double)(particleZ - flameOffset), 0.0D, 0.0D, 0.0D);
                world.addParticle("flame", (double)(particleX + randomOffset), (double)particleY, (double)(particleZ - flameOffset), 0.0D, 0.0D, 0.0D);
            }
            else if (var6 == 3)
            {
                world.addParticle("smoke", (double)(particleX + randomOffset), (double)particleY, (double)(particleZ + flameOffset), 0.0D, 0.0D, 0.0D);
                world.addParticle("flame", (double)(particleX + randomOffset), (double)particleY, (double)(particleZ + flameOffset), 0.0D, 0.0D, 0.0D);
            }

        }
    }

    public override int getTexture(int side)
    {
        return side == 1 ? textureId + 17 : (side == 0 ? textureId + 17 : (side == 3 ? textureId - 1 : textureId));
    }

    public override bool onUse(World world, int x, int y, int z, EntityPlayer player)
    {
        if (world.isRemote)
        {
            return true;
        }
        else
        {
            BlockEntityFurnace furnace = (BlockEntityFurnace)world.getBlockEntity(x, y, z);
            player.openFurnaceScreen(furnace);
            return true;
        }
    }

    public static void updateLitState(bool lit, World world, int x, int y, int z)
    {
        int meta = world.getBlockMeta(x, y, z);
        BlockEntity furnace = world.getBlockEntity(x, y, z);
        ignoreBlockRemoval = true;
        if (lit)
        {
            world.setBlock(x, y, z, Block.LIT_FURNACE.id);
        }
        else
        {
            world.setBlock(x, y, z, Block.FURNACE.id);
        }

        ignoreBlockRemoval = false;
        world.setBlockMeta(x, y, z, meta);
        furnace.cancelRemoval();
        world.setBlockEntity(x, y, z, furnace);
    }

    protected override BlockEntity getBlockEntity()
    {
        return new BlockEntityFurnace();
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
        if (!ignoreBlockRemoval)
        {
            BlockEntityFurnace furnace = (BlockEntityFurnace)world.getBlockEntity(x, y, z);

            for (int slotIndex = 0; slotIndex < furnace.size(); ++slotIndex)
            {
                ItemStack stack = furnace.getStack(slotIndex);
                if (stack != null)
                {
                    float offsetX = random.nextFloat() * 0.8F + 0.1F;
                    float offsetY = random.nextFloat() * 0.8F + 0.1F;
                    float offsetZ = random.nextFloat() * 0.8F + 0.1F;

                    while (stack.count > 0)
                    {
                        int var11 = random.nextInt(21) + 10;
                        if (var11 > stack.count)
                        {
                            var11 = stack.count;
                        }

                        stack.count -= var11;
                        EntityItem droppedItem = new EntityItem(world, (double)((float)x + offsetX), (double)((float)y + offsetY), (double)((float)z + offsetZ), new ItemStack(stack.itemId, var11, stack.getDamage()));
                        float var13 = 0.05F;
                        droppedItem.velocityX = (double)((float)random.nextGaussian() * var13);
                        droppedItem.velocityY = (double)((float)random.nextGaussian() * var13 + 0.2F);
                        droppedItem.velocityZ = (double)((float)random.nextGaussian() * var13);
                        world.spawnEntity(droppedItem);
                    }
                }
            }
        }

        base.onBreak(world, x, y, z);
    }
}