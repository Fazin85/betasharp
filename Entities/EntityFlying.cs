using betareborn.Blocks;
using betareborn.Util.Maths;
using betareborn.Worlds;

namespace betareborn.Entities
{
    public class EntityFlying : EntityLiving
    {
        public static readonly new java.lang.Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(EntityFlying).TypeHandle);

        public EntityFlying(World var1) : base(var1)
        {
        }

        protected override void onLanding(float var1)
        {
        }

        public override void travel(float var1, float var2)
        {
            if (isInWater())
            {
                moveNonSolid(var1, var2, 0.02F);
                move(velocityX, velocityY, velocityZ);
                velocityX *= (double)0.8F;
                velocityY *= (double)0.8F;
                velocityZ *= (double)0.8F;
            }
            else if (isTouchingLava())
            {
                moveNonSolid(var1, var2, 0.02F);
                move(velocityX, velocityY, velocityZ);
                velocityX *= 0.5D;
                velocityY *= 0.5D;
                velocityZ *= 0.5D;
            }
            else
            {
                float var3 = 0.91F;
                if (onGround)
                {
                    var3 = 546.0F * 0.1F * 0.1F * 0.1F;
                    int var4 = world.GetBlockId(MathHelper.floor_double(x), MathHelper.floor_double(boundingBox.minY) - 1, MathHelper.floor_double(z));
                    if (var4 > 0)
                    {
                        var3 = Block.BLOCKS[var4].slipperiness * 0.91F;
                    }
                }

                float var8 = 0.16277136F / (var3 * var3 * var3);
                moveNonSolid(var1, var2, onGround ? 0.1F * var8 : 0.02F);
                var3 = 0.91F;
                if (onGround)
                {
                    var3 = 546.0F * 0.1F * 0.1F * 0.1F;
                    int var5 = world.GetBlockId(MathHelper.floor_double(x), MathHelper.floor_double(boundingBox.minY) - 1, MathHelper.floor_double(z));
                    if (var5 > 0)
                    {
                        var3 = Block.BLOCKS[var5].slipperiness * 0.91F;
                    }
                }

                move(velocityX, velocityY, velocityZ);
                velocityX *= (double)var3;
                velocityY *= (double)var3;
                velocityZ *= (double)var3;
            }

            lastWalkAnimationSpeed = walkAnimationSpeed;
            double var10 = x - prevX;
            double var9 = z - prevZ;
            float var7 = MathHelper.sqrt_double(var10 * var10 + var9 * var9) * 4.0F;
            if (var7 > 1.0F)
            {
                var7 = 1.0F;
            }

            walkAnimationSpeed += (var7 - walkAnimationSpeed) * 0.4F;
            field_703_S += walkAnimationSpeed;
        }

        public override bool isOnLadder()
        {
            return false;
        }
    }

}