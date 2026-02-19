using BetaSharp.Blocks;
using BetaSharp.Entities;
using BetaSharp.Items;
using BetaSharp.NBT;
using java.awt.@event;

namespace BetaSharp.Inventories;

public class InventoryPlayer : java.lang.Object, IInventory
{
    public ItemStack?[] Main { get; private set; } = new ItemStack[36];
    public ItemStack?[] Armor { get; private set; } = new ItemStack[4];

    public const int HotbarSize = 9;

    public int SelectedSlot
    {
        get;
        set
        {
            // between 0-8 (9 slots)
            // if slotIndex is out of bounds it wont change the value
            field = value switch
            {
                < 0 or > HotbarSize - 1 => field,
                _ => value,
            };
        }
    } // between zero and nine

    public EntityPlayer Player { get; }
    private ItemStack CursorStack { get; set; }
    public bool Dirty = false;

    public InventoryPlayer(EntityPlayer player)
    {
        Player = player;
    }

    public ItemStack? GetSelectedItem()
    {
        return Main[SelectedSlot];
    }

    private int GetInventorySlotContainItem(int itemId)
    {
        for (int slotIndex = 0; slotIndex < Main.Length; ++slotIndex)
        {
            if (Main[slotIndex] != null && Main[slotIndex]!.itemId == itemId)
            {
                return slotIndex;
            }
        }

        return -1;
    }

    private int StoreItemStack(ItemStack itemStack)
    {
        for (int slotIndex = 0; slotIndex < Main.Length; ++slotIndex)
        {
            if (
                Main[slotIndex] != null &&
                Main[slotIndex]!.itemId == itemStack.itemId &&
                Main[slotIndex]!.isStackable() &&
                Main[slotIndex]!.count < Main[slotIndex]!.getMaxCount() &&
                Main[slotIndex]!.count < this.MaxCountPerStack &&
                (!Main[slotIndex]!.getHasSubtypes() || Main[slotIndex]!.getDamage() == itemStack.getDamage())
                )
            {
                return slotIndex;
            }
        }

        return -1;
    }

    private int GetFirstEmptyStack()
    {
        for (int slotIndex = 0; slotIndex < Main.Length; ++slotIndex)
        {
            if (Main[slotIndex] == null)
            {
                return slotIndex;
            }
        }

        return -1;
    }

    // middle click functionality, just searches for
    // the blockid and returns the slotIndex that has that blockId inside of it
    public void SetCurrentItem(int itemId)
    {
        int slotIndex = GetInventorySlotContainItem(itemId);
        SelectedSlot = slotIndex; // if slotIndex is out of bounds it wont change the value
    }

    public void ChangeCurrentItem(int scrollDirection)
    {
        int futureSlot = (SelectedSlot - scrollDirection) % 9;

        SelectedSlot = futureSlot switch
        {
            < 0 => 8,
            _ => futureSlot,
        };
    }

    private int StorePartialItemStack(ItemStack itemStack)
    {
        int itemId = itemStack.itemId;
        int remainingCount = itemStack.count;
        int slotIndex = StoreItemStack(itemStack);
        if (slotIndex < 0)
        {
            slotIndex = GetFirstEmptyStack();
        }

        if (slotIndex < 0)
        {
            return remainingCount;
        }
        else
        {
            if (Main[slotIndex] == null)
            {
                Main[slotIndex] = new ItemStack(itemId, 0, itemStack.getDamage());
            }

            int spaceAvailable = remainingCount;
            if (remainingCount > Main[slotIndex]!.getMaxCount() - Main[slotIndex]!.count)
            {
                spaceAvailable = Main[slotIndex]!.getMaxCount() - Main[slotIndex]!.count;
            }

            if (spaceAvailable > this.MaxCountPerStack - Main[slotIndex]!.count)
            {
                spaceAvailable = this.MaxCountPerStack - Main[slotIndex]!.count;
            }

            if (spaceAvailable == 0)
            {
                return remainingCount;
            }
            else
            {
                remainingCount -= spaceAvailable;
                Main[slotIndex]!.count += spaceAvailable;
                Main[slotIndex]!.bobbingAnimationTime = 5;
                return remainingCount;
            }
        }
    }

