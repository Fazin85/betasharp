using betareborn.Entities;
using betareborn.Items;
using betareborn.Materials;
using betareborn.Worlds;

namespace betareborn.Blocks
{
    public class BlockWeb : Block
    {
        public BlockWeb(int var1, int var2) : base(var1, var2, Material.COBWEB)
        {
        }

        public override void onEntityCollidedWithBlock(World var1, int var2, int var3, int var4, Entity var5)
        {
            var5.isInWeb = true;
        }

        public override bool isOpaque()
        {
            return false;
        }

        public override Box getCollisionShape(World var1, int var2, int var3, int var4)
        {
            return null;
        }

        public override int getRenderType()
        {
            return 1;
        }

        public override bool isFullCube()
        {
            return false;
        }

        public override int getDroppedItemId(int var1, java.util.Random var2)
        {
            return Item.silk.id;
        }
    }

}