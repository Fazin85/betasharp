using BetaSharp.Client.Rendering.Core;
using java.awt.image;
using java.io;
using javax.imageio;
using Silk.NET.OpenGL.Legacy;

namespace BetaSharp.Client.Resource.Pack;

public class BuiltInTexturePack : TexturePack
{
    private int texturePackName = -1;
    private readonly BufferedImage texturePackThumbnail;

    public BuiltInTexturePack()
    {
        texturePackFileName = "Default";
        firstDescriptionLine = "The default look of Minecraft";

        try
        {
            texturePackThumbnail = ImageIO.read(new ByteArrayInputStream(AssetManager.Instance.getAsset("pack.png").getBinaryContent()));
        }
        catch (java.io.IOException ex)
        {
            ex.printStackTrace();
        }

    }

    public override void unload(Minecraft var1)
    {
        if (texturePackThumbnail != null)
        {
            var1.textureManager.delete(texturePackName);
        }

    }

    public override void bindThumbnailTexture(Minecraft var1)
    {
        if (texturePackThumbnail != null && texturePackName < 0)
        {
            texturePackName = var1.textureManager.load(texturePackThumbnail);
        }

        if (texturePackThumbnail != null)
        {
            var1.textureManager.bindTexture(texturePackName);
        }
        else
        {
            GLManager.GL.BindTexture(GLEnum.Texture2D, (uint)var1.textureManager.getTextureId("/gui/unknown_pack.png"));
        }

    }
}