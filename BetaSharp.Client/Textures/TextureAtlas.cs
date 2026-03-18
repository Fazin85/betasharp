using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using BetaSharp.Client.Rendering.Core;
using Microsoft.Extensions.Logging;
using Silk.NET.OpenGL.Legacy;

namespace BetaSharp.Client.Textures;

/// <summary>
/// Coordonnées UV normalisées (0-1) d'une texture dans l'atlas.
/// </summary>
public readonly struct UVRegion
{
    public readonly float U0, V0, U1, V1;

    public UVRegion(float u0, float v0, float u1, float v1)
    {
        U0 = u0; V0 = v0;
        U1 = u1; V1 = v1;
    }

    public float LerpU(float t) => U0 + (U1 - U0) * t;
    public float LerpV(float t) => V0 + (V1 - V0) * t;

    public override string ToString() => $"UV({U0:F3},{V0:F3} → {U1:F3},{V1:F3})";
}

/// <summary>
/// Nœud pour l'algorithme de bin-packing (guillotine).
/// </summary>
internal class AtlasNode
{
    public int X, Y, Width, Height;
    public AtlasNode? Left, Right;
    public bool Occupied;

    public AtlasNode(int x, int y, int w, int h)
    {
        X = x; Y = y; Width = w; Height = h;
    }

    public AtlasNode? Insert(int w, int h)
    {
        if (Left != null && Right != null)
            return Left.Insert(w, h) ?? Right.Insert(w, h);

        if (Occupied) return null;
        if (w > Width || h > Height) return null;

        if (w == Width && h == Height)
        {
            Occupied = true;
            return this;
        }

        int dw = Width - w;
        int dh = Height - h;

        if (dw > dh)
        {
            Left = new AtlasNode(X, Y, w, Height);
            Right = new AtlasNode(X + w, Y, Width - w, Height);
        }
        else
        {
            Left = new AtlasNode(X, Y, Width, h);
            Right = new AtlasNode(X, Y + h, Width, Height - h);
        }

        return Left.Insert(w, h);
    }
}

/// <summary>
/// Génère et gère un atlas de textures OpenGL à partir de fichiers PNG individuels.
/// Utilise SixLabors.ImageSharp — pas de dépendance System.Drawing.
/// </summary>
public class TextureAtlas : IDisposable
{
    private readonly ILogger<TextureAtlas> _logger = Log.Instance.For<TextureAtlas>();

    public uint GlTextureId { get; private set; }
    public int AtlasWidth { get; private set; }
    public int AtlasHeight { get; private set; }

    private readonly Dictionary<string, UVRegion> _uvMap = new(StringComparer.OrdinalIgnoreCase);

    private const string MissingKey = "__missing__";
    private const int MaxAtlasSize = 65536;
    private const int Padding = 1;

    // ── API publique ─────────────────────────────────────────────────────────

    /// <summary>
    /// Charge et coud toutes les textures depuis un dictionnaire nom → chemin fichier.
    /// Génère la texture OpenGL prête à l'emploi.
    /// </summary>
    public void Stitch(Dictionary<string, string> texturePaths)
    {
        _uvMap.Clear();

        // 1. Charger tous les bitmaps
        var loaded = new List<(string Name, Image<Rgba32> Img)>();

        foreach (var (name, path) in texturePaths)
        {
            try
            {
                loaded.Add((name, Image.Load<Rgba32>(path)));
            }
            catch (Exception e)
            {
                _logger.LogWarning($"[Atlas] Impossible de charger '{path}' : {e.Message}");
            }
        }

        // Texture "missing" magenta 16x16
        loaded.Add((MissingKey, CreateMissingTexture()));

        // 2. Trier par hauteur décroissante (meilleur remplissage)
        loaded.Sort((a, b) => (b.Img.Width * b.Img.Height).CompareTo(a.Img.Width * a.Img.Height));

        // 3. Calculer la taille minimale de l'atlas
        int totalArea = loaded.Sum(t => (t.Img.Width + Padding * 2) * (t.Img.Height + Padding * 2));
        int size = 128;
        while (size * size < totalArea * 2 && size < MaxAtlasSize)
            size *= 2;

        AtlasWidth = size;
        AtlasHeight = size;

        // 4. Bin-packing
        var root = new AtlasNode(0, 0, size, size);
        var placements = new List<(string Name, Image<Rgba32> Img, int X, int Y)>();
        var failed = new List<string>();

        foreach (var (name, img) in loaded)
        {
            int pw = img.Width + Padding * 2;
            int ph = img.Height + Padding * 2;

            AtlasNode? node = root.Insert(pw, ph);
            if (node != null)
                placements.Add((name, img, node.X + Padding, node.Y + Padding));
            else
            {
                _logger.LogWarning($"[Atlas] '{name}' ({img.Width}x{img.Height}) ne rentre pas dans {size}x{size}");
                failed.Add(name);
            }
        }

        // 5. Composer l'atlas pixel par pixel (ImageSharp)
        using Image<Rgba32> atlas = new Image<Rgba32>(size, size);

        foreach (var (name, img, destX, destY) in placements)
        {
            // Copie pixel par pixel — compatible toutes versions ImageSharp
            for (int py = 0; py < img.Height; py++)
                for (int px = 0; px < img.Width; px++)
                    atlas[destX + px, destY + py] = img[px, py];

            _uvMap[name] = new UVRegion(
                (float)destX / size,
                (float)destY / size,
                (float)(destX + img.Width) / size,
                (float)(destY + img.Height) / size);
        }

        // Textures qui n'ont pas rentré → region missing
        foreach (string name in failed)
            if (_uvMap.TryGetValue(MissingKey, out UVRegion m))
                _uvMap[name] = m;

        // 6. Libérer les images sources
        foreach (var (_, img) in loaded)
            img.Dispose();

        // 7. Upload OpenGL
        UploadToGPU(atlas);

        _logger.LogInformation(
            $"[Atlas] Cousu : {placements.Count} textures dans {size}x{size}px" +
            (failed.Count > 0 ? $", {failed.Count} ratées." : "."));
    }

