using BetaSharp.Client.Rendering.Core;
using BetaSharp.Client.Rendering.Entities;
using BetaSharp.Entities;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds.Core;

namespace BetaSharp.Client.Entities.FX;

public class EntityPickupFX : EntityFX
{
    private readonly int maxAge;
    private readonly Entity source;

    private readonly Entity target;
    private readonly float yOffset;
    private int currentAge;

    public EntityPickupFX(IBlockWorldContext world, Entity target, Entity source, float yOffset) : base(world, target.x, target.y, target.z, target.velocityX, target.velocityY, target.velocityZ)
    {
        this.target = target;
        this.source = source;
        this.yOffset = yOffset;
        maxAge = 3;
    }

    public override void renderParticle(Tessellator t, float partialTick, float rotX, float rotY, float rotZ, float upX, float upZ)
    {
        float lifeProgress = (currentAge + partialTick) / maxAge;
        lifeProgress *= lifeProgress;
        double targetX = target.x;
        double targetY = target.y;
        double targetZ = target.z;
        double sourceX = source.lastTickX + (source.x - source.lastTickX) * partialTick;
        double sourceY = source.lastTickY + (source.y - source.lastTickY) * partialTick + yOffset;
        double sourceZ = source.lastTickZ + (source.z - source.lastTickZ) * partialTick;
        double renderX = targetX + (sourceX - targetX) * lifeProgress;
        double renderY = targetY + (sourceY - targetY) * lifeProgress;
        double renderZ = targetZ + (sourceZ - targetZ) * lifeProgress;
        int itemX = MathHelper.Floor(renderX);
        int itemY = MathHelper.Floor(renderY + standingEyeHeight / 2.0F);
        int itemZ = MathHelper.Floor(renderZ);
        float luminance = _level.Lighting.GetLuminance(itemX, itemY, itemZ);
        renderX -= interpPosX;
        renderY -= interpPosY;
        renderZ -= interpPosZ;
        GLManager.GL.Color4(luminance, luminance, luminance, 1.0F);
        EntityRenderDispatcher.instance.renderEntityWithPosYaw(target, renderX, renderY, renderZ, target.yaw, partialTick);
    }

    public override void tick()
    {
        ++currentAge;
        if (currentAge == maxAge)
        {
            markDead();
        }
    }

    public override int getFXLayer() => 3;
}
