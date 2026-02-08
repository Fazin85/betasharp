using betareborn.Entities;
using betareborn.Inventorys;
using betareborn.Items;
using java.util;

namespace betareborn.Containers
{
    public abstract class Container : java.lang.Object
    {
        public List field_20123_d = new ArrayList();
        public List slots = new ArrayList();
        public int windowId = 0;
        private short field_20917_a = 0;
        protected List field_20121_g = new ArrayList();
        private Set field_20918_b = new HashSet();

        protected void addSlot(Slot var1)
        {
            var1.slotNumber = slots.size();
            slots.add(var1);
            field_20123_d.add((Object)null);
        }

        public virtual void updateCraftingResults()
        {
            for (int var1 = 0; var1 < slots.size(); ++var1)
            {
                ItemStack var2 = ((Slot)slots.get(var1)).getStack();
                ItemStack var3 = (ItemStack)field_20123_d.get(var1);
                if (!ItemStack.areItemStacksEqual(var3, var2))
                {
                    var3 = var2 == null ? null : var2.copy();
                    field_20123_d.set(var1, var3);

                    for (int var4 = 0; var4 < field_20121_g.size(); ++var4)
                    {
                        ((ICrafting)field_20121_g.get(var4)).func_20159_a(this, var1, var3);
                    }
                }
            }

        }

        public Slot getSlot(int var1)
        {
            return (Slot)slots.get(var1);
        }

        public virtual ItemStack getStackInSlot(int var1)
        {
            Slot var2 = (Slot)slots.get(var1);
            return var2 != null ? var2.getStack() : null;
        }

        public ItemStack func_27280_a(int var1, int var2, bool var3, EntityPlayer var4)
        {
            ItemStack var5 = null;
            if (var2 == 0 || var2 == 1)
            {
                InventoryPlayer var6 = var4.inventory;
                if (var1 == -999)
                {
                    if (var6.getItemStack() != null && var1 == -999)
                    {
                        if (var2 == 0)
                        {
                            var4.dropPlayerItem(var6.getItemStack());
                            var6.setItemStack((ItemStack)null);
                        }

                        if (var2 == 1)
                        {
                            var4.dropPlayerItem(var6.getItemStack().splitStack(1));
                            if (var6.getItemStack().count == 0)
                            {
                                var6.setItemStack((ItemStack)null);
                            }
                        }
                    }
                }
                else
                {
                    int var10;
                    if (var3)
                    {
                        ItemStack var7 = getStackInSlot(var1);
                        if (var7 != null)
                        {
                            int var8 = var7.count;
                            var5 = var7.copy();
                            Slot var9 = (Slot)slots.get(var1);
                            if (var9 != null && var9.getStack() != null)
                            {
                                var10 = var9.getStack().count;
                                if (var10 < var8)
                                {
                                    func_27280_a(var1, var2, var3, var4);
                                }
                            }
                        }
                    }
                    else
                    {
                        Slot var12 = (Slot)slots.get(var1);
                        if (var12 != null)
                        {
                            var12.onSlotChanged();
                            ItemStack var13 = var12.getStack();
                            ItemStack var14 = var6.getItemStack();
                            if (var13 != null)
                            {
                                var5 = var13.copy();
                            }

                            if (var13 == null)
                            {
                                if (var14 != null && var12.isItemValid(var14))
                                {
                                    var10 = var2 == 0 ? var14.count : 1;
                                    if (var10 > var12.getSlotStackLimit())
                                    {
                                        var10 = var12.getSlotStackLimit();
                                    }

                                    var12.putStack(var14.splitStack(var10));
                                    if (var14.count == 0)
                                    {
                                        var6.setItemStack((ItemStack)null);
                                    }
                                }
                            }
                            else if (var14 == null)
                            {
                                var10 = var2 == 0 ? var13.count : (var13.count + 1) / 2;
                                ItemStack var11 = var12.decrStackSize(var10);
                                var6.setItemStack(var11);
                                if (var13.count == 0)
                                {
                                    var12.putStack((ItemStack)null);
                                }

                                var12.onPickupFromSlot(var6.getItemStack());
                            }
                            else if (var12.isItemValid(var14))
                            {
                                if (var13.itemID != var14.itemID || var13.getHasSubtypes() && var13.getItemDamage() != var14.getItemDamage())
                                {
                                    if (var14.count <= var12.getSlotStackLimit())
                                    {
                                        var12.putStack(var14);
                                        var6.setItemStack(var13);
                                    }
                                }
                                else
                                {
                                    var10 = var2 == 0 ? var14.count : 1;
                                    if (var10 > var12.getSlotStackLimit() - var13.count)
                                    {
                                        var10 = var12.getSlotStackLimit() - var13.count;
                                    }

                                    if (var10 > var14.getMaxCount() - var13.count)
                                    {
                                        var10 = var14.getMaxCount() - var13.count;
                                    }

                                    var14.splitStack(var10);
                                    if (var14.count == 0)
                                    {
                                        var6.setItemStack((ItemStack)null);
                                    }

                                    var13.count += var10;
                                }
                            }
                            else if (var13.itemID == var14.itemID && var14.getMaxCount() > 1 && (!var13.getHasSubtypes() || var13.getItemDamage() == var14.getItemDamage()))
                            {
                                var10 = var13.count;
                                if (var10 > 0 && var10 + var14.count <= var14.getMaxCount())
                                {
                                    var14.count += var10;
                                    var13.splitStack(var10);
                                    if (var13.count == 0)
                                    {
                                        var12.putStack((ItemStack)null);
                                    }

                                    var12.onPickupFromSlot(var6.getItemStack());
                                }
                            }
                        }
                    }
                }
            }

            return var5;
        }

