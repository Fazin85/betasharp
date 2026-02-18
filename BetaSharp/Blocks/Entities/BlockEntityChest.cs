using BetaSharp.Entities;
using BetaSharp.Inventories;
using BetaSharp.Items;
using BetaSharp.NBT;

namespace BetaSharp.Blocks.Entities;

public class BlockEntityChest : BlockEntity, IInventory
{
    private ItemStack[] inventory = new ItemStack[36];

    public int Size
    {
        get => 27; // realize i can do this now
    }

    public void MarkDirty() => markDirty(); // todo: fix

    public ItemStack GetStack(int stackIndex)
    {
        return inventory[stackIndex];
    }

    public ItemStack RemoveStack(int slot, int amount)
    {
        if (inventory[slot] != null)
        {
            ItemStack itemStack;
            if (inventory[slot].count <= amount)
            {
                itemStack = inventory[slot];
                inventory[slot] = null;
                markDirty();
                return itemStack;
            }
            else
            {
                itemStack = inventory[slot].Split(amount);
                if (inventory[slot].count == 0)
                {
                    inventory[slot] = null;
                }

                markDirty();
                return itemStack;
            }
        }
        else
        {
            return null;
        }
    }

    public void SetStack(int slot, ItemStack? stack)
    {
        inventory[slot] = stack;
        if (stack != null && stack.count > MaxCountPerStack)
        {
            stack.count = MaxCountPerStack;
        }

        markDirty();
    }

    public string Name
    {
        get => "Chest";
    }


    public override void readNbt(NBTTagCompound nbt)
    {
        base.readNbt(nbt);
        NBTTagList itemList = nbt.GetTagList("Items");
        inventory = new ItemStack[this.Size];

        for (int itemIndex = 0; itemIndex < itemList.TagCount(); ++itemIndex)
        {
            NBTTagCompound itemsTag = (NBTTagCompound)itemList.TagAt(itemIndex);
            int slot = itemsTag.GetByte("Slot") & 255;
            if (slot >= 0 && slot < inventory.Length)
            {
                inventory[slot] = new ItemStack(itemsTag);
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
                NBTTagCompound itemsTag = new NBTTagCompound();
                itemsTag.SetByte("Slot", (sbyte)slotIndex);
                inventory[slotIndex].writeToNBT(itemsTag);
                itemList.SetTag(itemsTag);
            }
        }

        nbt.SetTag("Items", itemList);
    }


    public int MaxCountPerStack
    {
        get => 64;
    }

    public bool CanPlayerUse(EntityPlayer player)
    {
        return world.getBlockEntity(x, y, z) != this ? false : player.getSquaredDistance(x + 0.5D, y + 0.5D, z + 0.5D) <= 64.0D;
    }
}
