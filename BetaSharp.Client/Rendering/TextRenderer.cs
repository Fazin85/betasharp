using BetaSharp.Client.Options;
using BetaSharp.Client.Rendering.Core;
using BetaSharp.Util;
using java.awt.image;
using java.io;
using java.lang;
using java.nio;
using javax.imageio;
using Silk.NET.OpenGL.Legacy;

namespace BetaSharp.Client.Rendering;

public class TextRenderer : java.lang.Object
{
    private readonly int[] charWidth = new int[256];
    public int fontTextureName = 0;
    private readonly int fontDisplayLists;
    private readonly IntBuffer buffer;
    private readonly ByteBuffer byteBuffer = GLAllocation.createDirectByteBuffer(1024 * sizeof(int));

    public TextRenderer(GameOptions var1, TextureManager var3)
    {
        buffer = byteBuffer.asIntBuffer();

        BufferedImage var4;
        try
        {
            var4 = ImageIO.read(new ByteArrayInputStream(AssetManager.Instance.getAsset("font/default.png").getBinaryContent()));
        }
        catch (java.io.IOException var18)
        {
            throw new RuntimeException(var18);
        }

        int var5 = var4.getWidth();
        int var6 = var4.getHeight();
        int[] var7 = new int[var5 * var6];
        var4.getRGB(0, 0, var5, var6, var7, 0, var5);

        int var9;
        int var10;
        int var11;
        int var12;
        int var15;
        int var16;
        for (int var8 = 0; var8 < 256; ++var8)
        {
            var9 = var8 % 16;
            var10 = var8 / 16;

            for (var11 = 7; var11 >= 0; --var11)
            {
                var12 = var9 * 8 + var11;
                bool var13 = true;

                for (int var14 = 0; var14 < 8 && var13; ++var14)
                {
                    var15 = (var10 * 8 + var14) * var5;
                    var16 = var7[var12 + var15] & 255;
                    if (var16 > 0)
                    {
                        var13 = false;
                    }
                }

                if (!var13)
                {
                    break;
                }
            }

            if (var8 == 32)
            {
                var11 = 2;
            }

            charWidth[var8] = var11 + 2;
        }

        fontTextureName = var3.load(var4);
        fontDisplayLists = GLAllocation.generateDisplayLists(288);
        Tessellator var19 = Tessellator.instance;

        for (var9 = 0; var9 < 256; ++var9)
        {
            GLManager.GL.NewList((uint)(fontDisplayLists + var9), GLEnum.Compile);
            var19.startDrawingQuads();
            var10 = var9 % 16 * 8;
            var11 = var9 / 16 * 8;
            float var20 = 7.99F;
            float var21 = 0.0F;
            float var23 = 0.0F;
            var19.addVertexWithUV(0.0D, (double)(0.0F + var20), 0.0D, (double)(var10 / 128.0F + var21), (double)((var11 + var20) / 128.0F + var23));
            var19.addVertexWithUV((double)(0.0F + var20), (double)(0.0F + var20), 0.0D, (double)((var10 + var20) / 128.0F + var21), (double)((var11 + var20) / 128.0F + var23));
            var19.addVertexWithUV((double)(0.0F + var20), 0.0D, 0.0D, (double)((var10 + var20) / 128.0F + var21), (double)(var11 / 128.0F + var23));
            var19.addVertexWithUV(0.0D, 0.0D, 0.0D, (double)(var10 / 128.0F + var21), (double)(var11 / 128.0F + var23));
            var19.draw();
            GLManager.GL.Translate(charWidth[var9], 0.0F, 0.0F);
            GLManager.GL.EndList();
        }

        for (var9 = 0; var9 < 32; ++var9)
        {
            var10 = (var9 >> 3 & 1) * 85;
            var11 = (var9 >> 2 & 1) * 170 + var10;
            var12 = (var9 >> 1 & 1) * 170 + var10;
            int var22 = (var9 >> 0 & 1) * 170 + var10;
            if (var9 == 6)
            {
                var11 += 85;
            }

            bool var24 = var9 >= 16;

            if (var24)
            {
                var11 /= 4;
                var12 /= 4;
                var22 /= 4;
            }

            GLManager.GL.NewList((uint)(fontDisplayLists + 256 + var9), GLEnum.Compile);
            GLManager.GL.Color3(var11 / 255.0F, var12 / 255.0F, var22 / 255.0F);
            GLManager.GL.EndList();
        }

    }

    public void drawStringWithShadow(string var1, int var2, int var3, uint color)
    {
        renderString(var1, var2 + 1, var3 + 1, color, true);
        drawString(var1, var2, var3, color);
    }

    public void drawString(string var1, int var2, int var3, uint color)
    {
        renderString(var1, var2, var3, color, false);
    }

