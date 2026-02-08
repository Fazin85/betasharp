using betareborn.Entities;
using betareborn.Items;

namespace betareborn.Inventorys
{
    public interface IInventory
    {
        int size();

        ItemStack getStack(int var1);

        ItemStack removeStack(int var1, int var2);

        void setStack(int var1, ItemStack var2);

        string getName();

        int getMaxCountPerStack();

        void markDirty();

        bool canPlayerUse(EntityPlayer var1);
    }

}