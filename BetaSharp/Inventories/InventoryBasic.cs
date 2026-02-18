using BetaSharp.Entities;
using BetaSharp.Items;
using java.util;

namespace BetaSharp.Inventories;


// basic bare-bones implementation of IInventory for ClientNetworkHandler.cs
// literally only used once
public class InventoryBasic(string inventoryTitle, int slotsCount) : IInventory
{
    private readonly ItemStack?[] _inventoryContents = new ItemStack[slotsCount]; // just _inventoryContents.Length
    //private List field_20073_d;

    public ItemStack? GetStack(int slotIndex)
    {
        return _inventoryContents[slotIndex];
    }

    public ItemStack? RemoveStack(int slotIndex, int amount)
    {
        ItemStack? slot = _inventoryContents[slotIndex];

        if (slot is null)
            return null;

        ItemStack removeStack;

        if (slot.count <= amount)
        {
            removeStack = slot;
            _inventoryContents[slotIndex] = null;
        }
        else
        {
            removeStack = slot.Split(amount);
            if (slot.count == 0)
            {
                _inventoryContents[slotIndex] = null;
            }
        }

        MarkDirty();
        return removeStack;
    }

    public void SetStack(int slotIndex, ItemStack? itemStack)
    {
        _inventoryContents[slotIndex] = itemStack;

        if (itemStack != null && itemStack.count > this.MaxCountPerStack)
        {
            itemStack.count = this.MaxCountPerStack;
        }

        MarkDirty();
    }

    public int Size
    {
        get => _inventoryContents.Length;
    }

    public string Name
    {
        get => inventoryTitle;
    }

    public int MaxCountPerStack
    {
        get => 64;
    }

    public void MarkDirty()
    {
        /*
        Console.WriteLine($"field_xx: {field_20073_d is null}");
        if (field_20073_d != null)
        {
            for (int slotIndex = 0; slotIndex < field_20073_d.size(); ++slotIndex)
            {
                ((IInvBasic)field_20073_d.get(slotIndex)).func_20134_a(this);
            }
        }
        */

    }

    public bool CanPlayerUse(EntityPlayer entityPlayer)
    {
        return true;
    }
}
