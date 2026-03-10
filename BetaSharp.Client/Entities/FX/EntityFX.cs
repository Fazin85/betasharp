using BetaSharp.Client.Rendering.Core;
using BetaSharp.Entities;
using BetaSharp.NBT;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds.Core;

namespace BetaSharp.Client.Entities.FX;

public class EntityFX : Entity
{
    public static double interpPosX;
    public static double interpPosY;
    public static double interpPosZ;
    protected int particleAge;
    protected float particleBlue;
    protected float particleGravity;
    protected float particleGreen;
    protected int particleMaxAge;
    protected float particleRed;
    protected float particleScale;

    protected int particleTextureIndex;
    protected float particleTextureJitterX;
    protected float particleTextureJitterY;

    public EntityFX(IBlockWorldContext world, double x, double y, double z, double velocityX, double velocityY, double velocityZ) : base(world)
    {
        setBoundingBoxSpacing(0.2F, 0.2F);
        standingEyeHeight = height / 2.0F;
        setPosition(x, y, z);
        particleRed = particleGreen = particleBlue = 1.0F;
        this.velocityX = velocityX + (Random.Shared.NextDouble() * 2.0D - 1.0D) * 0.4F;
        this.velocityY = velocityY + (Random.Shared.NextDouble() * 2.0D - 1.0D) * 0.4F;
        this.velocityZ = velocityZ + (Random.Shared.NextDouble() * 2.0D - 1.0D) * 0.4F;
        float velocityScale = (float)(Random.Shared.NextDouble() + Random.Shared.NextDouble() + 1.0D) * 0.15F;
        float speed = MathHelper.Sqrt(this.velocityX * this.velocityX + this.velocityY * this.velocityY + this.velocityZ * this.velocityZ);
        this.velocityX = this.velocityX / speed * velocityScale * 0.4F;
        this.velocityY = this.velocityY / speed * velocityScale * 0.4F + 0.1F;
        this.velocityZ = this.velocityZ / speed * velocityScale * 0.4F;
        particleTextureJitterX = random.NextFloat() * 3.0F;
        particleTextureJitterY = random.NextFloat() * 3.0F;
        particleScale = (random.NextFloat() * 0.5F + 0.5F) * 2.0F;
        particleMaxAge = (int)(4.0F / (random.NextFloat() * 0.9F + 0.1F));
        particleAge = 0;
    }

    public EntityFX scaleVelocity(float multiplier)
    {
        velocityX *= multiplier;
        velocityY = (velocityY - 0.1F) * multiplier + 0.1F;
        velocityZ *= multiplier;
        return this;
    }

    public EntityFX scaleSize(float scale)
    {
        setBoundingBoxSpacing(0.2F * scale, 0.2F * scale);
        particleScale *= scale;
        return this;
    }

    protected override bool bypassesSteppingEffects() => false;

    public override void tick()
    {
        prevX = x;
        prevY = y;
        prevZ = z;
        if (particleAge++ >= particleMaxAge)
        {
            markDead();
        }

        velocityY -= 0.04D * particleGravity;
        move(velocityX, velocityY, velocityZ);
        velocityX *= 0.98F;
        velocityY *= 0.98F;
        velocityZ *= 0.98F;
        if (onGround)
        {
            velocityX *= 0.7F;
            velocityZ *= 0.7F;
        }
    }

    public virtual void renderParticle(Tessellator t, float partialTick, float rotX, float rotY, float rotZ, float upX, float upZ)
    {
        float minU = particleTextureIndex % 16 / 16.0F;
        float maxU = minU + 0.999F / 16.0F;
        float minV = particleTextureIndex / 16 / 16.0F;
        float maxV = minV + 0.999F / 16.0F;
        float size = 0.1F * particleScale;
        float x = (float)(prevX + (this.x - prevX) * partialTick - interpPosX);
        float y = (float)(prevY + (this.y - prevY) * partialTick - interpPosY);
        float z = (float)(prevZ + (this.z - prevZ) * partialTick - interpPosZ);
        float brightness = getBrightnessAtEyes(partialTick);
        t.setColorOpaque_F(particleRed * brightness, particleGreen * brightness, particleBlue * brightness);
        t.addVertexWithUV(x - rotX * size - upX * size, y - rotY * size, z - rotZ * size - upZ * size, maxU, maxV);
        t.addVertexWithUV(x - rotX * size + upX * size, y + rotY * size, z - rotZ * size + upZ * size, maxU, minV);
        t.addVertexWithUV(x + rotX * size + upX * size, y + rotY * size, z + rotZ * size + upZ * size, minU, minV);
        t.addVertexWithUV(x + rotX * size - upX * size, y - rotY * size, z + rotZ * size - upZ * size, minU, maxV);
    }

    public virtual int getFXLayer() => 0;

    public override void writeNbt(NBTTagCompound nbt)
    {
    }

    public override void readNbt(NBTTagCompound nbt)
    {
    }
}
