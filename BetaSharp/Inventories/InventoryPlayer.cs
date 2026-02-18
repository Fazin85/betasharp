using BetaSharp.Blocks;
using BetaSharp.Entities;
using BetaSharp.Items;
using BetaSharp.NBT;

namespace BetaSharp.Inventories;

public class InventoryPlayer : java.lang.Object, IInventory
{

    public ItemStack[] _main = new ItemStack[36];
    public ItemStack[] _armor = new ItemStack[4];
    public int _selectedSlot = 0;
    public EntityPlayer _player;
    private ItemStack _cursorStack;
    public bool _dirty = false;

    public InventoryPlayer(EntityPlayer player)
    {
        this._player = player;
    }

    public static int GetHotbarSize()
    {
        return 9;
    }

    public ItemStack GetSelectedItem()
    {
        return _selectedSlot < 9 && _selectedSlot >= 0 ? _main[_selectedSlot] : null;
    }

    private int GetInventorySlotContainItem(int itemId)
    {
        for (int slotIndex = 0; slotIndex < _main.Length; ++slotIndex)
        {
            if (_main[slotIndex] != null && _main[slotIndex].itemId == itemId)
            {
                return slotIndex;
            }
        }

        return -1;
    }

    private int StoreItemStack(ItemStack itemStack)
    {
        for (int slotIndex = 0; slotIndex < _main.Length; ++slotIndex)
        {
            if (_main[slotIndex] != null && _main[slotIndex].itemId == itemStack.itemId && _main[slotIndex].isStackable() && _main[slotIndex].count < _main[slotIndex].getMaxCount() && _main[slotIndex].count < this.MaxCountPerStack && (!_main[slotIndex].getHasSubtypes() || _main[slotIndex].getDamage() == itemStack.getDamage()))
            {
                return slotIndex;
            }
        }

        return -1;
    }

    private int GetFirstEmptyStack()
    {
        for (int slotIndex = 0; slotIndex < _main.Length; ++slotIndex)
        {
            if (_main[slotIndex] == null)
            {
                return slotIndex;
            }
        }

        return -1;
    }

    public void SetCurrentItem(int itemId, bool var2)
    {
        int slotIndex = GetInventorySlotContainItem(itemId);
        if (slotIndex >= 0 && slotIndex < 9)
        {
            _selectedSlot = slotIndex;
        }
    }

