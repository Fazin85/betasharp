using BetaSharp.Blocks.Materials;
using BetaSharp.Items;
using BetaSharp.NBT;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds.Core;
using BetaSharp.Worlds.Core.Systems;

namespace BetaSharp.Entities;

public class EntitySquid : EntityWaterMob
{
    private float animationSpeed;
    public float prevSwimPhase;
    public float prevTentaclePhase;
    public float prevTentacleSpread;
    public float prevTiltAngle;
    private float randomMotionSpeed;
    private float randomMotionVecX;
    private float randomMotionVecY;
    private float randomMotionVecZ;
    private float squidRotation;
    public float swimPhase;
    public float tentaclePhase;
    public float tentacleSpread;
    public float tiltAngle;

    public EntitySquid(IWorldContext world) : base(world)
    {
        texture = "/mob/squid.png";
        setBoundingBoxSpacing(0.95F, 0.95F);
        animationSpeed = 1.0F / (random.NextFloat() + 1.0F) * 0.2F;
    }

    public override void writeNbt(NBTTagCompound nbt) => base.writeNbt(nbt);

    public override void readNbt(NBTTagCompound nbt) => base.readNbt(nbt);

    protected override string getLivingSound() => null;

    protected override string getHurtSound() => null;

    protected override string getDeathSound() => null;

    protected override float getSoundVolume() => 0.4F;

    protected override int getDropItemId() => 0;

    protected override void dropFewItems()
    {
        int dropCount = random.NextInt(3) + 1;

        for (int _ = 0; _ < dropCount; ++_)
        {
            dropItem(new ItemStack(Item.Dye, 1, 0), 0.0F);
        }
    }

    public override bool interact(EntityPlayer player) => false;

    public override bool isInWater() => world.Reader.UpdateMovementInFluid(boundingBox.Expand(0.0D, -0.6F, 0.0D), Material.Water, this);

    public override void tickMovement()
    {
        base.tickMovement();
        prevTiltAngle = tiltAngle;
        prevTentaclePhase = tentaclePhase;
        prevSwimPhase = swimPhase;
        prevTentacleSpread = tentacleSpread;
        swimPhase += animationSpeed;
        if (swimPhase > (float)Math.PI * 2.0F)
        {
            swimPhase -= (float)Math.PI * 2.0F;
            if (random.NextInt(10) == 0)
            {
                animationSpeed = 1.0F / (random.NextFloat() + 1.0F) * 0.2F;
            }
        }

        if (isInWater())
        {
            float phaseProgress;
            if (swimPhase < (float)Math.PI)
            {
                phaseProgress = swimPhase / (float)Math.PI;
                tentacleSpread = MathHelper.Sin(phaseProgress * phaseProgress * (float)Math.PI) * (float)Math.PI * 0.25F;
                if (phaseProgress > 0.75D)
                {
                    randomMotionSpeed = 1.0F;
                    squidRotation = 1.0F;
                }
                else
                {
                    squidRotation *= 0.8F;
                }
            }
            else
            {
                tentacleSpread = 0.0F;
                randomMotionSpeed *= 0.9F;
                squidRotation *= 0.99F;
            }

            if (!interpolateOnly)
            {
                velocityX = randomMotionVecX * randomMotionSpeed;
                velocityY = randomMotionVecY * randomMotionSpeed;
                velocityZ = randomMotionVecZ * randomMotionSpeed;
            }

            phaseProgress = MathHelper.Sqrt(velocityX * velocityX + velocityZ * velocityZ);
            bodyYaw += (-(float)Math.Atan2(velocityX, velocityZ) * 180.0F / (float)Math.PI - bodyYaw) * 0.1F;
            yaw = bodyYaw;
            tentaclePhase += (float)Math.PI * squidRotation * 1.5F;
            tiltAngle += (-(float)Math.Atan2(phaseProgress, velocityY) * 180.0F / (float)Math.PI - tiltAngle) * 0.1F;
        }
        else
        {
            tentacleSpread = MathHelper.Abs(MathHelper.Sin(swimPhase)) * (float)Math.PI * 0.25F;
            if (!interpolateOnly)
            {
                velocityX = 0.0D;
                velocityY -= 0.08D;
                velocityY *= 0.98F;
                velocityZ = 0.0D;
            }

            tiltAngle = (float)(tiltAngle + (-90.0F - tiltAngle) * 0.02D);
        }
    }

    public override void travel(float strafe, float forward) => move(velocityX, velocityY, velocityZ);

    public override void tickLiving()
    {
        if (random.NextInt(50) == 0 || !inWater || (randomMotionVecX == 0.0F && randomMotionVecY == 0.0F && randomMotionVecZ == 0.0F))
        {
            float randomAngle = random.NextFloat() * (float)Math.PI * 2.0F;
            randomMotionVecX = MathHelper.Cos(randomAngle) * 0.2F;
            randomMotionVecY = -0.1F + random.NextFloat() * 0.2F;
            randomMotionVecZ = MathHelper.Sin(randomAngle) * 0.2F;
        }

        func_27021_X();
    }
}
