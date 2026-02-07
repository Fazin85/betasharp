using betareborn.Materials;
using betareborn.Worlds;

namespace betareborn.Blocks
{
    public class BlockFlower : Block
    {
        public BlockFlower(int var1, int var2) : base(var1, Material.PLANT)
        {
            textureId = var2;
            setTickRandomly(true);
            float var3 = 0.2F;
            setBoundingBox(0.5F - var3, 0.0F, 0.5F - var3, 0.5F + var3, var3 * 3.0F, 0.5F + var3);
        }

        public override bool canPlaceBlockAt(World var1, int var2, int var3, int var4)
        {
            return base.canPlaceBlockAt(var1, var2, var3, var4) && canThisPlantGrowOnThisBlockID(var1.getBlockId(var2, var3 - 1, var4));
        }

        protected virtual bool canThisPlantGrowOnThisBlockID(int var1)
        {
            return var1 == Block.GRASS_BLOCK.id || var1 == Block.DIRT.id || var1 == Block.FARMLAND.id;
        }

        public override void neighborUpdate(World var1, int var2, int var3, int var4, int var5)
        {
            base.neighborUpdate(var1, var2, var3, var4, var5);
            func_268_h(var1, var2, var3, var4);
        }

        public override void onTick(World var1, int var2, int var3, int var4, java.util.Random var5)
        {
            func_268_h(var1, var2, var3, var4);
        }

        protected void func_268_h(World var1, int var2, int var3, int var4)
        {
            if (!canBlockStay(var1, var2, var3, var4))
            {
                dropBlockAsItem(var1, var2, var3, var4, var1.getBlockMeta(var2, var3, var4));
                var1.setBlockWithNotify(var2, var3, var4, 0);
            }

        }

        public override bool canBlockStay(World var1, int var2, int var3, int var4)
        {
            return (var1.getFullBlockLightValue(var2, var3, var4) >= 8 || var1.canBlockSeeTheSky(var2, var3, var4)) && canThisPlantGrowOnThisBlockID(var1.getBlockId(var2, var3 - 1, var4));
        }

        public override Box getCollisionShape(World var1, int var2, int var3, int var4)
        {
            return null;
        }

        public override bool isOpaque()
        {
            return false;
        }

        public override bool isFullCube()
        {
            return false;
        }

        public override int getRenderType()
        {
            return 1;
        }
    }

}