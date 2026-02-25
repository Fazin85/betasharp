using BetaSharp.Client.Rendering.Core.Textures;
using BetaSharp.Items;
using BetaSharp.Util.Maths;
using java.awt.image;
using java.io;
using javax.imageio;

namespace BetaSharp.Client.Textures;

public class ClockSprite : DynamicTexture
{
    private Minecraft mc;
    private int[] clock = new int[256];
    private int[] dial = new int[256];
    private double angle;
    private double angleDelta;
    private int resolution = 16;
    private int dialResolution = 16;

    public ClockSprite(Minecraft var1) : base(Item.Clock.getTextureId(0))
    {
        mc = var1;
        atlas = FXImage.Items;
    }

    public override void Setup(Minecraft mc)
    {
        this.mc = mc;
        TextureManager tm = mc.textureManager;
        string atlasPath = "/gui/items.png";
        
        var handle = tm.GetTextureId(atlasPath);
        if (handle.Texture != null)
        {
            resolution = handle.Texture.Width / 16;
        }
        else
        {
            resolution = 16;
        }

        int pixelCount = resolution * resolution;
        if (clock.Length != pixelCount)
        {
            clock = new int[pixelCount];
            dial = new int[pixelCount];
            pixels = new byte[pixelCount * 4];
        }

        try
        {
            using var stream = mc.texturePackList.SelectedTexturePack.GetResourceAsStream("gui/items.png");
            if (stream != null)
            {
                using var ms = new MemoryStream();
                stream.CopyTo(ms);
                BufferedImage var2 = ImageIO.read(new ByteArrayInputStream(ms.ToArray()));
                int localRes = var2.getWidth() / 16;
                int var3 = (sprite % 16) * localRes;
                int var4 = (sprite / 16) * localRes;

                if (localRes == resolution)
                {
                    var2.getRGB(var3, var4, resolution, resolution, clock, 0, resolution);
                }
                else
                {
                    int[] temp = new int[localRes * localRes];
                    var2.getRGB(var3, var4, localRes, localRes, temp, 0, localRes);
                    for (int y = 0; y < resolution; y++)
                    {
                        for (int x = 0; x < resolution; x++)
                        {
                            clock[y * resolution + x] = temp[(y * localRes / resolution) * localRes + (x * localRes / resolution)];
                        }
                    }
                }
            }
            
            using var dialStream = mc.texturePackList.SelectedTexturePack.GetResourceAsStream("misc/dial.png");
            if (dialStream != null)
            {
                using var ms = new MemoryStream();
                dialStream.CopyTo(ms);
                BufferedImage var2 = ImageIO.read(new ByteArrayInputStream(ms.ToArray()));
                dialResolution = var2.getWidth();
                int dialPixelCount = dialResolution * dialResolution;
                if (dial.Length != dialPixelCount)
                {
                    dial = new int[dialPixelCount];
                }
                var2.getRGB(0, 0, dialResolution, dialResolution, dial, 0, dialResolution); 
            }
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
            if (mc.world.dimension.IsNether)
            {
                var1 = Random.Shared.NextDouble() * (double)(float)Math.PI * 2.0D;
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

        int pixelCount = resolution * resolution;
        float invResMinus1 = 1.0f / (resolution - 1);

        for (int var9 = 0; var9 < pixelCount; ++var9)
        {
            int var10 = clock[var9] >> 24 & 255;
            int var11 = clock[var9] >> 16 & 255;
            int var12 = clock[var9] >> 8 & 255;
            int var13 = clock[var9] >> 0 & 255;
            
            if (Math.Abs(var11 - var13) < 10 && var12 < 40 && var11 > 100)
            {
                double var14 = -((var9 % resolution) * invResMinus1 - 0.5D);
                double var16 = (var9 / resolution) * invResMinus1 - 0.5D;
                int var18 = var11;
                int var19 = (int)((var14 * var7 + var16 * var5 + 0.5D) * dialResolution);
                int var20 = (int)((var16 * var7 - var14 * var5 + 0.5D) * dialResolution);
                int var21 = (var19 & (dialResolution - 1)) + (var20 & (dialResolution - 1)) * dialResolution;
                var10 = dial[var21] >> 24 & 255;
                var11 = (dial[var21] >> 16 & 255) * var11 / 255;
                var12 = (dial[var21] >> 8 & 255) * var18 / 255;
                var13 = (dial[var21] >> 0 & 255) * var18 / 255;
            }

            pixels[var9 * 4 + 0] = (byte)var11;
            pixels[var9 * 4 + 1] = (byte)var12;
            pixels[var9 * 4 + 2] = (byte)var13;
            pixels[var9 * 4 + 3] = (byte)var10;
        }
    }
}
