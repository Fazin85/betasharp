using System.Runtime.InteropServices;
using BetaSharp.Client.Options;
using BetaSharp.Client.Rendering.Core;
using BetaSharp.Client.Rendering.Core.Textures;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Fonts;

namespace BetaSharp.Client.Rendering;

public class TextRenderer
{
    private const string FontPath = "font/Monocraft.ttc";
    private const int AtlasSize = 2048;
    private const int AtlasFontSize = 32;
    private const int GlyphPadding = 2;
    private const float DisplayScale = 0.25f;

    private readonly Font _font;
    private readonly TextOptions _textOptions;
    private readonly Image<Rgba32> _atlasImage;
    private readonly Dictionary<char, GlyphInfo> _glyphCache = [];
    private int _atlasX;
    private int _atlasY;
    private readonly int _rowHeight;

    public TextureHandle? fontTextureName { get; }

    private readonly TextureManager _textureManager;

    [StructLayout(LayoutKind.Sequential)]
    private readonly struct GlyphInfo(
        float advanceWidth,
        float u0, float v0, float u1, float v1,
        float width, float height)
    {
        public readonly float AdvanceWidth = advanceWidth;
        public readonly float U0 = u0, V0 = v0, U1 = u1, V1 = v1;
        public readonly float Width = width, Height = height;
    }

    public TextRenderer(GameOptions options, TextureManager textureManager)
    {
        _textureManager = textureManager;
        string path = Path.Combine(AppContext.BaseDirectory, "font", "Monocraft.ttc");
        if (!File.Exists(path))
        {
            path = FontPath;
        }
        if (!File.Exists(path))
        {
            throw new InvalidOperationException($"Font file not found. Tried: {Path.Combine(AppContext.BaseDirectory, "font", "Monocraft.ttc")} and {FontPath}");
        }

        var collection = new FontCollection();
        IEnumerable<FontFamily> families = collection.AddCollection(path);
        FontFamily family = families.First();
        _font = family.CreateFont(AtlasFontSize);
        _textOptions = new TextOptions(_font);

        _rowHeight = AtlasFontSize + GlyphPadding;
        _atlasImage = new Image<Rgba32>(AtlasSize, AtlasSize);
        ClearAtlasRegion(0, 0, AtlasSize, AtlasSize);

        fontTextureName = textureManager.Load(_atlasImage);
        fontTextureName.Texture?.SetFilter(Silk.NET.OpenGL.Legacy.TextureMinFilter.Nearest, Silk.NET.OpenGL.Legacy.TextureMagFilter.Nearest);
    }

    private static void ClearAtlasRegion(Image<Rgba32> image, int x, int y, int w, int h)
    {
        image.Mutate(ctx => ctx.Fill(SixLabors.ImageSharp.Color.Transparent, new Rectangle(x, y, w, h)));
    }

    private void ClearAtlasRegion(int x, int y, int w, int h)
    {
        ClearAtlasRegion(_atlasImage, x, y, w, h);
    }

