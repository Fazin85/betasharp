using BetaSharp.Blocks;
using BetaSharp.Util.Maths;

namespace BetaSharp.Client.Textures;

public class LavaSideSprite : DynamicTexture
{

    protected float[] current = new float[256];
    protected float[] next = new float[256];
    protected float[] heat = new float[256];
    protected float[] heatDelta = new float[256];
    int ticks;

    public LavaSideSprite() : base(Block.FlowingLava.textureId + 1)
    {
        replicate = 2;
    }

    public override void tick()
    {
        ++ticks;

        int var2;
        float var3;
        int var5;
        int var6;
        int var7;
        int var8;
        int var9;
        for (int var1 = 0; var1 < 16; ++var1)
        {
            for (var2 = 0; var2 < 16; ++var2)
            {
                var3 = 0.0F;
                int var4 = (int)(MathHelper.Sin(var2 * (float)Math.PI * 2.0F / 16.0F) * 1.2F);
                var5 = (int)(MathHelper.Sin(var1 * (float)Math.PI * 2.0F / 16.0F) * 1.2F);

                for (var6 = var1 - 1; var6 <= var1 + 1; ++var6)
                {
                    for (var7 = var2 - 1; var7 <= var2 + 1; ++var7)
                    {
                        var8 = var6 + var4 & 15;
                        var9 = var7 + var5 & 15;
                        var3 += current[var8 + var9 * 16];
                    }
                }

                next[var1 + var2 * 16] = var3 / 10.0F + (heat[(var1 + 0 & 15) + (var2 + 0 & 15) * 16] + heat[(var1 + 1 & 15) + (var2 + 0 & 15) * 16] + heat[(var1 + 1 & 15) + (var2 + 1 & 15) * 16] + heat[(var1 + 0 & 15) + (var2 + 1 & 15) * 16]) / 4.0F * 0.8F;
                heat[var1 + var2 * 16] += heatDelta[var1 + var2 * 16] * 0.01F;
                if (heat[var1 + var2 * 16] < 0.0F)
                {
                    heat[var1 + var2 * 16] = 0.0F;
                }

                heatDelta[var1 + var2 * 16] -= 0.06F;
                if (java.lang.Math.random() < 0.005D)
                {
                    heatDelta[var1 + var2 * 16] = 1.5F;
                }
            }
        }

        (next, current) = (current, next);

        for (var2 = 0; var2 < 256; ++var2)
        {
            var3 = current[var2 - ticks / 3 * 16 & 255] * 2.0F;
            if (var3 > 1.0F)
            {
                var3 = 1.0F;
            }

            if (var3 < 0.0F)
            {
                var3 = 0.0F;
            }

            var5 = (int)(var3 * 100.0F + 155.0F);
            var6 = (int)(var3 * var3 * 255.0F);
            var7 = (int)(var3 * var3 * var3 * var3 * 128.0F);
            if (anaglyphEnabled)
            {
                var8 = (var5 * 30 + var6 * 59 + var7 * 11) / 100;
                var9 = (var5 * 30 + var6 * 70) / 100;
                int var10 = (var5 * 30 + var7 * 70) / 100;
                var5 = var8;
                var6 = var9;
                var7 = var10;
            }

            pixels[var2 * 4 + 0] = (byte)var5;
            pixels[var2 * 4 + 1] = (byte)var6;
            pixels[var2 * 4 + 2] = (byte)var7;
            pixels[var2 * 4 + 3] = 255;
        }

    }
}