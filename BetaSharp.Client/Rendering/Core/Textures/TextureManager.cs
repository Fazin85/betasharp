using BetaSharp.Client.Options;
using BetaSharp.Client.Resource.Pack;
using BetaSharp.Client.Textures;
using Silk.NET.OpenGL.Legacy;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Processing;
using static BetaSharp.Client.Textures.TextureAtlasMipmapGenerator;
using Microsoft.Extensions.Logging;

namespace BetaSharp.Client.Rendering.Core.Textures;

public class TextureManager : IDisposable
{
    private readonly ILogger _logger = Log.Instance.For<TextureManager>();
    private readonly Dictionary<string, TextureHandle> _textures = [];
    private readonly Dictionary<string, int[]> _colors = [];
    private readonly Dictionary<uint, (Image<Rgba32> Image, TextureHandle Handle)> _images = [];
    private readonly List<DynamicTexture> _dynamicTextures = [];
    private readonly GameOptions _gameOptions;
    private bool _clamp;
    private bool _blur;
    private readonly TexturePacks _texturePacks;
    private readonly Minecraft _mc;
    private readonly Image<Rgba32> _missingTextureImage = new(256, 256);

    // _atlasTileSizes est conservé pour la compatibilité avec les appelants legacy
    // qui font encore GetAtlasTileSize("/terrain.png") — à supprimer quand tout est migré
    private readonly Dictionary<string, int> _atlasTileSizes = [];

    public TextureManager(Minecraft mc, TexturePacks texturePacks, GameOptions options)
    {
        _mc = mc;
        _texturePacks = texturePacks;
        _gameOptions = options;
        _missingTextureImage.Mutate(ctx =>
        {
            ctx.BackgroundColor(Color.Magenta);
            ctx.Fill(Color.Black, new RectangleF(0, 0, 128, 128));
            ctx.Fill(Color.Black, new RectangleF(128, 128, 128, 128));
        });
    }

    // ── Couleurs ─────────────────────────────────────────────────────────────

    public int[] GetColors(string path)
    {
        if (_colors.TryGetValue(path, out int[]? cached)) return cached;
        try
        {
            using Image<Rgba32> img = LoadImageFromResource(path);
            int[] result = ReadColorsFromImage(img);
            _colors[path] = result;
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get colors from {Path}", path);
            int[] fallback = ReadColorsFromImage(_missingTextureImage);
            _colors[path] = fallback;
            return fallback;
        }
    }

    // Conservé pour compatibilité — retourne 16 si inconnu
    public int GetAtlasTileSize(string path)
        => _atlasTileSizes.TryGetValue(path, out int size) ? size : 16;

    // ── Chargement / bind ─────────────────────────────────────────────────────

    public TextureHandle Load(Image<Rgba32> image)
    {
        var texture = new GLTexture("Image_Direct");
        Load(image, texture, false);
        var handle = new TextureHandle(this, texture);
        _images[texture.Id] = (image, handle);
        return handle;
    }

    public TextureHandle GetTextureId(string path)
    {
        if (_textures.TryGetValue(path, out TextureHandle? handle)) return handle;

        var texture = new GLTexture(path);
        handle = new TextureHandle(this, texture);
        _textures[path] = handle;

        try
        {
            using Image<Rgba32> img = LoadImageFromResource(path);
            _atlasTileSizes[path] = img.Width / 16;
            Load(img, texture, path.Contains("terrain.png"));
            return handle;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load texture {Path}", path);
            Load(_missingTextureImage, texture, false);
            return handle;
        }
    }

    public void BindTexture(TextureHandle? handle) => handle?.Bind();

    // ── Upload GL ─────────────────────────────────────────────────────────────

    public unsafe void Load(Image<Rgba32> image, GLTexture texture, bool isTerrain)
    {
        texture.Bind();

        if (isTerrain)
        {
            int tileSize = image.Width / 16;
            Image<Rgba32>[] mips = GenerateMipmaps(image, tileSize);
            int mipCount = _gameOptions.UseMipmaps ? mips.Length : 1;

            for (int level = 0; level < mipCount; level++)
            {
                Image<Rgba32> mip = mips[level];
                byte[] pixels = new byte[mip.Width * mip.Height * 4];
                mip.CopyPixelDataTo(pixels);
                fixed (byte* ptr = pixels)
                    texture.Upload(mip.Width, mip.Height, ptr, level, PixelFormat.Rgba, InternalFormat.Rgba8);
                if (level > 0) mip.Dispose();
            }

            texture.SetFilter(
                _gameOptions.UseMipmaps ? TextureMinFilter.NearestMipmapNearest : TextureMinFilter.Nearest,
                TextureMagFilter.Nearest);
            texture.SetMaxLevel(mipCount - 1);

            float aniso = _gameOptions.AnisotropicLevel == 0
                ? 1.0f
                : Math.Clamp((float)Math.Pow(2, _gameOptions.AnisotropicLevel), 1.0f, GameOptions.MaxAnisotropy);
            texture.SetAnisotropicFilter(aniso);
            return;
        }

        texture.SetFilter(
            _blur ? TextureMinFilter.Linear : TextureMinFilter.Nearest,
            _blur ? TextureMagFilter.Linear : TextureMagFilter.Nearest);
        texture.SetWrap(
            _clamp ? TextureWrapMode.ClampToEdge : TextureWrapMode.Repeat,
            _clamp ? TextureWrapMode.ClampToEdge : TextureWrapMode.Repeat);

        byte[] rawPixels = new byte[image.Width * image.Height * 4];
        image.CopyPixelDataTo(rawPixels);
        fixed (byte* ptr = rawPixels)
            texture.Upload(image.Width, image.Height, ptr, 0, PixelFormat.Rgba, InternalFormat.Rgba);

        _clamp = false;
        _blur = false;
    }

