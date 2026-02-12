using betareborn.Entities;
using betareborn.Inventorys;
using betareborn.Items;
using betareborn.NBT;

namespace betareborn.Blocks.Entities
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
                ItemStack removedStack;
                if (inventory[slot].count <= amount)
                {
                    removedStack = inventory[slot];
                    inventory[slot] = null;
                    markDirty();
                    return removedStack;
                }
                else
                {
                    removedStack = inventory[slot].split(amount);
                    if (inventory[slot].count == 0)
                    {
                        inventory[slot] = null;
                    }

                    markDirty();
                    return removedStack;
                }
            }
            else
            {
                return null;
            }
        }

        public ItemStack getItemToDispose()
        {
            int selectedSlot = -1;
            int nonNullCount = 1;

            for (int slotIndex = 0; slotIndex < inventory.Length; ++slotIndex)
            {
                if (inventory[slotIndex] != null && random.nextInt(nonNullCount++) == 0)
                {
                    selectedSlot = slotIndex;
                }
            }

            if (selectedSlot >= 0)
            {
                return removeStack(selectedSlot, 1);
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
            NBTTagList itemList = nbt.getTagList("Items");
            inventory = new ItemStack[size()];

            for (int itemIndex = 0; itemIndex < itemList.tagCount(); ++itemIndex)
            {
                NBTTagCompound itemTag = (NBTTagCompound)itemList.tagAt(itemIndex);
                int slotIndex = itemTag.getByte("Slot") & 255;
                if (slotIndex >= 0 && slotIndex < inventory.Length)
                {
                    inventory[slotIndex] = new ItemStack(itemTag);
                }
            }

        }

        public override void writeNbt(NBTTagCompound nbt)
        {
            base.writeNbt(nbt);
            NBTTagList itemList = new NBTTagList();

            for (int slotIndex = 0; slotIndex < inventory.Length; ++slotIndex)
            {
                if (inventory[slotIndex] != null)
                {
                    NBTTagCompound itemTag = new NBTTagCompound();
                    itemTag.setByte("Slot", (sbyte)slotIndex);
                    inventory[slotIndex].writeToNBT(itemTag);
                    itemList.setTag(itemTag);
                }
            }

            nbt.setTag("Items", itemList);
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