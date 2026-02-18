using BetaSharp.Entities;
using BetaSharp.Items;
using BetaSharp.Screens;

namespace BetaSharp.Inventories;

public class InventoryCrafting : IInventory
{
    private readonly ItemStack?[] _stackList;
    private readonly int _gridWidth;
    private readonly ScreenHandler _eventHandler;

    public InventoryCrafting(ScreenHandler eventHandler, int gridWidth, int gridHeight)
    {
        Console.WriteLine($"new crafting: {gridWidth}, {gridHeight}"); // gets called twice
        int gridSize = gridWidth * gridHeight;

        _stackList = new ItemStack[gridSize];
        _eventHandler = eventHandler;
        _gridWidth = gridWidth;
    }

    public int Size
    {
        get => _stackList.Length;
    }

    public ItemStack? GetStack(int slotIndex)
    {
        // out-of-bounds check
        return slotIndex >= this.Size ? null : _stackList[slotIndex];
    }

    public ItemStack? GetStackAt(int x, int y)
    {
        if (x >= 0 && x < _gridWidth)
        {
            int slotIndex = x + y * _gridWidth;
            return GetStack(slotIndex);
        }
        else
        {
            return null;
        }
    }

    public string Name
    {
        get => "Crafting";
    }

    public ItemStack? RemoveStack(int slotIndex, int amount)
    {
        ItemStack? slot = _stackList[slotIndex];

        if (slot is null)
            return null;

        ItemStack removeStack;

        if (slot.count <= amount)
        {
            removeStack = slot;
            _stackList[slotIndex] = null;
        }
        else
        {
            removeStack = slot.Split(amount);

            if (slot.count == 0)
                _stackList[slotIndex] = null;
        }

        _eventHandler.onSlotUpdate(this);
        return removeStack;
    }

    public void SetStack(int slotIndex, ItemStack? itemStack)
    {
        _stackList[slotIndex] = itemStack;
        _eventHandler.onSlotUpdate(this);
    }

    public int MaxCountPerStack
    {
        get => 64;
    }


    public void MarkDirty()
    {
    }

    public bool CanPlayerUse(EntityPlayer entityPlayer)
    {
        return true;
    }
}
