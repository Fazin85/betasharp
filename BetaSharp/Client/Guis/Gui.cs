using BetaSharp.Client.Rendering.Core;
using Silk.NET.OpenGL.Legacy;

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
        var1.drawStringWithShadow(var2, var3 - var1.getStringWidth(var2) / 2, var4, color);
    }

    public void drawString(TextRenderer var1, string var2, int var3, int var4, uint color)
    {
        var1.drawStringWithShadow(var2, var3, var4, color);
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