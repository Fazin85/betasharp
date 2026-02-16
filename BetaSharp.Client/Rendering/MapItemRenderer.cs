using BetaSharp.Client.Options;
using BetaSharp.Client.Rendering.Core;
using BetaSharp.Entities;
using java.awt.image;
using java.util;
using Silk.NET.OpenGL.Legacy;

namespace BetaSharp.Client.Rendering;

public class MapItemRenderer
{
    private readonly int[] colors = new int[128*128];
    private readonly int _textureId;
    private readonly GameOptions _options;
    private readonly TextRenderer _textRenderer;

    public MapItemRenderer(TextRenderer textRenderer, GameOptions options, TextureManager textureManager)
    {
        _options = options;
        _textRenderer = textRenderer;
        _textureId = textureManager.load(new BufferedImage(128, 128, 2));

        for (int i = 0; i < 128*128; ++i)
        {
            colors[i] = 0;
        }

    }

    public void render(EntityPlayer player, TextureManager textureManager, MapState mapState)
    {
        for (int i = 0; i < 128*128; ++i)
        {
            byte color = mapState.colors[i];
            if (color / 4 == 0)
            {
                // render translucent checkerboard pattern for transparent pixels
                colors[i] = (i + i / 128 & 1) * 8 + 16 << 24;
            }
            else
            {
                uint var6 = MapColor.mapColorArray[color / 4].colorValue;
                int var7 = color & 3;
                byte var8 = 220;
                if (var7 == 2)
                {
                    var8 = 255;
                }

                if (var7 == 0)
                {
                    var8 = 180;
                }

                uint var9 = (var6 >> 16 & 255) * var8 / 255;
                uint var10 = (var6 >> 8 & 255) * var8 / 255;
                uint var11 = (var6 & 255) * var8 / 255;

                colors[i] = unchecked((int)(0xFF000000u | var9 << 16 | var10 << 8 | var11));
            }
        }

        textureManager.bind(colors, 128, 128, _textureId);
        byte var15 = 0;
        byte var16 = 0;
        Tessellator var17 = Tessellator.instance;
        float var18 = 0.0F;
        GLManager.GL.BindTexture(GLEnum.Texture2D, (uint)_textureId);
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
        textureManager.bindTexture(textureManager.getTextureId("/misc/mapicons.png"));
        Iterator var19 = mapState.icons.iterator();

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
        _textRenderer.drawString(mapState.id, var15, var16, 0xFF000000);
        GLManager.GL.PopMatrix();
    }
}
