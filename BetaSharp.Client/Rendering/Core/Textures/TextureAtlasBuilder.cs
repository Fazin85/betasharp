using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace BetaSharp.Client.Rendering.Core.Textures;

public class TextureAtlasBuilder : IDisposable
{
    private readonly int _tileSize;
    private readonly int _padding;
    private int _gridSize;
    private int _nextAvailableIndex = 0;

    private Image<Rgba32> _atlasImage;
    private readonly Dictionary<string, TextureRegion> _regionsByAlias = [];
    private readonly Dictionary<int, TextureRegion> _regionsByIndex = [];

    public int CurrentWidth => _atlasImage.Width;
    public int CurrentHeight => _atlasImage.Height;

    public TextureAtlasBuilder(int tileSize = 16, int padding = 1, int initialGridSize = 16)
    {
        _tileSize = tileSize;
        _padding = padding;
        _gridSize = initialGridSize;

        int slotSize = _tileSize + (_padding * 2);
        _atlasImage = new Image<Rgba32>(_gridSize * slotSize, _gridSize * slotSize);
    }

    /// <summary>
    /// Splits the original 16x16 legacy atlas and adds them with a default naming scheme.
    /// </summary>
    public void LoadLegacyAtlas(Image<Rgba32> legacyAtlas)
    {
        int legacyTilesX = legacyAtlas.Width / _tileSize;
        int legacyTilesY = legacyAtlas.Height / _tileSize;

        for (int y = 0; y < legacyTilesY; y++)
        {
            for (int x = 0; x < legacyTilesX; x++)
            {
                using Image<Rgba32> tile = legacyAtlas.Clone(ctx =>
                    ctx.Crop(new Rectangle(x * _tileSize, y * _tileSize, _tileSize, _tileSize)));

                int legacyIndex = y * legacyTilesX + x;
                AddTexture($"terrain_{legacyIndex}", tile);
            }
        }
    }

    public TextureRegion AddTexture(string alias, Image<Rgba32> texture)
    {
        if (_regionsByAlias.TryGetValue(alias, out var existingRegion))
            return existingRegion;

        // Resize texture if it doesn't match the expected tile size
        if (texture.Width != _tileSize || texture.Height != _tileSize)
        {
            texture.Mutate(x => x.Resize(_tileSize, _tileSize, KnownResamplers.NearestNeighbor));
        }

        // Check if we need to grow the atlas
        if (_nextAvailableIndex >= _gridSize * _gridSize)
        {
            GrowAtlas();
        }

        int slotX = _nextAvailableIndex % _gridSize;
        int slotY = _nextAvailableIndex / _gridSize;
        int slotSize = _tileSize + (_padding * 2);

        int destX = slotX * slotSize;
        int destY = slotY * slotSize;

        // Apply texture and padding
        ApplyPaddedTexture(texture, destX, destY);

        int pixelX = destX + _padding;
        int pixelY = destY + _padding;

        var region = new TextureRegion(alias, _nextAvailableIndex, pixelX, pixelY, _tileSize, _tileSize);
        _regionsByAlias[alias] = region;
        _regionsByIndex[_nextAvailableIndex] = region;

        _nextAvailableIndex++;
        return region;
    }

    private void ApplyPaddedTexture(Image<Rgba32> tile, int destX, int destY)
    {
        // 1. Draw the actual tile in the center of the slot
        _atlasImage.Mutate(ctx => ctx.DrawImage(tile, new Point(destX + _padding, destY + _padding), 1f));

        if (_padding <= 0) return;

        // 2. Extrude Edges (Padding) to prevent texture bleeding
        // Top edge
        using var topEdge = tile.Clone(ctx => ctx.Crop(new Rectangle(0, 0, _tileSize, 1)));
        for (int p = 0; p < _padding; p++)
            _atlasImage.Mutate(ctx => ctx.DrawImage(topEdge, new Point(destX + _padding, destY + p), 1f));

        // Bottom edge
        using var bottomEdge = tile.Clone(ctx => ctx.Crop(new Rectangle(0, _tileSize - 1, _tileSize, 1)));
        for (int p = 0; p < _padding; p++)
            _atlasImage.Mutate(ctx => ctx.DrawImage(bottomEdge, new Point(destX + _padding, destY + _tileSize + _padding + p), 1f));

        // Left edge
        using var leftEdge = tile.Clone(ctx => ctx.Crop(new Rectangle(0, 0, 1, _tileSize)));
        for (int p = 0; p < _padding; p++)
            _atlasImage.Mutate(ctx => ctx.DrawImage(leftEdge, new Point(destX + p, destY + _padding), 1f));

        // Right edge
        using var rightEdge = tile.Clone(ctx => ctx.Crop(new Rectangle(_tileSize - 1, 0, 1, _tileSize)));
        for (int p = 0; p < _padding; p++)
            _atlasImage.Mutate(ctx => ctx.DrawImage(rightEdge, new Point(destX + _tileSize + _padding + p, destY + _padding), 1f));

        // Corners (simplified: just copying the exact corner pixel)
        // You can expand this logic to fill the full corner grid if using padding > 1
        _atlasImage[destX, destY] = tile[0, 0]; // Top-Left
        _atlasImage[destX + _padding + _tileSize, destY] = tile[_tileSize - 1, 0]; // Top-Right
        _atlasImage[destX, destY + _padding + _tileSize] = tile[0, _tileSize - 1]; // Bottom-Left
        _atlasImage[destX + _padding + _tileSize, destY + _padding + _tileSize] = tile[_tileSize - 1, _tileSize - 1]; // Bottom-Right
    }

    private void GrowAtlas()
    {
        int newGridSize = _gridSize * 2;
        int slotSize = _tileSize + (_padding * 2);

        var newAtlas = new Image<Rgba32>(newGridSize * slotSize, newGridSize * slotSize);
        newAtlas.Mutate(ctx => ctx.DrawImage(_atlasImage, new Point(0, 0), 1f));

        _atlasImage.Dispose();
        _atlasImage = newAtlas;
        _gridSize = newGridSize;
    }

    public TextureRegion? GetRegion(string alias) => _regionsByAlias.GetValueOrDefault(alias);
    public TextureRegion? GetRegion(int index) => _regionsByIndex.GetValueOrDefault(index);
    public Image<Rgba32> Build() => _atlasImage.Clone();

    public void Dispose()
    {
        _atlasImage?.Dispose();
    }
}
