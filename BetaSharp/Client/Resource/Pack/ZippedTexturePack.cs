using BetaSharp.Client.Rendering.Core;
using java.awt.image;
using java.io;
using java.util.zip;
using javax.imageio;
using Silk.NET.OpenGL.Legacy;

namespace BetaSharp.Client.Resource.Pack;

public class ZippedTexturePack : TexturePack
{
    private ZipFile texturePackZipFile;
    private int texturePackName = -1;
    private BufferedImage texturePackThumbnail;
    private readonly java.io.File texturePackFile;

    public ZippedTexturePack(java.io.File var1)
    {
        texturePackFileName = var1.getName();
        texturePackFile = var1;
    }

    private static string truncateString(string var1)
    {
        if (var1 != null && var1.Length > 34)
        {
            var1 = var1[..34];
        }

        return var1;
    }

    public override void func_6485_a(Minecraft var1)
    {
        ZipFile var2 = null;
        InputStream var3 = null;

        try
        {
            var2 = new ZipFile(texturePackFile);

            try
            {
                var3 = var2.getInputStream(var2.getEntry("pack.txt"));
                BufferedReader var4 = new(new InputStreamReader(var3));
                firstDescriptionLine = truncateString(var4.readLine());
                secondDescriptionLine = truncateString(var4.readLine());
                var4.close();
                var3.close();
            }
            catch (java.lang.Exception var20)
            {
            }

            try
            {
                var3 = var2.getInputStream(var2.getEntry("pack.png"));
                texturePackThumbnail = ImageIO.read(var3);
                var3.close();
            }
            catch (java.lang.Exception var19)
            {
            }

            var2.close();
        }
        catch (java.lang.Exception var21)
        {
            var21.printStackTrace();
        }
        finally
        {
            try
            {
                var3.close();
            }
            catch (java.lang.Exception var18)
            {
            }

            try
            {
                var2.close();
            }
            catch (java.lang.Exception var17)
            {
            }

        }

    }

    public override void unload(Minecraft var1)
    {
        if (texturePackThumbnail != null)
        {
            var1.textureManager.delete(texturePackName);
        }

        closeTexturePackFile();
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

    public override void func_6482_a()
    {
        try
        {
            texturePackZipFile = new ZipFile(texturePackFile);
        }
        catch (java.lang.Exception var2)
        {
        }

    }

    public override void closeTexturePackFile()
    {
        try
        {
            texturePackZipFile.close();
        }
        catch (java.lang.Exception var2)
        {
        }

        texturePackZipFile = null;
    }

    public override InputStream getResourceAsStream(string var1)
    {
        try
        {
            ZipEntry var2 = texturePackZipFile.getEntry(var1[1..]);
            if (var2 != null)
            {
                return texturePackZipFile.getInputStream(var2);
            }
        }
        catch (java.lang.Exception var3)
        {
        }

        return base.getResourceAsStream(var1);
    }
}