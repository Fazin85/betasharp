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

    public CompassSprite(Minecraft mc) : base(Item.COMPASS.getTextureId(0))
    {
        this.mc = mc;
        atlas = FXImage.Items;

        try
        {
            BufferedImage var2 = ImageIO.read(new ByteArrayInputStream(AssetManager.Instance.getAsset("gui/items.png").getBinaryContent()));
            int var3 = sprite % 16 * 16;
            int var4 = sprite / 16 * 16;
            var2.getRGB(var3, var4, 16, 16, compass, 0, 16);
        }
        catch (java.io.IOException var5)
        {
            var5.printStackTrace();
        }

    }

    public override void tick()
    {
        for (int PixelIndex = 0; PixelIndex < 256; ++PixelIndex)
        {
            int PixelAlpha = compass[PixelIndex] >> 24 & 255;
            int Red = compass[PixelIndex] >> 16 & 255;
            int Green = compass[PixelIndex] >> 8 & 255;
            int Blue = compass[PixelIndex] >> 0 & 255;
            if (anaglyphEnabled)
            {
                int PixelAnaglyphRed = (Red * 30 + Green * 59 + Blue * 11) / 100;
                int PixelAnaglyphGreen = (Red * 30 + Green * 70) / 100;
                int PixelAnaglyphBlue = (Red * 30 + Blue * 70) / 100;
                Red = PixelAnaglyphRed;
                Green = PixelAnaglyphGreen;
                Blue = PixelAnaglyphBlue;
            }

            pixels[PixelIndex * 4 + 0] = (byte)Red;
            pixels[PixelIndex * 4 + 1] = (byte)Green;
            pixels[PixelIndex * 4 + 2] = (byte)Blue;
            pixels[PixelIndex * 4 + 3] = (byte)PixelAlpha;
        }

        double AngleToSpawn = 0.0D;
        if (mc.world != null && mc.player != null)
        {
            Vec3i SpawnPos = mc.world.getSpawnPos();
            double XDistToSpawn = SpawnPos.x - mc.player.x;
            double ZDistToSpawn = SpawnPos.z - mc.player.z;
            AngleToSpawn = (double)(mc.player.yaw - 90.0F) * Math.PI / 180.0D - java.lang.Math.atan2(ZDistToSpawn, XDistToSpawn);
            if (mc.world.Dimension.isNether)
            {
                AngleToSpawn = java.lang.Math.random() * (double)(float)Math.PI * 2.0D;
            }
        }

        double AngleAdjustment;
        for (AngleAdjustment = AngleToSpawn - angle; AngleAdjustment < -Math.PI; AngleAdjustment += Math.PI * 2.0D)
        {
        }

        while (AngleAdjustment >= Math.PI)
        {
            AngleAdjustment -= Math.PI * 2.0D;
        }

        if (AngleAdjustment < -1.0D)
        {
            AngleAdjustment = -1.0D;
        }

        if (AngleAdjustment > 1.0D)
        {
            AngleAdjustment = 1.0D;
        }

        angleDelta += AngleAdjustment * 0.1D;
        angleDelta *= 0.8D;
        angle += angleDelta;
        double SinAngle = java.lang.Math.sin(angle);
        double CosAngle = java.lang.Math.cos(angle);

        int NeedleOffset;
        int PixelX;
        int PixelY;
        int PixelIdx;
        int NeedleRed;
        int NeedleGreen;
        int NeedleBlue;
        short Alpha;
        int AnaglyphRed;
        int AnaglyphGreen;
        int AnaglyphBlue;
        for (NeedleOffset = -4; NeedleOffset <= 4; ++NeedleOffset)
        {
            PixelX = (int)(8.5D + CosAngle * NeedleOffset * 0.3D);
            PixelY = (int)(7.5D - SinAngle * NeedleOffset * 0.3D * 0.5D);
            PixelIdx = PixelY * 16 + PixelX;
            NeedleRed = 100;
            NeedleGreen = 100;
            NeedleBlue = 100;
            Alpha = 255;
            if (anaglyphEnabled)
            {
                AnaglyphRed = (NeedleRed * 30 + NeedleGreen * 59 + NeedleBlue * 11) / 100;
                AnaglyphGreen = (NeedleRed * 30 + NeedleGreen * 70) / 100;
                AnaglyphBlue = (NeedleRed * 30 + NeedleBlue * 70) / 100;
                NeedleRed = AnaglyphRed;
                NeedleGreen = AnaglyphGreen;
                NeedleBlue = AnaglyphBlue;
            }

            pixels[PixelIdx * 4 + 0] = (byte)NeedleRed;
            pixels[PixelIdx * 4 + 1] = (byte)NeedleGreen;
            pixels[PixelIdx * 4 + 2] = (byte)NeedleBlue;
            pixels[PixelIdx * 4 + 3] = (byte)Alpha;
        }

        for (NeedleOffset = -8; NeedleOffset <= 16; ++NeedleOffset)
        {
            PixelX = (int)(8.5D + SinAngle * NeedleOffset * 0.3D);
            PixelY = (int)(7.5D + CosAngle * NeedleOffset * 0.3D * 0.5D);
            PixelIdx = PixelY * 16 + PixelX;
            NeedleRed = NeedleOffset >= 0 ? 255 : 100;
            NeedleGreen = NeedleOffset >= 0 ? 20 : 100;
            NeedleBlue = NeedleOffset >= 0 ? 20 : 100;
            Alpha = 255;
            if (anaglyphEnabled)
            {
                AnaglyphRed = (NeedleRed * 30 + NeedleGreen * 59 + NeedleBlue * 11) / 100;
                AnaglyphGreen = (NeedleRed * 30 + NeedleGreen * 70) / 100;
                AnaglyphBlue = (NeedleRed * 30 + NeedleBlue * 70) / 100;
                NeedleRed = AnaglyphRed;
                NeedleGreen = AnaglyphGreen;
                NeedleBlue = AnaglyphBlue;
            }

            pixels[PixelIdx * 4 + 0] = (byte)NeedleRed;
            pixels[PixelIdx * 4 + 1] = (byte)NeedleGreen;
            pixels[PixelIdx * 4 + 2] = (byte)NeedleBlue;
            pixels[PixelIdx * 4 + 3] = (byte)Alpha;
        }

    }
}