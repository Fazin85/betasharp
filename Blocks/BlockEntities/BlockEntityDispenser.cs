using betareborn.Entities;
using betareborn.Inventorys;
using betareborn.Items;
using betareborn.NBT;

namespace betareborn.Blocks.BlockEntities
{
    public class BlockEntityDispenser : BlockEntity, IInventory
    {
        private ItemStack[] inventory = new ItemStack[9];
        private readonly java.util.Random random = new();

        public int size()
        {
            return 9;
        }

        public ItemStack getStack(int slot)
        {
            return inventory[slot];
        }

        public ItemStack removeStack(int slot, int amount)
        {
            if (inventory[slot] != null)
            {
                ItemStack var3;
                if (inventory[slot].count <= amount)
                {
                    var3 = inventory[slot];
                    inventory[slot] = null;
                    markDirty();
                    return var3;
                }
                else
                {
                    var3 = inventory[slot].splitStack(amount);
                    if (inventory[slot].count == 0)
                    {
                        inventory[slot] = null;
                    }

                    markDirty();
                    return var3;
                }
            }
            else
            {
                return null;
            }
        }

        public ItemStack getItemToDispose()
        {
            int var1 = -1;
            int var2 = 1;

            for (int var3 = 0; var3 < inventory.Length; ++var3)
            {
                if (inventory[var3] != null && random.nextInt(var2++) == 0)
                {
                    var1 = var3;
                }
            }

            if (var1 >= 0)
            {
                return removeStack(var1, 1);
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

            markDirty();
        }

        public string getName()
        {
            return "Trap";
        }

        public override void readNbt(NBTTagCompound nbt)
        {
            base.readNbt(nbt);
            NBTTagList var2 = nbt.getTagList("Items");
            inventory = new ItemStack[size()];

            for (int var3 = 0; var3 < var2.tagCount(); ++var3)
            {
                NBTTagCompound var4 = (NBTTagCompound)var2.tagAt(var3);
                int var5 = var4.getByte("Slot") & 255;
                if (var5 >= 0 && var5 < inventory.Length)
                {
                    inventory[var5] = new ItemStack(var4);
                }
            }

        }

        public override void writeNbt(NBTTagCompound nbt)
        {
            base.writeNbt(nbt);
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

        public bool canPlayerUse(EntityPlayer player)
        {
            return world.getBlockEntity(x, y, z) != this ? false : player.getSquaredDistance(x + 0.5D, y + 0.5D, z + 0.5D) <= 64.0D;
        }
    }
}