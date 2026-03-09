using BetaSharp.Blocks;
using BetaSharp.Blocks.Materials;
using BetaSharp.Client.Rendering.Core;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds.Core;

namespace BetaSharp.Client.Entities.FX;

public class EntityRainFX : EntityFX
{
    public EntityRainFX(IBlockWorldContext world, double x, double y, double z) : base(world, x, y, z, 0.0D, 0.0D, 0.0D)
    {
        velocityX *= 0.3F;
        velocityY = Random.Shared.NextDouble() * 0.2F + 0.1F;
        velocityZ *= 0.3F;
        particleRed = 1.0F;
        particleGreen = 1.0F;
        particleBlue = 1.0F;
        particleTextureIndex = 19 + random.NextInt(4);
        setBoundingBoxSpacing(0.01F, 0.01F);
        particleGravity = 0.06F;
        particleMaxAge = (int)(8.0D / (Random.Shared.NextDouble() * 0.8D + 0.2D));
    }

    public override void renderParticle(Tessellator t, float partialTick, float rotX, float rotY, float rotZ, float upX, float upZ) => base.renderParticle(t, partialTick, rotX, rotY, rotZ, upX, upZ);

    public override void tick()
    {
        prevX = x;
        prevY = y;
        prevZ = z;
        velocityY -= particleGravity;
        move(velocityX, velocityY, velocityZ);
        velocityX *= 0.98F;
        velocityY *= 0.98F;
        velocityZ *= 0.98F;
        if (particleMaxAge-- <= 0)
        {
            markDead();
        }

        if (onGround)
        {
            if (Random.Shared.NextDouble() < 0.5D)
            {
                markDead();
            }

            velocityX *= 0.7F;
            velocityZ *= 0.7F;
        }

        Material material = _level.BlocksReader.GetMaterial(MathHelper.Floor(x), MathHelper.Floor(y), MathHelper.Floor(z));
        if (material.IsFluid || material.IsSolid)
        {
            double height = MathHelper.Floor(y) + 1 - BlockFluid.getFluidHeightFromMeta(_level.BlocksReader.GetMeta(MathHelper.Floor(x), MathHelper.Floor(y), MathHelper.Floor(z)));
            if (y < height)
            {
                markDead();
            }
        }
    }
}
