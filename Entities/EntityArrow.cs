using betareborn.Blocks;
using betareborn.Items;
using betareborn.NBT;
using betareborn.Worlds;
using java.lang;

namespace betareborn.Entities
{
    public class EntityArrow : Entity
    {
        public static readonly new Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(EntityArrow).TypeHandle);

        private int xTile = -1;
        private int yTile = -1;
        private int zTile = -1;
        private int inTile = 0;
        private int field_28019_h = 0;
        private bool inGround = false;
        public bool doesArrowBelongToPlayer = false;
        public int arrowShake = 0;
        public EntityLiving owner;
        private int ticksInGround;
        private int ticksInAir = 0;

        public EntityArrow(World var1) : base(var1)
        {
            setSize(0.5F, 0.5F);
        }

        public EntityArrow(World var1, double var2, double var4, double var6) : base(var1)
        {
            setSize(0.5F, 0.5F);
            setPosition(var2, var4, var6);
            yOffset = 0.0F;
        }

        public EntityArrow(World var1, EntityLiving var2) : base(var1)
        {
            owner = var2;
            doesArrowBelongToPlayer = var2 is EntityPlayer;
            setSize(0.5F, 0.5F);
            setPositionAndAnglesKeepPrevAngles(var2.posX, var2.posY + (double)var2.getEyeHeight(), var2.posZ, var2.rotationYaw, var2.rotationPitch);
            posX -= (double)(MathHelper.cos(rotationYaw / 180.0F * (float)java.lang.Math.PI) * 0.16F);
            posY -= (double)0.1F;
            posZ -= (double)(MathHelper.sin(rotationYaw / 180.0F * (float)java.lang.Math.PI) * 0.16F);
            setPosition(posX, posY, posZ);
            yOffset = 0.0F;
            motionX = (double)(-MathHelper.sin(rotationYaw / 180.0F * (float)java.lang.Math.PI) * MathHelper.cos(rotationPitch / 180.0F * (float)java.lang.Math.PI));
            motionZ = (double)(MathHelper.cos(rotationYaw / 180.0F * (float)java.lang.Math.PI) * MathHelper.cos(rotationPitch / 180.0F * (float)java.lang.Math.PI));
            motionY = (double)(-MathHelper.sin(rotationPitch / 180.0F * (float)java.lang.Math.PI));
            setArrowHeading(motionX, motionY, motionZ, 1.5F, 1.0F);
        }

        protected override void entityInit()
        {
        }

        public void setArrowHeading(double var1, double var3, double var5, float var7, float var8)
        {
            float var9 = MathHelper.sqrt_double(var1 * var1 + var3 * var3 + var5 * var5);
            var1 /= (double)var9;
            var3 /= (double)var9;
            var5 /= (double)var9;
            var1 += rand.nextGaussian() * (double)0.0075F * (double)var8;
            var3 += rand.nextGaussian() * (double)0.0075F * (double)var8;
            var5 += rand.nextGaussian() * (double)0.0075F * (double)var8;
            var1 *= (double)var7;
            var3 *= (double)var7;
            var5 *= (double)var7;
            motionX = var1;
            motionY = var3;
            motionZ = var5;
            float var10 = MathHelper.sqrt_double(var1 * var1 + var5 * var5);
            prevRotationYaw = rotationYaw = (float)(java.lang.Math.atan2(var1, var5) * 180.0D / (double)((float)java.lang.Math.PI));
            prevRotationPitch = rotationPitch = (float)(java.lang.Math.atan2(var3, (double)var10) * 180.0D / (double)((float)java.lang.Math.PI));
            ticksInGround = 0;
        }

        public override void setVelocity(double var1, double var3, double var5)
        {
            motionX = var1;
            motionY = var3;
            motionZ = var5;
            if (prevRotationPitch == 0.0F && prevRotationYaw == 0.0F)
            {
                float var7 = MathHelper.sqrt_double(var1 * var1 + var5 * var5);
                prevRotationYaw = rotationYaw = (float)(java.lang.Math.atan2(var1, var5) * 180.0D / (double)((float)java.lang.Math.PI));
                prevRotationPitch = rotationPitch = (float)(java.lang.Math.atan2(var3, (double)var7) * 180.0D / (double)((float)java.lang.Math.PI));
                prevRotationPitch = rotationPitch;
                prevRotationYaw = rotationYaw;
                setPositionAndAnglesKeepPrevAngles(posX, posY, posZ, rotationYaw, rotationPitch);
                ticksInGround = 0;
            }

        }

