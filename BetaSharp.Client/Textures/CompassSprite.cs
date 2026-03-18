using BetaSharp.Client.Rendering.Core.Textures;
using BetaSharp.Client.Textures;
using BetaSharp.Items;
using BetaSharp.Util.Maths;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace BetaSharp.Client.Textures;

public class CompassSprite : DynamicTexture
{
    private Minecraft _mc;
    private int[] _compassPixels = new int[256];
    private int _resolution = 16;
    private byte[] _outputPixels = new byte[256 * 4];
    private double _angle = 0.0;
    private double _angleDelta = 0.0;

    private readonly string _tileId;

    public CompassSprite(Minecraft mc) : base(Item.Compass.getTextureId(0))
    {
        _mc = mc;
        _tileId = Item.Compass.getTextureId(0); // "compass" ou équivalent
    }

    public override void Setup(Minecraft mc)
    {
        _mc = mc;
        TextureAtlas atlas = mc.AtlasManager.Items;

        UVRegion uv = atlas.GetUV(_tileId);
        _resolution = Math.Max(1, (int)((uv.U1 - uv.U0) * atlas.AtlasWidth));

        int pixelCount = _resolution * _resolution;
        _compassPixels = new int[pixelCount];
        _outputPixels = new byte[pixelCount * 4];

        try
        {
            using var stream = mc.texturePackList.SelectedTexturePack
                .GetResourceAsStream("textures/items/compass.png");
            if (stream != null)
            {
                using Image<Rgba32> img = Image.Load<Rgba32>(stream);
                int localRes = img.Width;
                for (int py = 0; py < _resolution; py++)
                    for (int px = 0; px < _resolution; px++)
                    {
                        Rgba32 p = img[px * localRes / _resolution, py * localRes / _resolution];
                        _compassPixels[py * _resolution + px] =
                            (p.A << 24) | (p.R << 16) | (p.G << 8) | p.B;
                    }
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"[CompassSprite] Impossible de charger compass.png : {ex.Message}");
        }
    }

    /*public override void tick()
    {
        int pixelCount = _resolution * _resolution;
        float center = (_resolution - 1) / 2.0f;
        float needleScale = _resolution / 16.0f;

        // ── Copie du fond ────────────────────────────────────────────────
        for (int i = 0; i < pixelCount; i++)
        {
            int raw = _compassPixels[i];
            _outputPixels[i * 4 + 0] = (byte)((raw >> 16) & 0xFF); // R
            _outputPixels[i * 4 + 1] = (byte)((raw >> 8) & 0xFF); // G
            _outputPixels[i * 4 + 2] = (byte)((raw >> 0) & 0xFF); // B
            _outputPixels[i * 4 + 3] = (byte)((raw >> 24) & 0xFF); // A
        }

        // ── Calcul de l'angle cible ──────────────────────────────────────
        double targetAngle = 0.0;
        if (_mc.world != null && _mc.player != null)
        {
            Vec3i spawnPos = _mc.world.getSpawnPos();
            double dx = spawnPos.X - _mc.player.x;
            double dz = spawnPos.Z - _mc.player.z;
            targetAngle = (_mc.player.yaw - 90.0F) * Math.PI / 180.0
                        - Math.Atan2(dz, dx);
            if (_mc.world.dimension.IsNether)
                targetAngle = Random.Shared.NextDouble() * Math.PI * 2.0;
        }

        double delta = targetAngle - _angle;
        while (delta < -Math.PI) delta += Math.PI * 2.0;
        while (delta >= Math.PI) delta -= Math.PI * 2.0;
        delta = Math.Clamp(delta, -1.0, 1.0);
        _angleDelta += delta * 0.1;
        _angleDelta *= 0.8;
        _angle += _angleDelta;

        double sinA = Math.Sin(_angle);
        double cosA = Math.Cos(_angle);

        // ── Dessin de l'aiguille (grise — côté sud) ──────────────────────
        int halfQ = Math.Max(1, _resolution / 4);
        for (int i = -halfQ; i <= halfQ; i++)
        {
            int px = (int)(center + 0.5f + cosA * i * 0.3 * needleScale);
            int py = (int)(center - 0.5f - sinA * i * 0.3 * 0.5 * needleScale);
            if (px < 0 || px >= _resolution || py < 0 || py >= _resolution) continue;
            int idx = (py * _resolution + px) * 4;
            _outputPixels[idx + 0] = 100;
            _outputPixels[idx + 1] = 100;
            _outputPixels[idx + 2] = 100;
            _outputPixels[idx + 3] = 255;
        }

        // ── Dessin de l'aiguille (rouge Nord / grise Sud) ────────────────
        int halfH = Math.Max(1, _resolution / 2);
        for (int i = -halfH; i <= _resolution; i++)
        {
            int px = (int)(center + 0.5f + sinA * i * 0.3 * needleScale);
            int py = (int)(center - 0.5f + cosA * i * 0.3 * 0.5 * needleScale);
            if (px < 0 || px >= _resolution || py < 0 || py >= _resolution) continue;
            int idx = (py * _resolution + px) * 4;
            _outputPixels[idx + 0] = (byte)(i >= 0 ? 255 : 100); // R
            _outputPixels[idx + 1] = (byte)(i >= 0 ? 20 : 100); // G
            _outputPixels[idx + 2] = (byte)(i >= 0 ? 20 : 100); // B
            _outputPixels[idx + 3] = 255;
        }

        // ── Upload dans l'atlas ──────────────────────────────────────────
        TextureAtlasManager.Instance.Items.PatchTile(_tileId, _outputPixels);
    }*/
}
