using betareborn.Biomes;
using betareborn.Blocks;
using betareborn.Chunks;
using Silk.NET.Maths;

namespace betareborn.Worlds
{
    public class WorldProviderSky : WorldProvider
    {

        public override void registerWorldChunkManager()
        {
            worldChunkMgr = new WorldChunkManagerHell(Biome.sky, 0.5D, 0.0D);
            worldType = 1;
        }

        public override IChunkProvider getChunkProvider()
        {
            return new ChunkProviderSky(worldObj, worldObj.getRandomSeed());
        }

        public override float calculateCelestialAngle(long var1, float var3)
        {
            return 0.0F;
        }

        public override float[] calcSunriseSunsetColors(float var1, float var2)
        {
            return null;
        }

        public override Vector3D<double> func_4096_a(float var1, float var2)
        {
            int var3 = 8421536;
            float var4 = MathHelper.cos(var1 * (float)Math.PI * 2.0F) * 2.0F + 0.5F;
            if (var4 < 0.0F)
            {
                var4 = 0.0F;
            }

            if (var4 > 1.0F)
            {
                var4 = 1.0F;
            }

            float var5 = (float)(var3 >> 16 & 255) / 255.0F;
            float var6 = (float)(var3 >> 8 & 255) / 255.0F;
            float var7 = (float)(var3 & 255) / 255.0F;
            var5 *= var4 * 0.94F + 0.06F;
            var6 *= var4 * 0.94F + 0.06F;
            var7 *= var4 * 0.91F + 0.09F;
            return new((double)var5, (double)var6, (double)var7);
        }

        public override bool func_28112_c()
        {
            return false;
        }

        public override float getCloudHeight()
        {
            return 8.0F;
        }

        public override bool canCoordinateBeSpawn(int var1, int var2)
        {
            int var3 = worldObj.getFirstUncoveredBlock(var1, var2);
            return var3 == 0 ? false : Block.BLOCKS[var3].material.blocksMovement();
        }
    }

}