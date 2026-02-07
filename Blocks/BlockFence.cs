using betareborn.Materials;
using betareborn.Worlds;

namespace betareborn.Blocks
{
    public class BlockFence : Block
    {

        public BlockFence(int var1, int var2) : base(var1, var2, Material.WOOD)
        {
        }

        public override bool canPlaceBlockAt(World var1, int var2, int var3, int var4)
        {
            return var1.getBlockId(var2, var3 - 1, var4) == id ? true : (!var1.getMaterial(var2, var3 - 1, var4).isSolid() ? false : base.canPlaceBlockAt(var1, var2, var3, var4));
        }

        public override Box getCollisionShape(World var1, int var2, int var3, int var4)
        {
            return Box.createCached((double)var2, (double)var3, (double)var4, (double)(var2 + 1), (double)((float)var3 + 1.5F), (double)(var4 + 1));
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
            return 11;
        }
    }

}