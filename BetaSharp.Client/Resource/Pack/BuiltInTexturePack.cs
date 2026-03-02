using BetaSharp.Client.Rendering.Core.Textures;
using java.awt.image;
using java.io;
using javax.imageio;
using Microsoft.Extensions.Logging;
using Silk.NET.OpenGL.Legacy;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace BetaSharp.Client.Resource.Pack;

public class BuiltInTexturePack : TexturePack
{
    private readonly ILogger _logger = Log.Instance.For<BuiltInTexturePack>();
    private TextureHandle? _texturePackName;
    private readonly Image<Rgba32>? texturePackThumbnail;

    public BuiltInTexturePack()
    {
        TexturePackFileName = "Default";
        FirstDescriptionLine = "The default look of Minecraft";

        try
        {
            byte[] content = AssetManager.Instance.getAsset("pack.png").getBinaryContent();
            using (var ms = new MemoryStream(content))
            {
                texturePackThumbnail = Image.Load<Rgba32>(ms);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load built in texture pack");
        }

    }

    public override void Unload(Minecraft mc)
    {
        if (texturePackThumbnail != null && _texturePackName != null)
        {
            mc.textureManager.Delete(_texturePackName);

        }

    }

    public override TextureHandle? BindThumbnailTexture(Minecraft mc)
    {
        if (texturePackThumbnail != null && _texturePackName == null)
        {
            _texturePackName = mc.textureManager.Load(texturePackThumbnail);
        }

        if (texturePackThumbnail != null && _texturePackName != null)
        {
            mc.textureManager.BindTexture(_texturePackName);
            return _texturePackName;
        }
        var unknown = mc.textureManager.GetTextureId("/gui/unknown_pack.png");
        mc.textureManager.BindTexture(unknown);
        return unknown;
    }
}
