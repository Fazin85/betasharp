using BetaSharp.Entities;
using BetaSharp.Items;

namespace BetaSharp.Inventories;

public class InventoryLargeChest : java.lang.Object, IInventory
{
    private readonly IInventory _upperChest;
    private readonly IInventory _lowerChest;

    public InventoryLargeChest(string name, IInventory upperChest, IInventory lowerChest)
    {
        Name = name;
        _upperChest = upperChest;
        _lowerChest = lowerChest;
    }

    public int Size
    {
        get => _upperChest.Size + _lowerChest.Size;
    }

    public string Name
    {
        get;
    }

    public ItemStack? GetStack(int slotIndex)
    {
        return slotIndex >= _upperChest.Size ? _lowerChest.GetStack(slotIndex - _upperChest.Size) : _upperChest.GetStack(slotIndex);
    }

    public ItemStack? RemoveStack(int slotIndex, int amount)
    {
        return slotIndex >= _upperChest.Size ? _lowerChest.RemoveStack(slotIndex - _upperChest.Size, amount) : _upperChest.RemoveStack(slotIndex, amount);
    }

    public void SetStack(int slotIndex, ItemStack? itemStack)
    {
        if (slotIndex >= _upperChest.Size)
        {
            _lowerChest.SetStack(slotIndex - _upperChest.Size, itemStack);
        }
        else
        {
            _upperChest.SetStack(slotIndex, itemStack);
        }

    }
    public int MaxCountPerStack
    {
        get => _upperChest.MaxCountPerStack;
    }

    public void MarkDirty()
    {
        _upperChest.MarkDirty();
        _lowerChest.MarkDirty();
    }

    public bool CanPlayerUse(EntityPlayer entityPlayer)
    {
        return _upperChest.CanPlayerUse(entityPlayer) && _lowerChest.CanPlayerUse(entityPlayer);
    }
}
