using BetaSharp.Client.Rendering.Core;
using BetaSharp.Worlds.Core;

namespace BetaSharp.Client.Entities.FX;

public class EntityFlameFX : EntityFX
{
    private readonly float baseScale;

    public EntityFlameFX(IBlockWorldContext world, double x, double y, double z, double velocityX, double velocityY, double velocityZ) : base(world, x, y, z, velocityX, velocityY, velocityZ)
    {
        this.velocityX = this.velocityX * 0.01 + velocityX;
        this.velocityY = this.velocityY * 0.01 + velocityY;
        this.velocityZ = this.velocityZ * 0.01 + velocityZ;
        baseScale = particleScale;
        particleRed = particleGreen = particleBlue = 1.0F;
        particleMaxAge = (int)(8.0 / (Random.Shared.NextDouble() * 0.8 + 0.2)) + 4;
        noClip = true;
        particleTextureIndex = 48;
    }

    public override void renderParticle(Tessellator t, float partialTick, float rotX, float rotY, float rotZ, float upX, float upZ)
    {
        float lifeProgress = (particleAge + partialTick) / particleMaxAge;
        particleScale = baseScale * (1.0F - lifeProgress * lifeProgress * 0.5F);
        base.renderParticle(t, partialTick, rotX, rotY, rotZ, upX, upZ);
    }

    public override float getBrightnessAtEyes(float partialTick)
    {
        float lifeProgress = (particleAge + partialTick) / particleMaxAge;
        if (lifeProgress < 0.0F)
        {
            lifeProgress = 0.0F;
        }

        if (lifeProgress > 1.0F)
        {
            lifeProgress = 1.0F;
        }

        float baseBrightness = base.getBrightnessAtEyes(partialTick);
        return baseBrightness * lifeProgress + (1.0F - lifeProgress);
    }

    public override void tick()
    {
        prevX = x;
        prevY = y;
        prevZ = z;
        if (particleAge++ >= particleMaxAge)
        {
            markDead();
        }

        move(velocityX, velocityY, velocityZ);
        velocityX *= 0.96;
        velocityY *= 0.96;
        velocityZ *= 0.96;
        if (onGround)
        {
            velocityX *= 0.7;
            velocityZ *= 0.7;
        }
    }
}
