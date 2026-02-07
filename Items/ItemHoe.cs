using betareborn.Blocks;
using betareborn.Entities;
using betareborn.Worlds;

namespace betareborn.Items
{
    public class ItemHoe : Item
    {

        public ItemHoe(int var1, EnumToolMaterial var2) : base(var1)
        {
            maxStackSize = 1;
            setMaxDamage(var2.getMaxUses());
        }

        public override bool onItemUse(ItemStack var1, EntityPlayer var2, World var3, int var4, int var5, int var6, int var7)
        {
            int var8 = var3.getBlockId(var4, var5, var6);
            int var9 = var3.getBlockId(var4, var5 + 1, var6);
            if ((var7 == 0 || var9 != 0 || var8 != Block.GRASS_BLOCK.id) && var8 != Block.DIRT.id)
            {
                return false;
            }
            else
            {
                Block var10 = Block.FARMLAND;
                var3.playSoundEffect((double)((float)var4 + 0.5F), (double)((float)var5 + 0.5F), (double)((float)var6 + 0.5F), var10.soundGroup.func_1145_d(), (var10.soundGroup.getVolume() + 1.0F) / 2.0F, var10.soundGroup.getPitch() * 0.8F);
                if (var3.multiplayerWorld)
                {
                    return true;
                }
                else
                {
                    var3.setBlockWithNotify(var4, var5, var6, var10.id);
                    var1.damageItem(1, var2);
                    return true;
                }
            }
        }

        public override bool isFull3D()
        {
            return true;
        }
    }

}