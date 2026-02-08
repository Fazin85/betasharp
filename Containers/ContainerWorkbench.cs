using betareborn.Blocks;
using betareborn.Entities;
using betareborn.Inventorys;
using betareborn.Items;
using betareborn.Recipes;
using betareborn.Worlds;

namespace betareborn.Containers
{
    public class ContainerWorkbench : Container
    {

        public InventoryCrafting craftMatrix;
        public IInventory craftResult = new InventoryCraftResult();
        private World field_20133_c;
        private int field_20132_h;
        private int field_20131_i;
        private int field_20130_j;

        public ContainerWorkbench(InventoryPlayer var1, World var2, int var3, int var4, int var5)
        {
            craftMatrix = new InventoryCrafting(this, 3, 3);
            field_20133_c = var2;
            field_20132_h = var3;
            field_20131_i = var4;
            field_20130_j = var5;
            addSlot(new SlotCrafting(var1.player, craftMatrix, craftResult, 0, 124, 35));

            int var6;
            int var7;
            for (var6 = 0; var6 < 3; ++var6)
            {
                for (var7 = 0; var7 < 3; ++var7)
                {
                    addSlot(new Slot(craftMatrix, var7 + var6 * 3, 30 + var7 * 18, 17 + var6 * 18));
                }
            }

            for (var6 = 0; var6 < 3; ++var6)
            {
                for (var7 = 0; var7 < 9; ++var7)
                {
                    addSlot(new Slot(var1, var7 + var6 * 9 + 9, 8 + var7 * 18, 84 + var6 * 18));
                }
            }

            for (var6 = 0; var6 < 9; ++var6)
            {
                addSlot(new Slot(var1, var6, 8 + var6 * 18, 142));
            }

            onCraftMatrixChanged(craftMatrix);
        }

        public override void onCraftMatrixChanged(IInventory var1)
        {
            craftResult.setStack(0, CraftingManager.getInstance().findMatchingRecipe(craftMatrix));
        }

        public override void onCraftGuiClosed(EntityPlayer var1)
        {
            base.onCraftGuiClosed(var1);
            if (!field_20133_c.isRemote)
            {
                for (int var2 = 0; var2 < 9; ++var2)
                {
                    ItemStack var3 = craftMatrix.getStack(var2);
                    if (var3 != null)
                    {
                        var1.dropPlayerItem(var3);
                    }
                }

            }
        }

        public override bool isUsableByPlayer(EntityPlayer var1)
        {
            return field_20133_c.getBlockId(field_20132_h, field_20131_i, field_20130_j) != Block.CRAFTING_TABLE.id ? false : var1.getSquaredDistance((double)field_20132_h + 0.5D, (double)field_20131_i + 0.5D, (double)field_20130_j + 0.5D) <= 64.0D;
        }

        public override ItemStack getStackInSlot(int var1)
        {
            ItemStack var2 = null;
            Slot var3 = (Slot)slots.get(var1);
            if (var3 != null && var3.getHasStack())
            {
                ItemStack var4 = var3.getStack();
                var2 = var4.copy();
                if (var1 == 0)
                {
                    func_28125_a(var4, 10, 46, true);
                }
                else if (var1 >= 10 && var1 < 37)
                {
                    func_28125_a(var4, 37, 46, false);
                }
                else if (var1 >= 37 && var1 < 46)
                {
                    func_28125_a(var4, 10, 37, false);
                }
                else
                {
                    func_28125_a(var4, 10, 46, false);
                }

                if (var4.count == 0)
                {
                    var3.putStack((ItemStack)null);
                }
                else
                {
                    var3.onSlotChanged();
                }

                if (var4.count == var2.count)
                {
                    return null;
                }

                var3.onPickupFromSlot(var4);
            }

            return var2;
        }
    }

}