    private GlyphInfo GetOrCreateGlyph(char c)
    {
        if (_glyphCache.TryGetValue(c, out GlyphInfo info))
            return info;

        ReadOnlySpan<char> charSpan = stackalloc char[] { c };
        FontRectangle advanceRect = TextMeasurer.MeasureAdvance(charSpan, _textOptions);
        FontRectangle boundsRect = TextMeasurer.MeasureBounds(charSpan, _textOptions);

        float advanceWidth = advanceRect.Width;
        int cellW = Math.Max(1, (int)Math.Ceiling(advanceRect.Width) + GlyphPadding);
        int cellH = _rowHeight;

        if (_atlasX + cellW > AtlasSize)
        {
            _atlasX = 0;
            _atlasY += _rowHeight;
        }
        if (_atlasY + cellH > AtlasSize)
        {
            return new GlyphInfo(advanceWidth, 0, 0, 0, 0, 0, 0);
        }

        using (Image<Rgba32> glyphImage = new Image<Rgba32>(cellW, cellH))
        {
            ClearAtlasRegion(glyphImage, 0, 0, cellW, cellH);
            float drawX = (float)(-boundsRect.X) + 1f;
            float drawY = (float)(-boundsRect.Y) + 1f;
            glyphImage.Mutate(ctx => ctx.DrawText(
                c.ToString(),
                _font,
                SixLabors.ImageSharp.Color.White,
                new PointF(drawX, drawY)));

            glyphImage.ProcessPixelRows(_atlasImage, (srcAccessor, dstAccessor) =>
            {
                for (int gy = 0; gy < cellH; gy++)
                {
                    Span<Rgba32> srcRow = srcAccessor.GetRowSpan(gy);
                    Span<Rgba32> dstRow = dstAccessor.GetRowSpan(_atlasY + gy);
                    srcRow.Slice(0, cellW).CopyTo(dstRow.Slice(_atlasX, cellW));
                }
            });
        }

        float u0 = (float)_atlasX / AtlasSize;
        float v0 = (float)_atlasY / AtlasSize;
        float u1 = (float)(_atlasX + cellW) / AtlasSize;
        float v1 = (float)(_atlasY + cellH) / AtlasSize;

        UploadAtlasSubImage(_atlasX, _atlasY, cellW, cellH);

        info = new GlyphInfo(advanceWidth, u0, v0, u1, v1, cellW, cellH);
        _glyphCache[c] = info;
        _atlasX += cellW;
        return info;
    }

    private unsafe void UploadAtlasSubImage(int x, int y, int width, int height)
    {
        if (fontTextureName?.Texture == null) return;
        byte[] region = new byte[width * height * 4];
        int idx = 0;
        _atlasImage.ProcessPixelRows(accessor =>
        {
            for (int row = y; row < y + height; row++)
            {
                Span<Rgba32> pixelRow = accessor.GetRowSpan(row);
                for (int col = x; col < x + width; col++)
                {
                    Rgba32 p = pixelRow[col];
                    region[idx++] = p.R;
                    region[idx++] = p.G;
                    region[idx++] = p.B;
                    region[idx++] = p.A;
                }
            }
        });
        fixed (byte* ptr = region)
        {
            fontTextureName.Texture!.UploadSubImage(x, y, width, height, ptr);
        }
    }

    public void DrawStringWithShadow(ReadOnlySpan<char> text, int x, int y, Guis.Color color)
    {
        RenderString(text, x + 1, y + 1, color, true);
        DrawString(text, x, y, color);
    }

    public void DrawString(ReadOnlySpan<char> text, int x, int y, Guis.Color color)
    {
        RenderString(text, x, y, color, false);
    }

    public void RenderString(ReadOnlySpan<char> text, int x, int y, Guis.Color color, bool darken)
    {
        if (text.IsEmpty) return;

        if (darken)
            color = color.Darken();

        fontTextureName?.Bind();

        Tessellator tessellator = Tessellator.instance;
        tessellator.startDrawingQuads();
        tessellator.setColorRGBA(color);

        float currentX = x;
        float currentY = y;

        for (int i = 0; i < text.Length; ++i)
        {
            for (; text.Length > i + 1 && text[i] == 167; i += 2)
            {
                int colorCode = HexToDec(text[i + 1]);
                tessellator.setColorRGBA(Guis.Color.FromColorCode(colorCode, (byte)color.A, darken));
            }

            if (i < text.Length)
            {
                GlyphInfo glyph = GetOrCreateGlyph(text[i]);
                if (glyph.Width > 0 && glyph.Height > 0)
                {
                    float w = glyph.Width * DisplayScale;
                    float h = glyph.Height * DisplayScale;
                    tessellator.addVertexWithUV(currentX + 0, currentY + h, 0, glyph.U0, glyph.V1);
                    tessellator.addVertexWithUV(currentX + w, currentY + h, 0, glyph.U1, glyph.V1);
                    tessellator.addVertexWithUV(currentX + w, currentY + 0, 0, glyph.U1, glyph.V0);
                    tessellator.addVertexWithUV(currentX + 0, currentY + 0, 0, glyph.U0, glyph.V0);
                }
                currentX += glyph.AdvanceWidth * DisplayScale;
            }
        }

        tessellator.draw();
    }

