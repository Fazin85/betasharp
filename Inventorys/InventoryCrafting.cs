using betareborn.Containers;
using betareborn.Entities;
using betareborn.Items;

namespace betareborn.Inventorys
{
    public class InventoryCrafting : java.lang.Object, IInventory
    {
        private ItemStack[] stackList;
        private int field_21104_b;
        private Container eventHandler;

        public InventoryCrafting(Container var1, int var2, int var3)
        {
            int var4 = var2 * var3;
            stackList = new ItemStack[var4];
            eventHandler = var1;
            field_21104_b = var2;
        }

        public int size()
        {
            return stackList.Length;
        }

        public ItemStack getStack(int var1)
        {
            return var1 >= size() ? null : stackList[var1];
        }

        public ItemStack func_21103_b(int var1, int var2)
        {
            if (var1 >= 0 && var1 < field_21104_b)
            {
                int var3 = var1 + var2 * field_21104_b;
                return getStack(var3);
            }
            else
            {
                return null;
            }
        }

        public string getName()
        {
            return "Crafting";
        }

        public ItemStack removeStack(int var1, int var2)
        {
            if (stackList[var1] != null)
            {
                ItemStack var3;
                if (stackList[var1].count <= var2)
                {
                    var3 = stackList[var1];
                    stackList[var1] = null;
                    eventHandler.onCraftMatrixChanged(this);
                    return var3;
                }
                else
                {
                    var3 = stackList[var1].splitStack(var2);
                    if (stackList[var1].count == 0)
                    {
                        stackList[var1] = null;
                    }

                    eventHandler.onCraftMatrixChanged(this);
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
            stackList[var1] = var2;
            eventHandler.onCraftMatrixChanged(this);
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