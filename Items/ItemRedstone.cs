using betareborn.Blocks;
using betareborn.Entities;
using betareborn.Worlds;

namespace betareborn.Items
{
    public class ItemRedstone : Item
    {

        public ItemRedstone(int var1) : base(var1)
        {
        }

        public override bool onItemUse(ItemStack var1, EntityPlayer var2, World var3, int var4, int var5, int var6, int var7)
        {
            if (var3.getBlockId(var4, var5, var6) != Block.SNOW.id)
            {
                if (var7 == 0)
                {
                    --var5;
                }

                if (var7 == 1)
                {
                    ++var5;
                }

                if (var7 == 2)
                {
                    --var6;
                }

                if (var7 == 3)
                {
                    ++var6;
                }

                if (var7 == 4)
                {
                    --var4;
                }

                if (var7 == 5)
                {
                    ++var4;
                }

                if (!var3.isAir(var4, var5, var6))
                {
                    return false;
                }
            }

            if (Block.REDSTONE_WIRE.canPlaceBlockAt(var3, var4, var5, var6))
            {
                --var1.count;
                var3.setBlockWithNotify(var4, var5, var6, Block.REDSTONE_WIRE.id);
            }

            return true;
        }
    }

}