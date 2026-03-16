using System.Buffers;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace BetaSharp.Client.Rendering.Core.Textures;

public static class TextureAtlasMipmapGenerator
{
    public static Image<Rgba32>[] GenerateMipmaps(Image<Rgba32> atlas, int tileSize)
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

    /// <summary>
    /// Generates mipmaps for an atlas that is laid out as a uniform grid of
    /// tiles with padding/bleeding around each tile. Each tile (including its
    /// padding) is downsampled independently so that no samples are taken from
    /// neighbouring tiles at any mip level.
    /// </summary>
    /// <param name="atlas">The source atlas image at full resolution.</param>
    /// <param name="tileSize">The size of the actual tile contents (excluding padding).</param>
    /// <param name="padding">The number of padding pixels extruded around each tile.</param>
    /// <returns>An array of atlas images, one per mip level.</returns>
    public static Image<Rgba32>[] GenerateMipmapsWithPadding(Image<Rgba32> atlas, int tileSize, int padding)
    {
        if (tileSize <= 0)
            throw new ArgumentOutOfRangeException(nameof(tileSize));

        if (padding < 0)
            throw new ArgumentOutOfRangeException(nameof(padding));

        int baseSlotSize = tileSize + padding * 2;
        if (baseSlotSize <= 0)
            throw new ArgumentOutOfRangeException(nameof(tileSize));

        int tilesX = atlas.Width / baseSlotSize;
        int tilesY = atlas.Height / baseSlotSize;

        int maxMipLevels = (int)Math.Log2(tileSize) + 1;
        Image<Rgba32>[] mipLevels = new Image<Rgba32>[maxMipLevels];

        // Level 0 is just a clone of the original atlas
        mipLevels[0] = atlas.Clone();

        Image<Rgba32> prevLevel = mipLevels[0];
        int prevWidth = prevLevel.Width;
        int prevHeight = prevLevel.Height;

        for (int mipLevel = 1; mipLevel < maxMipLevels; mipLevel++)
        {
            // Follow standard mip sizing: each level is half the previous (at least 1px),
            // so the overall atlas forms a valid mip chain for GL.
            int newWidth = Math.Max(1, prevWidth / 2);
            int newHeight = Math.Max(1, prevHeight / 2);

            Image<Rgba32> nextLevel = new(newWidth, newHeight);

            // Compute per-tile slot sizes for previous and current levels.
            // We keep tiles isolated by only sampling within each slot rectangle.
            int prevSlotWidth = prevWidth / tilesX;
            int prevSlotHeight = prevHeight / tilesY;
            int levelSlotWidth = newWidth / tilesX;
            int levelSlotHeight = newHeight / tilesY;

            // For each tile, resample its slot rectangle independently
            for (int ty = 0; ty < tilesY; ty++)
            {
                for (int tx = 0; tx < tilesX; tx++)
                {
                    int srcX = tx * prevSlotWidth;
                    int srcY = ty * prevSlotHeight;
                    int dstX = tx * levelSlotWidth;
                    int dstY = ty * levelSlotHeight;

                    using Image<Rgba32> srcSlot = prevLevel.Clone(ctx =>
                        ctx.Crop(new Rectangle(srcX, srcY, prevSlotWidth, prevSlotHeight)));

                    // Resize the whole slot (tile + padding) down to the new slot size
                    srcSlot.Mutate(ctx => ctx.Resize(levelSlotWidth, levelSlotHeight, KnownResamplers.Box));

                    nextLevel.Mutate(ctx => ctx.DrawImage(srcSlot, new Point(dstX, dstY), 1f));
                }
            }

            mipLevels[mipLevel] = nextLevel;
            prevLevel = nextLevel;
            prevWidth = newWidth;
            prevHeight = newHeight;
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
