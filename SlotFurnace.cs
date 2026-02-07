using betareborn.Entities;
using betareborn.Items;

namespace betareborn
{
    public class SlotFurnace : Slot
    {

        private EntityPlayer thePlayer;

        public SlotFurnace(EntityPlayer var1, IInventory var2, int var3, int var4, int var5) : base(var2, var3, var4, var5)
        {
            thePlayer = var1;
        }

        public override bool isItemValid(ItemStack var1)
        {
            return false;
        }

        public override void onPickupFromSlot(ItemStack var1)
        {
            var1.onCrafting(thePlayer.worldObj, thePlayer);
            if (var1.itemID == Item.ingotIron.shiftedIndex)
            {
                thePlayer.addStat(Achievements.ACQUIRE_IRON, 1);
            }

            if (var1.itemID == Item.fishCooked.shiftedIndex)
            {
                thePlayer.addStat(Achievements.COOK_FISH, 1);
            }

            base.onPickupFromSlot(var1);
        }
    }

}