    public void InventoryTick()
    {
        for (int slotIndex = 0; slotIndex < Main.Length; ++slotIndex)
        {
            if (Main[slotIndex] != null)
            {
                Main[slotIndex]!.inventoryTick(Player.world, Player, slotIndex, SelectedSlot == slotIndex);
            }
        }
    }

    public bool ConsumeInventoryItem(int itemId)
    {
        int slotIndex = GetInventorySlotContainItem(itemId);
        if (slotIndex < 0)
        {
            return false;
        }
        else
        {
            if (--Main[slotIndex]!.count <= 0)
            {
                Main[slotIndex] = null;
            }

            return true;
        }
    }

    public bool AddItemStackToInventory(ItemStack itemStack)
    {
        int slotIndex;
        if (itemStack.isDamaged())
        {
            slotIndex = GetFirstEmptyStack();
            if (slotIndex >= 0)
            {
                Main[slotIndex] = ItemStack.clone(itemStack);
                Main[slotIndex]!.bobbingAnimationTime = 5;
                // it will be marked dead, don't worry
                itemStack.count = 0;
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
                slotIndex = itemStack.count;
                itemStack.count = StorePartialItemStack(itemStack);
            } while (itemStack.count > 0 && itemStack.count < slotIndex);

            return itemStack.count < slotIndex;
        }
    }

    public ItemStack? RemoveStack(int slotIndex, int amount)
    {
        ItemStack?[] targetArray = Main;
        if (slotIndex >= Main.Length)
        {
            targetArray = Armor;
            slotIndex -= Main.Length;
        }

        if (targetArray[slotIndex] != null)
        {
            ItemStack removeStack;
            if (targetArray[slotIndex]!.count <= amount)
            {
                removeStack = targetArray[slotIndex]!;
                targetArray[slotIndex] = null;
                return removeStack;
            }
            else
            {
                removeStack = targetArray[slotIndex]!.Split(amount);
                if (targetArray[slotIndex]!.count == 0)
                {
                    targetArray[slotIndex] = null;
                }

                return removeStack;
            }
        }
        else
        {
            return null;
        }
    }

    public void SetStack(int slotIndex, ItemStack? itemStack)
    {
        ItemStack?[] targetArray = Main;
        if (slotIndex >= targetArray.Length)
        {
            slotIndex -= targetArray.Length;
            targetArray = Armor;
        }

        targetArray[slotIndex] = itemStack;
    }

    public float GetStrVsBlock(Block block)
    {
        float miningSpeed = 1.0F;
        if (Main[SelectedSlot] != null)
        {
            miningSpeed *= Main[SelectedSlot]!.getMiningSpeedMultiplier(block);
        }

        return miningSpeed;
    }

    public NBTTagList WriteToNBT(NBTTagList nbt)
    {
        int slotIndex;
        NBTTagCompound itemTag;
        for (slotIndex = 0; slotIndex < Main.Length; ++slotIndex)
        {
            if (Main[slotIndex] != null)
            {
                itemTag = new NBTTagCompound();
                itemTag.SetByte("Slot", (sbyte)slotIndex);
                Main[slotIndex]!.writeToNBT(itemTag);
                nbt.SetTag(itemTag);
            }
        }

        for (slotIndex = 0; slotIndex < Armor.Length; ++slotIndex)
        {
            if (Armor[slotIndex] != null)
            {
                itemTag = new NBTTagCompound();
                itemTag.SetByte("Slot", (sbyte)(slotIndex + 100));
                Armor[slotIndex]!.writeToNBT(itemTag);
                nbt.SetTag(itemTag);
            }
        }

        return nbt;
    }

    public void ReadFromNBT(NBTTagList nbt)
    {
        Main = new ItemStack[36];
        Armor = new ItemStack[4];

        for (int i = 0; i < nbt.TagCount(); ++i)
        {
            NBTTagCompound itemTag = (NBTTagCompound)nbt.TagAt(i);
            int slotIndex = itemTag.GetByte("Slot") & 255;
            ItemStack itemStack = new ItemStack(itemTag);
            if (itemStack.getItem() != null)
            {
                if (slotIndex >= 0 && slotIndex < Main.Length)
                {
                    Main[slotIndex] = itemStack;
                }

                if (slotIndex >= 100 && slotIndex < Armor.Length + 100)
                {
                    Armor[slotIndex - 100] = itemStack;
                }
            }
        }

    }

