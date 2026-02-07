using betareborn.Blocks;
using betareborn.Items;
using betareborn.Materials;
using betareborn.NBT;
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
            setSize(1.5F, 0.6F);
            yOffset = height / 2.0F;
        }

        protected override bool canTriggerWalking()
        {
            return false;
        }

        protected override void entityInit()
        {
        }

        public override Box getCollisionBox(Entity var1)
        {
            return var1.boundingBox;
        }

        public override Box getBoundingBox()
        {
            return boundingBox;
        }

        public override bool canBePushed()
        {
            return true;
        }

        public EntityBoat(World var1, double var2, double var4, double var6) : this(var1)
        {
            setPosition(var2, var4 + (double)yOffset, var6);
            motionX = 0.0D;
            motionY = 0.0D;
            motionZ = 0.0D;
            prevPosX = var2;
            prevPosY = var4;
            prevPosZ = var6;
        }

        public override double getMountedYOffset()
        {
            return (double)height * 0.0D - (double)0.3F;
        }

        public override bool attackEntityFrom(Entity var1, int var2)
        {
            if (!worldObj.multiplayerWorld && !isDead)
            {
                boatRockDirection = -boatRockDirection;
                boatTimeSinceHit = 10;
                boatCurrentDamage += var2 * 10;
                setBeenAttacked();
                if (boatCurrentDamage > 40)
                {
                    if (riddenByEntity != null)
                    {
                        riddenByEntity.mountEntity(this);
                    }

                    int var3;
                    for (var3 = 0; var3 < 3; ++var3)
                    {
                        dropItemWithOffset(Block.PLANKS.id, 1, 0.0F);
                    }

                    for (var3 = 0; var3 < 2; ++var3)
                    {
                        dropItemWithOffset(Item.stick.id, 1, 0.0F);
                    }

                    setEntityDead();
                }

                return true;
            }
            else
            {
                return true;
            }
        }

        public override void performHurtAnimation()
        {
            boatRockDirection = -boatRockDirection;
            boatTimeSinceHit = 10;
            boatCurrentDamage += boatCurrentDamage * 10;
        }

        public override bool canBeCollidedWith()
        {
            return !isDead;
        }

        public override void setPositionAndRotation2(double var1, double var3, double var5, float var7, float var8, int var9)
        {
            field_9393_e = var1;
            field_9392_f = var3;
            field_9391_g = var5;
            field_9390_h = (double)var7;
            field_9389_i = (double)var8;
            field_9394_d = var9 + 4;
            motionX = field_9388_j;
            motionY = field_9387_k;
            motionZ = field_9386_l;
        }

        public override void setVelocity(double var1, double var3, double var5)
        {
            field_9388_j = motionX = var1;
            field_9387_k = motionY = var3;
            field_9386_l = motionZ = var5;
        }

        public override void onUpdate()
        {
            base.onUpdate();
            if (boatTimeSinceHit > 0)
            {
                --boatTimeSinceHit;
            }

            if (boatCurrentDamage > 0)
            {
                --boatCurrentDamage;
            }

            prevPosX = posX;
            prevPosY = posY;
            prevPosZ = posZ;
            byte var1 = 5;
            double var2 = 0.0D;

            for (int var4 = 0; var4 < var1; ++var4)
            {
                double var5 = boundingBox.minY + (boundingBox.maxY - boundingBox.minY) * (double)(var4 + 0) / (double)var1 - 0.125D;
                double var7 = boundingBox.minY + (boundingBox.maxY - boundingBox.minY) * (double)(var4 + 1) / (double)var1 - 0.125D;
                Box var9 = Box.createCached(boundingBox.minX, var5, boundingBox.minZ, boundingBox.maxX, var7, boundingBox.maxZ);
                if (worldObj.isAABBInMaterial(var9, Material.WATER))
                {
                    var2 += 1.0D / (double)var1;
                }
            }

            double var6;
            double var8;
            double var10;
            double var21;
            if (worldObj.multiplayerWorld)
            {
                if (field_9394_d > 0)
                {
                    var21 = posX + (field_9393_e - posX) / (double)field_9394_d;
                    var6 = posY + (field_9392_f - posY) / (double)field_9394_d;
                    var8 = posZ + (field_9391_g - posZ) / (double)field_9394_d;

                    for (var10 = field_9390_h - (double)rotationYaw; var10 < -180.0D; var10 += 360.0D)
                    {
                    }

                    while (var10 >= 180.0D)
                    {
                        var10 -= 360.0D;
                    }

                    rotationYaw = (float)((double)rotationYaw + var10 / (double)field_9394_d);
                    rotationPitch = (float)((double)rotationPitch + (field_9389_i - (double)rotationPitch) / (double)field_9394_d);
                    --field_9394_d;
                    setPosition(var21, var6, var8);
                    setRotation(rotationYaw, rotationPitch);
                }
                else
                {
                    var21 = posX + motionX;
                    var6 = posY + motionY;
                    var8 = posZ + motionZ;
                    setPosition(var21, var6, var8);
                    if (onGround)
                    {
                        motionX *= 0.5D;
                        motionY *= 0.5D;
                        motionZ *= 0.5D;
                    }

                    motionX *= (double)0.99F;
                    motionY *= (double)0.95F;
                    motionZ *= (double)0.99F;
                }

            }
            else
            {
                if (var2 < 1.0D)
                {
                    var21 = var2 * 2.0D - 1.0D;
                    motionY += (double)0.04F * var21;
                }
                else
                {
                    if (motionY < 0.0D)
                    {
                        motionY /= 2.0D;
                    }

                    motionY += (double)0.007F;
                }

                if (riddenByEntity != null)
                {
                    motionX += riddenByEntity.motionX * 0.2D;
                    motionZ += riddenByEntity.motionZ * 0.2D;
                }

                var21 = 0.4D;
                if (motionX < -var21)
                {
                    motionX = -var21;
                }

                if (motionX > var21)
                {
                    motionX = var21;
                }

                if (motionZ < -var21)
                {
                    motionZ = -var21;
                }

                if (motionZ > var21)
                {
                    motionZ = var21;
                }

                if (onGround)
                {
                    motionX *= 0.5D;
                    motionY *= 0.5D;
                    motionZ *= 0.5D;
                }

                moveEntity(motionX, motionY, motionZ);
                var6 = java.lang.Math.sqrt(motionX * motionX + motionZ * motionZ);
                if (var6 > 0.15D)
                {
                    var8 = java.lang.Math.cos((double)rotationYaw * java.lang.Math.PI / 180.0D);
                    var10 = java.lang.Math.sin((double)rotationYaw * java.lang.Math.PI / 180.0D);

                    for (int var12 = 0; (double)var12 < 1.0D + var6 * 60.0D; ++var12)
                    {
                        double var13 = (double)(rand.nextFloat() * 2.0F - 1.0F);
                        double var15 = (double)(rand.nextInt(2) * 2 - 1) * 0.7D;
                        double var17;
                        double var19;
                        if (rand.nextBoolean())
                        {
                            var17 = posX - var8 * var13 * 0.8D + var10 * var15;
                            var19 = posZ - var10 * var13 * 0.8D - var8 * var15;
                            worldObj.addParticle("splash", var17, posY - 0.125D, var19, motionX, motionY, motionZ);
                        }
                        else
                        {
                            var17 = posX + var8 + var10 * var13 * 0.7D;
                            var19 = posZ + var10 - var8 * var13 * 0.7D;
                            worldObj.addParticle("splash", var17, posY - 0.125D, var19, motionX, motionY, motionZ);
                        }
                    }
                }

                if (isCollidedHorizontally && var6 > 0.15D)
                {
                    if (!worldObj.multiplayerWorld)
                    {
                        setEntityDead();

                        int var22;
                        for (var22 = 0; var22 < 3; ++var22)
                        {
                            dropItemWithOffset(Block.PLANKS.id, 1, 0.0F);
                        }

                        for (var22 = 0; var22 < 2; ++var22)
                        {
                            dropItemWithOffset(Item.stick.id, 1, 0.0F);
                        }
                    }
                }
                else
                {
                    motionX *= (double)0.99F;
                    motionY *= (double)0.95F;
                    motionZ *= (double)0.99F;
                }

                rotationPitch = 0.0F;
                var8 = (double)rotationYaw;
                var10 = prevPosX - posX;
                double var23 = prevPosZ - posZ;
                if (var10 * var10 + var23 * var23 > 0.001D)
                {
                    var8 = (double)((float)(java.lang.Math.atan2(var23, var10) * 180.0D / java.lang.Math.PI));
                }

                double var14;
                for (var14 = var8 - (double)rotationYaw; var14 >= 180.0D; var14 -= 360.0D)
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

                rotationYaw = (float)((double)rotationYaw + var14);
                setRotation(rotationYaw, rotationPitch);
                var var16 = worldObj.getEntitiesWithinAABBExcludingEntity(this, boundingBox.expand((double)0.2F, 0.0D, (double)0.2F));
                int var24;
                if (var16 != null && var16.Count > 0)
                {
                    for (var24 = 0; var24 < var16.Count; ++var24)
                    {
                        Entity var18 = var16[var24];
                        if (var18 != riddenByEntity && var18.canBePushed() && var18 is EntityBoat)
                        {
                            var18.applyEntityCollision(this);
                        }
                    }
                }

                for (var24 = 0; var24 < 4; ++var24)
                {
                    int var25 = MathHelper.floor_double(posX + ((double)(var24 % 2) - 0.5D) * 0.8D);
                    int var26 = MathHelper.floor_double(posY);
                    int var20 = MathHelper.floor_double(posZ + ((double)(var24 / 2) - 0.5D) * 0.8D);
                    if (worldObj.getBlockId(var25, var26, var20) == Block.SNOW.id)
                    {
                        worldObj.setBlockWithNotify(var25, var26, var20, 0);
                    }
                }

                if (riddenByEntity != null && riddenByEntity.isDead)
                {
                    riddenByEntity = null;
                }

            }
        }

        public override void updateRiderPosition()
        {
            if (riddenByEntity != null)
            {
                double var1 = java.lang.Math.cos((double)rotationYaw * java.lang.Math.PI / 180.0D) * 0.4D;
                double var3 = java.lang.Math.sin((double)rotationYaw * java.lang.Math.PI / 180.0D) * 0.4D;
                riddenByEntity.setPosition(posX + var1, posY + getMountedYOffset() + riddenByEntity.getYOffset(), posZ + var3);
            }
        }

        public override void writeEntityToNBT(NBTTagCompound var1)
        {
        }

        public override void readEntityFromNBT(NBTTagCompound var1)
        {
        }

        public override float getShadowSize()
        {
            return 0.0F;
        }

        public override bool interact(EntityPlayer var1)
        {
            if (riddenByEntity != null && riddenByEntity is EntityPlayer && riddenByEntity != var1)
            {
                return true;
            }
            else
            {
                if (!worldObj.multiplayerWorld)
                {
                    var1.mountEntity(this);
                }

                return true;
            }
        }
    }

}