    /// <summary>
    /// Retourne la région UV. Retourne "missing" si le nom est inconnu.
    /// </summary>
    public UVRegion GetUV(string name)
    {
        
        try
        {
            if (_uvMap.TryGetValue(name, out UVRegion uv))
                return uv;
        }
        catch (Exception ex)
        {
            if (name == "") return _uvMap.TryGetValue(MissingKey, out UVRegion nullRegion) ? nullRegion : default;
            _logger.LogError($"Impossible d'acquérir la texture demandée : {ex.Message} / was asking for texture {name}");
            _logger.LogDebug($"{ex.StackTrace}");
            return _uvMap.TryGetValue(MissingKey, out UVRegion miRegion) ? miRegion : default;
        }
        

        _logger.LogWarning($"[Atlas] Texture inconnue : '{name}'");
        return _uvMap.TryGetValue(MissingKey, out UVRegion missing) ? missing : default;
    }

    public bool HasTexture(string name) => _uvMap.ContainsKey(name);

    public IEnumerable<string> GetRegisteredTextures() => _uvMap.Keys;

    /// <summary>Lie la texture OpenGL pour le rendu.</summary>
    public void Bind() => GLManager.GL.BindTexture(GLEnum.Texture2D, GlTextureId);

    /// <summary>
    /// Exporte l'atlas courant en PNG dans le dossier de l'exécutable.
    /// Utile pour vérifier visuellement le résultat du stitching.
    ///
    /// Exemple : atlas.ExportToFile("terrain_atlas");
    /// → génère "terrain_atlas_2048x2048.png" à côté de l'exe
    /// </summary>
    public void ExportToFile(string baseName = "atlas")
    {
        if (GlTextureId == 0)
        {
            _logger.LogWarning("[Atlas] ExportToFile : atlas non initialisé, rien à exporter.");
            return;
        }

        // Relire les pixels depuis le GPU
        int size = AtlasWidth * AtlasHeight * 4; // RGBA
        byte[] pixels = new byte[size];

        GLManager.GL.BindTexture(GLEnum.Texture2D, GlTextureId);
        unsafe
        {
            fixed (byte* ptr = pixels)
            {
                GLManager.GL.GetGl().GetTexImage(
                        GLEnum.Texture2D,
                        0,
                        GLEnum.Rgba,
                        GLEnum.UnsignedByte,
                        pixels.AsSpan()); 
                    
            }
        }
        GLManager.GL.BindTexture(GLEnum.Texture2D, 0);

        // Reconstruire une Image<Rgba32> depuis les pixels
        using Image<Rgba32> img = new Image<Rgba32>(AtlasWidth, AtlasHeight);
        int idx = 0;
        for (int y = 0; y < AtlasHeight; y++)
            for (int x = 0; x < AtlasWidth; x++)
            {
                img[x, y] = new Rgba32(
                    pixels[idx++],  // R
                    pixels[idx++],  // G
                    pixels[idx++],  // B
                    pixels[idx++]); // A
            }

        // Chemin de sortie à côté de l'exécutable
        string fileName = $"{baseName}_{AtlasWidth}x{AtlasHeight}.png";
        string outputPath = System.IO.Path.Combine(
            AppContext.BaseDirectory,
            fileName);

        img.SaveAsPng(outputPath);

        _logger.LogInformation($"[Atlas] Exporté : '{outputPath}'");
    }

