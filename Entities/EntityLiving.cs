using betareborn.Blocks;
using betareborn.Items;
using betareborn.Materials;
using betareborn.NBT;
using betareborn.Worlds;
using java.lang;

namespace betareborn.Entities
{
    public abstract class EntityLiving : Entity
    {
        public static readonly new Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(EntityLiving).TypeHandle);
        public int heartsHalvesLife = 20;
        public float field_9365_p;
        public float field_9363_r;
        public float renderYawOffset = 0.0F;
        public float prevRenderYawOffset = 0.0F;
        protected float field_9362_u;
        protected float field_9361_v;
        protected float field_9360_w;
        protected float field_9359_x;
        protected bool field_9358_y = true;
        protected string texture = "/mob/char.png";
        protected bool field_9355_A = true;
        protected float field_9353_B = 0.0F;
        protected string field_9351_C = null;
        protected float field_9349_D = 1.0F;
        protected int scoreValue = 0;
        protected float field_9345_F = 0.0F;
        public bool isMultiplayerEntity = false;
        public float prevSwingProgress;
        public float swingProgress;
        public int health = 10;
        public int prevHealth;
        private int livingSoundTime;
        public int hurtTime;
        public int maxHurtTime;
        public float attackedAtYaw = 0.0F;
        public int deathTime = 0;
        public int attackTime = 0;
        public float cameraPitch;
        public float field_9328_R;
        protected bool unused_flag = false;
        public int field_9326_T = -1;
        public float field_9325_U = (float)(java.lang.Math.random() * (double)0.9F + (double)0.1F);
        public float field_705_Q;
        public float field_704_R;
        public float field_703_S;
        protected int newPosRotationIncrements;
        protected double newPosX;
        protected double newPosY;
        protected double newPosZ;
        protected double newRotationYaw;
        protected double newRotationPitch;
        float field_9348_ae = 0.0F;
        protected int field_9346_af = 0;
        protected int entityAge = 0;
        protected float moveStrafing;
        protected float moveForward;
        protected float randomYawVelocity;
        protected bool isJumping = false;
        protected float defaultPitch = 0.0F;
        protected float moveSpeed = 0.7F;
        private Entity currentTarget;
        protected int numTicksToChaseTarget = 0;

        public EntityLiving(World var1) : base(var1)
        {
            preventEntitySpawning = true;
            field_9363_r = (float)(java.lang.Math.random() + 1.0D) * 0.01F;
            setPosition(posX, posY, posZ);
            field_9365_p = (float)java.lang.Math.random() * 12398.0F;
            rotationYaw = (float)(java.lang.Math.random() * (double)((float)java.lang.Math.PI) * 2.0D);
            stepHeight = 0.5F;
        }

        protected override void entityInit()
        {
        }

        public bool canEntityBeSeen(Entity var1)
        {
            return worldObj.rayTraceBlocks(Vec3D.createVector(posX, posY + (double)getEyeHeight(), posZ), Vec3D.createVector(var1.posX, var1.posY + (double)var1.getEyeHeight(), var1.posZ)) == null;
        }

        public override string getEntityTexture()
        {
            return texture;
        }

        public override bool canBeCollidedWith()
        {
            return !isDead;
        }

        public override bool canBePushed()
        {
            return !isDead;
        }

        public override float getEyeHeight()
        {
            return height * 0.85F;
        }

        public virtual int getTalkInterval()
        {
            return 80;
        }

        public void playLivingSound()
        {
            string var1 = getLivingSound();
            if (var1 != null)
            {
                worldObj.playSoundAtEntity(this, var1, getSoundVolume(), (rand.nextFloat() - rand.nextFloat()) * 0.2F + 1.0F);
            }

        }

