using BetaSharp.Client.Rendering;
using BetaSharp.Client.Rendering.Core;
using Silk.NET.OpenGL.Legacy;

namespace BetaSharp.Client.Guis;

public class Gui
{
    protected float _zLevel = 0.0F;

    protected static void DrawHorizontalLine(int startX, int endX, int y, uint color)
    {
        if (endX < startX) (startX, endX) = (endX, startX);
        DrawRect(startX, y, endX + 1, y + 1, color);
    }

    protected static void DrawVerticalLine(int x, int startY, int endY, uint color)
    {
        if (endY < startY) (startY, endY) = (endY, startY);
        DrawRect(x, startY + 1, x + 1, endY, color);
    }

    protected static void DrawRect(int x1, int y1, int x2, int y2, uint color)
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

    protected static void DrawGradientRect(int right, int bottom, int left, int top, uint topColor, uint bottomColor)
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

    public static void DrawCenteredString(TextRenderer renderer, string text, int x, int y, uint color)
    {
        renderer.DrawStringWithShadow(text, x - renderer.GetStringWidth(text) / 2, y, color);
    }

    public static void DrawString(TextRenderer renderer, string text, int x, int y, uint color)
    {
        renderer.DrawStringWithShadow(text, x, y, color);
    }

    public void DrawTexturedModalRect(int x, int y, int u, int v, int width, int height)
    {
        float f = 0.00390625F;
        Tessellator tess = Tessellator.instance;
        tess.startDrawingQuads();
        tess.addVertexWithUV(x + 0, y + height, _zLevel, (double)((u + 0) * f), (double)((v + height) * f));
        tess.addVertexWithUV(x + width, y + height, _zLevel, (double)((u + width) * f), (double)((v + height) * f));
        tess.addVertexWithUV(x + width, y + 0, _zLevel, (double)((u + width) * f), (double)((v + 0) * f));
        tess.addVertexWithUV(x + 0, y + 0, _zLevel, (double)((u + 0) * f), (double)((v + 0) * f));
        tess.draw();
    }
}
