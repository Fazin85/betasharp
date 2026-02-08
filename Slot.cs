using betareborn.Inventorys;
using betareborn.Items;

namespace betareborn
{
    public class Slot : java.lang.Object
    {
        private readonly int slotIndex;
        private readonly IInventory inventory;
        public int slotNumber;
        public int xDisplayPosition;
        public int yDisplayPosition;

        public Slot(IInventory var1, int var2, int var3, int var4)
        {
            inventory = var1;
            slotIndex = var2;
            xDisplayPosition = var3;
            yDisplayPosition = var4;
        }

        public virtual void onPickupFromSlot(ItemStack var1)
        {
            onSlotChanged();
        }

        public virtual bool isItemValid(ItemStack var1)
        {
            return true;
        }

        public ItemStack getStack()
        {
            return inventory.getStack(slotIndex);
        }

        public bool getHasStack()
        {
            return getStack() != null;
        }

        public void putStack(ItemStack var1)
        {
            inventory.setStack(slotIndex, var1);
            onSlotChanged();
        }

        public void onSlotChanged()
        {
            inventory.markDirty();
        }

        public virtual int getSlotStackLimit()
        {
            return inventory.getMaxCountPerStack();
        }

        public int getBackgroundIconIndex()
        {
            return -1;
        }

        public ItemStack decrStackSize(int var1)
        {
            return inventory.removeStack(slotIndex, var1);
        }
    }

}