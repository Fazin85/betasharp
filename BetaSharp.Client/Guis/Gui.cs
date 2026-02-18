using BetaSharp.Client.Rendering.Core;
using Silk.NET.OpenGL.Legacy;
using BetaSharp.Util;
using BetaSharp.Client.Rendering;

namespace BetaSharp.Client.Guis;

public class Gui
{
    protected float _zLevel = 0.0F;

    private static readonly uint[] _colorCodes =
    {
        0xFF000000u, 0xFF0000AAu, 0xFF00AA00u, 0xFF00AAAAu,
        0xFFAA0000u, 0xFFAA00AAu, 0xFFFFAA00u, 0xFFAAAAAAu,
        0xFF555555u, 0xFF5555FFu, 0xFF55FF55u, 0xFF55FFFFu,
        0xFFFF5555u, 0xFFFF55FFu, 0xFFFFFF55u, 0xFFFFFFFFu
    };

    protected void DrawHorizontalLine(int startX, int endX, int y, uint color)
    {
        if (endX < startX) (startX, endX) = (endX, startX);
        DrawRect(startX, y, endX + 1, y + 1, color);
    }

    protected void DrawVerticalLine(int x, int startY, int endY, uint color)
    {
        if (endY < startY) (startY, endY) = (endY, startY);
        DrawRect(x, startY + 1, x + 1, endY, color);
    }

    protected void DrawRect(int x1, int y1, int x2, int y2, uint color)
    {
        if (x1 < x2) (x1, x2) = (x2, x1);
        if (y1 < y2) (y1, y2) = (y2, y1);

        float a = (color >> 24 & 255) / 255.0F;
        float r = (color >> 16 & 255) / 255.0F;
        float g = (color >> 8 & 255) / 255.0F;
        float b = (color & 255) / 255.0F;
        Tessellator tess = Tessellator.instance;

        GLManager.GL.Enable(GLEnum.Blend);
        GLManager.GL.Disable(GLEnum.Texture2D);
        GLManager.GL.BlendFunc(GLEnum.SrcAlpha, GLEnum.OneMinusSrcAlpha);
        GLManager.GL.Color4(r, g, b, a);

        tess.startDrawingQuads();
        tess.addVertex(x1, y2, 0.0D);
        tess.addVertex(x2, y2, 0.0D);
        tess.addVertex(x2, y1, 0.0D);
        tess.addVertex(x1, y1, 0.0D);
        tess.draw();

        GLManager.GL.Enable(GLEnum.Texture2D);
        GLManager.GL.Disable(GLEnum.Blend);
    }

    protected void DrawGradientRect(int right, int bottom, int left, int top, uint topColor, uint bottomColor)
    {
        float a1 = (topColor >> 24 & 255) / 255.0F;
        float r1 = (topColor >> 16 & 255) / 255.0F;
        float g1 = (topColor >> 8 & 255) / 255.0F;
        float b1 = (topColor & 255) / 255.0F;

        float a2 = (bottomColor >> 24 & 255) / 255.0F;
        float r2 = (bottomColor >> 16 & 255) / 255.0F;
        float g2 = (bottomColor >> 8 & 255) / 255.0F;
        float b2 = (bottomColor & 255) / 255.0F;

        GLManager.GL.Disable(GLEnum.Texture2D);
        GLManager.GL.Enable(GLEnum.Blend);
        GLManager.GL.Disable(GLEnum.AlphaTest);
        GLManager.GL.BlendFunc(GLEnum.SrcAlpha, GLEnum.OneMinusSrcAlpha);
        GLManager.GL.ShadeModel(GLEnum.Smooth);

        Tessellator tess = Tessellator.instance;
        tess.startDrawingQuads();
        tess.setColorRGBA_F(r1, g1, b1, a1);
        tess.addVertex(left, bottom, 0.0D);
        tess.addVertex(right, bottom, 0.0D);
        tess.setColorRGBA_F(r2, g2, b2, a2);
        tess.addVertex(right, top, 0.0D);
        tess.addVertex(left, top, 0.0D);
        tess.draw();

        GLManager.GL.ShadeModel(GLEnum.Flat);
        GLManager.GL.Disable(GLEnum.Blend);
        GLManager.GL.Enable(GLEnum.AlphaTest);
        GLManager.GL.Enable(GLEnum.Texture2D);
    }

    public void DrawCenteredString(TextRenderer renderer, string text, int x, int y, uint color)
    {
        // Check if text contains any color codes like &e, &8, &a, etc.
        if (HasColorCodes(text))
        {
            // Draw with color support
            DrawStringWithColors(renderer, text, x - renderer.getStringWidth(RemoveColorCodes(text)) / 2, y);
        }
        else
        {
            renderer.drawStringWithShadow(text, x - renderer.getStringWidth(text) / 2, y, color);
        }
    }

    public void DrawString(TextRenderer renderer, string text, int x, int y, uint color)
    {
        if (HasColorCodes(text))
        {
            DrawStringWithColors(renderer, text, x, y);
        }
        else
        {
            renderer.drawStringWithShadow(text, x, y, color);
        }
    }

