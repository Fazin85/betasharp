using betareborn.Entities;
using betareborn.Materials;
using betareborn.Worlds;

namespace betareborn.Blocks
{
    public class BlockSoulSand : Block
    {

        public BlockSoulSand(int var1, int var2) : base(var1, var2, Material.SAND)
        {
        }

        public override Box getCollisionShape(World var1, int var2, int var3, int var4)
        {
            float var5 = 2.0F / 16.0F;
            return Box.createCached((double)var2, (double)var3, (double)var4, (double)(var2 + 1), (double)((float)(var3 + 1) - var5), (double)(var4 + 1));
        }

        public override void onEntityCollidedWithBlock(World var1, int var2, int var3, int var4, Entity var5)
        {
            var5.motionX *= 0.4D;
            var5.motionZ *= 0.4D;
        }
    }
}