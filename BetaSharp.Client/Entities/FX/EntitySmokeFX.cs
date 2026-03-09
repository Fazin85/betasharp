using BetaSharp.Client.Rendering.Core;
using BetaSharp.Worlds.Core;

namespace BetaSharp.Client.Entities.FX;

public class EntitySmokeFX : EntityFX
{
    private readonly float baseScale;


    public EntitySmokeFX(IBlockWorldContext world, double x, double y, double z, double velocityX, double velocityY, double velocityZ) : this(world, x, y, z, velocityX, velocityY, velocityZ, 1.0F)
    {
    }

    public EntitySmokeFX(IBlockWorldContext world, double x, double y, double z, double velocityX, double velocityY, double velocityZ, float particleScale) : base(world, x, y, z, 0.0D, 0.0D, 0.0D)
    {
        this.velocityX *= 0.1F;
        this.velocityY *= 0.1F;
        this.velocityZ *= 0.1F;
        this.velocityX += velocityX;
        this.velocityY += velocityY;
        this.velocityZ += velocityZ;
        particleRed = particleGreen = particleBlue = (float)(Random.Shared.NextDouble() * 0.3F);
        this.particleScale *= 12.0F / 16.0F;
        this.particleScale *= particleScale;
        baseScale = this.particleScale;
        particleMaxAge = (int)(8.0D / (Random.Shared.NextDouble() * 0.8D + 0.2D));
        particleMaxAge = (int)(particleMaxAge * particleScale);
        noClip = false;
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

        particleTextureIndex = 7 - particleAge * 8 / particleMaxAge;
        velocityY += 0.004D;
        move(velocityX, velocityY, velocityZ);
        if (y == prevY)
        {
            velocityX *= 1.1D;
            velocityZ *= 1.1D;
        }

        velocityX *= 0.96F;
        velocityY *= 0.96F;
        velocityZ *= 0.96F;
        if (onGround)
        {
            velocityX *= 0.7F;
            velocityZ *= 0.7F;
        }
    }
}
