namespace BetaSharp.Client.Rendering.Core.Textures;

public struct TextureRegion(string alias, int index, int pixelX, int pixelY, int width, int height)
{
    public string Alias { get; } = alias;
    public int Index { get; } = index;

    public int PixelX { get; } = pixelX;
    public int PixelY { get; } = pixelY;
    public int Width { get; } = width;
    public int Height { get; } = height;

    public (float uMin, float vMin, float uMax, float vMax) GetNormalizedUVs(int atlasWidth, int atlasHeight)
    {
        float uMin = (float)PixelX / atlasWidth;
        float vMin = (float)PixelY / atlasHeight;
        float uMax = (float)(PixelX + Width) / atlasWidth;
        float vMax = (float)(PixelY + Height) / atlasHeight;
        return (uMin, vMin, uMax, vMax);
    }

}
