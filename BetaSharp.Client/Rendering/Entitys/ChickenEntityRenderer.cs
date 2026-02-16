using BetaSharp.Client.Rendering.Entitys.Models;
using BetaSharp.Entities;
using BetaSharp.Util.Maths;

namespace BetaSharp.Client.Rendering.Entitys;

public class ChickenEntityRenderer : LivingEntityRenderer
{

    public ChickenEntityRenderer(ModelBase Model, float ShadowSize) : base(Model, ShadowSize)
    {
    }

    public void renderChicken(EntityChicken Chicken, double X, double Y, double Z, float Yaw, float Pitch)
    {
        base.doRenderLiving(Chicken, X, Y, Z, Yaw, Pitch);
    }

    protected float getWingRotation(EntityChicken Chicken, float PartialTick)
    {
        float InterpolatedWingRotation = Chicken.PreviousWingRotation + (Chicken.WingRotation - Chicken.PreviousWingRotation) * PartialTick;
        float InterpolatedDestPos = Chicken.PreviousDestPos + (Chicken.DestPos - Chicken.PreviousDestPos) * PartialTick;
        return (MathHelper.sin(InterpolatedWingRotation) + 1.0F) * InterpolatedDestPos;
    }

    protected override float func_170_d(EntityLiving LivingEntity, float PartialTick)
    {
        return getWingRotation((EntityChicken)LivingEntity, PartialTick);
    }

    public override void doRenderLiving(EntityLiving LivingEntity, double X, double Y, double Z, float Yaw, float Pitch)
    {
        renderChicken((EntityChicken)LivingEntity, X, Y, Z, Yaw, Pitch);
    }

    public override void render(Entity Chicken, double X, double Y, double Z, float Yaw, float Pitch)
    {
        renderChicken((EntityChicken)Chicken, X, Y, Z, Yaw, Pitch);
    }
}