namespace BetaSharp.Client.Textures;

public class TextureAtlasMipmapGenerator
{
    public struct Color(byte r, byte g, byte b, byte a)
    {
        public byte R = r, G = g, B = b, A = a;
    }

    public class TextureAtlas(int width, int height)
    {
        public int Width { get; set; } = width;
        public int Height { get; set; } = height;
        public Color[] Pixels { get; set; } = new Color[width * height];
    }

    public static TextureAtlas[] GenerateMipmaps(TextureAtlas atlas, int tileSize)
    {
        const int TILES_PER_ROW = 16;
        const int TILES_PER_COLUMN = 16;

        int maxMipLevels = (int)Math.Log2(tileSize) + 1;
        TextureAtlas[] mipLevels = new TextureAtlas[maxMipLevels];

        mipLevels[0] = atlas;

        Color[][] tiles = ExtractTiles(atlas, tileSize, TILES_PER_ROW, TILES_PER_COLUMN);

        for (int mipLevel = 1; mipLevel < maxMipLevels; mipLevel++)
        {
            int newTileSize = tileSize >> mipLevel;

            Color[][] downsampledTiles = new Color[tiles.Length][];
            for (int i = 0; i < tiles.Length; i++)
            {
                downsampledTiles[i] = DownsampleTile(tiles[i], tileSize >> mipLevel - 1, newTileSize);
            }

            mipLevels[mipLevel] = PackTilesIntoAtlas(downsampledTiles, newTileSize, TILES_PER_ROW, TILES_PER_COLUMN);

            tiles = downsampledTiles;
        }

        return mipLevels;
    }

    public static byte[] ToByteArray(Color[] pixels)
    {
        byte[] bytes = new byte[pixels.Length * 4];
        for (int i = 0; i < pixels.Length; i++)
        {
            bytes[i * 4 + 0] = pixels[i].R;
            bytes[i * 4 + 1] = pixels[i].G;
            bytes[i * 4 + 2] = pixels[i].B;
            bytes[i * 4 + 3] = pixels[i].A;
        }
        return bytes;
    }

    private static Color[][] ExtractTiles(TextureAtlas atlas, int tileSize, int tilesPerRow, int tilesPerColumn)
    {
        Color[][] tiles = new Color[tilesPerRow * tilesPerColumn][];

        for (int tileY = 0; tileY < tilesPerColumn; tileY++)
        {
            for (int tileX = 0; tileX < tilesPerRow; tileX++)
            {
                int tileIndex = tileY * tilesPerRow + tileX;
                tiles[tileIndex] = new Color[tileSize * tileSize];

                for (int y = 0; y < tileSize; y++)
                {
                    for (int x = 0; x < tileSize; x++)
                    {
                        int atlasX = tileX * tileSize + x;
                        int atlasY = tileY * tileSize + y;
                        int atlasIndex = atlasY * atlas.Width + atlasX;
                        int tilePixelIndex = y * tileSize + x;

                        tiles[tileIndex][tilePixelIndex] = atlas.Pixels[atlasIndex];
                    }
                }
            }
        }

        return tiles;
    }

    public static Color[] DownsampleTile(Color[] tile, int currentSize, int newSize)
    {
        Color[] downsampled = new Color[newSize * newSize];
        int scale = currentSize / newSize;

        for (int y = 0; y < newSize; y++)
        {
            for (int x = 0; x < newSize; x++)
            {
                float r = 0, g = 0, b = 0, a = 0;
                float totalAlpha = 0;

                for (int sy = 0; sy < scale; sy++)
                {
                    for (int sx = 0; sx < scale; sx++)
                    {
                        int sourceX = x * scale + sx;
                        int sourceY = y * scale + sy;
                        int sourceIndex = sourceY * currentSize + sourceX;

                        Color c = tile[sourceIndex];
                        float alpha = c.A / 255f;

                        r += c.R * alpha;
                        g += c.G * alpha;
                        b += c.B * alpha;
                        a += c.A;
                        totalAlpha += alpha;
                    }
                }

                int count = scale * scale;
                byte finalA = (byte)(a / count);

                byte finalR, finalG, finalB;
                if (totalAlpha > 0.001f)
                {
                    finalR = (byte)(r / totalAlpha);
                    finalG = (byte)(g / totalAlpha);
                    finalB = (byte)(b / totalAlpha);
                }
                else
                {
                    finalR = 0;
                    finalG = 0;
                    finalB = 0;
                }

                downsampled[y * newSize + x] = new Color(finalR, finalG, finalB, finalA);
            }
        }

        return downsampled;
    }

    private static TextureAtlas PackTilesIntoAtlas(Color[][] tiles, int tileSize, int tilesPerRow, int tilesPerColumn)
    {
        int atlasWidth = tileSize * tilesPerRow;
        int atlasHeight = tileSize * tilesPerColumn;

        TextureAtlas atlas = new(atlasWidth, atlasHeight);

        for (int tileY = 0; tileY < tilesPerColumn; tileY++)
        {
            for (int tileX = 0; tileX < tilesPerRow; tileX++)
            {
                int tileIndex = tileY * tilesPerRow + tileX;
                Color[] tilePixels = tiles[tileIndex];

                for (int y = 0; y < tileSize; y++)
                {
                    for (int x = 0; x < tileSize; x++)
                    {
                        int atlasX = tileX * tileSize + x;
                        int atlasY = tileY * tileSize + y;
                        int atlasIndex = atlasY * atlasWidth + atlasX;
                        int tilePixelIndex = y * tileSize + x;

                        atlas.Pixels[atlasIndex] = tilePixels[tilePixelIndex];
                    }
                }
            }
        }

        return atlas;
    }
}