        public override void onEntityUpdate()
        {
            prevSwingProgress = swingProgress;
            base.onEntityUpdate();
            if (rand.nextInt(1000) < livingSoundTime++)
            {
                livingSoundTime = -getTalkInterval();
                playLivingSound();
            }

            if (isEntityAlive() && isEntityInsideOpaqueBlock())
            {
                attackEntityFrom(null, 1);
            }

            if (isImmuneToFire || worldObj.multiplayerWorld)
            {
                fire = 0;
            }

            int var1;
            if (isEntityAlive() && isInsideOfMaterial(Material.WATER) && !canBreatheUnderwater())
            {
                --air;
                if (air == -20)
                {
                    air = 0;

                    for (var1 = 0; var1 < 8; ++var1)
                    {
                        float var2 = rand.nextFloat() - rand.nextFloat();
                        float var3 = rand.nextFloat() - rand.nextFloat();
                        float var4 = rand.nextFloat() - rand.nextFloat();
                        worldObj.addParticle("bubble", posX + (double)var2, posY + (double)var3, posZ + (double)var4, motionX, motionY, motionZ);
                    }

                    attackEntityFrom(null, 2);
                }

                fire = 0;
            }
            else
            {
                air = maxAir;
            }

            cameraPitch = field_9328_R;
            if (attackTime > 0)
            {
                --attackTime;
            }

            if (hurtTime > 0)
            {
                --hurtTime;
            }

            if (heartsLife > 0)
            {
                --heartsLife;
            }

            if (health <= 0)
            {
                ++deathTime;
                if (deathTime > 20)
                {
                    onEntityDeath();
                    setEntityDead();

                    for (var1 = 0; var1 < 20; ++var1)
                    {
                        double var8 = rand.nextGaussian() * 0.02D;
                        double var9 = rand.nextGaussian() * 0.02D;
                        double var6 = rand.nextGaussian() * 0.02D;
                        worldObj.addParticle("explode", posX + (double)(rand.nextFloat() * width * 2.0F) - (double)width, posY + (double)(rand.nextFloat() * height), posZ + (double)(rand.nextFloat() * width * 2.0F) - (double)width, var8, var9, var6);
                    }
                }
            }

            field_9359_x = field_9360_w;
            prevRenderYawOffset = renderYawOffset;
            prevRotationYaw = rotationYaw;
            prevRotationPitch = rotationPitch;
        }

        public void animateSpawn()
        {
            for (int var1 = 0; var1 < 20; ++var1)
            {
                double var2 = rand.nextGaussian() * 0.02D;
                double var4 = rand.nextGaussian() * 0.02D;
                double var6 = rand.nextGaussian() * 0.02D;
                double var8 = 10.0D;
                worldObj.addParticle("explode", posX + (double)(rand.nextFloat() * width * 2.0F) - (double)width - var2 * var8, posY + (double)(rand.nextFloat() * height) - var4 * var8, posZ + (double)(rand.nextFloat() * width * 2.0F) - (double)width - var6 * var8, var2, var4, var6);
            }

        }

        public override void updateRidden()
        {
            base.updateRidden();
            field_9362_u = field_9361_v;
            field_9361_v = 0.0F;
        }

        public override void setPositionAndRotation2(double var1, double var3, double var5, float var7, float var8, int var9)
        {
            yOffset = 0.0F;
            newPosX = var1;
            newPosY = var3;
            newPosZ = var5;
            newRotationYaw = (double)var7;
            newRotationPitch = (double)var8;
            newPosRotationIncrements = var9;
        }

