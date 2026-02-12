using betareborn.Blocks.Materials;
using betareborn.Items;
using betareborn.NBT;
using betareborn.Util.Maths;
using betareborn.Worlds;

namespace betareborn.Entities
{
    public class EntitySquid : EntityWaterMob
    {
        public static readonly new java.lang.Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(EntitySquid).TypeHandle);

        public float field_21089_a = 0.0F;
        public float field_21088_b = 0.0F;
        public float field_21087_c = 0.0F;
        public float field_21086_f = 0.0F;
        public float field_21085_g = 0.0F;
        public float field_21084_h = 0.0F;
        public float field_21083_i = 0.0F;
        public float field_21082_j = 0.0F;
        private float randomMotionSpeed = 0.0F;
        private float field_21080_l = 0.0F;
        private float field_21079_m = 0.0F;
        private float randomMotionVecX = 0.0F;
        private float randomMotionVecY = 0.0F;
        private float randomMotionVecZ = 0.0F;

        public EntitySquid(World var1) : base(var1)
        {
            texture = "/mob/squid.png";
            setBoundingBoxSpacing(0.95F, 0.95F);
            field_21080_l = 1.0F / (random.nextFloat() + 1.0F) * 0.2F;
        }

        public override void writeNbt(NBTTagCompound var1)
        {
            base.writeNbt(var1);
        }

        public override void readNbt(NBTTagCompound var1)
        {
            base.readNbt(var1);
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
            int var1 = random.nextInt(3) + 1;

            for (int var2 = 0; var2 < var1; ++var2)
            {
                dropItem(new ItemStack(Item.DYE, 1, 0), 0.0F);
            }

        }

        public override bool interact(EntityPlayer var1)
        {
            return false;
        }

        public override bool isInWater()
        {
            return world.updateMovementInFluid(boundingBox.expand(0.0D, (double)-0.6F, 0.0D), Material.WATER, this);
        }

        public override void tickMovement()
        {
            base.tickMovement();
            field_21088_b = field_21089_a;
            field_21086_f = field_21087_c;
            field_21084_h = field_21085_g;
            field_21082_j = field_21083_i;
            field_21085_g += field_21080_l;
            if (field_21085_g > (float)Math.PI * 2.0F)
            {
                field_21085_g -= (float)Math.PI * 2.0F;
                if (random.nextInt(10) == 0)
                {
                    field_21080_l = 1.0F / (random.nextFloat() + 1.0F) * 0.2F;
                }
            }

            if (isInWater())
            {
                float var1;
                if (field_21085_g < (float)Math.PI)
                {
                    var1 = field_21085_g / (float)Math.PI;
                    field_21083_i = MathHelper.sin(var1 * var1 * (float)Math.PI) * (float)Math.PI * 0.25F;
                    if ((double)var1 > 0.75D)
                    {
                        randomMotionSpeed = 1.0F;
                        field_21079_m = 1.0F;
                    }
                    else
                    {
                        field_21079_m *= 0.8F;
                    }
                }
                else
                {
                    field_21083_i = 0.0F;
                    randomMotionSpeed *= 0.9F;
                    field_21079_m *= 0.99F;
                }

                if (!interpolateOnly)
                {
                    velocityX = (double)(randomMotionVecX * randomMotionSpeed);
                    velocityY = (double)(randomMotionVecY * randomMotionSpeed);
                    velocityZ = (double)(randomMotionVecZ * randomMotionSpeed);
                }

                var1 = MathHelper.sqrt_double(velocityX * velocityX + velocityZ * velocityZ);
                bodyYaw += (-((float)java.lang.Math.atan2(velocityX, velocityZ)) * 180.0F / (float)Math.PI - bodyYaw) * 0.1F;
                yaw = bodyYaw;
                field_21087_c += (float)Math.PI * field_21079_m * 1.5F;
                field_21089_a += (-((float)java.lang.Math.atan2((double)var1, velocityY)) * 180.0F / (float)Math.PI - field_21089_a) * 0.1F;
            }
            else
            {
                field_21083_i = MathHelper.abs(MathHelper.sin(field_21085_g)) * (float)Math.PI * 0.25F;
                if (!interpolateOnly)
                {
                    velocityX = 0.0D;
                    velocityY -= 0.08D;
                    velocityY *= (double)0.98F;
                    velocityZ = 0.0D;
                }

                field_21089_a = (float)((double)field_21089_a + (double)(-90.0F - field_21089_a) * 0.02D);
            }

        }

        public override void travel(float var1, float var2)
        {
            move(velocityX, velocityY, velocityZ);
        }

        public override void tickLiving()
        {
            if (random.nextInt(50) == 0 || !inWater || randomMotionVecX == 0.0F && randomMotionVecY == 0.0F && randomMotionVecZ == 0.0F)
            {
                float var1 = random.nextFloat() * (float)Math.PI * 2.0F;
                randomMotionVecX = MathHelper.cos(var1) * 0.2F;
                randomMotionVecY = -0.1F + random.nextFloat() * 0.2F;
                randomMotionVecZ = MathHelper.sin(var1) * 0.2F;
            }

            func_27021_X();
        }
    }

}