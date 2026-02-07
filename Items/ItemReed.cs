using betareborn.Blocks;
using betareborn.Entities;
using betareborn.Worlds;

namespace betareborn.Items
{
    public class ItemReed : Item
    {

        private int field_320_a;

        public ItemReed(int var1, Block var2) : base(var1)
        {
            field_320_a = var2.id;
        }

        public override bool onItemUse(ItemStack var1, EntityPlayer var2, World var3, int var4, int var5, int var6, int var7)
        {
            if (var3.getBlockId(var4, var5, var6) == Block.SNOW.id)
            {
                var7 = 0;
            }
            else
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
            }

            if (var1.count == 0)
            {
                return false;
            }
            else
            {
                if (var3.canBlockBePlacedAt(field_320_a, var4, var5, var6, false, var7))
                {
                    Block var8 = Block.BLOCKS[field_320_a];
                    if (var3.setBlockWithNotify(var4, var5, var6, field_320_a))
                    {
                        Block.BLOCKS[field_320_a].onBlockPlaced(var3, var4, var5, var6, var7);
                        Block.BLOCKS[field_320_a].onBlockPlacedBy(var3, var4, var5, var6, var2);
                        var3.playSoundEffect((double)((float)var4 + 0.5F), (double)((float)var5 + 0.5F), (double)((float)var6 + 0.5F), var8.soundGroup.func_1145_d(), (var8.soundGroup.getVolume() + 1.0F) / 2.0F, var8.soundGroup.getPitch() * 0.8F);
                        --var1.count;
                    }
                }

                return true;
            }
        }
    }

}