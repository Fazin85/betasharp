using betareborn.Blocks;
using betareborn.Blocks.Materials;
using betareborn.Items;
using betareborn.NBT;
using betareborn.Util.Maths;
using betareborn.Worlds;
using java.lang;

namespace betareborn.Entities
{
    public class EntityBoat : Entity
    {

        public static readonly new Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(EntityBoat).TypeHandle);
        public int boatCurrentDamage;
        public int boatTimeSinceHit;
        public int boatRockDirection;
        private int field_9394_d;
        private double field_9393_e;
        private double field_9392_f;
        private double field_9391_g;
        private double field_9390_h;
        private double field_9389_i;
        private double field_9388_j;
        private double field_9387_k;
        private double field_9386_l;

        public EntityBoat(World var1) : base(var1)
        {
            boatCurrentDamage = 0;
            boatTimeSinceHit = 0;
            boatRockDirection = 1;
            preventEntitySpawning = true;
            setBoundingBoxSpacing(1.5F, 0.6F);
            standingEyeHeight = height / 2.0F;
        }

        protected override bool bypassesSteppingEffects()
        {
            return false;
        }

        protected override void initDataTracker()
        {
        }

        public override Box? getCollisionAgainstShape(Entity var1)
        {
            return var1.boundingBox;
        }

        public override Box? getBoundingBox()
        {
            return boundingBox;
        }

        public override bool isPushable()
        {
            return true;
        }

        public EntityBoat(World var1, double var2, double var4, double var6) : this(var1)
        {
            setPosition(var2, var4 + (double)standingEyeHeight, var6);
            velocityX = 0.0D;
            velocityY = 0.0D;
            velocityZ = 0.0D;
            prevX = var2;
            prevY = var4;
            prevZ = var6;
        }

        public override double getPassengerRidingHeight()
        {
            return (double)height * 0.0D - (double)0.3F;
        }

        public override bool damage(Entity var1, int var2)
        {
            if (!world.isRemote && !dead)
            {
                boatRockDirection = -boatRockDirection;
                boatTimeSinceHit = 10;
                boatCurrentDamage += var2 * 10;
                scheduleVelocityUpdate();
                if (boatCurrentDamage > 40)
                {
                    if (passenger != null)
                    {
                        passenger.setVehicle(this);
                    }

                    int var3;
                    for (var3 = 0; var3 < 3; ++var3)
                    {
                        dropItem(Block.PLANKS.id, 1, 0.0F);
                    }

                    for (var3 = 0; var3 < 2; ++var3)
                    {
                        dropItem(Item.STICK.id, 1, 0.0F);
                    }

                    markDead();
                }

                return true;
            }
            else
            {
                return true;
            }
        }

        public override void animateHurt()
        {
            boatRockDirection = -boatRockDirection;
            boatTimeSinceHit = 10;
            boatCurrentDamage += boatCurrentDamage * 10;
        }

        public override bool isCollidable()
        {
            return !dead;
        }

        public override void setPositionAndAnglesAvoidEntities(double var1, double var3, double var5, float var7, float var8, int var9)
        {
            field_9393_e = var1;
            field_9392_f = var3;
            field_9391_g = var5;
            field_9390_h = (double)var7;
            field_9389_i = (double)var8;
            field_9394_d = var9 + 4;
            velocityX = field_9388_j;
            velocityY = field_9387_k;
            velocityZ = field_9386_l;
        }

        public override void setVelocityClient(double var1, double var3, double var5)
        {
            field_9388_j = velocityX = var1;
            field_9387_k = velocityY = var3;
            field_9386_l = velocityZ = var5;
        }

