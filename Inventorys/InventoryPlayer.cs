using betareborn.Blocks;
using betareborn.Entities;
using betareborn.Items;
using betareborn.NBT;

namespace betareborn.Inventorys
{
    public class InventoryPlayer : java.lang.Object, IInventory
    {

        public ItemStack[] main = new ItemStack[36];
        public ItemStack[] armor = new ItemStack[4];
        public int selectedSlot = 0;
        public EntityPlayer player;
        private ItemStack cursorStack;
        public bool dirty = false;

        public InventoryPlayer(EntityPlayer var1)
        {
            player = var1;
        }

        public static int getHotbarSize()
        {
            return 9;
        }

        public ItemStack getSelectedItem()
        {
            return selectedSlot < 9 && selectedSlot >= 0 ? main[selectedSlot] : null;
        }

        private int getInventorySlotContainItem(int var1)
        {
            for (int var2 = 0; var2 < main.Length; ++var2)
            {
                if (main[var2] != null && main[var2].itemId == var1)
                {
                    return var2;
                }
            }

            return -1;
        }

        private int storeItemStack(ItemStack var1)
        {
            for (int var2 = 0; var2 < main.Length; ++var2)
            {
                if (main[var2] != null && main[var2].itemId == var1.itemId && main[var2].isStackable() && main[var2].count < main[var2].getMaxCount() && main[var2].count < getMaxCountPerStack() && (!main[var2].getHasSubtypes() || main[var2].getDamage() == var1.getDamage()))
                {
                    return var2;
                }
            }

            return -1;
        }

        private int getFirstEmptyStack()
        {
            for (int var1 = 0; var1 < main.Length; ++var1)
            {
                if (main[var1] == null)
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
                selectedSlot = var3;
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

            for (selectedSlot -= var1; selectedSlot < 0; selectedSlot += 9)
            {
            }

            while (selectedSlot >= 9)
            {
                selectedSlot -= 9;
            }

        }

        private int storePartialItemStack(ItemStack var1)
        {
            int var2 = var1.itemId;
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
                if (main[var4] == null)
                {
                    main[var4] = new ItemStack(var2, 0, var1.getDamage());
                }

                int var5 = var3;
                if (var3 > main[var4].getMaxCount() - main[var4].count)
                {
                    var5 = main[var4].getMaxCount() - main[var4].count;
                }

                if (var5 > getMaxCountPerStack() - main[var4].count)
                {
                    var5 = getMaxCountPerStack() - main[var4].count;
                }

                if (var5 == 0)
                {
                    return var3;
                }
                else
                {
                    var3 -= var5;
                    main[var4].count += var5;
                    main[var4].bobbingAnimationTime = 5;
                    return var3;
                }
            }
        }

