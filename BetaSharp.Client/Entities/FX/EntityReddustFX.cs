using BetaSharp.Client.Rendering.Core;
using BetaSharp.Worlds.Core;

namespace BetaSharp.Client.Entities.FX;

public class EntityReddustFX : EntityFX
{
    private readonly float baseScale;


    public EntityReddustFX(IBlockWorldContext world, double x, double y, double z, float red, float green, float blue) : this(world, x, y, z, 1.0F, red, green, blue)
    {
    }

    public EntityReddustFX(IBlockWorldContext world, double x, double y, double z, float particleScale, float red, float green, float blue) : base(world, x, y, z, 0.0D, 0.0D, 0.0D)
    {
        velocityX *= 0.1F;
        velocityY *= 0.1F;
        velocityZ *= 0.1F;
        if (red == 0.0F)
        {
            red = 1.0F;
        }

        float colorVariation = (float)Random.Shared.NextDouble() * 0.4F + 0.6F;
        particleRed = ((float)(Random.Shared.NextDouble() * 0.2) + 0.8F) * red * colorVariation;
        particleGreen = ((float)(Random.Shared.NextDouble() * 0.2) + 0.8F) * green * colorVariation;
        particleBlue = ((float)(Random.Shared.NextDouble() * 0.2) + 0.8F) * blue * colorVariation;
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
