using System.IO.Compression;
using BetaSharp.Client.Rendering.Core;
using Silk.NET.OpenGL.Legacy;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace BetaSharp.Client.Resource.Pack;

public class ZippedTexturePack : TexturePack
{
    private ZipArchive? _texturePackZipFile;
    private int _texturePackName = -1;
    private Image<Rgba32>? _texturePackThumbnail;

    private readonly FileInfo _texturePackFile;

    public ZippedTexturePack(FileInfo file)
    {
        TexturePackFileName = file.Name;
        _texturePackFile = file;
    }

    private static string TruncateString(string? str)
    {
        if (str != null && str.Length > 34)
        {
            return str[..34];
        }
        return str ?? string.Empty;
    }

    public override void func_6485_a(Minecraft mc)
    {
        try
        {
            using var archive = ZipFile.OpenRead(_texturePackFile.FullName);

            var packTxtEntry = archive.GetEntry("pack.txt");
            if (packTxtEntry != null)
            {
                using var stream = packTxtEntry.Open();
                using var reader = new StreamReader(stream); // Replaces BufferedReader
                FirstDescriptionLine = TruncateString(reader.ReadLine());
                SecondDescriptionLine = TruncateString(reader.ReadLine());
            }

            var packPngEntry = archive.GetEntry("pack.png");
            if (packPngEntry != null)
            {
                using var stream = packPngEntry.Open();
                _texturePackThumbnail = Image.Load<Rgba32>(stream); // Native ImageSharp load
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex);
        }
    }

    public override void Unload(Minecraft mc)
    {
        if (_texturePackThumbnail != null)
        {
            mc.textureManager.Delete(_texturePackName);
            _texturePackThumbnail.Dispose();
        }

        CloseTexturePackFile();
    }

    public override void BindThumbnailTexture(Minecraft mc)
    {
        if (_texturePackThumbnail != null && _texturePackName < 0)
        {
            _texturePackName = mc.textureManager.Load(_texturePackThumbnail);
        }

        if (_texturePackThumbnail != null)
        {
            mc.textureManager.BindTexture(_texturePackName);
        }
        else
        {
            GLManager.GL.BindTexture(GLEnum.Texture2D, (uint)mc.textureManager.GetTextureId("/gui/unknown_pack.png"));
        }

    }

    public override void func_6482_a()
    {
        try
        {
            // Opens the zip file and keeps it open for reading resources
            _texturePackZipFile = ZipFile.OpenRead(_texturePackFile.FullName);
        }
        catch (Exception) { }
    }

    public override void CloseTexturePackFile()
    {
        _texturePackZipFile?.Dispose();
        _texturePackZipFile = null;
    }

    public override Stream? GetResourceAsStream(string path)
    {
        try
        {
            string entryName = path.StartsWith("/") ? path[1..] : path;

            var entry = _texturePackZipFile?.GetEntry(entryName);
            if (entry != null)
            {
                var ms = new MemoryStream();
                using (var entryStream = entry.Open())
                {
                    entryStream.CopyTo(ms);
                }
                ms.Position = 0;
                return ms;
            }
        }
        catch (Exception) { }

        return base.GetResourceAsStream(path);
    }
}