        public override void onUpdate()
        {
            base.onUpdate();
            onLivingUpdate();
            double var1 = posX - prevPosX;
            double var3 = posZ - prevPosZ;
            float var5 = MathHelper.sqrt_double(var1 * var1 + var3 * var3);
            float var6 = renderYawOffset;
            float var7 = 0.0F;
            field_9362_u = field_9361_v;
            float var8 = 0.0F;
            if (var5 > 0.05F)
            {
                var8 = 1.0F;
                var7 = var5 * 3.0F;
                var6 = (float)java.lang.Math.atan2(var3, var1) * 180.0F / (float)java.lang.Math.PI - 90.0F;
            }

            if (swingProgress > 0.0F)
            {
                var6 = rotationYaw;
            }

            if (!onGround)
            {
                var8 = 0.0F;
            }

            field_9361_v += (var8 - field_9361_v) * 0.3F;

            float var9;
            for (var9 = var6 - renderYawOffset; var9 < -180.0F; var9 += 360.0F)
            {
            }

            while (var9 >= 180.0F)
            {
                var9 -= 360.0F;
            }

            renderYawOffset += var9 * 0.3F;

            float var10;
            for (var10 = rotationYaw - renderYawOffset; var10 < -180.0F; var10 += 360.0F)
            {
            }

            while (var10 >= 180.0F)
            {
                var10 -= 360.0F;
            }

            bool var11 = var10 < -90.0F || var10 >= 90.0F;
            if (var10 < -75.0F)
            {
                var10 = -75.0F;
            }

            if (var10 >= 75.0F)
            {
                var10 = 75.0F;
            }

            renderYawOffset = rotationYaw - var10;
            if (var10 * var10 > 2500.0F)
            {
                renderYawOffset += var10 * 0.2F;
            }

            if (var11)
            {
                var7 *= -1.0F;
            }

            while (rotationYaw - prevRotationYaw < -180.0F)
            {
                prevRotationYaw -= 360.0F;
            }

            while (rotationYaw - prevRotationYaw >= 180.0F)
            {
                prevRotationYaw += 360.0F;
            }

            while (renderYawOffset - prevRenderYawOffset < -180.0F)
            {
                prevRenderYawOffset -= 360.0F;
            }

            while (renderYawOffset - prevRenderYawOffset >= 180.0F)
            {
                prevRenderYawOffset += 360.0F;
            }

            while (rotationPitch - prevRotationPitch < -180.0F)
            {
                prevRotationPitch -= 360.0F;
            }

            while (rotationPitch - prevRotationPitch >= 180.0F)
            {
                prevRotationPitch += 360.0F;
            }

            field_9360_w += var7;
        }

        protected override void setSize(float var1, float var2)
        {
            base.setSize(var1, var2);
        }

        public virtual void heal(int var1)
        {
            if (health > 0)
            {
                health += var1;
                if (health > 20)
                {
                    health = 20;
                }

                heartsLife = heartsHalvesLife / 2;
            }
        }

        public override bool attackEntityFrom(Entity var1, int var2)
        {
            if (worldObj.multiplayerWorld)
            {
                return false;
            }
            else
            {
                entityAge = 0;
                if (health <= 0)
                {
                    return false;
                }
                else
                {
                    field_704_R = 1.5F;
                    bool var3 = true;
                    if ((float)heartsLife > (float)heartsHalvesLife / 2.0F)
                    {
                        if (var2 <= field_9346_af)
                        {
                            return false;
                        }

                        damageEntity(var2 - field_9346_af);
                        field_9346_af = var2;
                        var3 = false;
                    }
                    else
                    {
                        field_9346_af = var2;
                        prevHealth = health;
                        heartsLife = heartsHalvesLife;
                        damageEntity(var2);
                        hurtTime = maxHurtTime = 10;
                    }

                    attackedAtYaw = 0.0F;
                    if (var3)
                    {
                        worldObj.func_9425_a(this, (byte)2);
                        setBeenAttacked();
                        if (var1 != null)
                        {
                            double var4 = var1.posX - posX;

                            double var6;
                            for (var6 = var1.posZ - posZ; var4 * var4 + var6 * var6 < 1.0E-4D; var6 = (java.lang.Math.random() - java.lang.Math.random()) * 0.01D)
                            {
                                var4 = (java.lang.Math.random() - java.lang.Math.random()) * 0.01D;
                            }

                            attackedAtYaw = (float)(java.lang.Math.atan2(var6, var4) * 180.0D / (double)((float)java.lang.Math.PI)) - rotationYaw;
                            knockBack(var1, var2, var4, var6);
                        }
                        else
                        {
                            attackedAtYaw = (float)((int)(java.lang.Math.random() * 2.0D) * 180);
                        }
                    }

                    if (health <= 0)
                    {
                        if (var3)
                        {
                            worldObj.playSoundAtEntity(this, getDeathSound(), getSoundVolume(), (rand.nextFloat() - rand.nextFloat()) * 0.2F + 1.0F);
                        }

                        onDeath(var1);
                    }
                    else if (var3)
                    {
                        worldObj.playSoundAtEntity(this, getHurtSound(), getSoundVolume(), (rand.nextFloat() - rand.nextFloat()) * 0.2F + 1.0F);
                    }

                    return true;
                }
            }
        }

