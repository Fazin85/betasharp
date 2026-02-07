using betareborn.Blocks;
using betareborn.Entities;
using betareborn.Worlds;

namespace betareborn.Items
{
    public class ItemSeeds : Item
    {

        private int field_318_a;

        public ItemSeeds(int var1, int var2) : base(var1)
        {
            field_318_a = var2;
        }

        public override bool onItemUse(ItemStack var1, EntityPlayer var2, World var3, int var4, int var5, int var6, int var7)
        {
            if (var7 != 1)
            {
                return false;
            }
            else
            {
                int var8 = var3.getBlockId(var4, var5, var6);
                if (var8 == Block.FARMLAND.id && var3.isAir(var4, var5 + 1, var6))
                {
                    var3.setBlockWithNotify(var4, var5 + 1, var6, field_318_a);
                    --var1.count;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }

}