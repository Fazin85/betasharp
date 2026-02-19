using BetaSharp.Client.Options;
using BetaSharp.Client.Rendering.Core;
using BetaSharp.Util;
using Silk.NET.OpenGL.Legacy;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace BetaSharp.Client.Rendering;

public class TextRenderer
{
    private readonly int[] _charWidth = new int[256];
    public int FontTextureName = 0;
    private readonly int _fontDisplayLists;

    private readonly uint[] _listBuffer = new uint[1024];
    private int _bufferPosition = 0;

    public TextRenderer(GameOptions var1, TextureManager textureManager)
    {
        Image<Rgba32> fontImage;
        try
        {
            fontImage = Image.Load<Rgba32>(new MemoryStream(AssetManager.Instance.getAsset("font/default.png").getBinaryContent()));
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to load default font.", ex);
        }

        int width = fontImage.Width;
        int height = fontImage.Height;
        int[] pixels = new int[width * height];
        fontImage.ProcessPixelRows(accessor =>
        {
            for (int y = 0; y < accessor.Height; y++)
            {
                var row = accessor.GetRowSpan(y);
                for (int x = 0; x < accessor.Width; x++)
                {
                    var p = row[x];
                    pixels[y * width + x] = (p.A << 24) | (p.R << 16) | (p.G << 8) | p.B;
                }
            }
        });

        for (int charIndex = 0; charIndex < 256; ++charIndex)
        {
            int col = charIndex % 16;
            int row = charIndex / 16;
            int charWidth = 0;

            for (int bit = 7; bit >= 0; --bit)
            {
                int xOffset = col * 8 + bit;
                bool columnIsEmpty = true;

                for (int var14 = 0; var14 < 8 && columnIsEmpty; ++var14)
                {
                    int pixelIndex = (row * 8 + var14) * width;
                    int alpha = pixels[xOffset + pixelIndex] & 255;
                    if (alpha > 0)
                    {
                        columnIsEmpty = false;
                    }
                }

                if (!columnIsEmpty)
                {
                    break;
                }
            }

            if (charIndex == 32)
            {
                charWidth = 2;
            }

            _charWidth[charIndex] = charWidth + 2;
        }

        FontTextureName = textureManager.Load(fontImage);
        _fontDisplayLists = GLAllocation.generateDisplayLists(288);
        Tessellator tess = Tessellator.instance;

        for (int charIndex = 0; charIndex < 256; ++charIndex)
        {
            GLManager.GL.NewList((uint)(_fontDisplayLists + charIndex), GLEnum.Compile);
            tess.startDrawingQuads();
            int u = charIndex % 16 * 8;
            int v = charIndex / 16 * 8;
            float texSize = 7.99F;

            tess.addVertexWithUV(0.0D, (double)(0.0F + texSize), 0.0D, (double)(u / 128.0F), (double)((v + texSize) / 128.0F));
            tess.addVertexWithUV((double)(0.0F + texSize), (double)(0.0F + texSize), 0.0D, (double)((u + texSize) / 128.0F), (double)((v + texSize) / 128.0F));
            tess.addVertexWithUV((double)(0.0F + texSize), 0.0D, 0.0D, (double)((u + texSize) / 128.0F), (double)(v / 128.0F));
            tess.addVertexWithUV(0.0D, 0.0D, 0.0D, (double)(u / 128.0F), (double)(v / 128.0F));

            tess.draw();
            GLManager.GL.Translate(_charWidth[charIndex], 0.0F, 0.0F);
            GLManager.GL.EndList();
        }

        for (int colorIndex = 0; colorIndex < 32; ++colorIndex)
        {
            int baseColor = (colorIndex >> 3 & 1) * 85;
            int r = (colorIndex >> 2 & 1) * 170 + baseColor;
            int g = (colorIndex >> 1 & 1) * 170 + baseColor;
            int b = (colorIndex >> 0 & 1) * 170 + baseColor;

            if (colorIndex == 6)
            {
                r += 85;
            }

            if (colorIndex >= 16)
            {
                r /= 4;
                g /= 4;
                b /= 4;
            }

            GLManager.GL.NewList((uint)(_fontDisplayLists + 256 + colorIndex), GLEnum.Compile);
            GLManager.GL.Color3(r / 255.0F, g / 255.0F, b / 255.0F);
            GLManager.GL.EndList();
        }

    }

    public void DrawStringWithShadow(string text, int x, int y, uint color)
    {
        RenderString(text, x + 1, y + 1, color, true);
        DrawString(text, x, y, color);
    }

    public void DrawString(string text, int x, int y, uint color)
    {
        RenderString(text, x, y, color, false);
    }