    public int Size
    {
        get => Main.Length + 4;
    }

    public ItemStack? GetStack(int slotIndex)
    {
        // we can combine inventory and armor, and make armor a span?
        ItemStack?[] targetArray = Main;
        if (slotIndex >= targetArray.Length)
        {
            slotIndex -= targetArray.Length;
            targetArray = Armor;
        }

        return targetArray[slotIndex];
    }


    public string Name
    {
        get => "Inventory";
    }

    public int MaxCountPerStack
    {
        get => 64;
    }

    public int GetDamageVsEntity(Entity entity)
    {
        ItemStack? itemStack = GetStack(SelectedSlot);
        return itemStack != null ? itemStack.getAttackDamage(entity) : 1;
    }

    public bool CanHarvestBlock(Block block)
    {
        if (block.material.IsHandHarvestable)
        {
            return true;
        }
        else
        {
            ItemStack? itemStack = GetStack(SelectedSlot);
            return itemStack != null ? itemStack.isSuitableFor(block) : false;
        }
    }

    public ItemStack? ArmorItemInSlot(int slotIndex)
    {
        return Armor[slotIndex];
    }

    public int GetTotalArmorValue()
    {
        int totalArmor = 0;
        int durabilitySum = 0;
        int totalMaxDurability = 0;

        for (int slotIndex = 0; slotIndex < Armor.Length; ++slotIndex)
        {
            if (Armor[slotIndex] != null && Armor[slotIndex]!.getItem() is ItemArmor)
            {
                int maxDurability = Armor[slotIndex]!.getMaxDamage();
                int pieceDamage = Armor[slotIndex]!.getDamage2();
                int remainingDurability = maxDurability - pieceDamage;
                durabilitySum += remainingDurability;
                totalMaxDurability += maxDurability;
                int armorValue = ((ItemArmor)Armor[slotIndex]!.getItem()).damageReduceAmount;
                totalArmor += armorValue;
            }
        }

        if (totalMaxDurability == 0)
        {
            return 0;
        }
        else
        {
            return (totalArmor - 1) * durabilitySum / totalMaxDurability + 1;
        }
    }

    public void DamageArmor(int durabilityLoss)
    {
        for (int slotIndex = 0; slotIndex < Armor.Length; ++slotIndex)
        {
            if (Armor[slotIndex] != null && Armor[slotIndex]!.getItem() is ItemArmor)
            {
                Armor[slotIndex]!.damageItem(durabilityLoss, Player);
                if (Armor[slotIndex]!.count == 0)
                {
                    Armor[slotIndex]!.onRemoved(Player);
                    Armor[slotIndex] = null;
                }
            }
        }

    }

    public void DropInventory()
    {
        int slotIndex;
        for (slotIndex = 0; slotIndex < Main.Length; ++slotIndex)
        {
            if (Main[slotIndex] != null)
            {
                Player.dropItem(Main[slotIndex]!, true);
                Main[slotIndex] = null;
            }
        }

        for (slotIndex = 0; slotIndex < Armor.Length; ++slotIndex)
        {
            if (Armor[slotIndex] != null)
            {
                Player.dropItem(Armor[slotIndex]!, true);
                Armor[slotIndex] = null;
            }
        }

    }

    public void MarkDirty()
    {
        Dirty = true;
    }

    public void SetItemStack(ItemStack itemStack)
    {
        CursorStack = itemStack;
        Player.onCursorStackChanged(itemStack);
    }

    public ItemStack GetCursorStack()
    {
        return CursorStack;
    }

    public bool CanPlayerUse(EntityPlayer entityPlayer)
    {
        return Player.dead ? false : entityPlayer.getSquaredDistance(Player) <= 64.0D;
    }

    public bool Contains(ItemStack itemStack)
    {
        int slotIndex;
        for (slotIndex = 0; slotIndex < Armor.Length; ++slotIndex)
        {
            if (Armor[slotIndex] != null && Armor[slotIndex]!.equals(itemStack))
            {
                return true;
            }
        }

        for (slotIndex = 0; slotIndex < Main.Length; ++slotIndex)
        {
            if (Main[slotIndex] != null && Main[slotIndex]!.equals(itemStack))
            {
                return true;
            }
        }

        return false;
    }
}
