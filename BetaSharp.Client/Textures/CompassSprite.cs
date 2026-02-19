using BetaSharp.Items;
using BetaSharp.Util.Maths;
using java.awt.image;
using java.io;
using javax.imageio;

namespace BetaSharp.Client.Textures;

public class CompassSprite : DynamicTexture
{

    private readonly Minecraft mc;
    private readonly int[] compass = new int[256];
    private double angle;
    private double angleDelta;

    public CompassSprite(Minecraft var1) : base(Item.Compass.getTextureId(0))
    {
        mc = var1;
        atlas = FXImage.Items;

        try
        {
            BufferedImage var2 = ImageIO.read(new ByteArrayInputStream(AssetManager.Instance.getAsset("gui/items.png").getBinaryContent()));
            int var3 = sprite % 16 * 16;
            int var4 = sprite / 16 * 16;
            var2.getRGB(var3, var4, 16, 16, compass, 0, 16);
        }
        catch (java.io.IOException ex)
        {
            ex.printStackTrace();
        }

    }

    public override void tick()
    {
        for (int var1 = 0; var1 < 256; ++var1)
        {
            int var2 = compass[var1] >> 24 & 255;
            int var3 = compass[var1] >> 16 & 255;
            int var4 = compass[var1] >> 8 & 255;
            int var5 = compass[var1] >> 0 & 255;
            if (anaglyphEnabled)
            {
                int var6 = (var3 * 30 + var4 * 59 + var5 * 11) / 100;
                int var7 = (var3 * 30 + var4 * 70) / 100;
                int var8 = (var3 * 30 + var5 * 70) / 100;
                var3 = var6;
                var4 = var7;
                var5 = var8;
            }

            pixels[var1 * 4 + 0] = (byte)var3;
            pixels[var1 * 4 + 1] = (byte)var4;
            pixels[var1 * 4 + 2] = (byte)var5;
            pixels[var1 * 4 + 3] = (byte)var2;
        }

        double var20 = 0.0D;
        if (mc.world != null && mc.player != null)
        {
            Vec3i var21 = mc.world.getSpawnPos();
            double var23 = var21.x - mc.player.x;
            double var25 = var21.z - mc.player.z;
            var20 = (double)(mc.player.yaw - 90.0F) * Math.PI / 180.0D - java.lang.Math.atan2(var25, var23);
            if (mc.world.dimension.isNether)
            {
                var20 = java.lang.Math.random() * (double)(float)Math.PI * 2.0D;
            }
        }

        double var22;
        for (var22 = var20 - angle; var22 < -Math.PI; var22 += Math.PI * 2.0D)
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
        double var24 = java.lang.Math.sin(angle);
        double var26 = java.lang.Math.cos(angle);

        int var9;
        int var10;
        int var11;
        int var12;
        int var13;
        int var14;
        int var15;
        short var16;
        int var17;
        int var18;
        int var19;
        for (var9 = -4; var9 <= 4; ++var9)
        {
            var10 = (int)(8.5D + var26 * var9 * 0.3D);
            var11 = (int)(7.5D - var24 * var9 * 0.3D * 0.5D);
            var12 = var11 * 16 + var10;
            var13 = 100;
            var14 = 100;
            var15 = 100;
            var16 = 255;
            if (anaglyphEnabled)
            {
                var17 = (var13 * 30 + var14 * 59 + var15 * 11) / 100;
                var18 = (var13 * 30 + var14 * 70) / 100;
                var19 = (var13 * 30 + var15 * 70) / 100;
                var13 = var17;
                var14 = var18;
                var15 = var19;
            }

            pixels[var12 * 4 + 0] = (byte)var13;
            pixels[var12 * 4 + 1] = (byte)var14;
            pixels[var12 * 4 + 2] = (byte)var15;
            pixels[var12 * 4 + 3] = (byte)var16;
        }

        for (var9 = -8; var9 <= 16; ++var9)
        {
            var10 = (int)(8.5D + var24 * var9 * 0.3D);
            var11 = (int)(7.5D + var26 * var9 * 0.3D * 0.5D);
            var12 = var11 * 16 + var10;
            var13 = var9 >= 0 ? 255 : 100;
            var14 = var9 >= 0 ? 20 : 100;
            var15 = var9 >= 0 ? 20 : 100;
            var16 = 255;
            if (anaglyphEnabled)
            {
                var17 = (var13 * 30 + var14 * 59 + var15 * 11) / 100;
                var18 = (var13 * 30 + var14 * 70) / 100;
                var19 = (var13 * 30 + var15 * 70) / 100;
                var13 = var17;
                var14 = var18;
                var15 = var19;
            }

            pixels[var12 * 4 + 0] = (byte)var13;
            pixels[var12 * 4 + 1] = (byte)var14;
            pixels[var12 * 4 + 2] = (byte)var15;
            pixels[var12 * 4 + 3] = (byte)var16;
        }

    }
}
