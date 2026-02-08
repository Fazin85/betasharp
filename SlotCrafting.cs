using betareborn.Blocks;
using betareborn.Entities;
using betareborn.Inventorys;
using betareborn.Items;

namespace betareborn
{
    public class SlotCrafting : Slot
    {

        private readonly IInventory craftMatrix;
        private EntityPlayer thePlayer;

        public SlotCrafting(EntityPlayer var1, IInventory var2, IInventory var3, int var4, int var5, int var6) : base(var3, var4, var5, var6)
        {
            thePlayer = var1;
            craftMatrix = var2;
        }

        public override bool isItemValid(ItemStack var1)
        {
            return false;
        }

        public override void onPickupFromSlot(ItemStack var1)
        {
            var1.onCrafting(thePlayer.worldObj, thePlayer);
            if (var1.itemID == Block.CRAFTING_TABLE.id)
            {
                thePlayer.increaseStat(Achievements.BUILD_WORKBENCH, 1);
            }
            else if (var1.itemID == Item.pickaxeWood.id)
            {
                thePlayer.increaseStat(Achievements.BUILD_PICKAXE, 1);
            }
            else if (var1.itemID == Block.FURNACE.id)
            {
                thePlayer.increaseStat(Achievements.BUILD_FURNACE, 1);
            }
            else if (var1.itemID == Item.hoeWood.id)
            {
                thePlayer.increaseStat(Achievements.BUILD_HOE, 1);
            }
            else if (var1.itemID == Item.bread.id)
            {
                thePlayer.increaseStat(Achievements.MAKE_BREAD, 1);
            }
            else if (var1.itemID == Item.cake.id)
            {
                thePlayer.increaseStat(Achievements.BAKE_CAKE, 1);
            }
            else if (var1.itemID == Item.pickaxeStone.id)
            {
                thePlayer.increaseStat(Achievements.CRAFT_STONE_PICKAXE, 1);
            }
            else if (var1.itemID == Item.swordWood.id)
            {
                thePlayer.increaseStat(Achievements.CRAFT_SWORD, 1);
            }

            for (int var2 = 0; var2 < craftMatrix.size(); ++var2)
            {
                ItemStack var3 = craftMatrix.getStack(var2);
                if (var3 != null)
                {
                    craftMatrix.removeStack(var2, 1);
                    if (var3.getItem().hasContainerItem())
                    {
                        craftMatrix.setStack(var2, new ItemStack(var3.getItem().getContainerItem()));
                    }
                }
            }

        }
    }

}