using betareborn.Entities;
using betareborn.Items;

namespace betareborn.Inventorys
{
    public class InventoryLargeChest : java.lang.Object, IInventory
    {
        private string name;
        private IInventory upperChest;
        private IInventory lowerChest;

        public InventoryLargeChest(string var1, IInventory var2, IInventory var3)
        {
            name = var1;
            upperChest = var2;
            lowerChest = var3;
        }

        public int size()
        {
            return upperChest.size() + lowerChest.size();
        }

        public string getName()
        {
            return name;
        }

        public ItemStack getStack(int var1)
        {
            return var1 >= upperChest.size() ? lowerChest.getStack(var1 - upperChest.size()) : upperChest.getStack(var1);
        }

        public ItemStack removeStack(int var1, int var2)
        {
            return var1 >= upperChest.size() ? lowerChest.removeStack(var1 - upperChest.size(), var2) : upperChest.removeStack(var1, var2);
        }

        public void setStack(int var1, ItemStack var2)
        {
            if (var1 >= upperChest.size())
            {
                lowerChest.setStack(var1 - upperChest.size(), var2);
            }
            else
            {
                upperChest.setStack(var1, var2);
            }

        }

        public int getMaxCountPerStack()
        {
            return upperChest.getMaxCountPerStack();
        }

        public void markDirty()
        {
            upperChest.markDirty();
            lowerChest.markDirty();
        }

        public bool canPlayerUse(EntityPlayer var1)
        {
            return upperChest.canPlayerUse(var1) && lowerChest.canPlayerUse(var1);
        }
    }

}