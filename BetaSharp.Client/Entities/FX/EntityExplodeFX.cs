using BetaSharp.Client.Rendering.Core;
using BetaSharp.Worlds.Core;

namespace BetaSharp.Client.Entities.FX;

public class EntityExplodeFX : EntityFX
{
    public EntityExplodeFX(IBlockWorldContext world, double x, double y, double z, double velocityX, double velocityY, double velocityZ) : base(world, x, y, z, velocityX, velocityY, velocityZ)
    {
        this.velocityX = velocityX + (Random.Shared.NextDouble() * 2.0D - 1.0D) * 0.05F;
        this.velocityY = velocityY + (Random.Shared.NextDouble() * 2.0D - 1.0D) * 0.05F;
        this.velocityZ = velocityZ + (Random.Shared.NextDouble() * 2.0D - 1.0D) * 0.05F;
        particleRed = particleGreen = particleBlue = random.NextFloat() * 0.3F + 0.7F;
        particleScale = random.NextFloat() * random.NextFloat() * 6.0F + 1.0F;
        particleMaxAge = (int)(16.0 / (random.NextFloat() * 0.8 + 0.2)) + 2;
    }

    public override void renderParticle(Tessellator t, float partialTick, float rotX, float rotY, float rotZ, float upX, float upZ) => base.renderParticle(t, partialTick, rotX, rotY, rotZ, upX, upZ);

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
        velocityY += 0.004;
        move(velocityX, velocityY, velocityZ);
        velocityX *= 0.9;
        velocityY *= 0.9;
        velocityZ *= 0.9;
        if (onGround)
        {
            velocityX *= 0.7;
            velocityZ *= 0.7;
        }
    }
}
