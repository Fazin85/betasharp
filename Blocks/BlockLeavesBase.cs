using betareborn.Materials;

namespace betareborn.Blocks
{
    public class BlockLeavesBase : Block
    {
        protected bool graphicsLevel;

        protected BlockLeavesBase(int var1, int var2, Material var3, bool var4) : base(var1, var2, var3)
        {
            graphicsLevel = var4;
        }

        public override bool isOpaque()
        {
            return false;
        }

        public override bool isSideVisible(BlockView var1, int var2, int var3, int var4, int var5)
        {
            int var6 = var1.getBlockId(var2, var3, var4);
            return !graphicsLevel && var6 == id ? false : base.isSideVisible(var1, var2, var3, var4, var5);
        }
    }

}