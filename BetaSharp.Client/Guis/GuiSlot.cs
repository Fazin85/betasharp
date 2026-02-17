using BetaSharp.Client.Input;
using BetaSharp.Client.Rendering.Core;
using java.util;
using Silk.NET.OpenGL.Legacy;

namespace BetaSharp.Client.Guis;

public abstract class GuiSlot
{
    private readonly Minecraft mc;
    private readonly int width;
    private readonly int height;
    protected readonly int top;
    protected readonly int bottom;
    private readonly int right;
    private readonly int left;
    protected readonly int posZ;
    private int scrollUpButtonID;
    private int scrollDownButtonID;
    private float initialClickY = -2.0F;
    private float scrollMultiplier;
    private float amountScrolled;
    private int selectedElement = -1;
    private long lastClicked = 0L;
    private bool field_25123_p = true;
    private bool field_27262_q;
    private int field_27261_r;

    public GuiSlot(Minecraft var1, int var2, int var3, int var4, int var5, int var6)
    {
        mc = var1;
        width = var2;
        height = var3;
        top = var4;
        bottom = var5;
        posZ = var6;
        left = 0;
        right = var2;
    }

    public void func_27258_a(bool var1)
    {
        field_25123_p = var1;
    }

    protected void func_27259_a(bool var1, int var2)
    {
        field_27262_q = var1;
        field_27261_r = var2;
        if (!var1)
        {
            field_27261_r = 0;
        }

    }

    public abstract int GetSize();

    protected abstract void ElementClicked(int var1, bool var2);

    protected abstract bool isSelected(int var1);

    protected virtual int getContentHeight()
    {
        return GetSize() * posZ + field_27261_r;
    }

    protected abstract void drawBackground();

    protected abstract void drawSlot(int var1, int var2, int var3, int var4, Tessellator var5);

    protected virtual void func_27260_a(int var1, int var2, Tessellator var3)
    {
    }

    protected virtual void func_27255_a(int var1, int var2)
    {
    }

    protected virtual void func_27257_b(int var1, int var2)
    {
    }

    public int func_27256_c(int var1, int var2)
    {
        int var3 = width / 2 - 110;
        int var4 = width / 2 + 110;
        int var5 = var2 - top - field_27261_r + (int)amountScrolled - 4;
        int var6 = var5 / posZ;
        return var1 >= var3 && var1 <= var4 && var6 >= 0 && var5 >= 0 && var6 < GetSize() ? var6 : -1;
    }

    public void RegisterScrollButtons(List<GuiButton> var1, int var2, int var3)
    {
        scrollUpButtonID = var2;
        scrollDownButtonID = var3;
    }

    private void bindAmountScrolled()
    {
        int var1 = getContentHeight() - (bottom - top - 4);
        if (var1 < 0)
        {
            var1 /= 2;
        }

        if (amountScrolled < 0.0F)
        {
            amountScrolled = 0.0F;
        }

        if (amountScrolled > var1)
        {
            amountScrolled = var1;
        }

    }

    public void actionPerformed(GuiButton var1)
    {
        if (var1.Enabled)
        {
            if (var1.Id == scrollUpButtonID)
            {
                amountScrolled -= posZ * 2 / 3;
                initialClickY = -2.0F;
                bindAmountScrolled();
            }
            else if (var1.Id == scrollDownButtonID)
            {
                amountScrolled += posZ * 2 / 3;
                initialClickY = -2.0F;
                bindAmountScrolled();
            }

        }
    }

