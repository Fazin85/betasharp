using BetaSharp.Blocks;
using BetaSharp.Inventorys;
using BetaSharp.Items;
using BetaSharp.Screens;
using BetaSharp.Screens.Slots;

namespace BetaSharp;

class SlotArmor : Slot
{
    readonly int armorType;
    readonly PlayerScreenHandler inventory;

    public SlotArmor(PlayerScreenHandler var1, IInventory var2, int var3, int var4, int var5, int var6) : base(var2, var3, var4, var5)
    {
        inventory = var1;
        armorType = var6;
    }


    public override int getMaxItemCount()
    {
        return 1;
    }

    public override bool canInsert(ItemStack var1)
    {
        return var1.getItem() is ItemArmor ? ((ItemArmor)var1.getItem()).armorType == armorType : (var1.getItem().id == Block.Pumpkin.id ? armorType == 0 : false);
    }
}