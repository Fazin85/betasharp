using BetaSharp.Client.Rendering.Core;
using BetaSharp.Client.Rendering.Entitys.Models;
using BetaSharp.Entities;

namespace BetaSharp.Client.Rendering.Entitys;

public class GiantEntityRenderer : LivingEntityRenderer
{

    private readonly float scale;

    public GiantEntityRenderer(ModelBase var1, float var2, float var3) : base(var1, var2 * var3)
    {
        scale = var3;
    }

    protected void preRenderScale(EntityGiantZombie var1, float var2)
    {
        GLManager.GL.Scale(scale, scale, scale);
    }

    protected override void preRenderCallback(EntityLiving var1, float var2)
    {
        preRenderScale((EntityGiantZombie)var1, var2);
    }
}