    public unsafe void renderString(string var1, int var2, int var3, uint color, bool var5)
    {
        if (var1 != null)
        {
            if (var5)
            {
                uint var6 = color & 0xFF000000;
                color = (color & 0x00FCFCFC) >> 2;
                color += var6;
            }

            GLManager.GL.BindTexture(GLEnum.Texture2D, (uint)fontTextureName);
            float var10 = (color >> 16 & 255) / 255.0F;
            float var7 = (color >> 8 & 255) / 255.0F;
            float var8 = (color & 255) / 255.0F;
            float var9 = (color >> 24 & 255) / 255.0F;
            if (var9 == 0.0F)
            {
                var9 = 1.0F;
            }

            GLManager.GL.Color4(var10, var7, var8, var9);
            buffer.clear();
            GLManager.GL.PushMatrix();
            GLManager.GL.Translate(var2, var3, 0.0F);

            for (int i = 0; i < var1.Length; ++i)
            {
                int var11;
                for (; var1.Length > i + 1 && var1[i] == 167; i += 2)
                {
                    var11 = "0123456789abcdef".IndexOf(var1.ToLower()[i + 1]);
                    if (var11 < 0 || var11 > 15)
                    {
                        var11 = 15;
                    }

                    buffer.put(fontDisplayLists + 256 + var11 + (var5 ? 16 : 0));
                    if (buffer.remaining() == 0)
                    {
                        buffer.flip();
                        CallLists();
                        buffer.clear();
                    }
                }

                if (i < var1.Length)
                {
                    var11 = ChatAllowedCharacters.allowedCharacters.IndexOf(var1[i]);
                    if (var11 >= 0)
                    {
                        buffer.put(fontDisplayLists + var11 + 32);
                    }
                }

                if (buffer.remaining() == 0)
                {
                    buffer.flip();
                    CallLists();
                    buffer.clear();
                }
            }

            buffer.flip();
            CallLists();
            GLManager.GL.PopMatrix();
        }

        void CallLists()
        {
            BufferHelper.UsePointer(byteBuffer, (ptr) =>
            {
                GLManager.GL.CallLists((uint)buffer.remaining(), GLEnum.UnsignedInt, (byte*)ptr);
            });
        }
    }

    public int getStringWidth(string var1)
    {
        if (var1 == null)
        {
            return 0;
        }
        else
        {
            int var2 = 0;

            for (int var3 = 0; var3 < var1.Length; ++var3)
            {
                if (var1[var3] == 167)
                {
                    ++var3;
                }
                else
                {
                    int var4 = ChatAllowedCharacters.allowedCharacters.IndexOf(var1[var3]);
                    if (var4 >= 0)
                    {
                        var2 += charWidth[var4 + 32];
                    }
                }
            }

            return var2;
        }
    }

    public void func_27278_a(string var1, int var2, int var3, int var4, uint color)
    {
        string[] var6 = var1.Split("\n");
        if (var6.Length > 1)
        {
            for (int var11 = 0; var11 < var6.Length; ++var11)
            {
                func_27278_a(var6[var11], var2, var3, var4, color);
                var3 += func_27277_a(var6[var11], var4);
            }

        }
        else
        {
            string[] var7 = var1.Split(" ");
            int var8 = 0;

            while (var8 < var7.Length)
            {
                string var9;
                for (var9 = var7[var8++] + " "; var8 < var7.Length && getStringWidth(var9 + var7[var8]) < var4; var9 = var9 + var7[var8++] + " ")
                {
                }

                int var10;
                for (; getStringWidth(var9) > var4; var9 = var9[var10..])
                {
                    for (var10 = 0; getStringWidth(var9[..(var10 + 1)]) <= var4; ++var10)
                    {
                    }

                    if (var9[..var10].Trim().Length > 0)
                    {
                        drawString(var9[..var10], var2, var3, color);
                        var3 += 8;
                    }
                }

                if (var9.Trim().Length > 0)
                {
                    drawString(var9, var2, var3, color);
                    var3 += 8;
                }
            }

        }
    }

    public int func_27277_a(string var1, int var2)
    {
        string[] var3 = var1.Split("\n");
        int var5;
        if (var3.Length > 1)
        {
            int var9 = 0;

            for (var5 = 0; var5 < var3.Length; ++var5)
            {
                var9 += func_27277_a(var3[var5], var2);
            }

            return var9;
        }
        else
        {
            string[] var4 = var1.Split(" ");
            var5 = 0;
            int var6 = 0;

            while (var5 < var4.Length)
            {
                string var7;
                for (var7 = var4[var5++] + " "; var5 < var4.Length && getStringWidth(var7 + var4[var5]) < var2; var7 = var7 + var4[var5++] + " ")
                {
                }

                int var8;
                for (; getStringWidth(var7) > var2; var7 = var7[var8..])
                {
                    for (var8 = 0; getStringWidth(var7[..(var8 + 1)]) <= var2; ++var8)
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