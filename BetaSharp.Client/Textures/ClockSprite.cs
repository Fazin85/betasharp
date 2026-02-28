using BetaSharp.Client.Rendering.Core.Textures;
using BetaSharp.Items;
using BetaSharp.Util.Maths;
using java.awt.image;
using java.io;
using javax.imageio;

namespace BetaSharp.Client.Textures;

public class ClockSprite : DynamicTexture
{
    private Minecraft _mc;
    private int[] _clock = new int[256];
    private int[] _dial = new int[256];
    private double _angle;
    private double _angleDelta;
    private int _resolution = 16;
    private int _dialResolution = 16;

    public ClockSprite(Minecraft var1) : base(Item.Clock.getTextureId(0))
    {
        _mc = var1;
        Atlas = FxImage.Items;
    }

    public override void Setup(Minecraft mc)
    {
        this._mc = mc;
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
        if (_clock.Length != pixelCount)
        {
            _clock = new int[pixelCount];
            _dial = new int[pixelCount];
            Pixels = new byte[pixelCount * 4];
        }

        try
        {
            using var stream = mc.texturePackList.SelectedTexturePack.GetResourceAsStream("gui/items.png");
            if (stream != null)
            {
                using var ms = new MemoryStream();
                stream.CopyTo(ms);
                BufferedImage image = ImageIO.read(new ByteArrayInputStream(ms.ToArray()));
                int atlasResolution = image.getWidth() / 16;
                int sourceX = (Sprite % 16) * atlasResolution;
                int sourceY = (Sprite / 16) * atlasResolution;

                if (atlasResolution == _resolution)
                {
                    image.getRGB(sourceX, sourceY, _resolution, _resolution, _clock, 0, _resolution);
                }
                else
                {
                    int[] temp = new int[atlasResolution * atlasResolution];
                    image.getRGB(sourceX, sourceY, atlasResolution, atlasResolution, temp, 0, atlasResolution);
                    for (int y = 0; y < _resolution; y++)
                    {
                        for (int x = 0; x < _resolution; x++)
                        {
                            _clock[y * _resolution + x] = temp[(y * atlasResolution / _resolution) * atlasResolution + (x * atlasResolution / _resolution)];
                        }
                    }
                }
            }

            using var dialStream = mc.texturePackList.SelectedTexturePack.GetResourceAsStream("misc/dial.png");
            if (dialStream != null)
            {
                using var ms = new MemoryStream();
                dialStream.CopyTo(ms);
                BufferedImage dialImage = ImageIO.read(new ByteArrayInputStream(ms.ToArray()));
                _dialResolution = dialImage.getWidth();
                int dialPixelCount = _dialResolution * _dialResolution;
                if (_dial.Length != dialPixelCount)
                {
                    _dial = new int[dialPixelCount];
                }

                dialImage.getRGB(0, 0, _dialResolution, _dialResolution, _dial, 0, _dialResolution);
            }
        }
        catch (java.io.IOException ex)
        {
            ex.printStackTrace();
        }
    }

    public override void tick()
    {
        double targetAngle = 0.0D;
        if (_mc.world != null && _mc.player != null)
        {
            float worldTime = _mc.world.getTime(1.0F);
            targetAngle = (double)(-worldTime * (float)Math.PI * 2.0F);
            if (_mc.world.dimension.IsNether)
            {
                targetAngle = Random.Shared.NextDouble() * (double)(float)Math.PI * 2.0D;
            }
        }

        double angleDifference;
        for (angleDifference = targetAngle - _angle; angleDifference < -Math.PI; angleDifference += Math.PI * 2.0D)
        {
        }

        while (angleDifference >= Math.PI)
        {
            angleDifference -= Math.PI * 2.0D;
        }

        if (angleDifference < -1.0D) angleDifference = -1.0D;
        if (angleDifference > 1.0D) angleDifference = 1.0D;

        _angleDelta += angleDifference * 0.1D;
        _angleDelta *= 0.8D;
        _angle += _angleDelta;

        double sinAngle = Math.Sin(_angle);
        double cosAngle = Math.Cos(_angle);

        int pixelCount = _resolution * _resolution;
        float invResMinus1 = 1.0f / (_resolution - 1);

        for (int pixelIdx = 0; pixelIdx < pixelCount; ++pixelIdx)
        {
            int alpha = _clock[pixelIdx] >> 24 & 255;
            int red = _clock[pixelIdx] >> 16 & 255;
            int green = _clock[pixelIdx] >> 8 & 255;
            int blue = _clock[pixelIdx] >> 0 & 255;

            // Logic to detect the "clock face" area (looks for specific bluish-gray tint)
            if (Math.Abs(red - blue) < 10 && green < 40 && red > 100)
            {
                double relX = -((pixelIdx % _resolution) * invResMinus1 - 0.5D);
                double relY = (pixelIdx / _resolution) * invResMinus1 - 0.5D;
                int origRed = red;

                int dialX = (int)((relX * cosAngle + relY * sinAngle + 0.5D) * _dialResolution);
                int dialY = (int)((relY * cosAngle - relX * sinAngle + 0.5D) * _dialResolution);

                int dialIdx = (dialX & (_dialResolution - 1)) + (dialY & (_dialResolution - 1)) * _dialResolution;

                alpha = _dial[dialIdx] >> 24 & 255;
                red = (_dial[dialIdx] >> 16 & 255) * red / 255;
                green = (_dial[dialIdx] >> 8 & 255) * origRed / 255;
                blue = (_dial[dialIdx] >> 0 & 255) * origRed / 255;
            }

            Pixels[pixelIdx * 4 + 0] = (byte)red;
            Pixels[pixelIdx * 4 + 1] = (byte)green;
            Pixels[pixelIdx * 4 + 2] = (byte)blue;
            Pixels[pixelIdx * 4 + 3] = (byte)alpha;
        }
    }
}
