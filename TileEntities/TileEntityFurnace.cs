using betareborn.Blocks;
using betareborn.Entities;
using betareborn.Items;
using betareborn.Materials;
using betareborn.NBT;

namespace betareborn.TileEntities
{
    public class TileEntityFurnace : TileEntity, IInventory
    {
        private ItemStack[] inventory = new ItemStack[3];
        public int burnTime = 0;
        public int fuelTime = 0;
        public int cookTime = 0;

        public int size()
        {
            return inventory.Length;
        }

        public ItemStack getStack(int slot)
        {
            return inventory[slot];
        }

        public ItemStack removeStack(int slot, int stack)
        {
            if (inventory[slot] != null)
            {
                ItemStack var3;
                if (inventory[slot].count <= stack)
                {
                    var3 = inventory[slot];
                    inventory[slot] = null;
                    return var3;
                }
                else
                {
                    var3 = inventory[slot].splitStack(stack);
                    if (inventory[slot].count == 0)
                    {
                        inventory[slot] = null;
                    }

                    return var3;
                }
            }
            else
            {
                return null;
            }
        }

        public void setStack(int slot, ItemStack stack)
        {
            inventory[slot] = stack;
            if (stack != null && stack.count > getMaxCountPerStack())
            {
                stack.count = getMaxCountPerStack();
            }

        }

        public string getName()
        {
            return "Furnace";
        }

        public override void readNbt(NBTTagCompound nbt)
        {
            base.readNbt(nbt);
            NBTTagList var2 = nbt.getTagList("Items");
            inventory = new ItemStack[size()];

            for (int var3 = 0; var3 < var2.tagCount(); ++var3)
            {
                NBTTagCompound var4 = (NBTTagCompound)var2.tagAt(var3);
                sbyte var5 = var4.getByte("Slot");
                if (var5 >= 0 && var5 < inventory.Length)
                {
                    inventory[var5] = new ItemStack(var4);
                }
            }

            burnTime = nbt.getShort("BurnTime");
            cookTime = nbt.getShort("CookTime");
            fuelTime = getFuelTime(inventory[1]);
        }

        public override void writeNbt(NBTTagCompound nbt)
        {
            base.writeNbt(nbt);
            nbt.setShort("BurnTime", (short)burnTime);
            nbt.setShort("CookTime", (short)cookTime);
            NBTTagList var2 = new NBTTagList();

            for (int var3 = 0; var3 < inventory.Length; ++var3)
            {
                if (inventory[var3] != null)
                {
                    NBTTagCompound var4 = new NBTTagCompound();
                    var4.setByte("Slot", (sbyte)var3);
                    inventory[var3].writeToNBT(var4);
                    var2.setTag(var4);
                }
            }

            nbt.setTag("Items", var2);
        }

        public int getMaxCountPerStack()
        {
            return 64;
        }

        public int getCookTimeDelta(int multiplier)
        {
            return cookTime * multiplier / 200;
        }

        public int getFuelTimeDelta(int multiplier)
        {
            if (fuelTime == 0)
            {
                fuelTime = 200;
            }

            return burnTime * multiplier / fuelTime;
        }

        public bool isBurning()
        {
            return burnTime > 0;
        }

        public override void tick()
        {
            bool var1 = burnTime > 0;
            bool var2 = false;
            if (burnTime > 0)
            {
                --burnTime;
            }

            if (!world.multiplayerWorld)
            {
                if (burnTime == 0 && canAcceptRecipeOutput())
                {
                    fuelTime = burnTime = getFuelTime(inventory[1]);
                    if (burnTime > 0)
                    {
                        var2 = true;
                        if (inventory[1] != null)
                        {
                            --inventory[1].count;
                            if (inventory[1].count == 0)
                            {
                                inventory[1] = null;
                            }
                        }
                    }
                }

                if (isBurning() && canAcceptRecipeOutput())
                {
                    ++cookTime;
                    if (cookTime == 200)
                    {
                        cookTime = 0;
                        craftRecipe();
                        var2 = true;
                    }
                }
                else
                {
                    cookTime = 0;
                }

                if (var1 != burnTime > 0)
                {
                    var2 = true;
                    BlockFurnace.updateFurnaceBlockState(burnTime > 0, world, x, y, z);
                }
            }

            if (var2)
            {
                markDirty();
            }

        }

        private bool canAcceptRecipeOutput()
        {
            if (inventory[0] == null)
            {
                return false;
            }
            else
            {
                ItemStack var1 = SmeltingRecipeManager.getInstance().craft(inventory[0].getItem().id);
                return var1 == null ? false : (inventory[2] == null ? true : (!inventory[2].isItemEqual(var1) ? false : (inventory[2].count < getMaxCountPerStack() && inventory[2].count < inventory[2].getMaxCount() ? true : inventory[2].count < var1.getMaxCount())));
            }
        }

        public void craftRecipe()
        {
            if (canAcceptRecipeOutput())
            {
                ItemStack var1 = SmeltingRecipeManager.getInstance().craft(inventory[0].getItem().id);
                if (inventory[2] == null)
                {
                    inventory[2] = var1.copy();
                }
                else if (inventory[2].itemID == var1.itemID)
                {
                    ++inventory[2].count;
                }

                --inventory[0].count;
                if (inventory[0].count <= 0)
                {
                    inventory[0] = null;
                }

            }
        }

        private int getFuelTime(ItemStack itemStack)
        {
            if (itemStack == null)
            {
                return 0;
            }
            else
            {
                int var2 = itemStack.getItem().id;
                return var2 < 256 && Block.BLOCKS[var2].material == Material.WOOD ? 300 : (var2 == Item.stick.id ? 100 : (var2 == Item.coal.id ? 1600 : (var2 == Item.bucketLava.id ? 20000 : (var2 == Block.SAPLING.id ? 100 : 0))));
            }
        }

        public bool canPlayerUse(EntityPlayer player)
        {
            return world.getBlockTileEntity(x, y, z) != this ? false : player.getSquaredDistance((double)x + 0.5D, (double)y + 0.5D, (double)z + 0.5D) <= 64.0D;
        }
    }

}