        public override void onUpdate()
        {
            base.onUpdate();
            if (prevRotationPitch == 0.0F && prevRotationYaw == 0.0F)
            {
                float var1 = MathHelper.sqrt_double(motionX * motionX + motionZ * motionZ);
                prevRotationYaw = rotationYaw = (float)(java.lang.Math.atan2(motionX, motionZ) * 180.0D / (double)((float)java.lang.Math.PI));
                prevRotationPitch = rotationPitch = (float)(java.lang.Math.atan2(motionY, (double)var1) * 180.0D / (double)((float)java.lang.Math.PI));
            }

            int var15 = worldObj.getBlockId(xTile, yTile, zTile);
            if (var15 > 0)
            {
                Block.BLOCKS[var15].updateBoundingBox(worldObj, xTile, yTile, zTile);
                Box var2 = Block.BLOCKS[var15].getCollisionShape(worldObj, xTile, yTile, zTile);
                if (var2 != null && var2.contains(Vec3D.createVector(posX, posY, posZ)))
                {
                    inGround = true;
                }
            }

            if (arrowShake > 0)
            {
                --arrowShake;
            }

            if (inGround)
            {
                var15 = worldObj.getBlockId(xTile, yTile, zTile);
                int var18 = worldObj.getBlockMeta(xTile, yTile, zTile);
                if (var15 == inTile && var18 == field_28019_h)
                {
                    ++ticksInGround;
                    if (ticksInGround == 1200)
                    {
                        setEntityDead();
                    }

                }
                else
                {
                    inGround = false;
                    motionX *= (double)(rand.nextFloat() * 0.2F);
                    motionY *= (double)(rand.nextFloat() * 0.2F);
                    motionZ *= (double)(rand.nextFloat() * 0.2F);
                    ticksInGround = 0;
                    ticksInAir = 0;
                }
            }
            else
            {
                ++ticksInAir;
                Vec3D var16 = Vec3D.createVector(posX, posY, posZ);
                Vec3D var17 = Vec3D.createVector(posX + motionX, posY + motionY, posZ + motionZ);
                MovingObjectPosition var3 = worldObj.func_28105_a(var16, var17, false, true);
                var16 = Vec3D.createVector(posX, posY, posZ);
                var17 = Vec3D.createVector(posX + motionX, posY + motionY, posZ + motionZ);
                if (var3 != null)
                {
                    var17 = Vec3D.createVector(var3.hitVec.xCoord, var3.hitVec.yCoord, var3.hitVec.zCoord);
                }

                Entity var4 = null;
                var var5 = worldObj.getEntitiesWithinAABBExcludingEntity(this, boundingBox.stretch(motionX, motionY, motionZ).expand(1.0D, 1.0D, 1.0D));
                double var6 = 0.0D;

                float var10;
                for (int var8 = 0; var8 < var5.Count; ++var8)
                {
                    Entity var9 = var5[var8];
                    if (var9.canBeCollidedWith() && (var9 != owner || ticksInAir >= 5))
                    {
                        var10 = 0.3F;
                        Box var11 = var9.boundingBox.expand((double)var10, (double)var10, (double)var10);
                        MovingObjectPosition var12 = var11.raycast(var16, var17);
                        if (var12 != null)
                        {
                            double var13 = var16.distanceTo(var12.hitVec);
                            if (var13 < var6 || var6 == 0.0D)
                            {
                                var4 = var9;
                                var6 = var13;
                            }
                        }
                    }
                }

                if (var4 != null)
                {
                    var3 = new MovingObjectPosition(var4);
                }

                float var19;
                if (var3 != null)
                {
                    if (var3.entityHit != null)
                    {
                        if (var3.entityHit.attackEntityFrom(owner, 4))
                        {
                            worldObj.playSoundAtEntity(this, "random.drr", 1.0F, 1.2F / (rand.nextFloat() * 0.2F + 0.9F));
                            setEntityDead();
                        }
                        else
                        {
                            motionX *= (double)-0.1F;
                            motionY *= (double)-0.1F;
                            motionZ *= (double)-0.1F;
                            rotationYaw += 180.0F;
                            prevRotationYaw += 180.0F;
                            ticksInAir = 0;
                        }
                    }
                    else
                    {
                        xTile = var3.blockX;
                        yTile = var3.blockY;
                        zTile = var3.blockZ;
                        inTile = worldObj.getBlockId(xTile, yTile, zTile);
                        field_28019_h = worldObj.getBlockMeta(xTile, yTile, zTile);
                        motionX = (double)((float)(var3.hitVec.xCoord - posX));
                        motionY = (double)((float)(var3.hitVec.yCoord - posY));
                        motionZ = (double)((float)(var3.hitVec.zCoord - posZ));
                        var19 = MathHelper.sqrt_double(motionX * motionX + motionY * motionY + motionZ * motionZ);
                        posX -= motionX / (double)var19 * (double)0.05F;
                        posY -= motionY / (double)var19 * (double)0.05F;
                        posZ -= motionZ / (double)var19 * (double)0.05F;
                        worldObj.playSoundAtEntity(this, "random.drr", 1.0F, 1.2F / (rand.nextFloat() * 0.2F + 0.9F));
                        inGround = true;
                        arrowShake = 7;
                    }
                }

                posX += motionX;
                posY += motionY;
                posZ += motionZ;
                var19 = MathHelper.sqrt_double(motionX * motionX + motionZ * motionZ);
                rotationYaw = (float)(java.lang.Math.atan2(motionX, motionZ) * 180.0D / (double)((float)java.lang.Math.PI));

                for (rotationPitch = (float)(java.lang.Math.atan2(motionY, (double)var19) * 180.0D / (double)((float)java.lang.Math.PI)); rotationPitch - prevRotationPitch < -180.0F; prevRotationPitch -= 360.0F)
                {
                }

                while (rotationPitch - prevRotationPitch >= 180.0F)
                {
                    prevRotationPitch += 360.0F;
                }

                while (rotationYaw - prevRotationYaw < -180.0F)
                {
                    prevRotationYaw -= 360.0F;
                }

                while (rotationYaw - prevRotationYaw >= 180.0F)
                {
                    prevRotationYaw += 360.0F;
                }

                rotationPitch = prevRotationPitch + (rotationPitch - prevRotationPitch) * 0.2F;
                rotationYaw = prevRotationYaw + (rotationYaw - prevRotationYaw) * 0.2F;
                float var20 = 0.99F;
                var10 = 0.03F;
                if (isInWater())
                {
                    for (int var21 = 0; var21 < 4; ++var21)
                    {
                        float var22 = 0.25F;
                        worldObj.addParticle("bubble", posX - motionX * (double)var22, posY - motionY * (double)var22, posZ - motionZ * (double)var22, motionX, motionY, motionZ);
                    }

                    var20 = 0.8F;
                }

                motionX *= (double)var20;
                motionY *= (double)var20;
                motionZ *= (double)var20;
                motionY -= (double)var10;
                setPosition(posX, posY, posZ);
            }
        }

