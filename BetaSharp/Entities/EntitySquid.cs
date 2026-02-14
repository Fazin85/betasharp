using BetaSharp.Blocks.Materials;
using BetaSharp.Items;
using BetaSharp.NBT;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds;

namespace BetaSharp.Entities;

public class EntitySquid : EntityWaterMob
{
    public static readonly new java.lang.Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(EntitySquid).TypeHandle);

    public float tiltAngle = 0.0F;
    public float prevTiltAngle = 0.0F;
    public float tentaclePhase = 0.0F;
    public float prevTentaclePhase = 0.0F;
    public float swimPhase = 0.0F;
    public float prevSwimPhase = 0.0F;
    public float tentacleSpread = 0.0F;
    public float prevTentacleSpread = 0.0F;
    private float randomMotionSpeed = 0.0F;
    private float animationSpeed = 0.0F;
    private float squidRotation = 0.0F;
    private float randomMotionVecX = 0.0F;
    private float randomMotionVecY = 0.0F;
    private float randomMotionVecZ = 0.0F;

    public EntitySquid(World world) : base(world)
    {
        texture = "/mob/squid.png";
        setBoundingBoxSpacing(0.95F, 0.95F);
        animationSpeed = 1.0F / (random.nextFloat() + 1.0F) * 0.2F;
    }

    public override void writeNbt(NBTTagCompound nbt)
    {
        base.writeNbt(nbt);
    }

    public override void readNbt(NBTTagCompound nbt)
    {
        base.readNbt(nbt);
    }

    protected override String getLivingSound()
    {
        return null;
    }

    protected override String getHurtSound()
    {
        return null;
    }

    protected override String getDeathSound()
    {
        return null;
    }

    protected override float getSoundVolume()
    {
        return 0.4F;
    }

    protected override int getDropItemId()
    {
        return 0;
    }

    protected override void dropFewItems()
    {
        int dropCount = random.nextInt(3) + 1;

        for (int _ = 0; _ < dropCount; ++_)
        {
            dropItem(new ItemStack(Item.DYE, 1, 0), 0.0F);
        }

    }

    public override bool interact(EntityPlayer player)
    {
        return false;
    }

    public override bool isInWater()
    {
        return world.updateMovementInFluid(boundingBox.expand(0.0D, (double)-0.6F, 0.0D), Material.Water, this);
    }

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
            if (random.nextInt(10) == 0)
            {
                animationSpeed = 1.0F / (random.nextFloat() + 1.0F) * 0.2F;
            }
        }

        if (isInWater())
        {
            float phaseProgress;
            if (swimPhase < (float)Math.PI)
            {
                phaseProgress = swimPhase / (float)Math.PI;
                tentacleSpread = MathHelper.sin(phaseProgress * phaseProgress * (float)Math.PI) * (float)Math.PI * 0.25F;
                if ((double)phaseProgress > 0.75D)
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
                velocityX = (double)(randomMotionVecX * randomMotionSpeed);
                velocityY = (double)(randomMotionVecY * randomMotionSpeed);
                velocityZ = (double)(randomMotionVecZ * randomMotionSpeed);
            }

            phaseProgress = MathHelper.sqrt_double(velocityX * velocityX + velocityZ * velocityZ);
            bodyYaw += (-((float)System.Math.Atan2(velocityX, velocityZ)) * 180.0F / (float)Math.PI - bodyYaw) * 0.1F;
            yaw = bodyYaw;
            tentaclePhase += (float)Math.PI * squidRotation * 1.5F;
            tiltAngle += (-((float)System.Math.Atan2((double)phaseProgress, velocityY)) * 180.0F / (float)Math.PI - tiltAngle) * 0.1F;
        }
        else
        {
            tentacleSpread = MathHelper.abs(MathHelper.sin(swimPhase)) * (float)Math.PI * 0.25F;
            if (!interpolateOnly)
            {
                velocityX = 0.0D;
                velocityY -= 0.08D;
                velocityY *= (double)0.98F;
                velocityZ = 0.0D;
            }

            tiltAngle = (float)((double)tiltAngle + (double)(-90.0F - tiltAngle) * 0.02D);
        }

    }

    public override void travel(float strafe, float forward)
    {
        move(velocityX, velocityY, velocityZ);
    }

    public override void tickLiving()
    {
        if (random.nextInt(50) == 0 || !inWater || randomMotionVecX == 0.0F && randomMotionVecY == 0.0F && randomMotionVecZ == 0.0F)
        {
            float randomAngle = random.nextFloat() * (float)Math.PI * 2.0F;
            randomMotionVecX = MathHelper.cos(randomAngle) * 0.2F;
            randomMotionVecY = -0.1F + random.nextFloat() * 0.2F;
            randomMotionVecZ = MathHelper.sin(randomAngle) * 0.2F;
        }

        func_27021_X();
    }
}