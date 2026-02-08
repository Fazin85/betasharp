using betareborn.Entities;
using betareborn.Items;
using betareborn.Worlds;
using betareborn.Blocks.BlockEntities;
using betareborn.Blocks.Materials;
using betareborn.Inventorys;

namespace betareborn.Blocks
{
    public class BlockChest : BlockWithEntity
    {
        private java.util.Random random = new();

        public BlockChest(int id) : base(id, Material.WOOD)
        {
            textureId = 26;
        }

        public override int getTextureId(BlockView blockView, int x, int y, int z, int side)
        {
            if (side == 1)
            {
                return textureId - 1;
            }
            else if (side == 0)
            {
                return textureId - 1;
            }
            else
            {
                int var6 = blockView.getBlockId(x, y, z - 1);
                int var7 = blockView.getBlockId(x, y, z + 1);
                int var8 = blockView.getBlockId(x - 1, y, z);
                int var9 = blockView.getBlockId(x + 1, y, z);
                int var10;
                int var11;
                int var12;
                sbyte var13;
                if (var6 != id && var7 != id)
                {
                    if (var8 != id && var9 != id)
                    {
                        sbyte var14 = 3;
                        if (Block.BLOCKS_OPAQUE[var6] && !Block.BLOCKS_OPAQUE[var7])
                        {
                            var14 = 3;
                        }

                        if (Block.BLOCKS_OPAQUE[var7] && !Block.BLOCKS_OPAQUE[var6])
                        {
                            var14 = 2;
                        }

                        if (Block.BLOCKS_OPAQUE[var8] && !Block.BLOCKS_OPAQUE[var9])
                        {
                            var14 = 5;
                        }

                        if (Block.BLOCKS_OPAQUE[var9] && !Block.BLOCKS_OPAQUE[var8])
                        {
                            var14 = 4;
                        }

                        return side == var14 ? textureId + 1 : textureId;
                    }
                    else if (side != 4 && side != 5)
                    {
                        var10 = 0;
                        if (var8 == id)
                        {
                            var10 = -1;
                        }

                        var11 = blockView.getBlockId(var8 == id ? x - 1 : x + 1, y, z - 1);
                        var12 = blockView.getBlockId(var8 == id ? x - 1 : x + 1, y, z + 1);
                        if (side == 3)
                        {
                            var10 = -1 - var10;
                        }

                        var13 = 3;
                        if ((Block.BLOCKS_OPAQUE[var6] || Block.BLOCKS_OPAQUE[var11]) && !Block.BLOCKS_OPAQUE[var7] && !Block.BLOCKS_OPAQUE[var12])
                        {
                            var13 = 3;
                        }

                        if ((Block.BLOCKS_OPAQUE[var7] || Block.BLOCKS_OPAQUE[var12]) && !Block.BLOCKS_OPAQUE[var6] && !Block.BLOCKS_OPAQUE[var11])
                        {
                            var13 = 2;
                        }

                        return (side == var13 ? textureId + 16 : textureId + 32) + var10;
                    }
                    else
                    {
                        return textureId;
                    }
                }
                else if (side != 2 && side != 3)
                {
                    var10 = 0;
                    if (var6 == id)
                    {
                        var10 = -1;
                    }

                    var11 = blockView.getBlockId(x - 1, y, var6 == id ? z - 1 : z + 1);
                    var12 = blockView.getBlockId(x + 1, y, var6 == id ? z - 1 : z + 1);
                    if (side == 4)
                    {
                        var10 = -1 - var10;
                    }

                    var13 = 5;
                    if ((Block.BLOCKS_OPAQUE[var8] || Block.BLOCKS_OPAQUE[var11]) && !Block.BLOCKS_OPAQUE[var9] && !Block.BLOCKS_OPAQUE[var12])
                    {
                        var13 = 5;
                    }

                    if ((Block.BLOCKS_OPAQUE[var9] || Block.BLOCKS_OPAQUE[var12]) && !Block.BLOCKS_OPAQUE[var8] && !Block.BLOCKS_OPAQUE[var11])
                    {
                        var13 = 4;
                    }

                    return (side == var13 ? textureId + 16 : textureId + 32) + var10;
                }
                else
                {
                    return textureId;
                }
            }
        }

        public override int getTexture(int side)
        {
            return side == 1 ? textureId - 1 : (side == 0 ? textureId - 1 : (side == 3 ? textureId + 1 : textureId));
        }