    public void Dispose()
    {
        if (GlTextureId != 0)
        {
            GLManager.GL.DeleteTexture(GlTextureId);
            GlTextureId = 0;
        }
    }

    // ── Privé ────────────────────────────────────────────────────────────────

    private unsafe void UploadToGPU(Image<Rgba32> atlas)
    {
        if (GlTextureId != 0)
        {
            GLManager.GL.DeleteTexture(GlTextureId);
            GlTextureId = 0;
        }

        GlTextureId = GLManager.GL.GenTexture();
        GLManager.GL.BindTexture(GLEnum.Texture2D, GlTextureId);

        // Nearest neighbor — style pixel art Minecraft
        GLManager.GL.TexParameter(GLEnum.Texture2D, GLEnum.TextureMinFilter, (int)GLEnum.Nearest);
        GLManager.GL.TexParameter(GLEnum.Texture2D, GLEnum.TextureMagFilter, (int)GLEnum.Nearest);
        GLManager.GL.TexParameter(GLEnum.Texture2D, GLEnum.TextureWrapS, (int)GLEnum.ClampToEdge);
        GLManager.GL.TexParameter(GLEnum.Texture2D, GLEnum.TextureWrapT, (int)GLEnum.ClampToEdge);

        // Extraire les pixels RGBA en tableau contigu
        byte[] pixels = new byte[atlas.Width * atlas.Height * 4];
        int idx = 0;
        for (int y = 0; y < atlas.Height; y++)
            for (int x = 0; x < atlas.Width; x++)
            {
                Rgba32 p = atlas[x, y];
                pixels[idx++] = p.R;
                pixels[idx++] = p.G;
                pixels[idx++] = p.B;
                pixels[idx++] = p.A;
            }

        fixed (byte* ptr = pixels)
        {
            GLManager.GL.TexImage2D(
                GLEnum.Texture2D,
                0,
                (int)GLEnum.Rgba,
                (uint)atlas.Width,
                (uint)atlas.Height,
                0,
                GLEnum.Rgba,
                GLEnum.UnsignedByte,
                ptr);
        }

        GLManager.GL.BindTexture(GLEnum.Texture2D, 0);
        _logger.LogInformation($"[Atlas] Uploadé en VRAM : ID={GlTextureId}, {atlas.Width}x{atlas.Height}");
    }
    /// <summary>
    /// Patch une tuile déjà enregistrée dans l'atlas avec de nouveaux pixels RGBA.
    /// Utilisé par les textures dynamiques (horloge, boussole, feu...).
    /// pixels.Length doit être == tileW * tileH * 4 (RGBA byte).
    /// </summary>
    /*
    public unsafe void PatchTile(string name, byte[] pixels) ce truc éclate tout, je le réactive plus tard et tant pis pour les boussoles et tout
    {
        if (!_uvMap.TryGetValue(name, out UVRegion uv))
            return;

        // Convertit UV normalisés → coordonnées pixel dans l'atlas
        int x = (int)(uv.U0 * AtlasWidth);
        int y = (int)(uv.V0 * AtlasHeight);
        int w = (int)((uv.U1 - uv.U0) * AtlasWidth);
        int h = (int)((uv.V1 - uv.V0) * AtlasHeight);

        if (pixels.Length != w * h * 4)
            throw new ArgumentException($"PatchTile '{name}': attendu {w * h * 4} bytes, reçu {pixels.Length}");

        GLManager.GL.BindTexture(GLEnum.Texture2D, GlTextureId);
        fixed (byte* ptr = pixels)
        {
            GLManager.GL.TexSubImage2D(
                GLEnum.Texture2D,
                0,              // mip level
                x, y, (uint)w, (uint)h,
                GLEnum.Rgba,
                GLEnum.UnsignedByte,
                ptr);
        }
        GLManager.GL.BindTexture(GLEnum.Texture2D, 0);
    }*/
    private static Image<Rgba32> CreateMissingTexture(int size = 16)
    {
        var img = new Image<Rgba32>(size, size);
        int half = size / 2;

        for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
            {
                // Damier magenta/noir classique
                bool magenta = (x < half) == (y < half);
                img[x, y] = magenta
                    ? new Rgba32(255, 0, 255, 255)
                    : new Rgba32(0, 0, 0, 255);
            }

        return img;
    }
}
