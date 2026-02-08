using betareborn.Blocks;
using betareborn.Entities;
using betareborn.Items;
using betareborn.NBT;

namespace betareborn.Inventorys
{
    public class InventoryPlayer : java.lang.Object, IInventory
    {

        public ItemStack[] mainInventory = new ItemStack[36];
        public ItemStack[] armorInventory = new ItemStack[4];
        public int currentItem = 0;
        public EntityPlayer player;
        private ItemStack itemStack;
        public bool inventoryChanged = false;

        public InventoryPlayer(EntityPlayer var1)
        {
            player = var1;
        }

        public ItemStack getCurrentItem()
        {
            return currentItem < 9 && currentItem >= 0 ? mainInventory[currentItem] : null;
        }

        private int getInventorySlotContainItem(int var1)
        {
            for (int var2 = 0; var2 < mainInventory.Length; ++var2)
            {
                if (mainInventory[var2] != null && mainInventory[var2].itemID == var1)
                {
                    return var2;
                }
            }

            return -1;
        }

        private int storeItemStack(ItemStack var1)
        {
            for (int var2 = 0; var2 < mainInventory.Length; ++var2)
            {
                if (mainInventory[var2] != null && mainInventory[var2].itemID == var1.itemID && mainInventory[var2].isStackable() && mainInventory[var2].count < mainInventory[var2].getMaxCount() && mainInventory[var2].count < getMaxCountPerStack() && (!mainInventory[var2].getHasSubtypes() || mainInventory[var2].getItemDamage() == var1.getItemDamage()))
                {
                    return var2;
                }
            }

            return -1;
        }

        private int getFirstEmptyStack()
        {
            for (int var1 = 0; var1 < mainInventory.Length; ++var1)
            {
                if (mainInventory[var1] == null)
                {
                    return var1;
                }
            }

            return -1;
        }

        public void setCurrentItem(int var1, bool var2)
        {
            int var3 = getInventorySlotContainItem(var1);
            if (var3 >= 0 && var3 < 9)
            {
                currentItem = var3;
            }
        }

        public void changeCurrentItem(int var1)
        {
            if (var1 > 0)
            {
                var1 = 1;
            }

            if (var1 < 0)
            {
                var1 = -1;
            }

            for (currentItem -= var1; currentItem < 0; currentItem += 9)
            {
            }

            while (currentItem >= 9)
            {
                currentItem -= 9;
            }

        }

        private int storePartialItemStack(ItemStack var1)
        {
            int var2 = var1.itemID;
            int var3 = var1.count;
            int var4 = storeItemStack(var1);
            if (var4 < 0)
            {
                var4 = getFirstEmptyStack();
            }

            if (var4 < 0)
            {
                return var3;
            }
            else
            {
                if (mainInventory[var4] == null)
                {
                    mainInventory[var4] = new ItemStack(var2, 0, var1.getItemDamage());
                }

                int var5 = var3;
                if (var3 > mainInventory[var4].getMaxCount() - mainInventory[var4].count)
                {
                    var5 = mainInventory[var4].getMaxCount() - mainInventory[var4].count;
                }

                if (var5 > getMaxCountPerStack() - mainInventory[var4].count)
                {
                    var5 = getMaxCountPerStack() - mainInventory[var4].count;
                }

                if (var5 == 0)
                {
                    return var3;
                }
                else
                {
                    var3 -= var5;
                    mainInventory[var4].count += var5;
                    mainInventory[var4].animationsToGo = 5;
                    return var3;
                }
            }
        }

        public void decrementAnimations()
        {
            for (int var1 = 0; var1 < mainInventory.Length; ++var1)
            {
                if (mainInventory[var1] != null)
                {
                    mainInventory[var1].updateAnimation(player.worldObj, player, var1, currentItem == var1);
                }
            }

        }

        public bool consumeInventoryItem(int var1)
        {
            int var2 = getInventorySlotContainItem(var1);
            if (var2 < 0)
            {
                return false;
            }
            else
            {
                if (--mainInventory[var2].count <= 0)
                {
                    mainInventory[var2] = null;
                }

                return true;
            }
        }

