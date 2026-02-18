using BetaSharp.Entities;
using BetaSharp.Items;

namespace BetaSharp.Inventories;

public interface IInventory
{
    int Size { get; }
    string Name { get; }

    int MaxCountPerStack { get; } // should this be static?

    ItemStack? GetStack(int slotIndex);

    ItemStack? RemoveStack(int slotIndex, int amount);

    void SetStack(int slotIndex, ItemStack? itemStack);

    // dirty == state has changed
    void MarkDirty();

    bool CanPlayerUse(EntityPlayer entityPlayer);
}
