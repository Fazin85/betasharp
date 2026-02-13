using BetaSharp.Client;
using BetaSharp.Client.Rendering.Core;
using BetaSharp.Entities;
using java.awt.image;
using java.util;
using Silk.NET.OpenGL.Legacy;

namespace BetaSharp;

public class MapItemRenderer
{
    private int[] field_28159_a = new int[16384];
    private int field_28158_b;
    private GameOptions field_28161_c;
    private TextRenderer field_28160_d;

    public MapItemRenderer(TextRenderer var1, GameOptions var2, TextureManager var3)
    {
        field_28161_c = var2;
        field_28160_d = var1;
        field_28158_b = var3.load(new BufferedImage(128, 128, 2));

        for (int var4 = 0; var4 < 16384; ++var4)
        {
            field_28159_a[var4] = 0;
        }

    }

    public void func_28157_a(EntityPlayer var1, TextureManager var2, MapState var3)
    {
        for (int var4 = 0; var4 < 16384; ++var4)
        {
            byte var5 = var3.colors[var4];
            if (var5 / 4 == 0)
            {
                field_28159_a[var4] = (var4 + var4 / 128 & 1) * 8 + 16 << 24;
            }
            else
            {
                int var6 = MapColor.mapColorArray[var5 / 4].colorValue;
                int var7 = var5 & 3;
                short var8 = 220;
                if (var7 == 2)
                {
                    var8 = 255;
                }

                if (var7 == 0)
                {
                    var8 = 180;
                }

                int var9 = (var6 >> 16 & 255) * var8 / 255;
                int var10 = (var6 >> 8 & 255) * var8 / 255;
                int var11 = (var6 & 255) * var8 / 255;

                field_28159_a[var4] = -16777216 | var9 << 16 | var10 << 8 | var11;
            }
        }

        var2.bind(field_28159_a, 128, 128, field_28158_b);
        byte var15 = 0;
        byte var16 = 0;
        Tessellator var17 = Tessellator.instance;
        float var18 = 0.0F;
        GLManager.GL.BindTexture(GLEnum.Texture2D, (uint)field_28158_b);
        GLManager.GL.Enable(GLEnum.Blend);
        GLManager.GL.Disable(GLEnum.AlphaTest);
        var17.startDrawingQuads();
        var17.addVertexWithUV((double)((float)(var15 + 0) + var18), (double)((float)(var16 + 128) - var18), (double)-0.01F, 0.0D, 1.0D);
        var17.addVertexWithUV((double)((float)(var15 + 128) - var18), (double)((float)(var16 + 128) - var18), (double)-0.01F, 1.0D, 1.0D);
        var17.addVertexWithUV((double)((float)(var15 + 128) - var18), (double)((float)(var16 + 0) + var18), (double)-0.01F, 1.0D, 0.0D);
        var17.addVertexWithUV((double)((float)(var15 + 0) + var18), (double)((float)(var16 + 0) + var18), (double)-0.01F, 0.0D, 0.0D);
        var17.draw();
        GLManager.GL.Enable(GLEnum.AlphaTest);
        GLManager.GL.Disable(GLEnum.Blend);
        var2.bindTexture(var2.getTextureId("/misc/mapicons.png"));
        Iterator var19 = var3.icons.iterator();

        while (var19.hasNext())
        {
            MapCoord var20 = (MapCoord)var19.next();
            GLManager.GL.PushMatrix();
            GLManager.GL.Translate((float)var15 + (float)var20.x / 2.0F + 64.0F, (float)var16 + (float)var20.z / 2.0F + 64.0F, -0.02F);
            GLManager.GL.Rotate((float)(var20.rotation * 360) / 16.0F, 0.0F, 0.0F, 1.0F);
            GLManager.GL.Scale(4.0F, 4.0F, 3.0F);
            GLManager.GL.Translate(-(2.0F / 16.0F), 2.0F / 16.0F, 0.0F);
            float var21 = (float)(var20.type % 4 + 0) / 4.0F;
            float var22 = (float)(var20.type / 4 + 0) / 4.0F;
            float var23 = (float)(var20.type % 4 + 1) / 4.0F;
            float var24 = (float)(var20.type / 4 + 1) / 4.0F;
            var17.startDrawingQuads();
            var17.addVertexWithUV(-1.0D, 1.0D, 0.0D, (double)var21, (double)var22);
            var17.addVertexWithUV(1.0D, 1.0D, 0.0D, (double)var23, (double)var22);
            var17.addVertexWithUV(1.0D, -1.0D, 0.0D, (double)var23, (double)var24);
            var17.addVertexWithUV(-1.0D, -1.0D, 0.0D, (double)var21, (double)var24);
            var17.draw();
            GLManager.GL.PopMatrix();
        }

        GLManager.GL.PushMatrix();
        GLManager.GL.Translate(0.0F, 0.0F, -0.04F);
        GLManager.GL.Scale(1.0F, 1.0F, 1.0F);
        field_28160_d.drawString(var3.id, var15, var16, -16777216);
        GLManager.GL.PopMatrix();
    }
}