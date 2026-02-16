using BetaSharp.Client.Rendering.Core;
using BetaSharp.Entities;
using Silk.NET.OpenGL.Legacy;

namespace BetaSharp.Client.Rendering.Entitys;

public class ProjectileEntityRenderer : EntityRenderer
{

    private readonly int itemIconIndex;

    public ProjectileEntityRenderer(int var1)
    {
        itemIconIndex = var1;
    }

    public override void render(Entity var1, double var2, double var4, double var6, float var8, float var9)
    {
        GLManager.GL.PushMatrix();
        GLManager.GL.Translate((float)var2, (float)var4, (float)var6);
        GLManager.GL.Enable(GLEnum.RescaleNormal);
        GLManager.GL.Scale(0.5F, 0.5F, 0.5F);
        loadTexture("/gui/items.png");
        Tessellator var10 = Tessellator.instance;
        float var11 = (itemIconIndex % 16 * 16 + 0) / 256.0F;
        float var12 = (itemIconIndex % 16 * 16 + 16) / 256.0F;
        float var13 = (itemIconIndex / 16 * 16 + 0) / 256.0F;
        float var14 = (itemIconIndex / 16 * 16 + 16) / 256.0F;
        float var15 = 1.0F;
        float var16 = 0.5F;
        float var17 = 0.25F;
        GLManager.GL.Rotate(180.0F - dispatcher.playerViewY, 0.0F, 1.0F, 0.0F);
        GLManager.GL.Rotate(-dispatcher.playerViewX, 1.0F, 0.0F, 0.0F);
        var10.startDrawingQuads();
        var10.setNormal(0.0F, 1.0F, 0.0F);
        var10.addVertexWithUV((double)(0.0F - var16), (double)(0.0F - var17), 0.0D, (double)var11, (double)var14);
        var10.addVertexWithUV((double)(var15 - var16), (double)(0.0F - var17), 0.0D, (double)var12, (double)var14);
        var10.addVertexWithUV((double)(var15 - var16), (double)(1.0F - var17), 0.0D, (double)var12, (double)var13);
        var10.addVertexWithUV((double)(0.0F - var16), (double)(1.0F - var17), 0.0D, (double)var11, (double)var13);
        var10.draw();
        GLManager.GL.Disable(GLEnum.RescaleNormal);
        GLManager.GL.PopMatrix();
    }
}