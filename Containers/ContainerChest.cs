using betareborn.Entities;
using betareborn.Inventorys;
using betareborn.Items;

namespace betareborn.Containers
{
    public class ContainerChest : Container
    {

        private IInventory field_20125_a;
        private int field_27282_b;

        public ContainerChest(IInventory var1, IInventory var2)
        {
            field_20125_a = var2;
            field_27282_b = var2.size() / 9;
            int var3 = (field_27282_b - 4) * 18;

            int var4;
            int var5;
            for (var4 = 0; var4 < field_27282_b; ++var4)
            {
                for (var5 = 0; var5 < 9; ++var5)
                {
                    addSlot(new Slot(var2, var5 + var4 * 9, 8 + var5 * 18, 18 + var4 * 18));
                }
            }

            for (var4 = 0; var4 < 3; ++var4)
            {
                for (var5 = 0; var5 < 9; ++var5)
                {
                    addSlot(new Slot(var1, var5 + var4 * 9 + 9, 8 + var5 * 18, 103 + var4 * 18 + var3));
                }
            }

            for (var4 = 0; var4 < 9; ++var4)
            {
                addSlot(new Slot(var1, var4, 8 + var4 * 18, 161 + var3));
            }

        }

        public override bool isUsableByPlayer(EntityPlayer var1)
        {
            return field_20125_a.canPlayerUse(var1);
        }

        public override ItemStack getStackInSlot(int var1)
        {
            ItemStack var2 = null;
            Slot var3 = (Slot)slots.get(var1);
            if (var3 != null && var3.getHasStack())
            {
                ItemStack var4 = var3.getStack();
                var2 = var4.copy();
                if (var1 < field_27282_b * 9)
                {
                    func_28125_a(var4, field_27282_b * 9, slots.size(), true);
                }
                else
                {
                    func_28125_a(var4, 0, field_27282_b * 9, false);
                }

                if (var4.count == 0)
                {
                    var3.putStack((ItemStack)null);
                }
                else
                {
                    var3.onSlotChanged();
                }
            }

            return var2;
        }
    }

}