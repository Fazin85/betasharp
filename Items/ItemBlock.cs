using betareborn.Blocks;
using betareborn.Entities;
using betareborn.Worlds;

namespace betareborn.Items
{
    public class ItemBlock : Item
    {

        private int blockID;

        public ItemBlock(int var1) : base(var1)
        {
            blockID = var1 + 256;
            setIconIndex(Block.BLOCKS[var1 + 256].getTexture(2));
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
            else if (var5 == 127 && Block.BLOCKS[blockID].material.isSolid())
            {
                return false;
            }
            else if (var3.canBlockBePlacedAt(blockID, var4, var5, var6, false, var7))
            {
                Block var8 = Block.BLOCKS[blockID];
                if (var3.setBlockAndMetadataWithNotify(var4, var5, var6, blockID, getPlacedBlockMetadata(var1.getItemDamage())))
                {
                    Block.BLOCKS[blockID].onBlockPlaced(var3, var4, var5, var6, var7);
                    Block.BLOCKS[blockID].onBlockPlacedBy(var3, var4, var5, var6, var2);
                    var3.playSoundEffect((double)((float)var4 + 0.5F), (double)((float)var5 + 0.5F), (double)((float)var6 + 0.5F), var8.soundGroup.func_1145_d(), (var8.soundGroup.getVolume() + 1.0F) / 2.0F, var8.soundGroup.getPitch() * 0.8F);
                    --var1.count;
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        public override String getItemNameIS(ItemStack var1)
        {
            return Block.BLOCKS[blockID].getBlockName();
        }

        public override String getItemName()
        {
            return Block.BLOCKS[blockID].getBlockName();
        }
    }

}