using BetaSharp.Client.Rendering.Core;
using Silk.NET.OpenGL.Legacy;
using BetaSharp.Util;
using BetaSharp.Client.Rendering;

namespace BetaSharp.Client.Guis;

public class Gui : java.lang.Object
{
    protected float zLevel = 0.0F;

    protected void func_27100_a(int var1, int var2, int var3, uint color)
    {
        if (var2 < var1)
        {
            (var1, var2) = (var2, var1);
        }

        drawRect(var1, var3, var2 + 1, var3 + 1, color);
    }

    protected void func_27099_b(int var1, int var2, int var3, uint color)
    {
        if (var3 < var2)
        {
            (var2, var3) = (var3, var2);
        }

        drawRect(var1, var2 + 1, var1 + 1, var3, color);
    }

    protected void drawRect(int var1, int var2, int var3, int var4, uint color)
    {
        int var6;
        if (var1 < var3)
        {
            var6 = var1;
            var1 = var3;
            var3 = var6;
        }

        if (var2 < var4)
        {
            var6 = var2;
            var2 = var4;
            var4 = var6;
        }

        float var11 = (color >> 24 & 255) / 255.0F;
        float var7 = (color >> 16 & 255) / 255.0F;
        float var8 = (color >> 8 & 255) / 255.0F;
        float var9 = (color & 255) / 255.0F;
        Tessellator var10 = Tessellator.instance;
        GLManager.GL.Enable(GLEnum.Blend);
        GLManager.GL.Disable(GLEnum.Texture2D);
        GLManager.GL.BlendFunc(GLEnum.SrcAlpha, GLEnum.OneMinusSrcAlpha);
        GLManager.GL.Color4(var7, var8, var9, var11);
        var10.startDrawingQuads();
        var10.addVertex(var1, var4, 0.0D);
        var10.addVertex(var3, var4, 0.0D);
        var10.addVertex(var3, var2, 0.0D);
        var10.addVertex(var1, var2, 0.0D);
        var10.draw();
        GLManager.GL.Enable(GLEnum.Texture2D);
        GLManager.GL.Disable(GLEnum.Blend);
    }

    protected void drawGradientRect(int right, int bottom, int left, int top, uint topColor, uint bottomColor)
    {
        float var7 = (topColor >> 24 & 255) / 255.0F;
        float var8 = (topColor >> 16 & 255) / 255.0F;
        float var9 = (topColor >> 8 & 255) / 255.0F;
        float var10 = (topColor & 255) / 255.0F;
        float var11 = (bottomColor >> 24 & 255) / 255.0F;
        float var12 = (bottomColor >> 16 & 255) / 255.0F;
        float var13 = (bottomColor >> 8 & 255) / 255.0F;
        float var14 = (bottomColor & 255) / 255.0F;
        GLManager.GL.Disable(GLEnum.Texture2D);
        GLManager.GL.Enable(GLEnum.Blend);
        GLManager.GL.Disable(GLEnum.AlphaTest);
        GLManager.GL.BlendFunc(GLEnum.SrcAlpha, GLEnum.OneMinusSrcAlpha);
        GLManager.GL.ShadeModel(GLEnum.Smooth);
        Tessellator tess = Tessellator.instance;
        tess.startDrawingQuads();
        tess.setColorRGBA_F(var8, var9, var10, var7);
        tess.addVertex(left, bottom, 0.0D);
        tess.addVertex(right, bottom, 0.0D);
        tess.setColorRGBA_F(var12, var13, var14, var11);
        tess.addVertex(right, top, 0.0D);
        tess.addVertex(left, top, 0.0D);
        tess.draw();
        GLManager.GL.ShadeModel(GLEnum.Flat);
        GLManager.GL.Disable(GLEnum.Blend);
        GLManager.GL.Enable(GLEnum.AlphaTest);
        GLManager.GL.Enable(GLEnum.Texture2D);
    }

    public void drawCenteredString(TextRenderer var1, string var2, int var3, int var4, uint color)
    {
        // Check if text contains any color codes like &e, &8, &a, etc.
        if (HasColorCodes(var2))
        {
            // Draw with color support
            DrawStringWithColors(var1, var2, var3 - var1.getStringWidth(RemoveColorCodes(var2)) / 2, var4);
        }
        else
        {
            var1.drawStringWithShadow(var2, var3 - var1.getStringWidth(var2) / 2, var4, color);
        }
    }

    public void drawString(TextRenderer var1, string var2, int var3, int var4, uint color)
    {
        // Check if text contains any color codes
        if (HasColorCodes(var2))
        {
            DrawStringWithColors(var1, var2, var3, var4);
        }
        else
        {
            var1.drawStringWithShadow(var2, var3, var4, color);
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
                drawRect(currentX, y + 9, currentX + segWidth, y + 10, currentColor);
            }

            // Strikethrough
            if (strikethrough)
            {
                drawRect(currentX, y + 4, currentX + segWidth, y + 5, currentColor);
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
                // Color codes reset formatting
                if ((code >= '0' && code <= '9') || (code >= 'a' && code <= 'f'))
                {
                    currentColor = code switch
                    {
                        '0' => 0xFF000000u,
                        '1' => 0xFF0000AAu,
                        '2' => 0xFF00AA00u,
                        '3' => 0xFF00AAAAu,
                        '4' => 0xFFAA0000u,
                        '5' => 0xFFAA00AAu,
                        '6' => 0xFFFFAA00u,
                        '7' => 0xFFAAAAAAu,
                        '8' => 0xFF555555u,
                        '9' => 0xFF5555FFu,
                        'a' => 0xFF55FF55u,
                        'b' => 0xFF55FFFFu,
                        'c' => 0xFFFF5555u,
                        'd' => 0xFFFF55FFu,
                        'e' => 0xFFFFFF55u,
                        'f' => 0xFFFFFFFFu,
                        _ => currentColor
                    };
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

    public void drawTexturedModalRect(int var1, int var2, int var3, int var4, int var5, int var6)
    {
        float var7 = 0.00390625F;
        float var8 = 0.00390625F;
        Tessellator var9 = Tessellator.instance;
        var9.startDrawingQuads();
        var9.addVertexWithUV(var1 + 0, var2 + var6, zLevel, (double)((var3 + 0) * var7), (double)((var4 + var6) * var8));
        var9.addVertexWithUV(var1 + var5, var2 + var6, zLevel, (double)((var3 + var5) * var7), (double)((var4 + var6) * var8));
        var9.addVertexWithUV(var1 + var5, var2 + 0, zLevel, (double)((var3 + var5) * var7), (double)((var4 + 0) * var8));
        var9.addVertexWithUV(var1 + 0, var2 + 0, zLevel, (double)((var3 + 0) * var7), (double)((var4 + 0) * var8));
        var9.draw();
    }
}