        public void inventoryTick()
        {
            for (int var1 = 0; var1 < main.Length; ++var1)
            {
                if (main[var1] != null)
                {
                    main[var1].inventoryTick(player.world, player, var1, selectedSlot == var1);
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
                if (--main[var2].count <= 0)
                {
                    main[var2] = null;
                }

                return true;
            }
        }

        public bool addItemStackToInventory(ItemStack var1)
        {
            int var2;
            if (var1.isDamaged())
            {
                var2 = getFirstEmptyStack();
                if (var2 >= 0)
                {
                    main[var2] = ItemStack.clone(var1);
                    main[var2].bobbingAnimationTime = 5;
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
            ItemStack[] var3 = main;
            if (var1 >= main.Length)
            {
                var3 = armor;
                var1 -= main.Length;
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
                    var4 = var3[var1].split(var2);
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
            ItemStack[] var3 = main;
            if (var1 >= var3.Length)
            {
                var1 -= var3.Length;
                var3 = armor;
            }

            var3[var1] = var2;
        }

        public float getStrVsBlock(Block var1)
        {
            float var2 = 1.0F;
            if (main[selectedSlot] != null)
            {
                var2 *= main[selectedSlot].getMiningSpeedMultiplier(var1);
            }

            return var2;
        }

        public NbtTagList writeToNBT(NbtTagList var1)
        {
            int var2;
            NbtTagCompound var3;
            for (var2 = 0; var2 < main.Length; ++var2)
            {
                if (main[var2] != null)
                {
                    var3 = new NbtTagCompound();
                    var3.SetByte("Slot", (sbyte)var2);
                    main[var2].writeToNBT(var3);
                    var1.SetTag(var3);
                }
            }

            for (var2 = 0; var2 < armor.Length; ++var2)
            {
                if (armor[var2] != null)
                {
                    var3 = new NbtTagCompound();
                    var3.SetByte("Slot", (sbyte)(var2 + 100));
                    armor[var2].writeToNBT(var3);
                    var1.SetTag(var3);
                }
            }

            return var1;
        }

        public void readFromNBT(NbtTagList var1)
        {
            main = new ItemStack[36];
            armor = new ItemStack[4];

            for (int var2 = 0; var2 < var1.TagCount(); ++var2)
            {
                NbtTagCompound var3 = (NbtTagCompound)var1.TagAt(var2);
                int var4 = var3.GetByte("Slot") & 255;
                ItemStack var5 = new ItemStack(var3);
                if (var5.getItem() != null)
                {
                    if (var4 >= 0 && var4 < main.Length)
                    {
                        main[var4] = var5;
                    }

                    if (var4 >= 100 && var4 < armor.Length + 100)
                    {
                        armor[var4 - 100] = var5;
                    }
                }
            }

        }

        public int size()
        {
            return main.Length + 4;
        }

        public ItemStack getStack(int var1)
        {
            ItemStack[] var2 = main;
            if (var1 >= var2.Length)
            {
                var1 -= var2.Length;
                var2 = armor;
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
            ItemStack var2 = getStack(selectedSlot);
            return var2 != null ? var2.getAttackDamage(var1) : 1;
        }

        public bool canHarvestBlock(Block var1)
        {
            if (var1.material.isHandHarvestable())
            {
                return true;
            }
            else
            {
                ItemStack var2 = getStack(selectedSlot);
                return var2 != null ? var2.isSuitableFor(var1) : false;
            }
        }

        public ItemStack armorItemInSlot(int var1)
        {
            return armor[var1];
        }

        public int getTotalArmorValue()
        {
            int var1 = 0;
            int var2 = 0;
            int var3 = 0;

            for (int var4 = 0; var4 < armor.Length; ++var4)
            {
                if (armor[var4] != null && armor[var4].getItem() is ItemArmor)
                {
                    int var5 = armor[var4].getMaxDamage();
                    int var6 = armor[var4].getDamage2();
                    int var7 = var5 - var6;
                    var2 += var7;
                    var3 += var5;
                    int var8 = ((ItemArmor)armor[var4].getItem()).damageReduceAmount;
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

        public void damageArmor(int durabilityLoss)
        {
            for (int var2 = 0; var2 < armor.Length; ++var2)
            {
                if (armor[var2] != null && armor[var2].getItem() is ItemArmor)
                {
                    armor[var2].damageItem(durabilityLoss, player);
                    if (armor[var2].count == 0)
                    {
                        armor[var2].onRemoved(player);
                        armor[var2] = null;
                    }
                }
            }

        }

        public void dropInventory()
        {
            int var1;
            for (var1 = 0; var1 < main.Length; ++var1)
            {
                if (main[var1] != null)
                {
                    player.dropItem(main[var1], true);
                    main[var1] = null;
                }
            }

            for (var1 = 0; var1 < armor.Length; ++var1)
            {
                if (armor[var1] != null)
                {
                    player.dropItem(armor[var1], true);
                    armor[var1] = null;
                }
            }

        }

        public void markDirty()
        {
            dirty = true;
        }

        public void setItemStack(ItemStack var1)
        {
            cursorStack = var1;
            player.onCursorStackChanged(var1);
        }

        public ItemStack getCursorStack()
        {
            return cursorStack;
        }

        public bool canPlayerUse(EntityPlayer var1)
        {
            return player.dead ? false : var1.getSquaredDistance(player) <= 64.0D;
        }

        public bool contains(ItemStack var1)
        {
            int var2;
            for (var2 = 0; var2 < armor.Length; ++var2)
            {
                if (armor[var2] != null && armor[var2].equals(var1))
                {
                    return true;
                }
            }

            for (var2 = 0; var2 < main.Length; ++var2)
            {
                if (main[var2] != null && main[var2].equals(var1))
                {
                    return true;
                }
            }

            return false;
        }
    }

}