        public override void performHurtAnimation()
        {
            hurtTime = maxHurtTime = 10;
            attackedAtYaw = 0.0F;
        }

        protected virtual void damageEntity(int var1)
        {
            health -= var1;
        }

        protected virtual float getSoundVolume()
        {
            return 1.0F;
        }

        protected virtual string getLivingSound()
        {
            return null;
        }

        protected virtual string getHurtSound()
        {
            return "random.hurt";
        }

        protected virtual string getDeathSound()
        {
            return "random.hurt";
        }

        public void knockBack(Entity var1, int var2, double var3, double var5)
        {
            float var7 = MathHelper.sqrt_double(var3 * var3 + var5 * var5);
            float var8 = 0.4F;
            motionX /= 2.0D;
            motionY /= 2.0D;
            motionZ /= 2.0D;
            motionX -= var3 / (double)var7 * (double)var8;
            motionY += (double)0.4F;
            motionZ -= var5 / (double)var7 * (double)var8;
            if (motionY > (double)0.4F)
            {
                motionY = (double)0.4F;
            }

        }

        public virtual void onDeath(Entity var1)
        {
            if (scoreValue >= 0 && var1 != null)
            {
                var1.addToPlayerScore(this, scoreValue);
            }

            if (var1 != null)
            {
                var1.onKillEntity(this);
            }

            unused_flag = true;
            if (!worldObj.multiplayerWorld)
            {
                dropFewItems();
            }

            worldObj.func_9425_a(this, (byte)3);
        }

        protected virtual void dropFewItems()
        {
            int var1 = getDropItemId();
            if (var1 > 0)
            {
                int var2 = rand.nextInt(3);

                for (int var3 = 0; var3 < var2; ++var3)
                {
                    dropItem(var1, 1);
                }
            }

        }

        protected virtual int getDropItemId()
        {
            return 0;
        }

        protected override void fall(float var1)
        {
            base.fall(var1);
            int var2 = (int)java.lang.Math.ceil((double)(var1 - 3.0F));
            if (var2 > 0)
            {
                attackEntityFrom(null, var2);
                int var3 = worldObj.getBlockId(MathHelper.floor_double(posX), MathHelper.floor_double(posY - (double)0.2F - (double)yOffset), MathHelper.floor_double(posZ));
                if (var3 > 0)
                {
                    BlockSoundGroup var4 = Block.BLOCKS[var3].soundGroup;
                    worldObj.playSoundAtEntity(this, var4.func_1145_d(), var4.getVolume() * 0.5F, var4.getPitch() * (12.0F / 16.0F));
                }
            }

        }

