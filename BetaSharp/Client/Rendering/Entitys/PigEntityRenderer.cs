using BetaSharp.Client.Rendering.Entitys.Models;
using BetaSharp.Entities;

namespace BetaSharp.Client.Rendering.Entitys;

public class PigEntityRenderer : LivingEntityRenderer
{

    public PigEntityRenderer(ModelBase var1, ModelBase var2, float var3) : base(var1, var3)
    {
        setRenderPassModel(var2);
    }

    protected bool renderSaddledPig(EntityPig var1, int var2, float var3)
    {
        loadTexture("/mob/saddle.png");
        return var2 == 0 && var1.getSaddled();
    }

    protected override bool shouldRenderPass(EntityLiving var1, int var2, float var3)
    {
        return renderSaddledPig((EntityPig)var1, var2, var3);
    }
}