    public void ChangeCurrentItem(int scrollDirection)
    {
        if (scrollDirection > 0)
        {
            scrollDirection = 1;
        }

        if (scrollDirection < 0)
        {
            scrollDirection = -1;
        }

        for (_selectedSlot -= scrollDirection; _selectedSlot < 0; _selectedSlot += 9)
        {
        }

        while (_selectedSlot >= 9)
        {
            _selectedSlot -= 9;
        }

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
            if (_main[slotIndex] == null)
            {
                _main[slotIndex] = new ItemStack(itemId, 0, itemStack.getDamage());
            }

            int spaceAvailable = remainingCount;
            if (remainingCount > _main[slotIndex].getMaxCount() - _main[slotIndex].count)
            {
                spaceAvailable = _main[slotIndex].getMaxCount() - _main[slotIndex].count;
            }

            if (spaceAvailable > this.MaxCountPerStack - _main[slotIndex].count)
            {
                spaceAvailable = this.MaxCountPerStack - _main[slotIndex].count;
            }

            if (spaceAvailable == 0)
            {
                return remainingCount;
            }
            else
            {
                remainingCount -= spaceAvailable;
                _main[slotIndex].count += spaceAvailable;
                _main[slotIndex].bobbingAnimationTime = 5;
                return remainingCount;
            }
        }
    }

    public void InventoryTick()
    {
        for (int slotIndex = 0; slotIndex < _main.Length; ++slotIndex)
        {
            if (_main[slotIndex] != null)
            {
                _main[slotIndex].inventoryTick(_player.world, _player, slotIndex, _selectedSlot == slotIndex);
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
            if (--_main[slotIndex].count <= 0)
            {
                _main[slotIndex] = null;
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
                _main[slotIndex] = ItemStack.clone(itemStack);
                _main[slotIndex].bobbingAnimationTime = 5;
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
        ItemStack[] targetArray = _main;
        if (slotIndex >= _main.Length)
        {
            targetArray = _armor;
            slotIndex -= _main.Length;
        }

        if (targetArray[slotIndex] != null)
        {
            ItemStack removeStack;
            if (targetArray[slotIndex].count <= amount)
            {
                removeStack = targetArray[slotIndex];
                targetArray[slotIndex] = null;
                return removeStack;
            }
            else
            {
                removeStack = targetArray[slotIndex].Split(amount);
                if (targetArray[slotIndex].count == 0)
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
        ItemStack[] targetArray = _main;
        if (slotIndex >= targetArray.Length)
        {
            slotIndex -= targetArray.Length;
            targetArray = _armor;
        }

        targetArray[slotIndex] = itemStack;
    }

    public float GetStrVsBlock(Block block)
    {
        float miningSpeed = 1.0F;
        if (_main[_selectedSlot] != null)
        {
            miningSpeed *= _main[_selectedSlot].getMiningSpeedMultiplier(block);
        }

        return miningSpeed;
    }

    public NBTTagList WriteToNBT(NBTTagList nbt)
    {
        int slotIndex;
        NBTTagCompound itemTag;
        for (slotIndex = 0; slotIndex < _main.Length; ++slotIndex)
        {
            if (_main[slotIndex] != null)
            {
                itemTag = new NBTTagCompound();
                itemTag.SetByte("Slot", (sbyte)slotIndex);
                _main[slotIndex].writeToNBT(itemTag);
                nbt.SetTag(itemTag);
            }
        }

        for (slotIndex = 0; slotIndex < _armor.Length; ++slotIndex)
        {
            if (_armor[slotIndex] != null)
            {
                itemTag = new NBTTagCompound();
                itemTag.SetByte("Slot", (sbyte)(slotIndex + 100));
                _armor[slotIndex].writeToNBT(itemTag);
                nbt.SetTag(itemTag);
            }
        }

        return nbt;
    }

    public void ReadFromNBT(NBTTagList nbt)
    {
        _main = new ItemStack[36];
        _armor = new ItemStack[4];

        for (int i = 0; i < nbt.TagCount(); ++i)
        {
            NBTTagCompound itemTag = (NBTTagCompound)nbt.TagAt(i);
            int slotIndex = itemTag.GetByte("Slot") & 255;
            ItemStack itemStack = new ItemStack(itemTag);
            if (itemStack.getItem() != null)
            {
                if (slotIndex >= 0 && slotIndex < _main.Length)
                {
                    _main[slotIndex] = itemStack;
                }

                if (slotIndex >= 100 && slotIndex < _armor.Length + 100)
                {
                    _armor[slotIndex - 100] = itemStack;
                }
            }
        }

    }

    public int Size
    {
        get => _main.Length + 4;
    }

    public ItemStack? GetStack(int slotIndex)
    {
        ItemStack[] targetArray = _main;
        if (slotIndex >= targetArray.Length)
        {
            slotIndex -= targetArray.Length;
            targetArray = _armor;
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
        ItemStack itemStack = GetStack(_selectedSlot);
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
            ItemStack itemStack = GetStack(_selectedSlot);
            return itemStack != null ? itemStack.isSuitableFor(block) : false;
        }
    }

    public ItemStack ArmorItemInSlot(int slotIndex)
    {
        return _armor[slotIndex];
    }

    public int GetTotalArmorValue()
    {
        int totalArmor = 0;
        int durabilitySum = 0;
        int totalMaxDurability = 0;

        for (int slotIndex = 0; slotIndex < _armor.Length; ++slotIndex)
        {
            if (_armor[slotIndex] != null && _armor[slotIndex].getItem() is ItemArmor)
            {
                int maxDurability = _armor[slotIndex].getMaxDamage();
                int pieceDamage = _armor[slotIndex].getDamage2();
                int remainingDurability = maxDurability - pieceDamage;
                durabilitySum += remainingDurability;
                totalMaxDurability += maxDurability;
                int armorValue = ((ItemArmor)_armor[slotIndex].getItem()).damageReduceAmount;
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
        for (int slotIndex = 0; slotIndex < _armor.Length; ++slotIndex)
        {
            if (_armor[slotIndex] != null && _armor[slotIndex].getItem() is ItemArmor)
            {
                _armor[slotIndex].damageItem(durabilityLoss, _player);
                if (_armor[slotIndex].count == 0)
                {
                    _armor[slotIndex].onRemoved(_player);
                    _armor[slotIndex] = null;
                }
            }
        }

    }

    public void DropInventory()
    {
        int slotIndex;
        for (slotIndex = 0; slotIndex < _main.Length; ++slotIndex)
        {
            if (_main[slotIndex] != null)
            {
                _player.dropItem(_main[slotIndex], true);
                _main[slotIndex] = null;
            }
        }

        for (slotIndex = 0; slotIndex < _armor.Length; ++slotIndex)
        {
            if (_armor[slotIndex] != null)
            {
                _player.dropItem(_armor[slotIndex], true);
                _armor[slotIndex] = null;
            }
        }

    }

    public void MarkDirty()
    {
        _dirty = true;
    }

    public void SetItemStack(ItemStack itemStack)
    {
        _cursorStack = itemStack;
        _player.onCursorStackChanged(itemStack);
    }

    public ItemStack GetCursorStack()
    {
        return _cursorStack;
    }

    public bool CanPlayerUse(EntityPlayer entityPlayer)
    {
        return _player.dead ? false : entityPlayer.getSquaredDistance(_player) <= 64.0D;
    }

    public bool Contains(ItemStack itemStack)
    {
        int slotIndex;
        for (slotIndex = 0; slotIndex < _armor.Length; ++slotIndex)
        {
            if (_armor[slotIndex] != null && _armor[slotIndex].equals(itemStack))
            {
                return true;
            }
        }

        for (slotIndex = 0; slotIndex < _main.Length; ++slotIndex)
        {
            if (_main[slotIndex] != null && _main[slotIndex].equals(itemStack))
            {
                return true;
            }
        }

        return false;
    }
}