        public virtual void moveEntityWithHeading(float var1, float var2)
        {
            double var3;
            if (isInWater())
            {
                var3 = posY;
                moveFlying(var1, var2, 0.02F);
                moveEntity(motionX, motionY, motionZ);
                motionX *= (double)0.8F;
                motionY *= (double)0.8F;
                motionZ *= (double)0.8F;
                motionY -= 0.02D;
                if (isCollidedHorizontally && isOffsetPositionInLiquid(motionX, motionY + (double)0.6F - posY + var3, motionZ))
                {
                    motionY = (double)0.3F;
                }
            }
            else if (handleLavaMovement())
            {
                var3 = posY;
                moveFlying(var1, var2, 0.02F);
                moveEntity(motionX, motionY, motionZ);
                motionX *= 0.5D;
                motionY *= 0.5D;
                motionZ *= 0.5D;
                motionY -= 0.02D;
                if (isCollidedHorizontally && isOffsetPositionInLiquid(motionX, motionY + (double)0.6F - posY + var3, motionZ))
                {
                    motionY = (double)0.3F;
                }
            }
            else
            {
                float var8 = 0.91F;
                if (onGround)
                {
                    var8 = 546.0F * 0.1F * 0.1F * 0.1F;
                    int var4 = worldObj.getBlockId(MathHelper.floor_double(posX), MathHelper.floor_double(boundingBox.minY) - 1, MathHelper.floor_double(posZ));
                    if (var4 > 0)
                    {
                        var8 = Block.BLOCKS[var4].slipperiness * 0.91F;
                    }
                }

                float var9 = 0.16277136F / (var8 * var8 * var8);
                moveFlying(var1, var2, onGround ? 0.1F * var9 : 0.02F);
                var8 = 0.91F;
                if (onGround)
                {
                    var8 = 546.0F * 0.1F * 0.1F * 0.1F;
                    int var5 = worldObj.getBlockId(MathHelper.floor_double(posX), MathHelper.floor_double(boundingBox.minY) - 1, MathHelper.floor_double(posZ));
                    if (var5 > 0)
                    {
                        var8 = Block.BLOCKS[var5].slipperiness * 0.91F;
                    }
                }

                if (isOnLadder())
                {
                    float var10 = 0.15F;
                    if (motionX < (double)(-var10))
                    {
                        motionX = (double)(-var10);
                    }

                    if (motionX > (double)var10)
                    {
                        motionX = (double)var10;
                    }

                    if (motionZ < (double)(-var10))
                    {
                        motionZ = (double)(-var10);
                    }

                    if (motionZ > (double)var10)
                    {
                        motionZ = (double)var10;
                    }

                    fallDistance = 0.0F;
                    if (motionY < -0.15D)
                    {
                        motionY = -0.15D;
                    }

                    if (isSneaking() && motionY < 0.0D)
                    {
                        motionY = 0.0D;
                    }
                }

                moveEntity(motionX, motionY, motionZ);
                if (isCollidedHorizontally && isOnLadder())
                {
                    motionY = 0.2D;
                }

                motionY -= 0.08D;
                motionY *= (double)0.98F;
                motionX *= (double)var8;
                motionZ *= (double)var8;
            }

            field_705_Q = field_704_R;
            var3 = posX - prevPosX;
            double var11 = posZ - prevPosZ;
            float var7 = MathHelper.sqrt_double(var3 * var3 + var11 * var11) * 4.0F;
            if (var7 > 1.0F)
            {
                var7 = 1.0F;
            }

            field_704_R += (var7 - field_704_R) * 0.4F;
            field_703_S += field_704_R;
        }

        public virtual bool isOnLadder()
        {
            int var1 = MathHelper.floor_double(posX);
            int var2 = MathHelper.floor_double(boundingBox.minY);
            int var3 = MathHelper.floor_double(posZ);
            return worldObj.getBlockId(var1, var2, var3) == Block.LADDER.id;
        }

        public override void writeEntityToNBT(NBTTagCompound var1)
        {
            var1.setShort("Health", (short)health);
            var1.setShort("HurtTime", (short)hurtTime);
            var1.setShort("DeathTime", (short)deathTime);
            var1.setShort("AttackTime", (short)attackTime);
        }

