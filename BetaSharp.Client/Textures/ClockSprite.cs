using BetaSharp.Client.Rendering.Core.Textures;
using BetaSharp.Client.Textures;
using BetaSharp.Items;
using BetaSharp.Util.Maths;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace BetaSharp.Client.Textures;

public class ClockSprite : DynamicTexture
{
    private Minecraft _mc;

    // Pixels de la face de l'horloge (fond) lus depuis l'atlas au Setup
    private int[] _clockPixels = new int[256];
    // Pixels du cadran (aiguille) lus depuis misc/dial.png
    private int[] _dialPixels = new int[256];

    // Dimensions
    private int _resolution = 16; // taille de la tuile horloge dans l'atlas
    private int _dialResolution = 16; // taille de dial.png (peut différer)

    // Buffer de sortie RGBA — uploadé dans l'atlas à chaque tick via PatchTile
    private byte[] _outputPixels = new byte[256 * 4];

    // Angle de l'aiguille (rad) et sa vélocité
    private double _angle = 0.0;
    private double _angleDelta = 0.0;

    // Nom de la tuile dans l'atlas Items
    private readonly string _tileId;

    public ClockSprite(Minecraft mc) : base(Item.Clock.getTextureId(0))
    {
        _mc = mc;
        _tileId = Item.Clock.getTextureId(0); // "clock" ou équivalent
    }

    public override void Setup(Minecraft mc)
    {
        _mc = mc;
        TextureAtlas atlas = TextureAtlasManager.Instance.Items;

        // 1. Résolution de la tuile dans l'atlas
        UVRegion uv = atlas.GetUV(_tileId);
        _resolution = (int)((uv.U1 - uv.U0) * atlas.AtlasWidth);
        _resolution = Math.Max(1, _resolution);

        int pixelCount = _resolution * _resolution;
        _clockPixels = new int[pixelCount];
        _outputPixels = new byte[pixelCount * 4];

        // 2. Lire les pixels actuels de la tuile depuis le GPU (fond de l'horloge)
        //    On les extrait de l'atlas via TexSubImage inverse (GetTexImage sur la région)
        //    Méthode plus simple : ExportToFile puis recrop — mais on utilise TexImage directement.
        //
        //    Alternative propre : l'atlas expose déjà ses pixels CPU pendant Stitch.
        //    Pour l'instant on lit depuis le pack de textures comme dans l'original.
        try
        {
            using var stream = mc.texturePackList.SelectedTexturePack
                .GetResourceAsStream("textures/items/clock.png");
            if (stream != null)
            {
                using Image<Rgba32> img = Image.Load<Rgba32>(stream);
                int localRes = img.Width; // tuile carrée

                for (int py = 0; py < _resolution; py++)
                    for (int px = 0; px < _resolution; px++)
                    {
                        int sx = px * localRes / _resolution;
                        int sy = py * localRes / _resolution;
                        Rgba32 p = img[sx, sy];
                        // Stockage ARGB comme l'original Java
                        _clockPixels[py * _resolution + px] =
                            (p.A << 24) | (p.R << 16) | (p.G << 8) | p.B;
                    }
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"[ClockSprite] Impossible de charger clock.png : {ex.Message}");
        }

        // 3. Charger dial.png
        try
        {
            using var dialStream = mc.texturePackList.SelectedTexturePack
                .GetResourceAsStream("misc/dial.png");
            if (dialStream != null)
            {
                using Image<Rgba32> dialImg = Image.Load<Rgba32>(dialStream);
                _dialResolution = dialImg.Width;
                int dialCount = _dialResolution * _dialResolution;
                _dialPixels = new int[dialCount];

                for (int py = 0; py < _dialResolution; py++)
                    for (int px = 0; px < _dialResolution; px++)
                    {
                        Rgba32 p = dialImg[px, py];
                        _dialPixels[py * _dialResolution + px] =
                            (p.A << 24) | (p.R << 16) | (p.G << 8) | p.B;
                    }
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"[ClockSprite] Impossible de charger dial.png : {ex.Message}");
        }
    }

    public override void tick()
    {
        /*
            // ── Calcul de l'angle cible ───────────────────────────────────────
            double targetAngle = 0.0;
            if (_mc.world != null && _mc.player != null)
            {
                float timeOfDay = _mc.world.getTime(1.0F);
                targetAngle = -(double)(timeOfDay * (float)Math.PI * 2.0F);
                if (_mc.world.dimension.IsNether)
                    targetAngle = Random.Shared.NextDouble() * Math.PI * 2.0;
            }

            // Interpolation douce de l'aiguille (inchangé depuis l'original)
            double delta = targetAngle - _angle;
            while (delta < -Math.PI) delta += Math.PI * 2.0;
            while (delta >= Math.PI) delta -= Math.PI * 2.0;
            delta = Math.Clamp(delta, -1.0, 1.0);
            _angleDelta += delta * 0.1;
            _angleDelta *= 0.8;
            _angle += _angleDelta;

            double sinA = Math.Sin(_angle);
            double cosA = Math.Cos(_angle);

            // ── Génération des pixels ─────────────────────────────────────────
            int pixelCount = _resolution * _resolution;
            float invResMinus1 = 1.0f / (_resolution - 1);

            for (int i = 0; i < pixelCount; i++)
            {
                int raw = _clockPixels[i];
                int a = (raw >> 24) & 0xFF;
                int r = (raw >> 16) & 0xFF;
                int g = (raw >> 8) & 0xFF;
                int b = (raw >> 0) & 0xFF;

                // Détection des pixels "dorés" du fond (zone de l'aiguille)
                if (Math.Abs(r - b) < 10 && g < 40 && r > 100)
                {
                    double u = -((i % _resolution) * invResMinus1 - 0.5);
                    double v = (i / _resolution) * invResMinus1 - 0.5;

                    int dialX = (int)((u * cosA + v * sinA + 0.5) * _dialResolution);
                    int dialY = (int)((v * cosA - u * sinA + 0.5) * _dialResolution);
                    int dialIdx = (dialX & (_dialResolution - 1))
                                + (dialY & (_dialResolution - 1)) * _dialResolution;

                    int d = _dialPixels[dialIdx];
                    int brightness = r; // luminosité du fond doré
                    a = (d >> 24) & 0xFF;
                    r = ((d >> 16) & 0xFF) * brightness / 255;
                    g = ((d >> 8) & 0xFF) * brightness / 255;
                    b = ((d >> 0) & 0xFF) * brightness / 255;
                }

                int idx = i * 4;
                _outputPixels[idx + 0] = (byte)r;
                _outputPixels[idx + 1] = (byte)g;
                _outputPixels[idx + 2] = (byte)b;
                _outputPixels[idx + 3] = (byte)a;
            }

            // ── Upload dans l'atlas via TexSubImage2D ─────────────────────────
            TextureAtlasManager.Instance.Items.PatchTile(_tileId, _outputPixels);
        }*/
    }
}
