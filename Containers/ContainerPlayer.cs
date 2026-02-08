using betareborn.Entities;
using betareborn.Inventorys;
using betareborn.Items;
using betareborn.Recipes;

namespace betareborn.Containers
{
    public class ContainerPlayer : Container
    {

        public InventoryCrafting craftMatrix;
        public IInventory craftResult;
        public bool isSinglePlayer;

        public ContainerPlayer(InventoryPlayer var1) : this(var1, true)
        {
        }

        public ContainerPlayer(InventoryPlayer var1, bool var2)
        {
            craftMatrix = new InventoryCrafting(this, 2, 2);
            craftResult = new InventoryCraftResult();
            isSinglePlayer = false;
            isSinglePlayer = var2;
            addSlot(new SlotCrafting(var1.player, craftMatrix, craftResult, 0, 144, 36));

            int var3;
            int var4;
            for (var3 = 0; var3 < 2; ++var3)
            {
                for (var4 = 0; var4 < 2; ++var4)
                {
                    addSlot(new Slot(craftMatrix, var4 + var3 * 2, 88 + var4 * 18, 26 + var3 * 18));
                }
            }

            for (var3 = 0; var3 < 4; ++var3)
            {
                addSlot(new SlotArmor(this, var1, var1.size() - 1 - var3, 8, 8 + var3 * 18, var3));
            }

            for (var3 = 0; var3 < 3; ++var3)
            {
                for (var4 = 0; var4 < 9; ++var4)
                {
                    addSlot(new Slot(var1, var4 + (var3 + 1) * 9, 8 + var4 * 18, 84 + var3 * 18));
                }
            }

            for (var3 = 0; var3 < 9; ++var3)
            {
                addSlot(new Slot(var1, var3, 8 + var3 * 18, 142));
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

            for (int var2 = 0; var2 < 4; ++var2)
            {
                ItemStack var3 = craftMatrix.getStack(var2);
                if (var3 != null)
                {
                    var1.dropPlayerItem(var3);
                    craftMatrix.setStack(var2, (ItemStack)null);
                }
            }

        }

        public override bool isUsableByPlayer(EntityPlayer var1)
        {
            return true;
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
                    func_28125_a(var4, 9, 45, true);
                }
                else if (var1 >= 9 && var1 < 36)
                {
                    func_28125_a(var4, 36, 45, false);
                }
                else if (var1 >= 36 && var1 < 45)
                {
                    func_28125_a(var4, 9, 36, false);
                }
                else
                {
                    func_28125_a(var4, 9, 45, false);
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