        public override void readEntityFromNBT(NBTTagCompound var1)
        {
            health = var1.getShort("Health");
            if (!var1.hasKey("Health"))
            {
                health = 10;
            }

            hurtTime = var1.getShort("HurtTime");
            deathTime = var1.getShort("DeathTime");
            attackTime = var1.getShort("AttackTime");
        }

        public override bool isEntityAlive()
        {
            return !isDead && health > 0;
        }

        public virtual bool canBreatheUnderwater()
        {
            return false;
        }

        public virtual void onLivingUpdate()
        {
            if (newPosRotationIncrements > 0)
            {
                double var1 = posX + (newPosX - posX) / (double)newPosRotationIncrements;
                double var3 = posY + (newPosY - posY) / (double)newPosRotationIncrements;
                double var5 = posZ + (newPosZ - posZ) / (double)newPosRotationIncrements;

                double var7;
                for (var7 = newRotationYaw - (double)rotationYaw; var7 < -180.0D; var7 += 360.0D)
                {
                }

                while (var7 >= 180.0D)
                {
                    var7 -= 360.0D;
                }

                rotationYaw = (float)((double)rotationYaw + var7 / (double)newPosRotationIncrements);
                rotationPitch = (float)((double)rotationPitch + (newRotationPitch - (double)rotationPitch) / (double)newPosRotationIncrements);
                --newPosRotationIncrements;
                setPosition(var1, var3, var5);
                setRotation(rotationYaw, rotationPitch);
                var var9 = worldObj.getCollidingBoundingBoxes(this, boundingBox.contract(1.0D / 32.0D, 0.0D, 1.0D / 32.0D));
                if (var9.Count > 0)
                {
                    double var10 = 0.0D;

                    for (int var12 = 0; var12 < var9.Count; ++var12)
                    {
                        Box var13 = var9[var12];
                        if (var13.maxY > var10)
                        {
                            var10 = var13.maxY;
                        }
                    }

                    var3 += var10 - boundingBox.minY;
                    setPosition(var1, var3, var5);
                }
            }

            if (isMovementBlocked())
            {
                isJumping = false;
                moveStrafing = 0.0F;
                moveForward = 0.0F;
                randomYawVelocity = 0.0F;
            }
            else if (!isMultiplayerEntity)
            {
                updatePlayerActionState();
            }

            bool var14 = isInWater();
            bool var2 = handleLavaMovement();
            if (isJumping)
            {
                if (var14)
                {
                    motionY += (double)0.04F;
                }
                else if (var2)
                {
                    motionY += (double)0.04F;
                }
                else if (onGround)
                {
                    jump();
                }
            }

            moveStrafing *= 0.98F;
            moveForward *= 0.98F;
            randomYawVelocity *= 0.9F;
            moveEntityWithHeading(moveStrafing, moveForward);
            var var15 = worldObj.getEntitiesWithinAABBExcludingEntity(this, boundingBox.expand((double)0.2F, 0.0D, (double)0.2F));
            if (var15 != null && var15.Count > 0)
            {
                for (int var4 = 0; var4 < var15.Count; ++var4)
                {
                    Entity var16 = var15[var4];
                    if (var16.canBePushed())
                    {
                        var16.applyEntityCollision(this);
                    }
                }
            }

        }

        protected virtual bool isMovementBlocked()
        {
            return health <= 0;
        }

        protected virtual void jump()
        {
            motionY = (double)0.42F;
        }

        protected virtual bool canDespawn()
        {
            return true;
        }

        protected void func_27021_X()
        {
            EntityPlayer var1 = worldObj.getClosestPlayerToEntity(this, -1.0D);
            if (canDespawn() && var1 != null)
            {
                double var2 = var1.posX - posX;
                double var4 = var1.posY - posY;
                double var6 = var1.posZ - posZ;
                double var8 = var2 * var2 + var4 * var4 + var6 * var6;
                if (var8 > 16384.0D)
                {
                    setEntityDead();
                }

                if (entityAge > 600 && rand.nextInt(800) == 0)
                {
                    if (var8 < 1024.0D)
                    {
                        entityAge = 0;
                    }
                    else
                    {
                        setEntityDead();
                    }
                }
            }

        }

