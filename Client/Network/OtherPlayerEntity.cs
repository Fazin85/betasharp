using betareborn.Entities;
using betareborn.Items;
using betareborn.Util.Maths;
using betareborn.Worlds;

namespace betareborn.Client.Network
{
    public class OtherPlayerEntity : EntityPlayer
    {
        public static readonly new java.lang.Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(OtherPlayerEntity).TypeHandle);

        private int lerpSteps;
        private double lerpX;
        private double lerpY;
        private double lerpZ;
        private double lerpYaw;
        private double lerpPitch;

        public OtherPlayerEntity(World var1, string var2) : base(var1)
        {
            name = var2;
            standingEyeHeight = 0.0F;
            stepHeight = 0.0F;
            if (var2 != null && var2.Length > 0)
            {
                skinUrl = "http://s3.amazonaws.com/MinecraftSkins/" + var2 + ".png";
            }

            noClip = true;
            sleepOffsetY = 0.25F;
            renderDistanceWeight = 10.0D;
        }

        protected override void resetEyeHeight()
        {
            standingEyeHeight = 0.0F;
        }

        public override bool damage(Entity var1, int var2)
        {
            return true;
        }

        public override void setPositionAndAnglesAvoidEntities(double var1, double var3, double var5, float var7, float var8, int var9)
        {
            lerpX = var1;
            lerpY = var3;
            lerpZ = var5;
            lerpYaw = var7;
            lerpPitch = var8;
            lerpSteps = var9;
        }

        public override void tick()
        {
            sleepOffsetY = 0.0F;
            base.tick();
            lastWalkAnimationSpeed = walkAnimationSpeed;
            double var1 = x - prevX;
            double var3 = z - prevZ;
            float var5 = MathHelper.sqrt_double(var1 * var1 + var3 * var3) * 4.0F;
            if (var5 > 1.0F)
            {
                var5 = 1.0F;
            }

            walkAnimationSpeed += (var5 - walkAnimationSpeed) * 0.4F;
            animationPhase += walkAnimationSpeed;
        }

        public override float getShadowRadius()
        {
            return 0.0F;
        }

        public override void tickMovement()
        {
            base.tickLiving();
            if (lerpSteps > 0)
            {
                double var1 = x + (lerpX - x) / lerpSteps;
                double var3 = y + (lerpY - y) / lerpSteps;
                double var5 = z + (lerpZ - z) / lerpSteps;

                double var7;
                for (var7 = lerpYaw - yaw; var7 < -180.0D; var7 += 360.0D)
                {
                }

                while (var7 >= 180.0D)
                {
                    var7 -= 360.0D;
                }

                yaw = (float)(yaw + var7 / lerpSteps);
                pitch = (float)(pitch + (lerpPitch - pitch) / lerpSteps);
                --lerpSteps;
                setPosition(var1, var3, var5);
                setRotation(yaw, pitch);
            }

            prevStepBobbingAmount = stepBobbingAmount;
            float var9 = MathHelper.sqrt_double(velocityX * velocityX + velocityZ * velocityZ);
            float var2 = (float)java.lang.Math.atan(-velocityY * (double)0.2F) * 15.0F;
            if (var9 > 0.1F)
            {
                var9 = 0.1F;
            }

            if (!onGround || health <= 0)
            {
                var9 = 0.0F;
            }

            if (onGround || health <= 0)
            {
                var2 = 0.0F;
            }

            stepBobbingAmount += (var9 - stepBobbingAmount) * 0.4F;
            tilt += (var2 - tilt) * 0.8F;
        }

        public override void setEquipmentStack(int var1, int var2, int var3)
        {
            ItemStack var4 = null;
            if (var2 >= 0)
            {
                var4 = new ItemStack(var2, 1, var3);
            }

            if (var1 == 0)
            {
                inventory.main[inventory.selectedSlot] = var4;
            }
            else
            {
                inventory.armor[var1 - 1] = var4;
            }

        }

        public override void spawn()
        {
        }
    }

}