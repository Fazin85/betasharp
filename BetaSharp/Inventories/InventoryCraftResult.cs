using BetaSharp.Entities;
using BetaSharp.Items;

namespace BetaSharp.Inventories;

public class InventoryCraftResult : IInventory
{
    private readonly ItemStack?[] _stackResult = new ItemStack[1];
    public string Name { get => "Result"; }
    public int Size { get => 1; }
    public int MaxCountPerStack { get => 64; }

    public ItemStack? GetStack(int slotIndex)
    {
        return _stackResult[slotIndex];
    }

    public ItemStack? RemoveStack(int slotIndex, int amount)
    {
        ItemStack? slot = _stackResult[slotIndex];

        if (slot is null)
            return null;

        ItemStack? removeStack = _stackResult[slotIndex];
        _stackResult[slotIndex] = null;
        return removeStack;
    }

    public void SetStack(int slotIndex, ItemStack? itemStack)
    {
        _stackResult[slotIndex] = itemStack;
    }

    public void MarkDirty()
    {
    }

    public bool CanPlayerUse(EntityPlayer entityPlayer)
    {
        return true;
    }
}
