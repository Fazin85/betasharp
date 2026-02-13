using BetaSharp.Client.Rendering.Core;
using Silk.NET.OpenGL.Legacy;

namespace BetaSharp.Client.Guis;

public class Gui : java.lang.Object
{
    protected float zLevel = 0.0F;

    protected void func_27100_a(int var1, int var2, int var3, int var4)
    {
        if (var2 < var1)
        {
            int var5 = var1;
            var1 = var2;
            var2 = var5;
        }

        drawRect(var1, var3, var2 + 1, var3 + 1, var4);
    }

    protected void func_27099_b(int var1, int var2, int var3, int var4)
    {
        if (var3 < var2)
        {
            int var5 = var2;
            var2 = var3;
            var3 = var5;
        }

        drawRect(var1, var2 + 1, var1 + 1, var3, var4);
    }

    protected void drawRect(int var1, int var2, int var3, int var4, int var5)
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

        float var11 = (var5 >> 24 & 255) / 255.0F;
        float var7 = (var5 >> 16 & 255) / 255.0F;
        float var8 = (var5 >> 8 & 255) / 255.0F;
        float var9 = (var5 & 255) / 255.0F;
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

    protected void drawGradientRect(int var1, int var2, int var3, int var4, int var5, int var6)
    {
        float var7 = (var5 >> 24 & 255) / 255.0F;
        float var8 = (var5 >> 16 & 255) / 255.0F;
        float var9 = (var5 >> 8 & 255) / 255.0F;
        float var10 = (var5 & 255) / 255.0F;
        float var11 = (var6 >> 24 & 255) / 255.0F;
        float var12 = (var6 >> 16 & 255) / 255.0F;
        float var13 = (var6 >> 8 & 255) / 255.0F;
        float var14 = (var6 & 255) / 255.0F;
        GLManager.GL.Disable(GLEnum.Texture2D);
        GLManager.GL.Enable(GLEnum.Blend);
        GLManager.GL.Disable(GLEnum.AlphaTest);
        GLManager.GL.BlendFunc(GLEnum.SrcAlpha, GLEnum.OneMinusSrcAlpha);
        GLManager.GL.ShadeModel(GLEnum.Smooth);
        Tessellator var15 = Tessellator.instance;
        var15.startDrawingQuads();
        var15.setColorRGBA_F(var8, var9, var10, var7);
        var15.addVertex(var3, var2, 0.0D);
        var15.addVertex(var1, var2, 0.0D);
        var15.setColorRGBA_F(var12, var13, var14, var11);
        var15.addVertex(var1, var4, 0.0D);
        var15.addVertex(var3, var4, 0.0D);
        var15.draw();
        GLManager.GL.ShadeModel(GLEnum.Flat);
        GLManager.GL.Disable(GLEnum.Blend);
        GLManager.GL.Enable(GLEnum.AlphaTest);
        GLManager.GL.Enable(GLEnum.Texture2D);
    }

    public void drawCenteredString(TextRenderer var1, string var2, int var3, int var4, int var5)
    {
        var1.drawStringWithShadow(var2, var3 - var1.getStringWidth(var2) / 2, var4, var5);
    }

    public void drawString(TextRenderer var1, string var2, int var3, int var4, int var5)
    {
        var1.drawStringWithShadow(var2, var3, var4, var5);
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