using betareborn.Entities;
using betareborn.Materials;
using betareborn.Worlds;

namespace betareborn.Blocks
{
    public class BlockSand : Block
    {
        public static bool fallInstantly = false;

        public BlockSand(int var1, int var2) : base(var1, var2, Material.SAND)
        {
        }

        public override void onBlockAdded(World var1, int var2, int var3, int var4)
        {
            var1.scheduleBlockUpdate(var2, var3, var4, this.id, this.tickRate());
        }

        public override void neighborUpdate(World var1, int var2, int var3, int var4, int var5)
        {
            var1.scheduleBlockUpdate(var2, var3, var4, this.id, this.tickRate());
        }

        public override void onTick(World var1, int var2, int var3, int var4, java.util.Random var5)
        {
            this.tryToFall(var1, var2, var3, var4);
        }

        private void tryToFall(World var1, int var2, int var3, int var4)
        {
            if (canFallBelow(var1, var2, var3 - 1, var4) && var3 >= 0)
            {
                sbyte var8 = 32;
                if (!fallInstantly && var1.checkChunksExist(var2 - var8, var3 - var8, var4 - var8, var2 + var8, var3 + var8, var4 + var8))
                {
                    EntityFallingSand var9 = new EntityFallingSand(var1, (double)((float)var2 + 0.5F), (double)((float)var3 + 0.5F), (double)((float)var4 + 0.5F), this.id);
                    var1.spawnEntity(var9);
                }
                else
                {
                    var1.setBlockWithNotify(var2, var3, var4, 0);

                    while (canFallBelow(var1, var2, var3 - 1, var4) && var3 > 0)
                    {
                        --var3;
                    }

                    if (var3 > 0)
                    {
                        var1.setBlockWithNotify(var2, var3, var4, this.id);
                    }
                }
            }

        }

        public override int tickRate()
        {
            return 3;
        }

        public static bool canFallBelow(World var0, int var1, int var2, int var3)
        {
            int var4 = var0.getBlockId(var1, var2, var3);
            if (var4 == 0)
            {
                return true;
            }
            else if (var4 == Block.FIRE.id)
            {
                return true;
            }
            else
            {
                Material var5 = Block.BLOCKS[var4].material;
                return var5 == Material.WATER ? true : var5 == Material.LAVA;
            }
        }
    }

}