    public void drawScreen(int var1, int var2, float var3)
    {
        drawBackground();
        int var4 = GetSize();
        int var5 = width / 2 + 124;
        int var6 = var5 + 6;
        int var9;
        int var10;
        int var11;
        int var13;
        int var19;
        if (Mouse.isButtonDown(0))
        {
            if (initialClickY == -1.0F)
            {
                bool var7 = true;
                if (var2 >= top && var2 <= bottom)
                {
                    int var8 = width / 2 - 110;
                    var9 = width / 2 + 110;
                    var10 = var2 - top - field_27261_r + (int)amountScrolled - 4;
                    var11 = var10 / posZ;
                    if (var1 >= var8 && var1 <= var9 && var11 >= 0 && var10 >= 0 && var11 < var4)
                    {
                        bool var12 = var11 == selectedElement && java.lang.System.currentTimeMillis() - lastClicked < 250L;
                        ElementClicked(var11, var12);
                        selectedElement = var11;
                        lastClicked = java.lang.System.currentTimeMillis();
                    }
                    else if (var1 >= var8 && var1 <= var9 && var10 < 0)
                    {
                        func_27255_a(var1 - var8, var2 - top + (int)amountScrolled - 4);
                        var7 = false;
                    }

                    if (var1 >= var5 && var1 <= var6)
                    {
                        scrollMultiplier = -1.0F;
                        var19 = getContentHeight() - (bottom - top - 4);
                        if (var19 < 1)
                        {
                            var19 = 1;
                        }

                        var13 = (int)((bottom - top) * (bottom - top) / (float)getContentHeight());
                        if (var13 < 32)
                        {
                            var13 = 32;
                        }

                        if (var13 > bottom - top - 8)
                        {
                            var13 = bottom - top - 8;
                        }

                        scrollMultiplier /= (bottom - top - var13) / (float)var19;
                    }
                    else
                    {
                        scrollMultiplier = 1.0F;
                    }

                    if (var7)
                    {
                        initialClickY = var2;
                    }
                    else
                    {
                        initialClickY = -2.0F;
                    }
                }
                else
                {
                    initialClickY = -2.0F;
                }
            }
            else if (initialClickY >= 0.0F)
            {
                amountScrolled -= (var2 - initialClickY) * scrollMultiplier;
                initialClickY = var2;
            }
        }
        else
        {
            initialClickY = -1.0F;
        }

        bindAmountScrolled();
        GLManager.GL.Disable(GLEnum.Lighting);
        GLManager.GL.Disable(GLEnum.Fog);
        Tessellator var16 = Tessellator.instance;
        GLManager.GL.BindTexture(GLEnum.Texture2D, (uint)mc.textureManager.getTextureId("/gui/background.png"));
        GLManager.GL.Color4(1.0F, 1.0F, 1.0F, 1.0F);
        float var17 = 32.0F;
        var16.startDrawingQuads();
        var16.setColorOpaque_I(2105376);
        var16.addVertexWithUV(left, bottom, 0.0D, (double)(left / var17), (double)((bottom + (int)amountScrolled) / var17));
        var16.addVertexWithUV(right, bottom, 0.0D, (double)(right / var17), (double)((bottom + (int)amountScrolled) / var17));
        var16.addVertexWithUV(right, top, 0.0D, (double)(right / var17), (double)((top + (int)amountScrolled) / var17));
        var16.addVertexWithUV(left, top, 0.0D, (double)(left / var17), (double)((top + (int)amountScrolled) / var17));
        var16.draw();
        var9 = width / 2 - 92 - 16;
        var10 = top + 4 - (int)amountScrolled;
        if (field_27262_q)
        {
            func_27260_a(var9, var10, var16);
        }

        int var14;
        for (var11 = 0; var11 < var4; ++var11)
        {
            var19 = var10 + var11 * posZ + field_27261_r;
            var13 = posZ - 4;
            if (var19 <= bottom && var19 + var13 >= top)
            {
                if (field_25123_p && isSelected(var11))
                {
                    var14 = width / 2 - 110;
                    int var15 = width / 2 + 110;
                    GLManager.GL.Color4(1.0F, 1.0F, 1.0F, 1.0F);
                    GLManager.GL.Disable(GLEnum.Texture2D);
                    var16.startDrawingQuads();
                    var16.setColorOpaque_I(8421504);
                    var16.addVertexWithUV(var14, var19 + var13 + 2, 0.0D, 0.0D, 1.0D);
                    var16.addVertexWithUV(var15, var19 + var13 + 2, 0.0D, 1.0D, 1.0D);
                    var16.addVertexWithUV(var15, var19 - 2, 0.0D, 1.0D, 0.0D);
                    var16.addVertexWithUV(var14, var19 - 2, 0.0D, 0.0D, 0.0D);
                    var16.setColorOpaque_I(0);
                    var16.addVertexWithUV(var14 + 1, var19 + var13 + 1, 0.0D, 0.0D, 1.0D);
                    var16.addVertexWithUV(var15 - 1, var19 + var13 + 1, 0.0D, 1.0D, 1.0D);
                    var16.addVertexWithUV(var15 - 1, var19 - 1, 0.0D, 1.0D, 0.0D);
                    var16.addVertexWithUV(var14 + 1, var19 - 1, 0.0D, 0.0D, 0.0D);
                    var16.draw();
                    GLManager.GL.Enable(GLEnum.Texture2D);
                }

                drawSlot(var11, var9, var19, var13, var16);
            }
        }

        GLManager.GL.Disable(GLEnum.DepthTest);
        byte var18 = 4;
        overlayBackground(0, top, 255, 255);
        overlayBackground(bottom, height, 255, 255);
        GLManager.GL.Enable(GLEnum.Blend);
        GLManager.GL.BlendFunc(GLEnum.SrcAlpha, GLEnum.OneMinusSrcAlpha);
        GLManager.GL.Disable(GLEnum.AlphaTest);
        GLManager.GL.ShadeModel(GLEnum.Smooth);
        GLManager.GL.Disable(GLEnum.Texture2D);
        var16.startDrawingQuads();
        var16.setColorRGBA_I(0, 0);
        var16.addVertexWithUV(left, top + var18, 0.0D, 0.0D, 1.0D);
        var16.addVertexWithUV(right, top + var18, 0.0D, 1.0D, 1.0D);
        var16.setColorRGBA_I(0, 255);
        var16.addVertexWithUV(right, top, 0.0D, 1.0D, 0.0D);
        var16.addVertexWithUV(left, top, 0.0D, 0.0D, 0.0D);
        var16.draw();
        var16.startDrawingQuads();
        var16.setColorRGBA_I(0, 255);
        var16.addVertexWithUV(left, bottom, 0.0D, 0.0D, 1.0D);
        var16.addVertexWithUV(right, bottom, 0.0D, 1.0D, 1.0D);
        var16.setColorRGBA_I(0, 0);
        var16.addVertexWithUV(right, bottom - var18, 0.0D, 1.0D, 0.0D);
        var16.addVertexWithUV(left, bottom - var18, 0.0D, 0.0D, 0.0D);
        var16.draw();
        var19 = getContentHeight() - (bottom - top - 4);
        if (var19 > 0)
        {
            var13 = (bottom - top) * (bottom - top) / getContentHeight();
            if (var13 < 32)
            {
                var13 = 32;
            }

            if (var13 > bottom - top - 8)
            {
                var13 = bottom - top - 8;
            }

            var14 = (int)amountScrolled * (bottom - top - var13) / var19 + top;
            if (var14 < top)
            {
                var14 = top;
            }

            var16.startDrawingQuads();
            var16.setColorRGBA_I(0, 255);
            var16.addVertexWithUV(var5, bottom, 0.0D, 0.0D, 1.0D);
            var16.addVertexWithUV(var6, bottom, 0.0D, 1.0D, 1.0D);
            var16.addVertexWithUV(var6, top, 0.0D, 1.0D, 0.0D);
            var16.addVertexWithUV(var5, top, 0.0D, 0.0D, 0.0D);
            var16.draw();
            var16.startDrawingQuads();
            var16.setColorRGBA_I(8421504, 255);
            var16.addVertexWithUV(var5, var14 + var13, 0.0D, 0.0D, 1.0D);
            var16.addVertexWithUV(var6, var14 + var13, 0.0D, 1.0D, 1.0D);
            var16.addVertexWithUV(var6, var14, 0.0D, 1.0D, 0.0D);
            var16.addVertexWithUV(var5, var14, 0.0D, 0.0D, 0.0D);
            var16.draw();
            var16.startDrawingQuads();
            var16.setColorRGBA_I(12632256, 255);
            var16.addVertexWithUV(var5, var14 + var13 - 1, 0.0D, 0.0D, 1.0D);
            var16.addVertexWithUV(var6 - 1, var14 + var13 - 1, 0.0D, 1.0D, 1.0D);
            var16.addVertexWithUV(var6 - 1, var14, 0.0D, 1.0D, 0.0D);
            var16.addVertexWithUV(var5, var14, 0.0D, 0.0D, 0.0D);
            var16.draw();
        }

        func_27257_b(var1, var2);
        GLManager.GL.Enable(GLEnum.Texture2D);
        GLManager.GL.ShadeModel(GLEnum.Flat);
        GLManager.GL.Enable(GLEnum.AlphaTest);
        GLManager.GL.Disable(GLEnum.Blend);
    }

    private void overlayBackground(int var1, int var2, int var3, int var4)
    {
        Tessellator var5 = Tessellator.instance;
        GLManager.GL.BindTexture(GLEnum.Texture2D, (uint)mc.textureManager.getTextureId("/gui/background.png"));
        GLManager.GL.Color4(1.0F, 1.0F, 1.0F, 1.0F);
        float var6 = 32.0F;
        var5.startDrawingQuads();
        var5.setColorRGBA_I(0x404040, var4);
        var5.addVertexWithUV(0.0D, var2, 0.0D, 0.0D, (double)(var2 / var6));
        var5.addVertexWithUV(width, var2, 0.0D, (double)(width / var6), (double)(var2 / var6));
        var5.setColorRGBA_I(0x404040, var3);
        var5.addVertexWithUV(width, var1, 0.0D, (double)(width / var6), (double)(var1 / var6));
        var5.addVertexWithUV(0.0D, var1, 0.0D, 0.0D, (double)(var1 / var6));
        var5.draw();
    }
}