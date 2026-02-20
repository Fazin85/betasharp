using BetaSharp.Client.Rendering.Core;
using java.awt.image;
using java.io;
using javax.imageio;
using Silk.NET.OpenGL.Legacy;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace BetaSharp.Client.Resource.Pack;

public class BuiltInTexturePack : TexturePack
{
    private int _texturePackName = -1;
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
            Log.Error(ex);
        }

    }

    public override void Unload(Minecraft mc)
    {
        if (texturePackThumbnail != null)
        {
            mc.textureManager.Delete(_texturePackName);
        }

    }

    public override void BindThumbnailTexture(Minecraft mc)
    {
        if (texturePackThumbnail != null && _texturePackName < 0)
        {
            _texturePackName = mc.textureManager.Load(texturePackThumbnail);
        }

        if (texturePackThumbnail != null)
        {
            mc.textureManager.BindTexture(_texturePackName);
        }
        else
        {
            GLManager.GL.BindTexture(GLEnum.Texture2D, (uint)mc.textureManager.GetTextureId("/gui/unknown_pack.png"));
        }

    }
}