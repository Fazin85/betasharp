using BetaSharp.Items;
using java.awt.image;
using java.io;
using javax.imageio;

namespace BetaSharp.Client.Textures;

public class ClockSprite : DynamicTexture
{

    private readonly Minecraft mc;
    private readonly int[] clock = new int[256];
    private readonly int[] dial = new int[256];
    private double angle;
    private double angleDelta;

    public ClockSprite(Minecraft var1) : base(Item.Clock.getTextureId(0))
    {
        mc = var1;
        atlas = FXImage.Items;

        try
        {
            BufferedImage var2 = ImageIO.read(new ByteArrayInputStream(AssetManager.Instance.getAsset("gui/items.png").getBinaryContent()));
            int var3 = sprite % 16 * 16;
            int var4 = sprite / 16 * 16;
            var2.getRGB(var3, var4, 16, 16, clock, 0, 16);
            var2 = ImageIO.read(new ByteArrayInputStream(AssetManager.Instance.getAsset("misc/dial.png").getBinaryContent()));
            var2.getRGB(0, 0, 16, 16, dial, 0, 16);
        }
        catch (java.io.IOException ex)
        {
            ex.printStackTrace();
        }

    }

    public override void tick()
    {
        double var1 = 0.0D;
        if (mc.world != null && mc.player != null)
        {
            float var3 = mc.world.getTime(1.0F);
            var1 = (double)(-var3 * (float)Math.PI * 2.0F);
            if (mc.world.dimension.isNether)
            {
                var1 = java.lang.Math.random() * (double)(float)Math.PI * 2.0D;
            }
        }

        double var22;
        for (var22 = var1 - angle; var22 < -Math.PI; var22 += Math.PI * 2.0D)
        {
        }

        while (var22 >= Math.PI)
        {
            var22 -= Math.PI * 2.0D;
        }

        if (var22 < -1.0D)
        {
            var22 = -1.0D;
        }

        if (var22 > 1.0D)
        {
            var22 = 1.0D;
        }

        angleDelta += var22 * 0.1D;
        angleDelta *= 0.8D;
        angle += angleDelta;
        double var5 = java.lang.Math.sin(angle);
        double var7 = java.lang.Math.cos(angle);

        for (int var9 = 0; var9 < 256; ++var9)
        {
            int var10 = clock[var9] >> 24 & 255;
            int var11 = clock[var9] >> 16 & 255;
            int var12 = clock[var9] >> 8 & 255;
            int var13 = clock[var9] >> 0 & 255;
            if (var11 == var13 && var12 == 0 && var13 > 0)
            {
                double var14 = -(var9 % 16 / 15.0D - 0.5D);
                double var16 = var9 / 16 / 15.0D - 0.5D;
                int var18 = var11;
                int var19 = (int)((var14 * var7 + var16 * var5 + 0.5D) * 16.0D);
                int var20 = (int)((var16 * var7 - var14 * var5 + 0.5D) * 16.0D);
                int var21 = (var19 & 15) + (var20 & 15) * 16;
                var10 = dial[var21] >> 24 & 255;
                var11 = (dial[var21] >> 16 & 255) * var11 / 255;
                var12 = (dial[var21] >> 8 & 255) * var18 / 255;
                var13 = (dial[var21] >> 0 & 255) * var18 / 255;
            }

            if (anaglyphEnabled)
            {
                int var23 = (var11 * 30 + var12 * 59 + var13 * 11) / 100;
                int var15 = (var11 * 30 + var12 * 70) / 100;
                int var24 = (var11 * 30 + var13 * 70) / 100;
                var11 = var23;
                var12 = var15;
                var13 = var24;
            }

            pixels[var9 * 4 + 0] = (byte)var11;
            pixels[var9 * 4 + 1] = (byte)var12;
            pixels[var9 * 4 + 2] = (byte)var13;
            pixels[var9 * 4 + 3] = (byte)var10;
        }

    }
}
