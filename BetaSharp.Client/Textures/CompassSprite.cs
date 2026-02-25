using BetaSharp.Client.Rendering.Core.Textures;
using BetaSharp.Items;
using BetaSharp.Util.Maths;
using java.awt.image;
using java.io;
using javax.imageio;

namespace BetaSharp.Client.Textures;

public class CompassSprite : DynamicTexture
{
    private Minecraft mc;
    private int[] compass = new int[256];
    private double angle;
    private double angleDelta;
    private int resolution = 16;

    public CompassSprite(Minecraft var1) : base(Item.Compass.getTextureId(0))
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
        if (compass.Length != pixelCount)
        {
            compass = new int[pixelCount];
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
                    var2.getRGB(var3, var4, resolution, resolution, compass, 0, resolution);
                }
                else
                {
                    int[] temp = new int[localRes * localRes];
                    var2.getRGB(var3, var4, localRes, localRes, temp, 0, localRes);
                    for (int y = 0; y < resolution; y++)
                    {
                        for (int x = 0; x < resolution; x++)
                        {
                            compass[y * resolution + x] = temp[(y * localRes / resolution) * localRes + (x * localRes / resolution)];
                        }
                    }
                }
            }
            
        }
        catch (java.io.IOException ex)
        {
            ex.printStackTrace();
        }
    }

    public override void tick()
    {
        int pixelCount = resolution * resolution;
        for (int var1 = 0; var1 < pixelCount; ++var1)
        {
            int var2 = compass[var1] >> 24 & 255;
            int var3 = compass[var1] >> 16 & 255;
            int var4 = compass[var1] >> 8 & 255;
            int var5 = compass[var1] >> 0 & 255;
            pixels[var1 * 4 + 0] = (byte)var3;
            pixels[var1 * 4 + 1] = (byte)var4;
            pixels[var1 * 4 + 2] = (byte)var5;
            pixels[var1 * 4 + 3] = (byte)var2;
        }

        double var20 = 0.0D;
        if (mc.world != null && mc.player != null)
        {
            Vec3i var21 = mc.world.getSpawnPos();
            double var23 = var21.X - mc.player.x;
            double var25 = var21.Z - mc.player.z;
            var20 = (double)(mc.player.yaw - 90.0F) * Math.PI / 180.0D - java.lang.Math.atan2(var25, var23);
            if (mc.world.dimension.IsNether)
            {
                var20 = Random.Shared.NextDouble() * (double)(float)Math.PI * 2.0D;
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

        float center = (resolution - 1) / 2.0f;
        float needleScale = resolution / 16.0f;

        for (int var9 = -Math.Max(1, resolution / 4); var9 <= Math.Max(1, resolution / 4); ++var9)
        {
            int var10 = (int)(center + 0.5f + var26 * var9 * 0.3D * needleScale);
            int var11 = (int)(center - 0.5f - var24 * var9 * 0.3D * 0.5D * needleScale);
            
            if (var10 < 0 || var10 >= resolution || var11 < 0 || var11 >= resolution) continue;

            int var12 = var11 * resolution + var10;
            int var13 = 100;
            int var14 = 100;
            int var15 = 100;
            short var16 = 255;
            pixels[var12 * 4 + 0] = (byte)var13;
            pixels[var12 * 4 + 1] = (byte)var14;
            pixels[var12 * 4 + 2] = (byte)var15;
            pixels[var12 * 4 + 3] = (byte)var16;
        }

        for (int var9 = -Math.Max(1, resolution / 2); var9 <= resolution; ++var9)
        {
            int var10 = (int)(center + 0.5f + var24 * var9 * 0.3D * needleScale);
            int var11 = (int)(center - 0.5f + var26 * var9 * 0.3D * 0.5D * needleScale);
            
            if (var10 < 0 || var10 >= resolution || var11 < 0 || var11 >= resolution) continue;

            int var12 = var11 * resolution + var10;
            int var13 = var9 >= 0 ? 255 : 100;
            int var14 = var9 >= 0 ? 20 : 100;
            int var15 = var9 >= 0 ? 20 : 100;
            short var16 = 255;
            pixels[var12 * 4 + 0] = (byte)var13;
            pixels[var12 * 4 + 1] = (byte)var14;
            pixels[var12 * 4 + 2] = (byte)var15;
            pixels[var12 * 4 + 3] = (byte)var16;
        }
    }
}
