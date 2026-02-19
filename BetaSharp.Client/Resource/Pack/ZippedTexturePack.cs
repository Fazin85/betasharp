using BetaSharp.Client.Rendering.Core;
using java.awt.image;
using java.io;
using java.util.zip;
using javax.imageio;
using Silk.NET.OpenGL.Legacy;

namespace BetaSharp.Client.Resource.Pack;

public class ZippedTexturePack : TexturePack
{
    private ZipFile? _texturePackZipFile;
    private int _texturePackName = -1;
    private BufferedImage _texturePackThumbnail;
    private readonly java.io.File _texturePackFile;

    public ZippedTexturePack(java.io.File var1)
    {
        texturePackFileName = var1.getName();
        _texturePackFile = var1;
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
        ZipFile? var2 = null;
        InputStream? var3 = null;

        try
        {
            var2 = new ZipFile(_texturePackFile);

            try
            {
                var3 = var2.getInputStream(var2.getEntry("pack.txt"));
                BufferedReader var4 = new(new InputStreamReader(var3));
                firstDescriptionLine = truncateString(var4.readLine());
                secondDescriptionLine = truncateString(var4.readLine());
                var4.close();
                var3.close();
            }
            catch (java.lang.Exception) { }

            try
            {
                var3 = var2.getInputStream(var2.getEntry("pack.png"));
                _texturePackThumbnail = ImageIO.read(var3);
                var3.close();
            }
            catch (java.lang.Exception) { }

            var2.close();
        }
        catch (java.lang.Exception ex)
        {
            ex.printStackTrace();
        }
        finally
        {
            try
            {
                var3?.close();
            }
            catch (java.lang.Exception) { }

            try
            {
                var2?.close();
            }
            catch (java.lang.Exception) { }

        }

    }

    public override void unload(Minecraft var1)
    {
        if (_texturePackThumbnail != null)
        {
            var1.textureManager.delete(_texturePackName);
        }

        closeTexturePackFile();
    }

    public override void bindThumbnailTexture(Minecraft var1)
    {
        if (_texturePackThumbnail != null && _texturePackName < 0)
        {
            _texturePackName = var1.textureManager.load(_texturePackThumbnail);
        }

        if (_texturePackThumbnail != null)
        {
            var1.textureManager.bindTexture(_texturePackName);
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
            _texturePackZipFile = new ZipFile(_texturePackFile);
        }
        catch (Exception) { }
    }

    public override void closeTexturePackFile()
    {
        try
        {
            _texturePackZipFile?.close();
        }
        catch (Exception) { }

        _texturePackZipFile = null;
    }

    public override InputStream getResourceAsStream(string var1)
    {
        try
        {
            ZipEntry var2 = _texturePackZipFile!.getEntry(var1[1..]);
            if (var2 != null)
            {
                return _texturePackZipFile.getInputStream(var2);
            }
        }
        catch (Exception) { }

        return base.getResourceAsStream(var1);
    }
}
