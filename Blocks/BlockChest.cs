using betareborn.Entities;
using betareborn.Items;
using betareborn.Materials;
using betareborn.TileEntities;
using betareborn.Worlds;

namespace betareborn.Blocks
{
    public class BlockChest : BlockContainer
    {
        private java.util.Random random = new();

        public BlockChest(int var1) : base(var1, Material.WOOD)
        {
            textureId = 26;
        }

        public override int getTexture(BlockView var1, int var2, int var3, int var4, int var5)
        {
            if (var5 == 1)
            {
                return textureId - 1;
            }
            else if (var5 == 0)
            {
                return textureId - 1;
            }
            else
            {
                int var6 = var1.getBlockId(var2, var3, var4 - 1);
                int var7 = var1.getBlockId(var2, var3, var4 + 1);
                int var8 = var1.getBlockId(var2 - 1, var3, var4);
                int var9 = var1.getBlockId(var2 + 1, var3, var4);
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

                        return var5 == var14 ? textureId + 1 : textureId;
                    }
                    else if (var5 != 4 && var5 != 5)
                    {
                        var10 = 0;
                        if (var8 == id)
                        {
                            var10 = -1;
                        }

                        var11 = var1.getBlockId(var8 == id ? var2 - 1 : var2 + 1, var3, var4 - 1);
                        var12 = var1.getBlockId(var8 == id ? var2 - 1 : var2 + 1, var3, var4 + 1);
                        if (var5 == 3)
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

                        return (var5 == var13 ? textureId + 16 : textureId + 32) + var10;
                    }
                    else
                    {
                        return textureId;
                    }
                }
                else if (var5 != 2 && var5 != 3)
                {
                    var10 = 0;
                    if (var6 == id)
                    {
                        var10 = -1;
                    }

                    var11 = var1.getBlockId(var2 - 1, var3, var6 == id ? var4 - 1 : var4 + 1);
                    var12 = var1.getBlockId(var2 + 1, var3, var6 == id ? var4 - 1 : var4 + 1);
                    if (var5 == 4)
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

                    return (var5 == var13 ? textureId + 16 : textureId + 32) + var10;
                }
                else
                {
                    return textureId;
                }
            }
        }

        public override int getTexture(int var1)
        {
            return var1 == 1 ? textureId - 1 : (var1 == 0 ? textureId - 1 : (var1 == 3 ? textureId + 1 : textureId));
        }

        public override bool canPlaceBlockAt(World var1, int var2, int var3, int var4)
        {
            int var5 = 0;
            if (var1.getBlockId(var2 - 1, var3, var4) == id)
            {
                ++var5;
            }

            if (var1.getBlockId(var2 + 1, var3, var4) == id)
            {
                ++var5;
            }

            if (var1.getBlockId(var2, var3, var4 - 1) == id)
            {
                ++var5;
            }

            if (var1.getBlockId(var2, var3, var4 + 1) == id)
            {
                ++var5;
            }

            return var5 > 1 ? false : (isThereANeighborChest(var1, var2 - 1, var3, var4) ? false : (isThereANeighborChest(var1, var2 + 1, var3, var4) ? false : (isThereANeighborChest(var1, var2, var3, var4 - 1) ? false : !isThereANeighborChest(var1, var2, var3, var4 + 1))));
        }

        private bool isThereANeighborChest(World var1, int var2, int var3, int var4)
        {
            return var1.getBlockId(var2, var3, var4) != id ? false : (var1.getBlockId(var2 - 1, var3, var4) == id ? true : (var1.getBlockId(var2 + 1, var3, var4) == id ? true : (var1.getBlockId(var2, var3, var4 - 1) == id ? true : var1.getBlockId(var2, var3, var4 + 1) == id)));
        }

        public override void onBlockRemoval(World var1, int var2, int var3, int var4)
        {
            TileEntityChest var5 = (TileEntityChest)var1.getBlockTileEntity(var2, var3, var4);

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
                        EntityItem var12 = new EntityItem(var1, (double)((float)var2 + var8), (double)((float)var3 + var9), (double)((float)var4 + var10), new ItemStack(var7.itemID, var11, var7.getItemDamage()));
                        float var13 = 0.05F;
                        var12.motionX = (double)((float)random.nextGaussian() * var13);
                        var12.motionY = (double)((float)random.nextGaussian() * var13 + 0.2F);
                        var12.motionZ = (double)((float)random.nextGaussian() * var13);
                        var1.spawnEntity(var12);
                    }
                }
            }

            base.onBlockRemoval(var1, var2, var3, var4);
        }

        public override bool onUse(World var1, int var2, int var3, int var4, EntityPlayer var5)
        {
            java.lang.Object var6 = (TileEntityChest)var1.getBlockTileEntity(var2, var3, var4);
            if (var1.shouldSuffocate(var2, var3 + 1, var4))
            {
                return true;
            }
            else if (var1.getBlockId(var2 - 1, var3, var4) == id && var1.shouldSuffocate(var2 - 1, var3 + 1, var4))
            {
                return true;
            }
            else if (var1.getBlockId(var2 + 1, var3, var4) == id && var1.shouldSuffocate(var2 + 1, var3 + 1, var4))
            {
                return true;
            }
            else if (var1.getBlockId(var2, var3, var4 - 1) == id && var1.shouldSuffocate(var2, var3 + 1, var4 - 1))
            {
                return true;
            }
            else if (var1.getBlockId(var2, var3, var4 + 1) == id && var1.shouldSuffocate(var2, var3 + 1, var4 + 1))
            {
                return true;
            }
            else
            {
                if (var1.getBlockId(var2 - 1, var3, var4) == id)
                {
                    var6 = new InventoryLargeChest("Large chest", (TileEntityChest)var1.getBlockTileEntity(var2 - 1, var3, var4), (IInventory)var6);
                }

                if (var1.getBlockId(var2 + 1, var3, var4) == id)
                {
                    var6 = new InventoryLargeChest("Large chest", (IInventory)var6, (TileEntityChest)var1.getBlockTileEntity(var2 + 1, var3, var4));
                }

                if (var1.getBlockId(var2, var3, var4 - 1) == id)
                {
                    var6 = new InventoryLargeChest("Large chest", (TileEntityChest)var1.getBlockTileEntity(var2, var3, var4 - 1), (IInventory)var6);
                }

                if (var1.getBlockId(var2, var3, var4 + 1) == id)
                {
                    var6 = new InventoryLargeChest("Large chest", (IInventory)var6, (TileEntityChest)var1.getBlockTileEntity(var2, var3, var4 + 1));
                }

                if (var1.multiplayerWorld)
                {
                    return true;
                }
                else
                {
                    var5.displayGUIChest((IInventory)var6);
                    return true;
                }
            }
        }

        protected override TileEntity getBlockEntity()
        {
            return new TileEntityChest();
        }
    }

}