        public virtual void onCraftGuiClosed(EntityPlayer var1)
        {
            InventoryPlayer var2 = var1.inventory;
            if (var2.getItemStack() != null)
            {
                var1.dropPlayerItem(var2.getItemStack());
                var2.setItemStack((ItemStack)null);
            }

        }

        public virtual void onCraftMatrixChanged(IInventory var1)
        {
            updateCraftingResults();
        }

        public void putStackInSlot(int var1, ItemStack var2)
        {
            getSlot(var1).putStack(var2);
        }

        public void putStacksInSlots(ItemStack[] var1)
        {
            for (int var2 = 0; var2 < var1.Length; ++var2)
            {
                getSlot(var2).putStack(var1[var2]);
            }

        }

        public virtual void func_20112_a(int var1, int var2)
        {
        }

        public short func_20111_a(InventoryPlayer var1)
        {
            ++field_20917_a;
            return field_20917_a;
        }

        public void func_20113_a(short var1)
        {
        }

        public void func_20110_b(short var1)
        {
        }

        public abstract bool isUsableByPlayer(EntityPlayer var1);

        protected void func_28125_a(ItemStack var1, int var2, int var3, bool var4)
        {
            int var5 = var2;
            if (var4)
            {
                var5 = var3 - 1;
            }

            Slot var6;
            ItemStack var7;
            if (var1.isStackable())
            {
                while (var1.count > 0 && (!var4 && var5 < var3 || var4 && var5 >= var2))
                {
                    var6 = (Slot)slots.get(var5);
                    var7 = var6.getStack();
                    if (var7 != null && var7.itemID == var1.itemID && (!var1.getHasSubtypes() || var1.getItemDamage() == var7.getItemDamage()))
                    {
                        int var8 = var7.count + var1.count;
                        if (var8 <= var1.getMaxCount())
                        {
                            var1.count = 0;
                            var7.count = var8;
                            var6.onSlotChanged();
                        }
                        else if (var7.count < var1.getMaxCount())
                        {
                            var1.count -= var1.getMaxCount() - var7.count;
                            var7.count = var1.getMaxCount();
                            var6.onSlotChanged();
                        }
                    }

                    if (var4)
                    {
                        --var5;
                    }
                    else
                    {
                        ++var5;
                    }
                }
            }

            if (var1.count > 0)
            {
                if (var4)
                {
                    var5 = var3 - 1;
                }
                else
                {
                    var5 = var2;
                }

                while (!var4 && var5 < var3 || var4 && var5 >= var2)
                {
                    var6 = (Slot)slots.get(var5);
                    var7 = var6.getStack();
                    if (var7 == null)
                    {
                        var6.putStack(var1.copy());
                        var6.onSlotChanged();
                        var1.count = 0;
                        break;
                    }

                    if (var4)
                    {
                        --var5;
                    }
                    else
                    {
                        ++var5;
                    }
                }
            }

        }
    }
}