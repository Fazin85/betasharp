using betareborn.Worlds;

namespace betareborn.Blocks
{
    public class BlockSapling : BlockFlower
    {
        public BlockSapling(int var1, int var2) : base(var1, var2)
        {
            float var3 = 0.4F;
            setBoundingBox(0.5F - var3, 0.0F, 0.5F - var3, 0.5F + var3, var3 * 2.0F, 0.5F + var3);
        }

        public override void onTick(World var1, int var2, int var3, int var4, java.util.Random var5)
        {
            if (!var1.multiplayerWorld)
            {
                base.onTick(var1, var2, var3, var4, var5);
                if (var1.getBlockLightValue(var2, var3 + 1, var4) >= 9 && var5.nextInt(30) == 0)
                {
                    int var6 = var1.getBlockMeta(var2, var3, var4);
                    if ((var6 & 8) == 0)
                    {
                        var1.setBlockMeta(var2, var3, var4, var6 | 8);
                    }
                    else
                    {
                        growTree(var1, var2, var3, var4, var5);
                    }
                }

            }
        }

        public override int getTexture(int var1, int var2)
        {
            var2 &= 3;
            return var2 == 1 ? 63 : (var2 == 2 ? 79 : base.getTexture(var1, var2));
        }

        public void growTree(World var1, int var2, int var3, int var4, java.util.Random var5)
        {
            int var6 = var1.getBlockMeta(var2, var3, var4) & 3;
            var1.setBlock(var2, var3, var4, 0);
            Object var7 = null;
            if (var6 == 1)
            {
                var7 = new WorldGenTaiga2();
            }
            else if (var6 == 2)
            {
                var7 = new WorldGenForest();
            }
            else
            {
                var7 = new WorldGenTrees();
                if (var5.nextInt(10) == 0)
                {
                    var7 = new WorldGenBigTree();
                }
            }

            if (!((WorldGenerator)var7).generate(var1, var5, var2, var3, var4))
            {
                var1.setBlockAndMetadata(var2, var3, var4, id, var6);
            }

        }

        protected override int damageDropped(int var1)
        {
            return var1 & 3;
        }
    }

}