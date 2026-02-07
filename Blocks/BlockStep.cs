using betareborn.Materials;
using betareborn.Worlds;

namespace betareborn.Blocks
{
    public class BlockStep : Block
    {
        public static readonly string[] field_22037_a = ["stone", "sand", "wood", "cobble"];
        private bool blockType;

        public BlockStep(int var1, bool var2) : base(var1, 6, Material.STONE)
        {
            blockType = var2;
            if (!var2)
            {
                setBoundingBox(0.0F, 0.0F, 0.0F, 1.0F, 0.5F, 1.0F);
            }

            setOpacity(255);
        }

        public override int getTexture(int var1, int var2)
        {
            return var2 == 0 ? (var1 <= 1 ? 6 : 5) : (var2 == 1 ? (var1 == 0 ? 208 : (var1 == 1 ? 176 : 192)) : (var2 == 2 ? 4 : (var2 == 3 ? 16 : 6)));
        }

        public override int getTexture(int var1)
        {
            return getTexture(var1, 0);
        }

        public override bool isOpaque()
        {
            return blockType;
        }

        public override void onBlockAdded(World var1, int var2, int var3, int var4)
        {
            if (this != Block.SLAB)
            {
                base.onBlockAdded(var1, var2, var3, var4);
            }

            int var5 = var1.getBlockId(var2, var3 - 1, var4);
            int var6 = var1.getBlockMeta(var2, var3, var4);
            int var7 = var1.getBlockMeta(var2, var3 - 1, var4);
            if (var6 == var7)
            {
                if (var5 == SLAB.id)
                {
                    var1.setBlockWithNotify(var2, var3, var4, 0);
                    var1.setBlockAndMetadataWithNotify(var2, var3 - 1, var4, Block.DOUBLE_SLAB.id, var6);
                }

            }
        }

        public override int getDroppedItemId(int var1, java.util.Random var2)
        {
            return Block.SLAB.id;
        }

        public override int quantityDropped(java.util.Random var1)
        {
            return blockType ? 2 : 1;
        }

        protected override int damageDropped(int var1)
        {
            return var1;
        }

        public override bool isFullCube()
        {
            return blockType;
        }

        public override bool isSideVisible(BlockView var1, int var2, int var3, int var4, int var5)
        {
            if (this != Block.SLAB)
            {
                base.isSideVisible(var1, var2, var3, var4, var5);
            }

            return var5 == 1 ? true : (!base.isSideVisible(var1, var2, var3, var4, var5) ? false : (var5 == 0 ? true : var1.getBlockId(var2, var3, var4) != id));
        }
    }

}