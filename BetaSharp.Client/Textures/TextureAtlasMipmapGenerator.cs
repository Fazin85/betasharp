using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace BetaSharp.Client.Textures;

public class TextureAtlasMipmapGenerator
{    public static Image<Rgba32>[] GenerateMipmaps(Image<Rgba32> atlas, int tileSize)
    {
        int maxMipLevels = (int)Math.Log2(tileSize) + 1;
        Image<Rgba32>[] mipLevels = new Image<Rgba32>[maxMipLevels];

        mipLevels[0] = atlas.Clone();

        for (int mipLevel = 1; mipLevel < maxMipLevels; mipLevel++)
        {
            int scale = 1 << mipLevel;
            int newWidth = atlas.Width / scale;
            int newHeight = atlas.Height / scale;
            mipLevels[mipLevel] = atlas.Clone(ctx => ctx.Resize(newWidth, newHeight, KnownResamplers.Box));
        }

        return mipLevels;
    }

    public static byte[] ToByteArray(Image<Rgba32> image)
    {
        byte[] bytes = new byte[image.Width * image.Height * 4];
        image.CopyPixelDataTo(bytes);
        return bytes;
    }
}