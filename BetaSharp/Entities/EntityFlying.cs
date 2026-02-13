using BetaSharp.Blocks;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds;

namespace BetaSharp.Entities;

public class EntityFlying : EntityLiving
{
    public static readonly new java.lang.Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(EntityFlying).TypeHandle);

    public EntityFlying(World world) : base(world)
    {
    }

    protected override void onLanding(float fallDistance)
    {
    }

    public override void travel(float strafe, float forward)
    {
        if (isInWater())
        {
            moveNonSolid(strafe, forward, 0.02F);
            move(velocityX, velocityY, velocityZ);
            velocityX *= (double)0.8F;
            velocityY *= (double)0.8F;
            velocityZ *= (double)0.8F;
        }
        else if (isTouchingLava())
        {
            moveNonSolid(strafe, forward, 0.02F);
            move(velocityX, velocityY, velocityZ);
            velocityX *= 0.5D;
            velocityY *= 0.5D;
            velocityZ *= 0.5D;
        }
        else
        {
            float friction = 0.91F;
            if (onGround)
            {
                friction = 546.0F * 0.1F * 0.1F * 0.1F;
                int groundBlockId = world.getBlockId(MathHelper.floor_double(x), MathHelper.floor_double(boundingBox.minY) - 1, MathHelper.floor_double(z));
                if (groundBlockId > 0)
                {
                    friction = Block.BLOCKS[groundBlockId].slipperiness * 0.91F;
                }
            }

            float accelerationFactor = 0.16277136F / (friction * friction * friction);
            moveNonSolid(strafe, forward, onGround ? 0.1F * accelerationFactor : 0.02F);
            friction = 0.91F;
            if (onGround)
            {
                friction = 546.0F * 0.1F * 0.1F * 0.1F;
                int groundBlockId = world.getBlockId(MathHelper.floor_double(x), MathHelper.floor_double(boundingBox.minY) - 1, MathHelper.floor_double(z));
                if (groundBlockId > 0)
                {
                    friction = Block.BLOCKS[groundBlockId].slipperiness * 0.91F;
                }
            }

            move(velocityX, velocityY, velocityZ);
            velocityX *= (double)friction;
            velocityY *= (double)friction;
            velocityZ *= (double)friction;
        }

        lastWalkAnimationSpeed = walkAnimationSpeed;
        double dx = x - prevX;
        double dy = z - prevZ;
        float distanceMoved = MathHelper.sqrt_double(dx * dx + dy * dy) * 4.0F;
        if (distanceMoved > 1.0F)
        {
            distanceMoved = 1.0F;
        }

        walkAnimationSpeed += (distanceMoved - walkAnimationSpeed) * 0.4F;
        animationPhase += walkAnimationSpeed;
    }

    public override bool isOnLadder()
    {
        return false;
    }
}