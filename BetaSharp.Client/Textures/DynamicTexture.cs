using BetaSharp.Client.Rendering.Core;
using Silk.NET.OpenGL.Legacy;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.IO;

namespace BetaSharp.Client.Textures;

public class DynamicTexture : java.lang.Object
{
    public byte[] pixels = new byte[1024];
    public int sprite;
    public int copyTo = 0;
    public int replicate = 1;
    public FXImage atlas = FXImage.Terrain;
    
    protected byte[][]? customFrames;
    protected int customFrameIndex;
    protected int customFrameCount;

    public enum FXImage
    {
        Terrain,
        Items
    }

    public DynamicTexture(int iconIdx)
    {
        sprite = iconIdx;
    }

    public virtual void Setup(Minecraft mc)
    {
    }

    public virtual void tick()
    {
    }

    protected virtual void TryLoadCustomTexture(Minecraft mc, string resourceName)
    {
        customFrames = null;
        customFrameIndex = 0;
        customFrameCount = 0;

        using Stream? stream = mc.texturePackList.SelectedTexturePack.GetResourceAsStream(resourceName);
        if (stream == null)
        {
            if (pixels.Length != 1024) pixels = new byte[1024];
            return;
        }

        try
        {
            string atlasPath = atlas == FXImage.Terrain ? "/terrain.png" : "/gui/items.png";
            int targetWidth = mc.textureManager.GetTextureId(atlasPath).Texture?.Width ?? 256;
            int targetTileSize = targetWidth / 16;
            if (targetTileSize < 1) targetTileSize = 1;

            using Image<Rgba32> image = Image.Load<Rgba32>(stream);
            int width = image.Width;
            int height = image.Height;

            if (height % width != 0) return;

            customFrameCount = height / width;

            if (width != targetTileSize)
            {
                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Size = new Size(targetTileSize, targetTileSize * customFrameCount),
                    Sampler = KnownResamplers.NearestNeighbor
                }));
                width = image.Width;
                height = image.Height;
            }

            customFrames = new byte[customFrameCount][];

            int pixelsPerFrame = width * width;
            int bytesPerFrame = pixelsPerFrame * 4;

            if (pixels.Length != bytesPerFrame)
            {
                pixels = new byte[bytesPerFrame];
            }

            for (int i = 0; i < customFrameCount; i++)
            {
                customFrames[i] = new byte[bytesPerFrame];
                using Image<Rgba32> frame = image.Clone(ctx => ctx.Crop(new Rectangle(0, i * width, width, width)));
                frame.CopyPixelDataTo(customFrames[i]);
            }
        }
        catch (Exception)
        {
            customFrames = null;
            if (pixels.Length != 1024) pixels = new byte[1024];
        }
    }
}