    public unsafe void RenderString(string text, int x, int y, uint color, bool darken)
    {
        if (string.IsNullOrEmpty(text)) return;

        uint alpha = color & 0xFF000000;
        if (darken)
        {
            color = (color & 0xFCFCFC) >> 2;
            color |= alpha;
        }
        // assume alpha was omitted and default to fully opaque

        GLManager.GL.BindTexture(GLEnum.Texture2D, (uint)FontTextureName);
        float a = (color >> 24 & 255) / 255.0F;
        float r = (color >> 16 & 255) / 255.0F;
        float g = (color >> 8 & 255) / 255.0F;
        float b = (color & 255) / 255.0F;
        if (a == 0.0F) a = 1.0F;


        GLManager.GL.Color4(r, g, b, a);
        GLManager.GL.PushMatrix();
        GLManager.GL.Translate(x, y, 0.0F);

        _bufferPosition = 0;

        for (int i = 0; i < text.Length; ++i)
        {
            // Handle color codes (e.g., §a, §c)
            while (text.Length > i + 1 && text[i] == 167) // 167 is the '§' character
            {
                int colorCode = "0123456789abcdef".IndexOf(char.ToLower(text[i + 1]));
                if (colorCode < 0 || colorCode > 15) colorCode = 15;

                _listBuffer[_bufferPosition++] = (uint)(_fontDisplayLists + 256 + colorCode + (darken ? 16 : 0));

                if (_bufferPosition == _listBuffer.Length) FlushBuffer();
                i += 2;
            }

            if (i < text.Length)
            {
                int charIndex = ChatAllowedCharacters.allowedCharacters.IndexOf(text[i]);
                if (charIndex >= 0)
                {
                    _listBuffer[_bufferPosition++] = (uint)(_fontDisplayLists + charIndex + 32);
                    if (_bufferPosition == _listBuffer.Length) FlushBuffer();
                }
            }
        }

        FlushBuffer();
        GLManager.GL.PopMatrix();

        void FlushBuffer()
        {
            if (_bufferPosition == 0) return;
            fixed (uint* ptr = _listBuffer)
            {
                GLManager.GL.CallLists((uint)_bufferPosition, GLEnum.UnsignedInt, ptr);
            }
            _bufferPosition = 0;
        }
    }

    public int GetStringWidth(string text)
    {
        if (string.IsNullOrEmpty(text)) return 0;

        int width = 0;

        for (int i = 0; i < text.Length; ++i)
        {
            if (text[i] == 167)
            {
                ++i;
            }
            else
            {
                int charIndex = ChatAllowedCharacters.allowedCharacters.IndexOf(text[i]);
                if (charIndex >= 0)
                {
                    width += _charWidth[charIndex + 32];
                }
            }
        }

        return width;

    }

    public void DrawStringWrapped(string text, int x, int y, int maxWidth, uint color)
    {
        if (text == null)
        {
            return;
        }

        string[] var6 = text.Split("\n");
        if (var6.Length > 1)
        {
            for (int var11 = 0; var11 < var6.Length; ++var11)
            {
                DrawStringWrapped(var6[var11], x, y, maxWidth, color);
                y += GetStringHeight(var6[var11], maxWidth);
            }
            return;
        }

        string[] var7 = text.Split(" ");
        int var8 = 0;

        while (var8 < var7.Length)
        {
            string var9;
            for (var9 = var7[var8++] + " "; var8 < var7.Length && GetStringWidth(var9 + var7[var8]) < maxWidth; var9 = var9 + var7[var8++] + " ")
            {
            }

            int var10;
            for (; GetStringWidth(var9) > maxWidth; var9 = var9[var10..])
            {
                for (var10 = 0; GetStringWidth(var9[..(var10 + 1)]) <= maxWidth; ++var10)
                {
                }

                if (var9[..var10].Trim().Length > 0)
                {
                    DrawString(var9[..var10], x, y, color);
                    y += 8;
                }
            }

            if (var9.Trim().Length > 0)
            {
                DrawString(var9, x, y, color);
                y += 8;
            }
        }
    }

    public int GetStringHeight(string text, int maxWidth)
    {
        if (text == null)
        {
            return 0;
        }

        string[] var3 = text.Split("\n");
        int var5;
        if (var3.Length > 1)
        {
            int var9 = 0;

            for (var5 = 0; var5 < var3.Length; ++var5)
            {
                var9 += GetStringHeight(var3[var5], maxWidth);
            }

            return var9;
        }
        else
        {
            string[] var4 = text.Split(" ");
            var5 = 0;
            int var6 = 0;

            while (var5 < var4.Length)
            {
                string var7;
                for (var7 = var4[var5++] + " "; var5 < var4.Length && GetStringWidth(var7 + var4[var5]) < maxWidth; var7 = var7 + var4[var5++] + " ")
                {
                }

                int var8;
                for (; GetStringWidth(var7) > maxWidth; var7 = var7[var8..])
                {
                    for (var8 = 0; GetStringWidth(var7[..(var8 + 1)]) <= maxWidth; ++var8)
                    {
                    }

                    if (var7[..var8].Trim().Length > 0)
                    {
                        var6 += 8;
                    }
                }

                if (var7.Trim().Length > 0)
                {
                    var6 += 8;
                }
            }

            if (var6 < 8)
            {
                var6 += 8;
            }

            return var6;
        }
    }
}
