using BetaSharp.Client.Rendering.Core;
using BetaSharp.Worlds.Core;

namespace BetaSharp.Client.Entities.FX;

public class EntityLavaFX : EntityFX
{
    private readonly float baseScale;

    public EntityLavaFX(IBlockWorldContext world, double x, double y, double z) : base(world, x, y, z, 0.0D, 0.0D, 0.0D)
    {
        velocityX *= 0.8F;
        velocityY *= 0.8F;
        velocityZ *= 0.8F;
        velocityY = random.NextFloat() * 0.4F + 0.05F;
        particleRed = particleGreen = particleBlue = 1.0F;
        particleScale *= random.NextFloat() * 2.0F + 0.2F;
        baseScale = particleScale;
        particleMaxAge = (int)(16.0 / (Random.Shared.NextDouble() * 0.8 + 0.2));
        noClip = false;
        particleTextureIndex = 49;
    }

    public override float getBrightnessAtEyes(float brightness) => 1.0F;

    public override void renderParticle(Tessellator t, float partialTick, float rotX, float rotY, float rotZ, float upX, float upZ)
    {
        float lifeProgress = (particleAge + partialTick) / particleMaxAge;
        particleScale = baseScale * (1.0F - lifeProgress * lifeProgress);
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

        float lifeProgress = particleAge / particleMaxAge;
        if (random.NextFloat() > lifeProgress)
        {
            _level.Broadcaster.AddParticle("smoke", x, y, z, velocityX, velocityY, velocityZ);
        }

        velocityY -= 0.03D;
        move(velocityX, velocityY, velocityZ);
        velocityX *= 0.999F;
        velocityY *= 0.999F;
        velocityZ *= 0.999F;
        if (onGround)
        {
            velocityX *= 0.7F;
            velocityZ *= 0.7F;
        }
    }
}