    private bool HasColorCodes(string text)
    {
        if (string.IsNullOrEmpty(text)) return false;

        for (int i = 0; i < text.Length - 1; i++)
        {
            if (text[i] == '&')
            {
                char nextChar = text[i + 1];
                if ((nextChar >= '0' && nextChar <= '9') ||
                    (nextChar >= 'a' && nextChar <= 'f') ||
                    (nextChar >= 'A' && nextChar <= 'F') ||
                    nextChar == 'r' || nextChar == 'R')
                {
                    return true;
                }
            }
        }

        return false;
    }

    private void DrawStringWithColors(TextRenderer renderer, string text, int x, int y)
    {
        int currentX = x;
        uint currentColor = 0xFFFFFFFF; // Default white
        bool bold = false;
        bool italic = false; // not used for rendering, reserved
        bool underline = false;
        bool strikethrough = false;
        bool obfuscated = false;

        var sb = new System.Text.StringBuilder();

        void FlushSegment()
        {
            if (sb.Length == 0) return;
            string seg = sb.ToString();

            // Apply obfuscation if needed
            string drawText = seg;
            if (obfuscated)
            {
                var rnd = new System.Random();
                var allowed = ChatAllowedCharacters.allowedCharacters;
                var ob = new System.Text.StringBuilder();
                for (int k = 0; k < drawText.Length; k++)
                {
                    char rc = allowed[rnd.Next(allowed.Length)];
                    ob.Append(rc);
                }
                drawText = ob.ToString();
            }

            // Draw the segment
            renderer.drawStringWithShadow(drawText, currentX, y, currentColor);

            // Bold: draw offset copy
            if (bold)
            {
                renderer.drawString(drawText, currentX + 1, y, currentColor);
            }

            int segWidth = renderer.getStringWidth(drawText);

            // Underline
            if (underline)
            {
                DrawRect(currentX, y + 9, currentX + segWidth, y + 10, currentColor);
            }

            // Strikethrough
            if (strikethrough)
            {
                DrawRect(currentX, y + 4, currentX + segWidth, y + 5, currentColor);
            }

            currentX += segWidth;
            sb.Clear();
        }

        int i = 0;
        while (i < text.Length)
        {
            if (i < text.Length - 1 && text[i] == '&')
            {
                // Flush any pending text before changing style/color
                FlushSegment();

                char code = char.ToLower(text[i + 1]);
                int colorIdx = "0123456789abcdef".IndexOf(code);
                // Color codes reset formatting
                if ((code >= '0' && code <= '9') || (code >= 'a' && code <= 'f'))
                {
                    if (colorIdx != -1)
                    {
                        currentColor = _colorCodes[colorIdx];
                        bold = obfuscated = strikethrough = underline = false;
                    }
                    // Reset formatting on color
                    bold = italic = underline = strikethrough = obfuscated = false;
                }
                else
                {
                    switch (code)
                    {
                        case 'k': // obfuscated
                            obfuscated = true; break;
                        case 'l': // bold
                            bold = true; break;
                        case 'm': // strikethrough
                            strikethrough = true; break;
                        case 'n': // underline
                            underline = true; break;
                        case 'o': // italic
                            italic = true; break;
                        case 'r': // reset
                            currentColor = 0xFFFFFFFFu;
                            bold = italic = underline = strikethrough = obfuscated = false;
                            break;
                        default:
                            break;
                    }
                }

                i += 2;
            }
            else
            {
                sb.Append(text[i]);
                i++;
            }
        }

        // Flush remaining
        FlushSegment();
    }

    private string RemoveColorCodes(string text)
    {
        if (text == null) return string.Empty;

        string result = text;
        // Remove all color codes (&0-&9, &a-&f, &r)
        for (char c = '0'; c <= '9'; c++)
        {
            result = result.Replace("&" + c, "");
        }
        for (char c = 'a'; c <= 'f'; c++)
        {
            result = result.Replace("&" + c, "");
            result = result.Replace("&" + char.ToUpper(c), "");
        }
        result = result.Replace("&r", "");
        result = result.Replace("&R", "");
        return result;
    }

    public void DrawTexturedModalRect(int x, int y, int u, int v, int width, int height)
    {
        float f = 0.00390625F; // 1/256
        Tessellator tess = Tessellator.instance;
        tess.startDrawingQuads();
        tess.addVertexWithUV(x + 0, y + height, _zLevel, (double)((u + 0) * f), (double)((v + height) * f));
        tess.addVertexWithUV(x + width, y + height, _zLevel, (double)((u + width) * f), (double)((v + height) * f));
        tess.addVertexWithUV(x + width, y + 0, _zLevel, (double)((u + width) * f), (double)((v + 0) * f));
        tess.addVertexWithUV(x + 0, y + 0, _zLevel, (double)((u + 0) * f), (double)((v + 0) * f));
        tess.draw();
    }
}