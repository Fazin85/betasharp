using betareborn.Entities;
using betareborn.Items;
using java.util;

namespace betareborn.Inventorys
{
    public class InventoryBasic : IInventory
    {

        private string inventoryTitle;
        private int slotsCount;
        private ItemStack[] inventoryContents;
        private List field_20073_d;

        public InventoryBasic(string var1, int var2)
        {
            inventoryTitle = var1;
            slotsCount = var2;
            inventoryContents = new ItemStack[var2];
        }

        public ItemStack getStack(int var1)
        {
            return inventoryContents[var1];
        }

        public ItemStack removeStack(int var1, int var2)
        {
            if (inventoryContents[var1] != null)
            {
                ItemStack var3;
                if (inventoryContents[var1].count <= var2)
                {
                    var3 = inventoryContents[var1];
                    inventoryContents[var1] = null;
                    markDirty();
                    return var3;
                }
                else
                {
                    var3 = inventoryContents[var1].splitStack(var2);
                    if (inventoryContents[var1].count == 0)
                    {
                        inventoryContents[var1] = null;
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

        public void setStack(int var1, ItemStack var2)
        {
            inventoryContents[var1] = var2;
            if (var2 != null && var2.count > getMaxCountPerStack())
            {
                var2.count = getMaxCountPerStack();
            }

            markDirty();
        }

        public int size()
        {
            return slotsCount;
        }

        public string getName()
        {
            return inventoryTitle;
        }

        public int getMaxCountPerStack()
        {
            return 64;
        }

        public void markDirty()
        {
            if (field_20073_d != null)
            {
                for (int var1 = 0; var1 < field_20073_d.size(); ++var1)
                {
                    ((IInvBasic)field_20073_d.get(var1)).func_20134_a(this);
                }
            }

        }

        public bool canPlayerUse(EntityPlayer var1)
        {
            return true;
        }
    }

}