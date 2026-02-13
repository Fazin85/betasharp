using BetaSharp.Client.Rendering.Entitys.Models;
using BetaSharp.Entities;
using BetaSharp.Util.Maths;

namespace BetaSharp.Client.Rendering.Entitys;

public class ChickenEntityRenderer : LivingEntityRenderer
{

    public ChickenEntityRenderer(ModelBase var1, float var2) : base(var1, var2)
    {
    }

    public void renderChicken(EntityChicken var1, double var2, double var4, double var6, float var8, float var9)
    {
        base.doRenderLiving(var1, var2, var4, var6, var8, var9);
    }

    protected float getWingRotation(EntityChicken var1, float var2)
    {
        float var3 = var1.field_756_e + (var1.field_752_b - var1.field_756_e) * var2;
        float var4 = var1.field_757_d + (var1.destPos - var1.field_757_d) * var2;
        return (MathHelper.sin(var3) + 1.0F) * var4;
    }

    protected override float func_170_d(EntityLiving var1, float var2)
    {
        return getWingRotation((EntityChicken)var1, var2);
    }

    public override void doRenderLiving(EntityLiving var1, double var2, double var4, double var6, float var8, float var9)
    {
        renderChicken((EntityChicken)var1, var2, var4, var6, var8, var9);
    }

    public override void render(Entity var1, double var2, double var4, double var6, float var8, float var9)
    {
        renderChicken((EntityChicken)var1, var2, var4, var6, var8, var9);
    }
}