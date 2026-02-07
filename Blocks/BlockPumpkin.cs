using betareborn.Entities;
using betareborn.Materials;
using betareborn.Worlds;

namespace betareborn.Blocks
{
    public class BlockPumpkin : Block
    {

        private bool blockType;

        public BlockPumpkin(int var1, int var2, bool var3) : base(var1, Material.PUMPKIN)
        {
            textureId = var2;
            setTickRandomly(true);
            blockType = var3;
        }

        public override int getTexture(int var1, int var2)
        {
            if (var1 == 1)
            {
                return textureId;
            }
            else if (var1 == 0)
            {
                return textureId;
            }
            else
            {
                int var3 = textureId + 1 + 16;
                if (blockType)
                {
                    ++var3;
                }

                return var2 == 2 && var1 == 2 ? var3 : (var2 == 3 && var1 == 5 ? var3 : (var2 == 0 && var1 == 3 ? var3 : (var2 == 1 && var1 == 4 ? var3 : textureId + 16)));
            }
        }

        public override int getTexture(int var1)
        {
            return var1 == 1 ? textureId : (var1 == 0 ? textureId : (var1 == 3 ? textureId + 1 + 16 : textureId + 16));
        }

        public override void onBlockAdded(World var1, int var2, int var3, int var4)
        {
            base.onBlockAdded(var1, var2, var3, var4);
        }

        public override bool canPlaceBlockAt(World var1, int var2, int var3, int var4)
        {
            int var5 = var1.getBlockId(var2, var3, var4);
            return (var5 == 0 || Block.BLOCKS[var5].material.isReplaceable()) && var1.shouldSuffocate(var2, var3 - 1, var4);
        }

        public override void onBlockPlacedBy(World var1, int var2, int var3, int var4, EntityLiving var5)
        {
            int var6 = MathHelper.floor_double((double)(var5.rotationYaw * 4.0F / 360.0F) + 2.5D) & 3;
            var1.setBlockMeta(var2, var3, var4, var6);
        }
    }

}