        public bool addItemStackToInventory(ItemStack var1)
        {
            int var2;
            if (var1.isItemDamaged())
            {
                var2 = getFirstEmptyStack();
                if (var2 >= 0)
                {
                    mainInventory[var2] = ItemStack.copyItemStack(var1);
                    mainInventory[var2].animationsToGo = 5;
                    var1.count = 0;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                do
                {
                    var2 = var1.count;
                    var1.count = storePartialItemStack(var1);
                } while (var1.count > 0 && var1.count < var2);

                return var1.count < var2;
            }
        }

        public ItemStack removeStack(int var1, int var2)
        {
            ItemStack[] var3 = mainInventory;
            if (var1 >= mainInventory.Length)
            {
                var3 = armorInventory;
                var1 -= mainInventory.Length;
            }

            if (var3[var1] != null)
            {
                ItemStack var4;
                if (var3[var1].count <= var2)
                {
                    var4 = var3[var1];
                    var3[var1] = null;
                    return var4;
                }
                else
                {
                    var4 = var3[var1].splitStack(var2);
                    if (var3[var1].count == 0)
                    {
                        var3[var1] = null;
                    }

                    return var4;
                }
            }
            else
            {
                return null;
            }
        }

        public void setStack(int var1, ItemStack var2)
        {
            ItemStack[] var3 = mainInventory;
            if (var1 >= var3.Length)
            {
                var1 -= var3.Length;
                var3 = armorInventory;
            }

            var3[var1] = var2;
        }

        public float getStrVsBlock(Block var1)
        {
            float var2 = 1.0F;
            if (mainInventory[currentItem] != null)
            {
                var2 *= mainInventory[currentItem].getStrVsBlock(var1);
            }

            return var2;
        }

        public NBTTagList writeToNBT(NBTTagList var1)
        {
            int var2;
            NBTTagCompound var3;
            for (var2 = 0; var2 < mainInventory.Length; ++var2)
            {
                if (mainInventory[var2] != null)
                {
                    var3 = new NBTTagCompound();
                    var3.setByte("Slot", (sbyte)var2);
                    mainInventory[var2].writeToNBT(var3);
                    var1.setTag(var3);
                }
            }

            for (var2 = 0; var2 < armorInventory.Length; ++var2)
            {
                if (armorInventory[var2] != null)
                {
                    var3 = new NBTTagCompound();
                    var3.setByte("Slot", (sbyte)(var2 + 100));
                    armorInventory[var2].writeToNBT(var3);
                    var1.setTag(var3);
                }
            }

            return var1;
        }

        public void readFromNBT(NBTTagList var1)
        {
            mainInventory = new ItemStack[36];
            armorInventory = new ItemStack[4];

            for (int var2 = 0; var2 < var1.tagCount(); ++var2)
            {
                NBTTagCompound var3 = (NBTTagCompound)var1.tagAt(var2);
                int var4 = var3.getByte("Slot") & 255;
                ItemStack var5 = new ItemStack(var3);
                if (var5.getItem() != null)
                {
                    if (var4 >= 0 && var4 < mainInventory.Length)
                    {
                        mainInventory[var4] = var5;
                    }

                    if (var4 >= 100 && var4 < armorInventory.Length + 100)
                    {
                        armorInventory[var4 - 100] = var5;
                    }
                }
            }

        }

        public int size()
        {
            return mainInventory.Length + 4;
        }

        public ItemStack getStack(int var1)
        {
            ItemStack[] var2 = mainInventory;
            if (var1 >= var2.Length)
            {
                var1 -= var2.Length;
                var2 = armorInventory;
            }

            return var2[var1];
        }

        public string getName()
        {
            return "Inventory";
        }

        public int getMaxCountPerStack()
        {
            return 64;
        }

        public int getDamageVsEntity(Entity var1)
        {
            ItemStack var2 = getStack(currentItem);
            return var2 != null ? var2.getDamageVsEntity(var1) : 1;
        }

        public bool canHarvestBlock(Block var1)
        {
            if (var1.material.isHandHarvestable())
            {
                return true;
            }
            else
            {
                ItemStack var2 = getStack(currentItem);
                return var2 != null ? var2.canHarvestBlock(var1) : false;
            }
        }

        public ItemStack armorItemInSlot(int var1)
        {
            return armorInventory[var1];
        }

        public int getTotalArmorValue()
        {
            int var1 = 0;
            int var2 = 0;
            int var3 = 0;

            for (int var4 = 0; var4 < armorInventory.Length; ++var4)
            {
                if (armorInventory[var4] != null && armorInventory[var4].getItem() is ItemArmor)
                {
                    int var5 = armorInventory[var4].getMaxDamage();
                    int var6 = armorInventory[var4].getItemDamageForDisplay();
                    int var7 = var5 - var6;
                    var2 += var7;
                    var3 += var5;
                    int var8 = ((ItemArmor)armorInventory[var4].getItem()).damageReduceAmount;
                    var1 += var8;
                }
            }

            if (var3 == 0)
            {
                return 0;
            }
            else
            {
                return (var1 - 1) * var2 / var3 + 1;
            }
        }

        public void damageArmor(int var1)
        {
            for (int var2 = 0; var2 < armorInventory.Length; ++var2)
            {
                if (armorInventory[var2] != null && armorInventory[var2].getItem() is ItemArmor)
                {
                    armorInventory[var2].damageItem(var1, player);
                    if (armorInventory[var2].count == 0)
                    {
                        armorInventory[var2].func_1097_a(player);
                        armorInventory[var2] = null;
                    }
                }
            }

        }

        public void dropAllItems()
        {
            int var1;
            for (var1 = 0; var1 < mainInventory.Length; ++var1)
            {
                if (mainInventory[var1] != null)
                {
                    player.dropPlayerItemWithRandomChoice(mainInventory[var1], true);
                    mainInventory[var1] = null;
                }
            }

            for (var1 = 0; var1 < armorInventory.Length; ++var1)
            {
                if (armorInventory[var1] != null)
                {
                    player.dropPlayerItemWithRandomChoice(armorInventory[var1], true);
                    armorInventory[var1] = null;
                }
            }

        }

        public void markDirty()
        {
            inventoryChanged = true;
        }

        public void setItemStack(ItemStack var1)
        {
            itemStack = var1;
            player.onItemStackChanged(var1);
        }

        public ItemStack getItemStack()
        {
            return itemStack;
        }

        public bool canPlayerUse(EntityPlayer var1)
        {
            return player.isDead ? false : var1.getDistanceSqToEntity(player) <= 64.0D;
        }

        public bool func_28018_c(ItemStack var1)
        {
            int var2;
            for (var2 = 0; var2 < armorInventory.Length; ++var2)
            {
                if (armorInventory[var2] != null && armorInventory[var2].isStackEqual(var1))
                {
                    return true;
                }
            }

            for (var2 = 0; var2 < mainInventory.Length; ++var2)
            {
                if (mainInventory[var2] != null && mainInventory[var2].isStackEqual(var1))
                {
                    return true;
                }
            }

            return false;
        }
    }

}