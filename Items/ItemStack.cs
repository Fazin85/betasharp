using betareborn.Blocks;
using betareborn.Entities;
using betareborn.NBT;
using betareborn.Worlds;

namespace betareborn.Items
{
    public class ItemStack : java.lang.Object
    {
        public int count;
        public int bobbingAnimationTime;
        public int itemId;
        private int damage;

        public ItemStack(Block var1) : this((Block)var1, 1)
        {
        }

        public ItemStack(int id, int count)
        {
            itemId = id;
            this.count = count;
        }

        public ItemStack(Block var1, int var2) : this(var1.id, var2, 0)
        {
        }

        public ItemStack(Block var1, int var2, int var3) : this(var1.id, var2, var3)
        {
        }

        public ItemStack(Item var1) : this(var1.id, 1, 0)
        {
        }

        public ItemStack(Item var1, int var2) : this(var1.id, var2, 0)
        {
        }

        public ItemStack(Item var1, int var2, int var3) : this(var1.id, var2, var3)
        {
        }

        public ItemStack(int var1, int var2, int var3)
        {
            count = 0;
            itemId = var1;
            count = var2;
            damage = var3;
        }

        public ItemStack(NbtTagCompound var1)
        {
            count = 0;
            readFromNBT(var1);
        }

        public ItemStack split(int var1)
        {
            count -= var1;
            return new ItemStack(itemId, var1, damage);
        }

        public Item getItem()
        {
            return Item.ITEMS[itemId];
        }

        public int getTextureId()
        {
            return getItem().getTextureId(this);
        }

        public bool useOnBlock(EntityPlayer var1, World var2, int var3, int var4, int var5, int var6)
        {
            bool var7 = getItem().useOnBlock(this, var1, var2, var3, var4, var5, var6);
            if (var7)
            {
                var1.increaseStat(Stats.Stats.USED[itemId], 1);
            }

            return var7;
        }

        public float getMiningSpeedMultiplier(Block var1)
        {
            return getItem().getMiningSpeedMultiplier(this, var1);
        }

        public ItemStack use(World var1, EntityPlayer var2)
        {
            return getItem().use(this, var1, var2);
        }

        public NbtTagCompound writeToNBT(NbtTagCompound var1)
        {
            var1.SetShort("id", (short)itemId);
            var1.SetByte("Count", (sbyte)count);
            var1.SetShort("Damage", (short)damage);
            return var1;
        }

        public void readFromNBT(NbtTagCompound var1)
        {
            itemId = var1.GetShort("id");
            count = var1.GetByte("Count");
            damage = var1.GetShort("Damage");
        }

        public int getMaxCount()
        {
            return getItem().getMaxCount();
        }

        public bool isStackable()
        {
            return getMaxCount() > 1 && (!isDamageable() || !isDamaged());
        }

        public bool isDamageable()
        {
            return Item.ITEMS[itemId].getMaxDamage() > 0;
        }

        public bool getHasSubtypes()
        {
            return Item.ITEMS[itemId].getHasSubtypes();
        }

        public bool isDamaged()
        {
            return isDamageable() && damage > 0;
        }

        public int getDamage2()
        {
            return damage;
        }

        public int getDamage()
        {
            return damage;
        }

        public void setDamage(int var1)
        {
            damage = var1;
        }

        public int getMaxDamage()
        {
            return Item.ITEMS[itemId].getMaxDamage();
        }

        public void damageItem(int var1, Entity var2)
        {
            if (isDamageable())
            {
                damage += var1;
                if (damage > getMaxDamage())
                {
                    if (var2 is EntityPlayer)
                    {
                        ((EntityPlayer)var2).increaseStat(Stats.Stats.BROKEN[itemId], 1);
                    }

                    --count;
                    if (count < 0)
                    {
                        count = 0;
                    }

                    damage = 0;
                }

            }
        }

        public void postHit(EntityLiving var1, EntityPlayer var2)
        {
            bool var3 = Item.ITEMS[itemId].postHit(this, var1, var2);
            if (var3)
            {
                var2.increaseStat(Stats.Stats.USED[itemId], 1);
            }

        }

        public void postMine(int var1, int var2, int var3, int var4, EntityPlayer var5)
        {
            bool var6 = Item.ITEMS[itemId].postMine(this, var1, var2, var3, var4, var5);
            if (var6)
            {
                var5.increaseStat(Stats.Stats.USED[itemId], 1);
            }

        }

        public int getAttackDamage(Entity var1)
        {
            return Item.ITEMS[itemId].getAttackDamage(var1);
        }

        public bool isSuitableFor(Block var1)
        {
            return Item.ITEMS[itemId].isSuitableFor(var1);
        }

        public void onRemoved(EntityPlayer var1)
        {
        }

        public void useOnEntity(EntityLiving var1)
        {
            Item.ITEMS[itemId].useOnEntity(this, var1);
        }

        public ItemStack copy()
        {
            return new ItemStack(itemId, count, damage);
        }

        public static bool areEqual(ItemStack var0, ItemStack var1)
        {
            return var0 == null && var1 == null ? true : (var0 != null && var1 != null ? var0.equals2(var1) : false);
        }

        private bool equals2(ItemStack var1)
        {
            return count != var1.count ? false : (itemId != var1.itemId ? false : damage == var1.damage);
        }

        public bool isItemEqual(ItemStack var1)
        {
            return itemId == var1.itemId && damage == var1.damage;
        }

        public string getItemName()
        {
            return Item.ITEMS[itemId].getItemNameIS(this);
        }

        public static ItemStack clone(ItemStack var0)
        {
            return var0 == null ? null : var0.copy();
        }

        public override string toString()
        {
            return count + "x" + Item.ITEMS[itemId].getItemName() + "@" + damage;
        }

        public void inventoryTick(World var1, Entity var2, int var3, bool var4)
        {
            if (bobbingAnimationTime > 0)
            {
                --bobbingAnimationTime;
            }

            Item.ITEMS[itemId].inventoryTick(this, var1, var2, var3, var4);
        }

        public void onCraft(World var1, EntityPlayer var2)
        {
            var2.increaseStat(Stats.Stats.CRAFTED[itemId], count);
            Item.ITEMS[itemId].onCraft(this, var1, var2);
        }

        public bool equals(ItemStack var1)
        {
            return itemId == var1.itemId && count == var1.count && damage == var1.damage;
        }
    }

}