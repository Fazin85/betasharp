using BetaSharp.Client.Rendering.Core;
using BetaSharp.Worlds.Core;

namespace BetaSharp.Client.Entities.FX;

public class EntitySnowShovelFX : EntityFX
{
    private readonly float baseScale;


    public EntitySnowShovelFX(IBlockWorldContext world, double x, double y, double z, double velocityX, double velocityY, double velocityZ) : this(world, x, y, z, velocityX, velocityY, velocityZ, 1.0F)
    {
    }

    public EntitySnowShovelFX(IBlockWorldContext world, double x, double y, double z, double velocityX, double velocityY, double velocityZ, float scaleMultiplier) : base(world, x, y, z, velocityX, velocityY, velocityZ)
    {
        this.velocityX *= 0.1F;
        this.velocityY *= 0.1F;
        this.velocityZ *= 0.1F;
        this.velocityX += velocityX;
        this.velocityY += velocityY;
        this.velocityZ += velocityZ;
        particleRed = particleGreen = particleBlue = 1.0F - (float)(Random.Shared.NextDouble() * 0.3F);
        particleScale *= 12.0F / 16.0F;
        particleScale *= scaleMultiplier;
        baseScale = particleScale;
        particleMaxAge = (int)(8.0D / (Random.Shared.NextDouble() * 0.8D + 0.2D));
        particleMaxAge = (int)(particleMaxAge * scaleMultiplier);
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
        velocityY -= 0.03D;
        move(velocityX, velocityY, velocityZ);
        velocityX *= 0.99F;
        velocityY *= 0.99F;
        velocityZ *= 0.99F;
        if (onGround)
        {
            velocityX *= 0.7F;
            velocityZ *= 0.7F;
        }
    }
}
