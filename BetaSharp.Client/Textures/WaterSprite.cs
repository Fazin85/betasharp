using BetaSharp.Blocks;
using BetaSharp.Util.Maths;

namespace BetaSharp.Client.Textures;

public class WaterSprite : DynamicTexture
{

    protected float[] current = new float[256];
    protected float[] next = new float[256];
    protected float[] heat = new float[256];
    protected float[] heatDelta = new float[256];

    public WaterSprite() : base(Block.FlowingWater.textureId)
    {
    }

    public override void tick()
    {
        int var1;
        int var2;
        float var3;
        int var5;
        int var6;
        for (var1 = 0; var1 < 16; ++var1)
        {
            for (var2 = 0; var2 < 16; ++var2)
            {
                var3 = 0.0F;

                for (int var4 = var1 - 1; var4 <= var1 + 1; ++var4)
                {
                    var5 = var4 & 15;
                    var6 = var2 & 15;
                    var3 += current[var5 + var6 * 16];
                }

                next[var1 + var2 * 16] = var3 / 3.3F + heat[var1 + var2 * 16] * 0.8F;
            }
        }

        for (var1 = 0; var1 < 16; ++var1)
        {
            for (var2 = 0; var2 < 16; ++var2)
            {
                heat[var1 + var2 * 16] += heatDelta[var1 + var2 * 16] * 0.05F;
                if (heat[var1 + var2 * 16] < 0.0F)
                {
                    heat[var1 + var2 * 16] = 0.0F;
                }

                heatDelta[var1 + var2 * 16] -= 0.1F;
                if (Random.Shared.NextDouble() < 0.05D)
                {
                    heatDelta[var1 + var2 * 16] = 0.5F;
                }
            }
        }

        (next, current) = (current, next);

        for (var2 = 0; var2 < 256; ++var2)
        {
            var3 = current[var2];
            if (var3 > 1.0F)
            {
                var3 = 1.0F;
            }

            if (var3 < 0.0F)
            {
                var3 = 0.0F;
            }

            float var13 = var3 * var3;
            var5 = (int)(32.0F + var13 * 32.0F);
            var6 = (int)(50.0F + var13 * 64.0F);
            int var7 = 255;
            int var8 = (int)(146.0F + var13 * 50.0F);
            if (anaglyphEnabled)
            {
                int var9 = (var5 * 30 + var6 * 59 + var7 * 11) / 100;
                int var10 = (var5 * 30 + var6 * 70) / 100;
                int var11 = (var5 * 30 + var7 * 70) / 100;
                var5 = var9;
                var6 = var10;
                var7 = var11;
            }

            pixels[var2 * 4 + 0] = (byte)var5;
            pixels[var2 * 4 + 1] = (byte)var6;
            pixels[var2 * 4 + 2] = (byte)var7;
            pixels[var2 * 4 + 3] = (byte)var8;
        }

    }
}
