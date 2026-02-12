using betareborn.NBT;
using betareborn.Util.Maths;
using betareborn.Worlds;

namespace betareborn.Entities
{
    public class EntityTNTPrimed : Entity
    {
        public static readonly new java.lang.Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(EntityTNTPrimed).TypeHandle);
        public int fuse;

        public EntityTNTPrimed(World var1) : base(var1)
        {
            fuse = 0;
            preventEntitySpawning = true;
            setBoundingBoxSpacing(0.98F, 0.98F);
            standingEyeHeight = height / 2.0F;
        }

        public EntityTNTPrimed(World var1, double var2, double var4, double var6) : base(var1)
        {
            setPosition(var2, var4, var6);
            float var8 = (float)(java.lang.Math.random() * (double)((float)Math.PI) * 2.0D);
            velocityX = (double)(-MathHelper.sin(var8 * (float)Math.PI / 180.0F) * 0.02F);
            velocityY = (double)0.2F;
            velocityZ = (double)(-MathHelper.cos(var8 * (float)Math.PI / 180.0F) * 0.02F);
            fuse = 80;
            prevX = var2;
            prevY = var4;
            prevZ = var6;
        }

        protected override void initDataTracker()
        {
        }

        protected override bool bypassesSteppingEffects()
        {
            return false;
        }

        public override bool isCollidable()
        {
            return !dead;
        }

        public override void tick()
        {
            prevX = x;
            prevY = y;
            prevZ = z;
            velocityY -= (double)0.04F;
            move(velocityX, velocityY, velocityZ);
            velocityX *= (double)0.98F;
            velocityY *= (double)0.98F;
            velocityZ *= (double)0.98F;
            if (onGround)
            {
                velocityX *= (double)0.7F;
                velocityZ *= (double)0.7F;
                velocityY *= -0.5D;
            }

            if (fuse-- <= 0)
            {
                if (!world.isRemote)
                {
                    markDead();
                    explode();
                }
                else
                {
                    markDead();
                }
            }
            else
            {
                world.addParticle("smoke", x, y + 0.5D, z, 0.0D, 0.0D, 0.0D);
            }

        }

        private void explode()
        {
            float var1 = 4.0F;
            world.createExplosion((Entity)null, x, y, z, var1);
        }

        public override void writeNbt(NBTTagCompound var1)
        {
            var1.SetByte("Fuse", (sbyte)fuse);
        }

        public override void readNbt(NBTTagCompound var1)
        {
            fuse = var1.GetByte("Fuse");
        }

        public override float getShadowRadius()
        {
            return 0.0F;
        }
    }

}