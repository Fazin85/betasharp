using BetaSharp.Client.Rendering;
using BetaSharp.Client.Rendering.Core;
using BetaSharp.Client.Rendering.Core.Textures;
using BetaSharp.Client.Rendering.Guis;
using Silk.NET.OpenGL.Legacy;

namespace BetaSharp.Client.Guis;

public class Gui
{
    protected float _zLevel = 0.0F;

    protected static void DrawHorizontalLine(GuiBatch batch, int startX, int endX, int y, Color color)
    {
        if (endX < startX) (startX, endX) = (endX, startX);
        DrawRect(batch, startX, y, endX + 1, y + 1, color);
    }

    protected static void DrawVerticalLine(GuiBatch batch, int x, int startY, int endY, Color color)
    {
        if (endY < startY) (startY, endY) = (endY, startY);
        DrawRect(batch, x, startY + 1, x + 1, endY, color);
    }

    protected static void DrawRect(GuiBatch batch, int x1, int y1, int x2, int y2, Color color)
    {
        if (x1 < x2) (x1, x2) = (x2, x1);
        if (y1 < y2) (y1, y2) = (y2, y1);

        GLManager.GL.Enable(GLEnum.Blend);
        GLManager.GL.BlendFunc(GLEnum.SrcAlpha, GLEnum.OneMinusSrcAlpha);

        batch.DrawRect(x1, y1, x2, y2, color, 0f);
    }

    protected static void DrawGradientRect(GuiBatch batch, int right, int bottom, int left, int top, Color topColor, Color bottomColor)
    {
        GLManager.GL.Enable(GLEnum.Blend);
        GLManager.GL.BlendFunc(GLEnum.SrcAlpha, GLEnum.OneMinusSrcAlpha);

        batch.DrawGradientRect(left, right, top, bottom, topColor, bottomColor, 0f);
    }

    public static void DrawCenteredString(TextRenderer renderer, string text, int x, int y, Color color)
    {
        renderer.DrawStringWithShadow(text, x - renderer.GetStringWidth(text) / 2, y, color);
    }

    public static void DrawString(TextRenderer renderer, string text, int x, int y, Color color)
    {
        renderer.DrawStringWithShadow(text, x, y, color);
    }

    public void DrawTexturedModalRect(GuiBatch batch, int x, int y, int u, int v, int width, int height, TextureHandle? texture = null)
    {
        const float f = 0.00390625F; // 1/256 for 256px textures
        uint texId = texture != null ? (uint)texture.Id : 0;
        batch.DrawTexturedQuad(x, y, width, height,
            (u + 0) * f, (v + height) * f,    // v flipped: top of quad gets texture top (high v)
            (u + width) * f, (v + 0) * f,
            Color.White, _zLevel, texId);
    }
}
