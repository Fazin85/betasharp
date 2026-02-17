using BetaSharp.Client.Rendering.Core;
using BetaSharp.Entities;
using BetaSharp.Items;
using Silk.NET.OpenGL.Legacy;

namespace BetaSharp.Client.Rendering.Entitys;

public class FireballEntityRenderer : EntityRenderer
{

    public void render(EntityFireball var1, double var2, double var4, double var6, float var8, float var9)
    {
        GLManager.GL.PushMatrix();
        GLManager.GL.Translate((float)var2, (float)var4, (float)var6);
        GLManager.GL.Enable(GLEnum.RescaleNormal);
        float var10 = 2.0F;
        GLManager.GL.Scale(var10 / 1.0F, var10 / 1.0F, var10 / 1.0F);
        int var11 = Item.Snowball.getTextureId(0);
        loadTexture("/gui/items.png");
        Tessellator var12 = Tessellator.instance;
        float var13 = (var11 % 16 * 16 + 0) / 256.0F;
        float var14 = (var11 % 16 * 16 + 16) / 256.0F;
        float var15 = (var11 / 16 * 16 + 0) / 256.0F;
        float var16 = (var11 / 16 * 16 + 16) / 256.0F;
        float var17 = 1.0F;
        float var18 = 0.5F;
        float var19 = 0.25F;
        GLManager.GL.Rotate(180.0F - dispatcher.playerViewY, 0.0F, 1.0F, 0.0F);
        GLManager.GL.Rotate(-dispatcher.playerViewX, 1.0F, 0.0F, 0.0F);
        var12.startDrawingQuads();
        var12.setNormal(0.0F, 1.0F, 0.0F);
        var12.addVertexWithUV((double)(0.0F - var18), (double)(0.0F - var19), 0.0D, (double)var13, (double)var16);
        var12.addVertexWithUV((double)(var17 - var18), (double)(0.0F - var19), 0.0D, (double)var14, (double)var16);
        var12.addVertexWithUV((double)(var17 - var18), (double)(1.0F - var19), 0.0D, (double)var14, (double)var15);
        var12.addVertexWithUV((double)(0.0F - var18), (double)(1.0F - var19), 0.0D, (double)var13, (double)var15);
        var12.draw();
        GLManager.GL.Disable(GLEnum.RescaleNormal);
        GLManager.GL.PopMatrix();
    }

    public override void render(Entity var1, double var2, double var4, double var6, float var8, float var9)
    {
        render((EntityFireball)var1, var2, var4, var6, var8, var9);
    }
}