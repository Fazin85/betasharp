using BetaSharp.Client.Rendering.Core;
using BetaSharp.Client.Rendering.Entitys.Models;
using BetaSharp.Entities;
using BetaSharp.Util.Maths;

namespace BetaSharp.Client.Rendering.Entitys;

public class BoatEntityRenderer : EntityRenderer
{

    protected ModelBase modelBoat;

    public BoatEntityRenderer()
    {
        shadowRadius = 0.5F;
        modelBoat = new ModelBoat();
    }

    public void render(EntityBoat var1, double var2, double var4, double var6, float var8, float var9)
    {
        GLManager.GL.PushMatrix();
        GLManager.GL.Translate((float)var2, (float)var4, (float)var6);
        GLManager.GL.Rotate(180.0F - var8, 0.0F, 1.0F, 0.0F);
        float var10 = var1.boatTimeSinceHit - var9;
        float var11 = var1.boatCurrentDamage - var9;
        if (var11 < 0.0F)
        {
            var11 = 0.0F;
        }

        if (var10 > 0.0F)
        {
            GLManager.GL.Rotate(MathHelper.sin(var10) * var10 * var11 / 10.0F * var1.boatRockDirection, 1.0F, 0.0F, 0.0F);
        }

        loadTexture("/terrain.png");
        float var12 = 12.0F / 16.0F;
        GLManager.GL.Scale(var12, var12, var12);
        GLManager.GL.Scale(1.0F / var12, 1.0F / var12, 1.0F / var12);
        loadTexture("/item/boat.png");
        GLManager.GL.Scale(-1.0F, -1.0F, 1.0F);
        modelBoat.render(0.0F, 0.0F, -0.1F, 0.0F, 0.0F, 1.0F / 16.0F);
        GLManager.GL.PopMatrix();
    }

    public override void render(Entity var1, double var2, double var4, double var6, float var8, float var9)
    {
        render((EntityBoat)var1, var2, var4, var6, var8, var9);
    }
}