        public override void tick()
        {
            base.tick();
            if (boatTimeSinceHit > 0)
            {
                --boatTimeSinceHit;
            }

            if (boatCurrentDamage > 0)
            {
                --boatCurrentDamage;
            }

            prevX = x;
            prevY = y;
            prevZ = z;
            byte var1 = 5;
            double var2 = 0.0D;

            for (int var4 = 0; var4 < var1; ++var4)
            {
                double var5 = boundingBox.minY + (boundingBox.maxY - boundingBox.minY) * (double)(var4 + 0) / (double)var1 - 0.125D;
                double var7 = boundingBox.minY + (boundingBox.maxY - boundingBox.minY) * (double)(var4 + 1) / (double)var1 - 0.125D;
                Box var9 = new Box(boundingBox.minX, var5, boundingBox.minZ, boundingBox.maxX, var7, boundingBox.maxZ);
                if (world.isFluidInBox(var9, Material.WATER))
                {
                    var2 += 1.0D / (double)var1;
                }
            }

            double var6;
            double var8;
            double var10;
            double var21;
            if (world.isRemote)
            {
                if (field_9394_d > 0)
                {
                    var21 = x + (field_9393_e - x) / (double)field_9394_d;
                    var6 = y + (field_9392_f - y) / (double)field_9394_d;
                    var8 = z + (field_9391_g - z) / (double)field_9394_d;

                    for (var10 = field_9390_h - (double)yaw; var10 < -180.0D; var10 += 360.0D)
                    {
                    }

                    while (var10 >= 180.0D)
                    {
                        var10 -= 360.0D;
                    }

                    yaw = (float)((double)yaw + var10 / (double)field_9394_d);
                    pitch = (float)((double)pitch + (field_9389_i - (double)pitch) / (double)field_9394_d);
                    --field_9394_d;
                    setPosition(var21, var6, var8);
                    setRotation(yaw, pitch);
                }
                else
                {
                    var21 = x + velocityX;
                    var6 = y + velocityY;
                    var8 = z + velocityZ;
                    setPosition(var21, var6, var8);
                    if (onGround)
                    {
                        velocityX *= 0.5D;
                        velocityY *= 0.5D;
                        velocityZ *= 0.5D;
                    }

                    velocityX *= (double)0.99F;
                    velocityY *= (double)0.95F;
                    velocityZ *= (double)0.99F;
                }

            }
            else
            {
                if (var2 < 1.0D)
                {
                    var21 = var2 * 2.0D - 1.0D;
                    velocityY += (double)0.04F * var21;
                }
                else
                {
                    if (velocityY < 0.0D)
                    {
                        velocityY /= 2.0D;
                    }

                    velocityY += (double)0.007F;
                }

                if (passenger != null)
                {
                    velocityX += passenger.velocityX * 0.2D;
                    velocityZ += passenger.velocityZ * 0.2D;
                }

                var21 = 0.4D;
                if (velocityX < -var21)
                {
                    velocityX = -var21;
                }

                if (velocityX > var21)
                {
                    velocityX = var21;
                }

                if (velocityZ < -var21)
                {
                    velocityZ = -var21;
                }

                if (velocityZ > var21)
                {
                    velocityZ = var21;
                }

                if (onGround)
                {
                    velocityX *= 0.5D;
                    velocityY *= 0.5D;
                    velocityZ *= 0.5D;
                }

                move(velocityX, velocityY, velocityZ);
                var6 = java.lang.Math.sqrt(velocityX * velocityX + velocityZ * velocityZ);
                if (var6 > 0.15D)
                {
                    var8 = java.lang.Math.cos((double)yaw * java.lang.Math.PI / 180.0D);
                    var10 = java.lang.Math.sin((double)yaw * java.lang.Math.PI / 180.0D);

                    for (int var12 = 0; (double)var12 < 1.0D + var6 * 60.0D; ++var12)
                    {
                        double var13 = (double)(random.nextFloat() * 2.0F - 1.0F);
                        double var15 = (double)(random.nextInt(2) * 2 - 1) * 0.7D;
                        double var17;
                        double var19;
                        if (random.nextBoolean())
                        {
                            var17 = x - var8 * var13 * 0.8D + var10 * var15;
                            var19 = z - var10 * var13 * 0.8D - var8 * var15;
                            world.addParticle("splash", var17, y - 0.125D, var19, velocityX, velocityY, velocityZ);
                        }
                        else
                        {
                            var17 = x + var8 + var10 * var13 * 0.7D;
                            var19 = z + var10 - var8 * var13 * 0.7D;
                            world.addParticle("splash", var17, y - 0.125D, var19, velocityX, velocityY, velocityZ);
                        }
                    }
                }

                if (horizontalCollison && var6 > 0.15D)
                {
                    if (!world.isRemote)
                    {
                        markDead();

                        int var22;
                        for (var22 = 0; var22 < 3; ++var22)
                        {
                            dropItem(Block.PLANKS.id, 1, 0.0F);
                        }

                        for (var22 = 0; var22 < 2; ++var22)
                        {
                            dropItem(Item.STICK.id, 1, 0.0F);
                        }
                    }
                }
                else
                {
                    velocityX *= (double)0.99F;
                    velocityY *= (double)0.95F;
                    velocityZ *= (double)0.99F;
                }

                pitch = 0.0F;
                var8 = (double)yaw;
                var10 = prevX - x;
                double var23 = prevZ - z;
                if (var10 * var10 + var23 * var23 > 0.001D)
                {
                    var8 = (double)((float)(java.lang.Math.atan2(var23, var10) * 180.0D / java.lang.Math.PI));
                }

                double var14;
                for (var14 = var8 - (double)yaw; var14 >= 180.0D; var14 -= 360.0D)
                {
                }

                while (var14 < -180.0D)
                {
                    var14 += 360.0D;
                }

                if (var14 > 20.0D)
                {
                    var14 = 20.0D;
                }

                if (var14 < -20.0D)
                {
                    var14 = -20.0D;
                }

                yaw = (float)((double)yaw + var14);
                setRotation(yaw, pitch);
                var var16 = world.getEntities(this, boundingBox.expand((double)0.2F, 0.0D, (double)0.2F));
                int var24;
                if (var16 != null && var16.Count > 0)
                {
                    for (var24 = 0; var24 < var16.Count; ++var24)
                    {
                        Entity var18 = var16[var24];
                        if (var18 != passenger && var18.isPushable() && var18 is EntityBoat)
                        {
                            var18.onCollision(this);
                        }
                    }
                }

                for (var24 = 0; var24 < 4; ++var24)
                {
                    int var25 = MathHelper.floor_double(x + ((double)(var24 % 2) - 0.5D) * 0.8D);
                    int var26 = MathHelper.floor_double(y);
                    int var20 = MathHelper.floor_double(z + ((double)(var24 / 2) - 0.5D) * 0.8D);
                    if (world.getBlockId(var25, var26, var20) == Block.SNOW.id)
                    {
                        world.setBlock(var25, var26, var20, 0);
                    }
                }

                if (passenger != null && passenger.dead)
                {
                    passenger = null;
                }

            }
        }

        public override void updatePassengerPosition()
        {
            if (passenger != null)
            {
                double var1 = java.lang.Math.cos((double)yaw * java.lang.Math.PI / 180.0D) * 0.4D;
                double var3 = java.lang.Math.sin((double)yaw * java.lang.Math.PI / 180.0D) * 0.4D;
                passenger.setPosition(x + var1, y + getPassengerRidingHeight() + passenger.getStandingEyeHeight(), z + var3);
            }
        }

        public override void writeNbt(NBTTagCompound var1)
        {
        }

        public override void readNbt(NBTTagCompound var1)
        {
        }

        public override float getShadowRadius()
        {
            return 0.0F;
        }

        public override bool interact(EntityPlayer var1)
        {
            if (passenger != null && passenger is EntityPlayer && passenger != var1)
            {
                return true;
            }
            else
            {
                if (!world.isRemote)
                {
                    var1.setVehicle(this);
                }

                return true;
            }
        }
    }

}