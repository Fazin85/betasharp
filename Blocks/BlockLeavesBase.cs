using betareborn.Blocks.Materials;
using betareborn.Worlds;

namespace betareborn.Blocks
{
    public class BlockLeavesBase : Block
    {
        protected bool graphicsLevel;

        protected BlockLeavesBase(int id, int textureId, Material material, bool graphicsLevel) : base(id, textureId, material)
        {
            this.graphicsLevel = graphicsLevel;
        }

        public override bool isOpaque()
        {
            return false;
        }

        public override bool isSideVisible(BlockView blockView, int x, int y, int z, int side)
        {
            int var6 = blockView.getBlockId(x, y, z);
            return !graphicsLevel && var6 == id ? false : base.isSideVisible(blockView, x, y, z, side);
        }
    }

}