using BetaSharp.Client.Rendering.Core;
using BetaSharp.Worlds.Core;

namespace BetaSharp.Client.Entities.FX;

public class EntityHeartFX : EntityFX
{
    private readonly float baseScale;


    public EntityHeartFX(IBlockWorldContext world, double x, double y, double z, double motionX, double motionY, double motionZ) : this(world, x, y, z, motionX, motionY, motionZ, 2.0F)
    {
    }

    public EntityHeartFX(IBlockWorldContext world, double x, double y, double z, double motionX, double motionY, double motionZ, float particleScale) : base(world, x, y, z, 0.0D, 0.0D, 0.0D)
    {
        velocityX *= 0.01;
        velocityY *= 0.01;
        velocityZ *= 0.01;
        velocityY += 0.1D;
        this.particleScale *= 12.0F / 16.0F;
        this.particleScale *= particleScale;
        baseScale = this.particleScale;
        particleMaxAge = 16;
        noClip = false;
        particleTextureIndex = 80;
    }

    public override void renderParticle(Tessellator t, float partialTick, float rotX, float rotY, float rotZ, float upX, float upZ)
    {
        float lifeProgress = (particleAge + partialTick) / particleMaxAge * 32.0F;
        if (lifeProgress < 0.0F)
        {
            lifeProgress = 0.0F;
        }

        if (lifeProgress > 1.0F)
        {
            lifeProgress = 1.0F;
        }

        particleScale = baseScale * lifeProgress;
        base.renderParticle(t, partialTick, rotX, rotY, rotZ, upX, upZ);
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
        if (y == prevY)
        {
            velocityX *= 1.1D;
            velocityZ *= 1.1D;
        }

        velocityX *= 0.86F;
        velocityY *= 0.86F;
        velocityZ *= 0.86F;
        if (onGround)
        {
            velocityX *= 0.7F;
            velocityZ *= 0.7F;
        }
    }
}
