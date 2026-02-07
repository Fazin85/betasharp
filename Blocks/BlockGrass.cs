using betareborn.Materials;
using betareborn.Worlds;

namespace betareborn.Blocks
{
    public class BlockGrass : Block
    {
        public BlockGrass(int var1) : base(var1, Material.SOLID_ORGANIC)
        {
            textureId = 3;
            setTickRandomly(true);
        }

        public override int getTexture(BlockView var1, int var2, int var3, int var4, int var5)
        {
            if (var5 == 1)
            {
                return 0;
            }
            else if (var5 == 0)
            {
                return 2;
            }
            else
            {
                Material var6 = var1.getMaterial(var2, var3 + 1, var4);
                return var6 != Material.SNOW_LAYER && var6 != Material.SNOW_BLOCK ? 3 : 68;
            }
        }

        public override int colorMultiplier(BlockView var1, int var2, int var3, int var4)
        {
            var1.getBiomeSource().getBiomesInArea(var2, var4, 1, 1);
            double var5 = var1.getBiomeSource().temperatureMap[0];
            double var7 = var1.getBiomeSource().downfallMap[0];
            return ColorizerGrass.getGrassColor(var5, var7);
        }

        public override void onTick(World var1, int var2, int var3, int var4, java.util.Random var5)
        {
            if (!var1.multiplayerWorld)
            {
                if (var1.getBlockLightValue(var2, var3 + 1, var4) < 4 && Block.BLOCK_LIGHT_OPACITY[var1.getBlockId(var2, var3 + 1, var4)] > 2)
                {
                    if (var5.nextInt(4) != 0)
                    {
                        return;
                    }

                    var1.setBlockWithNotify(var2, var3, var4, Block.DIRT.id);
                }
                else if (var1.getBlockLightValue(var2, var3 + 1, var4) >= 9)
                {
                    int var6 = var2 + var5.nextInt(3) - 1;
                    int var7 = var3 + var5.nextInt(5) - 3;
                    int var8 = var4 + var5.nextInt(3) - 1;
                    int var9 = var1.getBlockId(var6, var7 + 1, var8);
                    if (var1.getBlockId(var6, var7, var8) == Block.DIRT.id && var1.getBlockLightValue(var6, var7 + 1, var8) >= 4 && Block.BLOCK_LIGHT_OPACITY[var9] <= 2)
                    {
                        var1.setBlockWithNotify(var6, var7, var8, Block.GRASS_BLOCK.id);
                    }
                }

            }
        }

        public override int getDroppedItemId(int var1, java.util.Random var2)
        {
            return Block.DIRT.getDroppedItemId(0, var2);
        }
    }

}