    private static int HexToDec(char c)
    {
        int v = c;
        if (c <= '9') v -= '0';
        else if (c <= 'F') v += 10 - 'A';
        else if (c <= 'f') v += 10 - 'a';
        else return 15;
        return v <= 0 ? 0 : v;
    }

    public int GetStringWidth(ReadOnlySpan<char> text)
    {
        if (text.IsEmpty) return 0;
        float total = 0;
        for (int i = 0; i < text.Length; ++i)
        {
            if (text[i] == 167)
                ++i;
            else
                total += GetOrCreateGlyph(text[i]).AdvanceWidth * DisplayScale;
        }
        return (int)Math.Ceiling(total);
    }

    private int GetStringFitLength(ReadOnlySpan<char> text, int maxWidth)
    {
        float width = 0;
        int lastSpaceIndex = -1;
        int i = 0;
        for (; i < text.Length; ++i)
        {
            if (text[i] == 167)
            {
                ++i;
                continue;
            }
            if (text[i] == ' ')
                lastSpaceIndex = i;
            width += GetOrCreateGlyph(text[i]).AdvanceWidth * DisplayScale;
            if (width > maxWidth)
            {
                if (lastSpaceIndex > 0)
                    return lastSpaceIndex;
                return Math.Max(1, i);
            }
        }
        return text.Length;
    }

    private void ProcessWrappedText(ReadOnlySpan<char> text, int x, int y, int maxWidth, Guis.Color color, bool draw, ref int outHeight)
    {
        if (text.IsEmpty) return;

        int totalHeight = 0;
        int currentY = y;
        int lineHeight = (int)((AtlasFontSize + GlyphPadding) * DisplayScale);

        while (text.Length > 0)
        {
            int newlineIndex = text.IndexOf('\n');
            ReadOnlySpan<char> line;
            if (newlineIndex >= 0)
            {
                line = text.Slice(0, newlineIndex);
                text = text.Slice(newlineIndex + 1);
            }
            else
            {
                line = text;
                text = [];
            }

            while (line.Length > 0)
            {
                int fitLength = GetStringFitLength(line, maxWidth);
                ReadOnlySpan<char> subline = line.Slice(0, Math.Min(fitLength, line.Length));

                while (subline.Length > 0 && subline[subline.Length - 1] == ' ')
                    subline = subline.Slice(0, subline.Length - 1);

                if (subline.Length > 0 || fitLength > 0)
                {
                    if (draw && subline.Length > 0)
                        DrawString(subline, x, currentY, color);
                    currentY += lineHeight;
                    totalHeight += lineHeight;
                }

                line = line.Slice(Math.Min(fitLength, line.Length));
                while (line.Length > 0 && line[0] == ' ')
                    line = line.Slice(1);
            }
        }

        if (totalHeight < lineHeight) totalHeight = lineHeight;
        outHeight = totalHeight;
    }

    public void DrawStringWrapped(ReadOnlySpan<char> text, int x, int y, int maxWidth, Guis.Color color)
    {
        int dummyHeight = 0;
        ProcessWrappedText(text, x, y, maxWidth, color, true, ref dummyHeight);
    }

    public int GetStringHeight(ReadOnlySpan<char> text, int maxWidth)
    {
        int height = 0;
        ProcessWrappedText(text, 0, 0, maxWidth, Guis.Color.Black, false, ref height);
        return height;
    }
}
