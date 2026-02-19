using BetaSharp.Client.Rendering.Core;
using BetaSharp.Entities;
using BetaSharp.Util.Maths;
using Silk.NET.OpenGL.Legacy;

namespace BetaSharp.Client.Rendering.Entities;

public class PaintingEntityRenderer : EntityRenderer
{
    private readonly JavaRandom rand = new();

    public void func_158_a(EntityPainting var1, double var2, double var4, double var6, float var8, float var9)
    {
        rand.SetSeed(187L);
        GLManager.GL.PushMatrix();
        GLManager.GL.Translate((float)var2, (float)var4, (float)var6);
        GLManager.GL.Rotate(var8, 0.0F, 1.0F, 0.0F);
        GLManager.GL.Enable(GLEnum.RescaleNormal);
        loadTexture("/art/kz.png");
        EnumArt var10 = var1.art;
        float var11 = 1.0F / 16.0F;
        GLManager.GL.Scale(var11, var11, var11);
        func_159_a(var1, var10.sizeX, var10.sizeY, var10.offsetX, var10.offsetY);
        GLManager.GL.Disable(GLEnum.RescaleNormal);
        GLManager.GL.PopMatrix();
    }

    private void func_159_a(EntityPainting var1, int var2, int var3, int var4, int var5)
    {
        float var6 = -var2 / 2.0F;
        float var7 = -var3 / 2.0F;
        float var8 = -0.5F;
        float var9 = 0.5F;

        for (int var10 = 0; var10 < var2 / 16; ++var10)
        {
            for (int var11 = 0; var11 < var3 / 16; ++var11)
            {
                float var12 = var6 + (var10 + 1) * 16;
                float var13 = var6 + var10 * 16;
                float var14 = var7 + (var11 + 1) * 16;
                float var15 = var7 + var11 * 16;
                func_160_a(var1, (var12 + var13) / 2.0F, (var14 + var15) / 2.0F);
                float var16 = (var4 + var2 - var10 * 16) / 256.0F;
                float var17 = (var4 + var2 - (var10 + 1) * 16) / 256.0F;
                float var18 = (var5 + var3 - var11 * 16) / 256.0F;
                float var19 = (var5 + var3 - (var11 + 1) * 16) / 256.0F;
                float var20 = 12.0F / 16.0F;
                float var21 = 13.0F / 16.0F;
                float var22 = 0.0F;
                float var23 = 1.0F / 16.0F;
                float var24 = 12.0F / 16.0F;
                float var25 = 13.0F / 16.0F;
                float var26 = (1 / 512f);
                float var27 = (1 / 512f);
                float var28 = 385.0F / 512.0F;
                float var29 = 385.0F / 512.0F;
                float var30 = 0.0F;
                float var31 = 1.0F / 16.0F;
                Tessellator var32 = Tessellator.instance;
                var32.startDrawingQuads();
                var32.setNormal(0.0F, 0.0F, -1.0F);
                var32.addVertexWithUV((double)var12, (double)var15, (double)var8, (double)var17, (double)var18);
                var32.addVertexWithUV((double)var13, (double)var15, (double)var8, (double)var16, (double)var18);
                var32.addVertexWithUV((double)var13, (double)var14, (double)var8, (double)var16, (double)var19);
                var32.addVertexWithUV((double)var12, (double)var14, (double)var8, (double)var17, (double)var19);
                var32.setNormal(0.0F, 0.0F, 1.0F);
                var32.addVertexWithUV((double)var12, (double)var14, (double)var9, (double)var20, (double)var22);
                var32.addVertexWithUV((double)var13, (double)var14, (double)var9, (double)var21, (double)var22);
                var32.addVertexWithUV((double)var13, (double)var15, (double)var9, (double)var21, (double)var23);
                var32.addVertexWithUV((double)var12, (double)var15, (double)var9, (double)var20, (double)var23);
                var32.setNormal(0.0F, -1.0F, 0.0F);
                var32.addVertexWithUV((double)var12, (double)var14, (double)var8, (double)var24, (double)var26);
                var32.addVertexWithUV((double)var13, (double)var14, (double)var8, (double)var25, (double)var26);
                var32.addVertexWithUV((double)var13, (double)var14, (double)var9, (double)var25, (double)var27);
                var32.addVertexWithUV((double)var12, (double)var14, (double)var9, (double)var24, (double)var27);
                var32.setNormal(0.0F, 1.0F, 0.0F);
                var32.addVertexWithUV((double)var12, (double)var15, (double)var9, (double)var24, (double)var26);
                var32.addVertexWithUV((double)var13, (double)var15, (double)var9, (double)var25, (double)var26);
                var32.addVertexWithUV((double)var13, (double)var15, (double)var8, (double)var25, (double)var27);
                var32.addVertexWithUV((double)var12, (double)var15, (double)var8, (double)var24, (double)var27);
                var32.setNormal(-1.0F, 0.0F, 0.0F);
                var32.addVertexWithUV((double)var12, (double)var14, (double)var9, (double)var29, (double)var30);
                var32.addVertexWithUV((double)var12, (double)var15, (double)var9, (double)var29, (double)var31);
                var32.addVertexWithUV((double)var12, (double)var15, (double)var8, (double)var28, (double)var31);
                var32.addVertexWithUV((double)var12, (double)var14, (double)var8, (double)var28, (double)var30);
                var32.setNormal(1.0F, 0.0F, 0.0F);
                var32.addVertexWithUV((double)var13, (double)var14, (double)var8, (double)var29, (double)var30);
                var32.addVertexWithUV((double)var13, (double)var15, (double)var8, (double)var29, (double)var31);
                var32.addVertexWithUV((double)var13, (double)var15, (double)var9, (double)var28, (double)var31);
                var32.addVertexWithUV((double)var13, (double)var14, (double)var9, (double)var28, (double)var30);
                var32.draw();
            }
        }

    }

    private void func_160_a(EntityPainting var1, float var2, float var3)
    {
        int var4 = MathHelper.floor_double(var1.x);
        int var5 = MathHelper.floor_double(var1.y + (double)(var3 / 16.0F));
        int var6 = MathHelper.floor_double(var1.z);
        if (var1.direction == 0)
        {
            var4 = MathHelper.floor_double(var1.x + (double)(var2 / 16.0F));
        }

        if (var1.direction == 1)
        {
            var6 = MathHelper.floor_double(var1.z - (double)(var2 / 16.0F));
        }

        if (var1.direction == 2)
        {
            var4 = MathHelper.floor_double(var1.x - (double)(var2 / 16.0F));
        }

        if (var1.direction == 3)
        {
            var6 = MathHelper.floor_double(var1.z + (double)(var2 / 16.0F));
        }

        float var7 = dispatcher.world.getLuminance(var4, var5, var6);
        GLManager.GL.Color3(var7, var7, var7);
    }

    public override void render(Entity target, double x, double y, double z, float yaw, float tickDelta)
    {
        func_158_a((EntityPainting)target, x, y, z, yaw, tickDelta);
    }
}