        public virtual void updatePlayerActionState()
        {
            ++entityAge;
            EntityPlayer var1 = worldObj.getClosestPlayerToEntity(this, -1.0D);
            func_27021_X();
            moveStrafing = 0.0F;
            moveForward = 0.0F;
            float var2 = 8.0F;
            if (rand.nextFloat() < 0.02F)
            {
                var1 = worldObj.getClosestPlayerToEntity(this, (double)var2);
                if (var1 != null)
                {
                    currentTarget = var1;
                    numTicksToChaseTarget = 10 + rand.nextInt(20);
                }
                else
                {
                    randomYawVelocity = (rand.nextFloat() - 0.5F) * 20.0F;
                }
            }

            if (currentTarget != null)
            {
                faceEntity(currentTarget, 10.0F, (float)func_25026_x());
                if (numTicksToChaseTarget-- <= 0 || currentTarget.isDead || currentTarget.getDistanceSqToEntity(this) > (double)(var2 * var2))
                {
                    currentTarget = null;
                }
            }
            else
            {
                if (rand.nextFloat() < 0.05F)
                {
                    randomYawVelocity = (rand.nextFloat() - 0.5F) * 20.0F;
                }

                rotationYaw += randomYawVelocity;
                rotationPitch = defaultPitch;
            }

            bool var3 = isInWater();
            bool var4 = handleLavaMovement();
            if (var3 || var4)
            {
                isJumping = rand.nextFloat() < 0.8F;
            }

        }

        protected virtual int func_25026_x()
        {
            return 40;
        }

        public void faceEntity(Entity var1, float var2, float var3)
        {
            double var4 = var1.posX - posX;
            double var8 = var1.posZ - posZ;
            double var6;
            if (var1 is EntityLiving)
            {
                EntityLiving var10 = (EntityLiving)var1;
                var6 = posY + (double)getEyeHeight() - (var10.posY + (double)var10.getEyeHeight());
            }
            else
            {
                var6 = (var1.boundingBox.minY + var1.boundingBox.maxY) / 2.0D - (posY + (double)getEyeHeight());
            }

            double var14 = (double)MathHelper.sqrt_double(var4 * var4 + var8 * var8);
            float var12 = (float)(java.lang.Math.atan2(var8, var4) * 180.0D / (double)((float)java.lang.Math.PI)) - 90.0F;
            float var13 = (float)(-(java.lang.Math.atan2(var6, var14) * 180.0D / (double)((float)java.lang.Math.PI)));
            rotationPitch = -updateRotation(rotationPitch, var13, var3);
            rotationYaw = updateRotation(rotationYaw, var12, var2);
        }

        public bool hasCurrentTarget()
        {
            return currentTarget != null;
        }

        public Entity getCurrentTarget()
        {
            return currentTarget;
        }

        private float updateRotation(float var1, float var2, float var3)
        {
            float var4;
            for (var4 = var2 - var1; var4 < -180.0F; var4 += 360.0F)
            {
            }

            while (var4 >= 180.0F)
            {
                var4 -= 360.0F;
            }

            if (var4 > var3)
            {
                var4 = var3;
            }

            if (var4 < -var3)
            {
                var4 = -var3;
            }

            return var1 + var4;
        }

        public void onEntityDeath()
        {
        }

        public virtual bool canSpawn()
        {
            return worldObj.checkIfAABBIsClear(boundingBox) && worldObj.getCollidingBoundingBoxes(this, boundingBox).Count == 0 && !worldObj.getIsAnyLiquid(boundingBox);
        }

        protected override void kill()
        {
            attackEntityFrom(null, 4);
        }