    // ── Textures dynamiques ───────────────────────────────────────────────────

    public void AddDynamicTexture(DynamicTexture t)
    {
        _dynamicTextures.Add(t);
        t.Setup(_mc);
        t.tick();
    }

    /// <summary>
    /// Tick toutes les textures dynamiques.
    /// Chaque DynamicTexture appelle elle-même PatchTile() sur l'atlas concerné
    /// — plus besoin de gérer baseTexture/pixels/atlas ici.
    /// </summary>
    public void Tick()
    {
        //foreach (DynamicTexture texture in _dynamicTextures)
          //  texture.tick();
    }

    // ── Rechargement ─────────────────────────────────────────────────────────

    public void Reload()
    {
        _atlasTileSizes.Clear();

        // Textures standalone (GUI, skins, etc.)
        foreach (KeyValuePair<string, TextureHandle> entry in _textures)
        {
            entry.Value.Texture?.Dispose();
            var newTexture = new GLTexture(entry.Key);
            entry.Value.Texture = newTexture;
            try
            {
                using Image<Rgba32> img = LoadImageFromResource(entry.Key);
                _atlasTileSizes[entry.Key] = img.Width / 16;
                Load(img, newTexture, entry.Key.Contains("terrain.png"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to reload texture {Path}", entry.Key);
                _atlasTileSizes[entry.Key] = _missingTextureImage.Width / 16;
                Load(_missingTextureImage, newTexture, false);
            }
        }

        // Images chargées directement (cartes, etc.)
        var snapshot = new Dictionary<uint, (Image<Rgba32> Image, TextureHandle Handle)>(_images);
        _images.Clear();
        foreach (var (_, (image, handle)) in snapshot)
        {
            handle.Texture?.Dispose();
            var newTexture = new GLTexture(handle.Texture?.Source ?? "Image_Direct_Reload");
            handle.Texture = newTexture;
            Load(image, newTexture, false);
            _images[newTexture.Id] = (image, handle);
        }

        // Cache couleurs
        foreach (string key in new List<string>(_colors.Keys))
            GetColors(key);

        // Les DynamicTextures se re-Setup (rechargent clock.png, dial.png, etc.)
        foreach (DynamicTexture dt in _dynamicTextures)
            dt.Setup(_mc);
    }

    // ── Suppression ──────────────────────────────────────────────────────────

    public void Delete(GLTexture texture)
    {
        var entry = _textures.FirstOrDefault(x => x.Value.Texture == texture);
        if (entry.Key != null) _textures.Remove(entry.Key);
        _images.Remove(texture.Id);
        texture.Dispose();
    }

    public void Delete(TextureHandle handle)
    {
        if (handle.Texture != null) Delete(handle.Texture);
    }

    // ── Utilitaires internes ──────────────────────────────────────────────────

    private Image<Rgba32> LoadImageFromResource(string path)
    {
        TexturePack pack = _texturePacks.SelectedTexturePack;

        if (path.StartsWith("##"))
        {
            using Stream? s = pack.GetResourceAsStream(path[2..]);
            return s == null ? _missingTextureImage.Clone() : Rescale(Image.Load<Rgba32>(s));
        }

        string cleanPath = path;
        if (path.StartsWith("%clamp%")) { _clamp = true; cleanPath = path[7..]; }
        else if (path.StartsWith("%blur%")) { _blur = true; cleanPath = path[6..]; }

        using Stream? stream = pack.GetResourceAsStream(cleanPath);
        return stream == null ? _missingTextureImage.Clone() : Image.Load<Rgba32>(stream);
    }

    private Image<Rgba32> Rescale(Image<Rgba32> image)
    {
        int scale = image.Width / 16;
        var rescaled = new Image<Rgba32>(16, image.Height * scale);
        rescaled.Mutate(ctx =>
        {
            for (int i = 0; i < scale; i++)
            {
                using Image<Rgba32> frame = image.Clone(x =>
                    x.Crop(new SixLabors.ImageSharp.Rectangle(i * 16, 0, 16, image.Height)));
                ctx.DrawImage(frame, new SixLabors.ImageSharp.Point(0, i * image.Height), 1f);
            }
        });
        return rescaled;
    }

    private static int[] ReadColorsFromImage(Image<Rgba32> image)
    {
        int[] argb = new int[image.Width * image.Height];
        image.ProcessPixelRows(accessor =>
        {
            for (int y = 0; y < accessor.Height; y++)
            {
                Span<Rgba32> row = accessor.GetRowSpan(y);
                for (int x = 0; x < accessor.Width; x++)
                {
                    Rgba32 p = row[x];
                    argb[y * accessor.Width + x] = (p.A << 24) | (p.R << 16) | (p.G << 8) | p.B;
                }
            }
        });
        return argb;
    }

    // ── IDisposable ───────────────────────────────────────────────────────────

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        foreach (TextureHandle handle in _textures.Values)
            handle.Texture?.Dispose();
        _textures.Clear();

        foreach (var (image, handle) in _images.Values)
        {
            handle.Texture?.Dispose();
            image.Dispose();
        }
        _images.Clear();

        _missingTextureImage.Dispose();
        _colors.Clear();
        _dynamicTextures.Clear();
    }
}