        public override bool canPlaceAt(World world, int x, int y, int z)
        {
            int var5 = 0;
            if (world.getBlockId(x - 1, y, z) == id)
            {
                ++var5;
            }

            if (world.getBlockId(x + 1, y, z) == id)
            {
                ++var5;
            }

            if (world.getBlockId(x, y, z - 1) == id)
            {
                ++var5;
            }

            if (world.getBlockId(x, y, z + 1) == id)
            {
                ++var5;
            }

            return var5 > 1 ? false : (hasNeighbor(world, x - 1, y, z) ? false : (hasNeighbor(world, x + 1, y, z) ? false : (hasNeighbor(world, x, y, z - 1) ? false : !hasNeighbor(world, x, y, z + 1))));
        }

        private bool hasNeighbor(World world, int x, int y, int z)
        {
            return world.getBlockId(x, y, z) != id ? false : (world.getBlockId(x - 1, y, z) == id ? true : (world.getBlockId(x + 1, y, z) == id ? true : (world.getBlockId(x, y, z - 1) == id ? true : world.getBlockId(x, y, z + 1) == id)));
        }

        public override void onBreak(World world, int x, int y, int z)
        {
            BlockEntityChest var5 = (BlockEntityChest)world.getBlockEntity(x, y, z);

            for (int var6 = 0; var6 < var5.size(); ++var6)
            {
                ItemStack var7 = var5.getStack(var6);
                if (var7 != null)
                {
                    float var8 = random.nextFloat() * 0.8F + 0.1F;
                    float var9 = random.nextFloat() * 0.8F + 0.1F;
                    float var10 = random.nextFloat() * 0.8F + 0.1F;

                    while (var7.count > 0)
                    {
                        int var11 = random.nextInt(21) + 10;
                        if (var11 > var7.count)
                        {
                            var11 = var7.count;
                        }

                        var7.count -= var11;
                        EntityItem var12 = new EntityItem(world, (double)((float)x + var8), (double)((float)y + var9), (double)((float)z + var10), new ItemStack(var7.itemID, var11, var7.getItemDamage()));
                        float var13 = 0.05F;
                        var12.motionX = (double)((float)random.nextGaussian() * var13);
                        var12.motionY = (double)((float)random.nextGaussian() * var13 + 0.2F);
                        var12.motionZ = (double)((float)random.nextGaussian() * var13);
                        world.spawnEntity(var12);
                    }
                }
            }

            base.onBreak(world, x, y, z);
        }

        public override bool onUse(World world, int x, int y, int z, EntityPlayer player)
        {
            java.lang.Object var6 = (BlockEntityChest)world.getBlockEntity(x, y, z);
            if (world.shouldSuffocate(x, y + 1, z))
            {
                return true;
            }
            else if (world.getBlockId(x - 1, y, z) == id && world.shouldSuffocate(x - 1, y + 1, z))
            {
                return true;
            }
            else if (world.getBlockId(x + 1, y, z) == id && world.shouldSuffocate(x + 1, y + 1, z))
            {
                return true;
            }
            else if (world.getBlockId(x, y, z - 1) == id && world.shouldSuffocate(x, y + 1, z - 1))
            {
                return true;
            }
            else if (world.getBlockId(x, y, z + 1) == id && world.shouldSuffocate(x, y + 1, z + 1))
            {
                return true;
            }
            else
            {
                if (world.getBlockId(x - 1, y, z) == id)
                {
                    var6 = new InventoryLargeChest("Large chest", (BlockEntityChest)world.getBlockEntity(x - 1, y, z), (IInventory)var6);
                }

                if (world.getBlockId(x + 1, y, z) == id)
                {
                    var6 = new InventoryLargeChest("Large chest", (IInventory)var6, (BlockEntityChest)world.getBlockEntity(x + 1, y, z));
                }

                if (world.getBlockId(x, y, z - 1) == id)
                {
                    var6 = new InventoryLargeChest("Large chest", (BlockEntityChest)world.getBlockEntity(x, y, z - 1), (IInventory)var6);
                }

                if (world.getBlockId(x, y, z + 1) == id)
                {
                    var6 = new InventoryLargeChest("Large chest", (IInventory)var6, (BlockEntityChest)world.getBlockEntity(x, y, z + 1));
                }

                if (world.isRemote)
                {
                    return true;
                }
                else
                {
                    player.displayGUIChest((IInventory)var6);
                    return true;
                }
            }
        }

        protected override BlockEntity getBlockEntity()
        {
            return new BlockEntityChest();
        }
    }

}