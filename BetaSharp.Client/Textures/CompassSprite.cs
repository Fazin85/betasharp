using BetaSharp.Client.Rendering.Core.Textures;
using BetaSharp.Items;
using BetaSharp.Util.Maths;
using java.awt.image;
using java.io;
using javax.imageio;

namespace BetaSharp.Client.Textures;

public class CompassSprite : DynamicTexture
{
    private Minecraft _mc;
    private int[] _compass = new int[256];
    private double _angle;
    private double _angleDelta;
    private int _resolution = 16;

    public CompassSprite(Minecraft mc) : base(Item.Compass.getTextureId(0))
    {
        _mc = mc;
        Atlas = FxImage.Items;
    }

    public override void Setup(Minecraft mc)
    {
        _mc = mc;
        TextureManager tm = mc.textureManager;
        string atlasPath = "/gui/items.png";

        var handle = tm.GetTextureId(atlasPath);
        if (handle.Texture != null)
        {
            _resolution = handle.Texture.Width / 16;
        }
        else
        {
            _resolution = 16;
        }

        int pixelCount = _resolution * _resolution;
        if (_compass.Length != pixelCount)
        {
            _compass = new int[pixelCount];
            Pixels = new byte[pixelCount * 4];
        }

        try
        {
            using var stream = mc.texturePackList.SelectedTexturePack.GetResourceAsStream("gui/items.png");
            if (stream != null)
            {
                using var ms = new MemoryStream();
                stream.CopyTo(ms);
                BufferedImage atlasImage = ImageIO.read(new ByteArrayInputStream(ms.ToArray()));
                int localRes = atlasImage.getWidth() / 16;
                int sourceX = (Sprite % 16) * localRes;
                int sourceY = (Sprite / 16) * localRes;

                if (localRes == _resolution)
                {
                    atlasImage.getRGB(sourceX, sourceY, _resolution, _resolution, _compass, 0, _resolution);
                }
                else
                {
                    int[] temp = new int[localRes * localRes];
                    atlasImage.getRGB(sourceX, sourceY, localRes, localRes, temp, 0, localRes);
                    for (int y = 0; y < _resolution; y++)
                    {
                        for (int x = 0; x < _resolution; x++)
                        {
                            _compass[y * _resolution + x] = temp[(y * localRes / _resolution) * localRes + (x * localRes / _resolution)];
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
        int pixelCount = _resolution * _resolution;

        for (int i = 0; i < pixelCount; ++i)
        {
            int a = _compass[i] >> 24 & 255;
            int r = _compass[i] >> 16 & 255;
            int g = _compass[i] >> 8 & 255;
            int b = _compass[i] >> 0 & 255;
            Pixels[i * 4 + 0] = (byte)r;
            Pixels[i * 4 + 1] = (byte)g;
            Pixels[i * 4 + 2] = (byte)b;
            Pixels[i * 4 + 3] = (byte)a;
        }

        double targetAngle = 0.0D;
        if (_mc.world != null && _mc.player != null)
        {
            Vec3i spawnPos = _mc.world.getSpawnPos();
            double deltaX = spawnPos.X - _mc.player.x;
            double deltaZ = spawnPos.Z - _mc.player.z;

            targetAngle = (_mc.player.yaw - 90.0F) * Math.PI / 180.0D - Math.Atan2(deltaZ, deltaX);

            if (_mc.world.dimension.IsNether)
            {
                targetAngle = Random.Shared.NextDouble() * (float)Math.PI * 2.0D;
            }
        }

        double angleDiff;
        for (angleDiff = targetAngle - _angle; angleDiff < -Math.PI; angleDiff += Math.PI * 2.0D) ;
        while (angleDiff >= Math.PI) angleDiff -= Math.PI * 2.0D;

        if (angleDiff < -1.0D) angleDiff = -1.0D;
        if (angleDiff > 1.0D) angleDiff = 1.0D;

        _angleDelta += angleDiff * 0.1D;
        _angleDelta *= 0.8D;
        _angle += _angleDelta;

        double sinAngle = Math.Sin(_angle);
        double cosAngle = Math.Cos(_angle);

        float center = (_resolution - 1) / 2.0f;
        float needleScale = _resolution / 16.0f;

        for (int offset = -Math.Max(1, _resolution / 4); offset <= Math.Max(1, _resolution / 4); ++offset)
        {
            int pixelX = (int)(center + 0.5f + cosAngle * offset * 0.3D * needleScale);
            int pixelY = (int)(center - 0.5f - sinAngle * offset * 0.3D * 0.5D * needleScale);

            if (pixelX < 0 || pixelX >= _resolution || pixelY < 0 || pixelY >= _resolution) continue;

            int pixelIdx = pixelY * _resolution + pixelX;
            Pixels[pixelIdx * 4 + 0] = 100; // R
            Pixels[pixelIdx * 4 + 1] = 100; // G
            Pixels[pixelIdx * 4 + 2] = 100; // B
            Pixels[pixelIdx * 4 + 3] = 255; // A
        }

        for (int offset = -Math.Max(1, _resolution / 2); offset <= _resolution; ++offset)
        {
            int pixelX = (int)(center + 0.5f + sinAngle * offset * 0.3D * needleScale);
            int pixelY = (int)(center - 0.5f + cosAngle * offset * 0.3D * 0.5D * needleScale);

            if (pixelX < 0 || pixelX >= _resolution || pixelY < 0 || pixelY >= _resolution) continue;

            int pixelIdx = pixelY * _resolution + pixelX;
            bool isPointyEnd = offset >= 0;

            Pixels[pixelIdx * 4 + 0] = (byte)(isPointyEnd ? 255 : 100);
            Pixels[pixelIdx * 4 + 1] = (byte)(isPointyEnd ? 20 : 100);
            Pixels[pixelIdx * 4 + 2] = (byte)(isPointyEnd ? 20 : 100);
            Pixels[pixelIdx * 4 + 3] = 255;
        }
    }
}