        public float getSwingProgress(float var1)
        {
            float var2 = swingProgress - prevSwingProgress;
            if (var2 < 0.0F)
            {
                ++var2;
            }

            return prevSwingProgress + var2 * var1;
        }

        public Vec3D getPosition(float var1)
        {
            if (var1 == 1.0F)
            {
                return Vec3D.createVector(posX, posY, posZ);
            }
            else
            {
                double var2 = prevPosX + (posX - prevPosX) * (double)var1;
                double var4 = prevPosY + (posY - prevPosY) * (double)var1;
                double var6 = prevPosZ + (posZ - prevPosZ) * (double)var1;
                return Vec3D.createVector(var2, var4, var6);
            }
        }

        public override Vec3D getLookVec()
        {
            return getLook(1.0F);
        }

        public Vec3D getLook(float var1)
        {
            float var2;
            float var3;
            float var4;
            float var5;
            if (var1 == 1.0F)
            {
                var2 = MathHelper.cos(-rotationYaw * ((float)java.lang.Math.PI / 180.0F) - (float)java.lang.Math.PI);
                var3 = MathHelper.sin(-rotationYaw * ((float)java.lang.Math.PI / 180.0F) - (float)java.lang.Math.PI);
                var4 = -MathHelper.cos(-rotationPitch * ((float)java.lang.Math.PI / 180.0F));
                var5 = MathHelper.sin(-rotationPitch * ((float)java.lang.Math.PI / 180.0F));
                return Vec3D.createVector((double)(var3 * var4), (double)var5, (double)(var2 * var4));
            }
            else
            {
                var2 = prevRotationPitch + (rotationPitch - prevRotationPitch) * var1;
                var3 = prevRotationYaw + (rotationYaw - prevRotationYaw) * var1;
                var4 = MathHelper.cos(-var3 * ((float)java.lang.Math.PI / 180.0F) - (float)java.lang.Math.PI);
                var5 = MathHelper.sin(-var3 * ((float)java.lang.Math.PI / 180.0F) - (float)java.lang.Math.PI);
                float var6 = -MathHelper.cos(-var2 * ((float)java.lang.Math.PI / 180.0F));
                float var7 = MathHelper.sin(-var2 * ((float)java.lang.Math.PI / 180.0F));
                return Vec3D.createVector((double)(var5 * var6), (double)var7, (double)(var4 * var6));
            }
        }

        public MovingObjectPosition rayTrace(double var1, float var3)
        {
            Vec3D var4 = getPosition(var3);
            Vec3D var5 = getLook(var3);
            Vec3D var6 = var4.addVector(var5.xCoord * var1, var5.yCoord * var1, var5.zCoord * var1);
            return worldObj.rayTraceBlocks(var4, var6);
        }

        public virtual int getMaxSpawnedInChunk()
        {
            return 4;
        }

        public virtual ItemStack getHeldItem()
        {
            return null;
        }

        public override void handleHealthUpdate(sbyte var1)
        {
            if (var1 == 2)
            {
                field_704_R = 1.5F;
                heartsLife = heartsHalvesLife;
                hurtTime = maxHurtTime = 10;
                attackedAtYaw = 0.0F;
                worldObj.playSoundAtEntity(this, getHurtSound(), getSoundVolume(), (rand.nextFloat() - rand.nextFloat()) * 0.2F + 1.0F);
                attackEntityFrom(null, 0);
            }
            else if (var1 == 3)
            {
                worldObj.playSoundAtEntity(this, getDeathSound(), getSoundVolume(), (rand.nextFloat() - rand.nextFloat()) * 0.2F + 1.0F);
                health = 0;
                onDeath(null);
            }
            else
            {
                base.handleHealthUpdate(var1);
            }

        }

        public virtual bool isPlayerSleeping()
        {
            return false;
        }

        public virtual int getItemIcon(ItemStack var1)
        {
            return var1.getIconIndex();
        }
    }
}