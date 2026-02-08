using betareborn.Entities;
using betareborn.Items;

namespace betareborn.Inventorys
{
    public class InventoryCraftResult : IInventory
    {

        private ItemStack[] stackResult = new ItemStack[1];

        public int size()
        {
            return 1;
        }

        public ItemStack getStack(int var1)
        {
            return stackResult[var1];
        }

        public string getName()
        {
            return "Result";
        }

        public ItemStack removeStack(int var1, int var2)
        {
            if (stackResult[var1] != null)
            {
                ItemStack var3 = stackResult[var1];
                stackResult[var1] = null;
                return var3;
            }
            else
            {
                return null;
            }
        }

        public void setStack(int var1, ItemStack var2)
        {
            stackResult[var1] = var2;
        }

        public int getMaxCountPerStack()
        {
            return 64;
        }

        public void markDirty()
        {
        }

        public bool canPlayerUse(EntityPlayer var1)
        {
            return true;
        }
    }

}