        public override void writeEntityToNBT(NBTTagCompound var1)
        {
            var1.setShort("xTile", (short)xTile);
            var1.setShort("yTile", (short)yTile);
            var1.setShort("zTile", (short)zTile);
            var1.setByte("inTile", (sbyte)inTile);
            var1.setByte("inData", (sbyte)field_28019_h);
            var1.setByte("shake", (sbyte)arrowShake);
            var1.setByte("inGround", (sbyte)(inGround ? 1 : 0));
            var1.setBoolean("player", doesArrowBelongToPlayer);
        }

        public override void readEntityFromNBT(NBTTagCompound var1)
        {
            xTile = var1.getShort("xTile");
            yTile = var1.getShort("yTile");
            zTile = var1.getShort("zTile");
            inTile = var1.getByte("inTile") & 255;
            field_28019_h = var1.getByte("inData") & 255;
            arrowShake = var1.getByte("shake") & 255;
            inGround = var1.getByte("inGround") == 1;
            doesArrowBelongToPlayer = var1.getBoolean("player");
        }

        public override void onCollideWithPlayer(EntityPlayer var1)
        {
            if (!worldObj.multiplayerWorld)
            {
                if (inGround && doesArrowBelongToPlayer && arrowShake <= 0 && var1.inventory.addItemStackToInventory(new ItemStack(Item.arrow, 1)))
                {
                    worldObj.playSoundAtEntity(this, "random.pop", 0.2F, ((rand.nextFloat() - rand.nextFloat()) * 0.7F + 1.0F) * 2.0F);
                    var1.onItemPickup(this, 1);
                    setEntityDead();
                }

            }
        }

        public override float getShadowSize()
        {
            return 0.0F;
        }
    }

}