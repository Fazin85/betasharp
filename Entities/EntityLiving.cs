using betareborn.Blocks;
using betareborn.Blocks.Materials;
using betareborn.Items;
using betareborn.NBT;
using betareborn.Util.Hit;
using betareborn.Util.Maths;
using betareborn.Worlds;
using java.lang;

namespace betareborn.Entities
{
    public abstract class EntityLiving : Entity
    {
        public static readonly new Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(EntityLiving).TypeHandle);
        public int maxHealth = 20;
        public float field_9365_p;
        public float field_9363_r;
        public float bodyYaw = 0.0F;
        public float lastBodyYaw = 0.0F;
        protected float lastWalkProgress;
        protected float walkProgress;
        protected float totalWalkDistance;
        protected float lastTotalWalkDistance;
        protected bool field_9358_y = true;
        protected string texture = "/mob/char.png";
        protected bool field_9355_A = true;
        protected float rotationOffset = 0.0F;
        protected string modelName = null;
        protected float field_9349_D = 1.0F;
        protected int scoreAmount = 0;
        protected float field_9345_F = 0.0F;
        public bool interpolateOnly = false;
        public float lastSwingAnimationProgress;
        public float swingAnimationProgress;
        public int health = 10;
        public int lastHealth;
        private int livingSoundTime;
        public int hurtTime;
        public int maxHurtTime;
        public float attackedAtYaw = 0.0F;
        public int deathTime = 0;
        public int attackTime = 0;
        public float cameraPitch;
        public float tilt;
        protected bool unused_flag = false;
        public int field_9326_T = -1;
        public float field_9325_U = (float)(java.lang.Math.random() * (double)0.9F + (double)0.1F);
        public float lastWalkAnimationSpeed;
        public float walkAnimationSpeed;
        public float field_703_S;
        protected int newPosRotationIncrements;
        protected double newPosX;
        protected double newPosY;
        protected double newPosZ;
        protected double newRotationYaw;
        protected double newRotationPitch;
        protected int field_9346_af = 0;
        protected int entityAge = 0;
        protected float sidewaysSpeed;
        protected float forwardSpeed;
        protected float rotationSpeed;
        protected bool jumping = false;
        protected float defaultPitch = 0.0F;
        protected float movementSpeed = 0.7F;
        private Entity lookTarget;
        protected int lookTimer = 0;

        public EntityLiving(World var1) : base(var1)
        {
            preventEntitySpawning = true;
            field_9363_r = (float)(java.lang.Math.random() + 1.0D) * 0.01F;
            setPosition(x, y, z);
            field_9365_p = (float)java.lang.Math.random() * 12398.0F;
            yaw = (float)(java.lang.Math.random() * (double)((float)java.lang.Math.PI) * 2.0D);
            stepHeight = 0.5F;
        }

        protected override void initDataTracker()
        {
        }

        public bool canSee(Entity var1)
        {
            return world.raycast(Vec3D.createVector(x, y + (double)getEyeHeight(), z), Vec3D.createVector(var1.x, var1.y + (double)var1.getEyeHeight(), var1.z)) == null;
        }

        public override string getTexture()
        {
            return texture;
        }

        public override bool isCollidable()
        {
            return !dead;
        }

        public override bool isPushable()
        {
            return !dead;
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
                world.playSound(this, var1, getSoundVolume(), (random.nextFloat() - random.nextFloat()) * 0.2F + 1.0F);
            }

        }

        public override void baseTick()
        {
            lastSwingAnimationProgress = swingAnimationProgress;
            base.baseTick();
            if (random.nextInt(1000) < livingSoundTime++)
            {
                livingSoundTime = -getTalkInterval();
                playLivingSound();
            }

            if (isAlive() && isInsideWall())
            {
                damage(null, 1);
            }

            if (isImmuneToFire || world.isRemote)
            {
                fireTicks = 0;
            }

            int var1;
            if (isAlive() && isInFluid(Material.WATER) && !canBreatheUnderwater())
            {
                --air;
                if (air == -20)
                {
                    air = 0;

                    for (var1 = 0; var1 < 8; ++var1)
                    {
                        float var2 = random.nextFloat() - random.nextFloat();
                        float var3 = random.nextFloat() - random.nextFloat();
                        float var4 = random.nextFloat() - random.nextFloat();
                        world.addParticle("bubble", x + (double)var2, y + (double)var3, z + (double)var4, velocityX, velocityY, velocityZ);
                    }

                    damage(null, 2);
                }

                fireTicks = 0;
            }
            else
            {
                air = maxAir;
            }

            cameraPitch = tilt;
            if (attackTime > 0)
            {
                --attackTime;
            }

            if (hurtTime > 0)
            {
                --hurtTime;
            }

            if (hearts > 0)
            {
                --hearts;
            }

            if (health <= 0)
            {
                ++deathTime;
                if (deathTime > 20)
                {
                    onEntityDeath();
                    markDead();

                    for (var1 = 0; var1 < 20; ++var1)
                    {
                        double var8 = random.nextGaussian() * 0.02D;
                        double var9 = random.nextGaussian() * 0.02D;
                        double var6 = random.nextGaussian() * 0.02D;
                        world.addParticle("explode", x + (double)(random.nextFloat() * width * 2.0F) - (double)width, y + (double)(random.nextFloat() * height), z + (double)(random.nextFloat() * width * 2.0F) - (double)width, var8, var9, var6);
                    }
                }
            }

            lastTotalWalkDistance = totalWalkDistance;
            lastBodyYaw = bodyYaw;
            prevYaw = yaw;
            prevPitch = pitch;
        }

        //TODO: will this still work properly when we implement the server?
        public override void move(double var1, double var3, double var5)
        {
            if (!interpolateOnly || this is ClientPlayerEntity) base.move(var1, var3, var5);
        }

        public void animateSpawn()
        {
            for (int var1 = 0; var1 < 20; ++var1)
            {
                double var2 = random.nextGaussian() * 0.02D;
                double var4 = random.nextGaussian() * 0.02D;
                double var6 = random.nextGaussian() * 0.02D;
                double var8 = 10.0D;
                world.addParticle("explode", x + (double)(random.nextFloat() * width * 2.0F) - (double)width - var2 * var8, y + (double)(random.nextFloat() * height) - var4 * var8, z + (double)(random.nextFloat() * width * 2.0F) - (double)width - var6 * var8, var2, var4, var6);
            }

        }

        public override void tickRiding()
        {
            base.tickRiding();
            lastWalkProgress = walkProgress;
            walkProgress = 0.0F;
        }

        public override void setPositionAndAnglesAvoidEntities(double var1, double var3, double var5, float var7, float var8, int var9)
        {
            standingEyeHeight = 0.0F;
            newPosX = var1;
            newPosY = var3;
            newPosZ = var5;
            newRotationYaw = (double)var7;
            newRotationPitch = (double)var8;
            newPosRotationIncrements = var9;
        }

        public override void tick()
        {
            base.tick();
            tickMovement();
            double var1 = x - prevX;
            double var3 = z - prevZ;
            float var5 = MathHelper.sqrt_double(var1 * var1 + var3 * var3);
            float var6 = bodyYaw;
            float var7 = 0.0F;
            lastWalkProgress = walkProgress;
            float var8 = 0.0F;
            if (var5 > 0.05F)
            {
                var8 = 1.0F;
                var7 = var5 * 3.0F;
                var6 = (float)java.lang.Math.atan2(var3, var1) * 180.0F / (float)java.lang.Math.PI - 90.0F;
            }

            if (swingAnimationProgress > 0.0F)
            {
                var6 = yaw;
            }

            if (!onGround)
            {
                var8 = 0.0F;
            }

            walkProgress += (var8 - walkProgress) * 0.3F;

            float var9;
            for (var9 = var6 - bodyYaw; var9 < -180.0F; var9 += 360.0F)
            {
            }

            while (var9 >= 180.0F)
            {
                var9 -= 360.0F;
            }

            bodyYaw += var9 * 0.3F;

            float var10;
            for (var10 = yaw - bodyYaw; var10 < -180.0F; var10 += 360.0F)
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

            bodyYaw = yaw - var10;
            if (var10 * var10 > 2500.0F)
            {
                bodyYaw += var10 * 0.2F;
            }

            if (var11)
            {
                var7 *= -1.0F;
            }

            while (yaw - prevYaw < -180.0F)
            {
                prevYaw -= 360.0F;
            }

            while (yaw - prevYaw >= 180.0F)
            {
                prevYaw += 360.0F;
            }

            while (bodyYaw - lastBodyYaw < -180.0F)
            {
                lastBodyYaw -= 360.0F;
            }

            while (bodyYaw - lastBodyYaw >= 180.0F)
            {
                lastBodyYaw += 360.0F;
            }

            while (pitch - prevPitch < -180.0F)
            {
                prevPitch -= 360.0F;
            }

            while (pitch - prevPitch >= 180.0F)
            {
                prevPitch += 360.0F;
            }

            totalWalkDistance += var7;
        }

        protected override void setBoundingBoxSpacing(float var1, float var2)
        {
            base.setBoundingBoxSpacing(var1, var2);
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

                hearts = maxHealth / 2;
            }
        }

        public override bool damage(Entity var1, int var2)
        {
            if (world.isRemote)
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
                    walkAnimationSpeed = 1.5F;
                    bool var3 = true;
                    if ((float)hearts > (float)maxHealth / 2.0F)
                    {
                        if (var2 <= field_9346_af)
                        {
                            return false;
                        }

                        applyDamage(var2 - field_9346_af);
                        field_9346_af = var2;
                        var3 = false;
                    }
                    else
                    {
                        field_9346_af = var2;
                        lastHealth = health;
                        hearts = maxHealth;
                        applyDamage(var2);
                        hurtTime = maxHurtTime = 10;
                    }

                    attackedAtYaw = 0.0F;
                    if (var3)
                    {
                        world.broadcastEntityEvent(this, (byte)2);
                        scheduleVelocityUpdate();
                        if (var1 != null)
                        {
                            double var4 = var1.x - x;

                            double var6;
                            for (var6 = var1.z - z; var4 * var4 + var6 * var6 < 1.0E-4D; var6 = (java.lang.Math.random() - java.lang.Math.random()) * 0.01D)
                            {
                                var4 = (java.lang.Math.random() - java.lang.Math.random()) * 0.01D;
                            }

                            attackedAtYaw = (float)(java.lang.Math.atan2(var6, var4) * 180.0D / (double)((float)java.lang.Math.PI)) - yaw;
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
                            world.playSound(this, getDeathSound(), getSoundVolume(), (random.nextFloat() - random.nextFloat()) * 0.2F + 1.0F);
                        }

                        onKilledBy(var1);
                    }
                    else if (var3)
                    {
                        world.playSound(this, getHurtSound(), getSoundVolume(), (random.nextFloat() - random.nextFloat()) * 0.2F + 1.0F);
                    }

                    return true;
                }
            }
        }

        public override void animateHurt()
        {
            hurtTime = maxHurtTime = 10;
            attackedAtYaw = 0.0F;
        }

        protected virtual void applyDamage(int var1)
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
            velocityX /= 2.0D;
            velocityY /= 2.0D;
            velocityZ /= 2.0D;
            velocityX -= var3 / (double)var7 * (double)var8;
            velocityY += (double)0.4F;
            velocityZ -= var5 / (double)var7 * (double)var8;
            if (velocityY > (double)0.4F)
            {
                velocityY = (double)0.4F;
            }

        }

        public virtual void onKilledBy(Entity var1)
        {
            if (scoreAmount >= 0 && var1 != null)
            {
                var1.updateKilledAchievement(this, scoreAmount);
            }

            if (var1 != null)
            {
                var1.onKillOther(this);
            }

            unused_flag = true;
            if (!world.isRemote)
            {
                dropFewItems();
            }

            world.broadcastEntityEvent(this, (byte)3);
        }

        protected virtual void dropFewItems()
        {
            int var1 = getDropItemId();
            if (var1 > 0)
            {
                int var2 = random.nextInt(3);

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

        protected override void onLanding(float var1)
        {
            base.onLanding(var1);
            int var2 = (int)java.lang.Math.ceil((double)(var1 - 3.0F));
            if (var2 > 0)
            {
                damage(null, var2);
                int var3 = world.getBlockId(MathHelper.floor_double(x), MathHelper.floor_double(y - (double)0.2F - (double)standingEyeHeight), MathHelper.floor_double(z));
                if (var3 > 0)
                {
                    BlockSoundGroup var4 = Block.BLOCKS[var3].soundGroup;
                    world.playSound(this, var4.func_1145_d(), var4.getVolume() * 0.5F, var4.getPitch() * (12.0F / 16.0F));
                }
            }

        }

        public virtual void travel(float var1, float var2)
        {
            double var3;
            if (isInWater())
            {
                var3 = y;
                moveNonSolid(var1, var2, 0.02F);
                move(velocityX, velocityY, velocityZ);
                velocityX *= (double)0.8F;
                velocityY *= (double)0.8F;
                velocityZ *= (double)0.8F;
                velocityY -= 0.02D;
                if (horizontalCollison && getEntitiesInside(velocityX, velocityY + (double)0.6F - y + var3, velocityZ))
                {
                    velocityY = (double)0.3F;
                }
            }
            else if (isTouchingLava())
            {
                var3 = y;
                moveNonSolid(var1, var2, 0.02F);
                move(velocityX, velocityY, velocityZ);
                velocityX *= 0.5D;
                velocityY *= 0.5D;
                velocityZ *= 0.5D;
                velocityY -= 0.02D;
                if (horizontalCollison && getEntitiesInside(velocityX, velocityY + (double)0.6F - y + var3, velocityZ))
                {
                    velocityY = (double)0.3F;
                }
            }
            else
            {
                float var8 = 0.91F;
                if (onGround)
                {
                    var8 = 546.0F * 0.1F * 0.1F * 0.1F;
                    int var4 = world.getBlockId(MathHelper.floor_double(x), MathHelper.floor_double(boundingBox.minY) - 1, MathHelper.floor_double(z));
                    if (var4 > 0)
                    {
                        var8 = Block.BLOCKS[var4].slipperiness * 0.91F;
                    }
                }

                float var9 = 0.16277136F / (var8 * var8 * var8);
                moveNonSolid(var1, var2, onGround ? 0.1F * var9 : 0.02F);
                var8 = 0.91F;
                if (onGround)
                {
                    var8 = 546.0F * 0.1F * 0.1F * 0.1F;
                    int var5 = world.getBlockId(MathHelper.floor_double(x), MathHelper.floor_double(boundingBox.minY) - 1, MathHelper.floor_double(z));
                    if (var5 > 0)
                    {
                        var8 = Block.BLOCKS[var5].slipperiness * 0.91F;
                    }
                }

                if (isOnLadder())
                {
                    float var10 = 0.15F;
                    if (velocityX < (double)(-var10))
                    {
                        velocityX = (double)(-var10);
                    }

                    if (velocityX > (double)var10)
                    {
                        velocityX = (double)var10;
                    }

                    if (velocityZ < (double)(-var10))
                    {
                        velocityZ = (double)(-var10);
                    }

                    if (velocityZ > (double)var10)
                    {
                        velocityZ = (double)var10;
                    }

                    fallDistance = 0.0F;
                    if (velocityY < -0.15D)
                    {
                        velocityY = -0.15D;
                    }

                    if (isSneaking() && velocityY < 0.0D)
                    {
                        velocityY = 0.0D;
                    }
                }

                move(velocityX, velocityY, velocityZ);
                if (horizontalCollison && isOnLadder())
                {
                    velocityY = 0.2D;
                }

                velocityY -= 0.08D;
                velocityY *= (double)0.98F;
                velocityX *= (double)var8;
                velocityZ *= (double)var8;
            }

            lastWalkAnimationSpeed = walkAnimationSpeed;
            var3 = x - prevX;
            double var11 = z - prevZ;
            float var7 = MathHelper.sqrt_double(var3 * var3 + var11 * var11) * 4.0F;
            if (var7 > 1.0F)
            {
                var7 = 1.0F;
            }

            walkAnimationSpeed += (var7 - walkAnimationSpeed) * 0.4F;
            field_703_S += walkAnimationSpeed;
        }

        public virtual bool isOnLadder()
        {
            int var1 = MathHelper.floor_double(x);
            int var2 = MathHelper.floor_double(boundingBox.minY);
            int var3 = MathHelper.floor_double(z);
            return world.getBlockId(var1, var2, var3) == Block.LADDER.id;
        }

        public override void writeNbt(NbtTagCompound var1)
        {
            var1.SetShort("Health", (short)health);
            var1.SetShort("HurtTime", (short)hurtTime);
            var1.SetShort("DeathTime", (short)deathTime);
            var1.SetShort("AttackTime", (short)attackTime);
        }

        public override void readNbt(NbtTagCompound var1)
        {
            health = var1.GetShort("Health");
            if (!var1.HasKey("Health"))
            {
                health = 10;
            }

            hurtTime = var1.GetShort("HurtTime");
            deathTime = var1.GetShort("DeathTime");
            attackTime = var1.GetShort("AttackTime");
        }

        public override bool isAlive()
        {
            return !dead && health > 0;
        }

        public virtual bool canBreatheUnderwater()
        {
            return false;
        }

        public virtual void tickMovement()
        {
            if (newPosRotationIncrements > 0)
            {
                double var1 = x + (newPosX - x) / (double)newPosRotationIncrements;
                double var3 = y + (newPosY - y) / (double)newPosRotationIncrements;
                double var5 = z + (newPosZ - z) / (double)newPosRotationIncrements;

                double var7;
                for (var7 = newRotationYaw - (double)yaw; var7 < -180.0D; var7 += 360.0D)
                {
                }

                while (var7 >= 180.0D)
                {
                    var7 -= 360.0D;
                }

                yaw = (float)((double)yaw + var7 / (double)newPosRotationIncrements);
                pitch = (float)((double)pitch + (newRotationPitch - (double)pitch) / (double)newPosRotationIncrements);
                --newPosRotationIncrements;
                setPosition(var1, var3, var5);
                setRotation(yaw, pitch);
                var var9 = world.getEntityCollisions(this, boundingBox.contract(1.0D / 32.0D, 0.0D, 1.0D / 32.0D));
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
                jumping = false;
                sidewaysSpeed = 0.0F;
                forwardSpeed = 0.0F;
                rotationSpeed = 0.0F;
            }
            else if (!interpolateOnly)
            {
                tickLiving();
            }

            bool var14 = isInWater();
            bool var2 = isTouchingLava();
            if (jumping)
            {
                if (var14)
                {
                    velocityY += (double)0.04F;
                }
                else if (var2)
                {
                    velocityY += (double)0.04F;
                }
                else if (onGround)
                {
                    jump();
                }
            }

            sidewaysSpeed *= 0.98F;
            forwardSpeed *= 0.98F;
            rotationSpeed *= 0.9F;
            travel(sidewaysSpeed, forwardSpeed);
            var var15 = world.getEntities(this, boundingBox.expand((double)0.2F, 0.0D, (double)0.2F));
            if (var15 != null && var15.Count > 0)
            {
                for (int var4 = 0; var4 < var15.Count; ++var4)
                {
                    Entity var16 = var15[var4];
                    if (var16.isPushable())
                    {
                        var16.onCollision(this);
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
            velocityY = (double)0.42F;
        }

        protected virtual bool canDespawn()
        {
            return true;
        }

        protected void func_27021_X()
        {
            EntityPlayer var1 = world.getClosestPlayer(this, -1.0D);
            if (canDespawn() && var1 != null)
            {
                double var2 = var1.x - x;
                double var4 = var1.y - y;
                double var6 = var1.z - z;
                double var8 = var2 * var2 + var4 * var4 + var6 * var6;
                if (var8 > 16384.0D)
                {
                    markDead();
                }

                if (entityAge > 600 && random.nextInt(800) == 0)
                {
                    if (var8 < 1024.0D)
                    {
                        entityAge = 0;
                    }
                    else
                    {
                        markDead();
                    }
                }
            }

        }

        public virtual void tickLiving()
        {
            ++entityAge;
            EntityPlayer var1 = world.getClosestPlayer(this, -1.0D);
            func_27021_X();
            sidewaysSpeed = 0.0F;
            forwardSpeed = 0.0F;
            float var2 = 8.0F;
            if (random.nextFloat() < 0.02F)
            {
                var1 = world.getClosestPlayer(this, (double)var2);
                if (var1 != null)
                {
                    lookTarget = var1;
                    lookTimer = 10 + random.nextInt(20);
                }
                else
                {
                    rotationSpeed = (random.nextFloat() - 0.5F) * 20.0F;
                }
            }

            if (lookTarget != null)
            {
                faceEntity(lookTarget, 10.0F, (float)func_25026_x());
                if (lookTimer-- <= 0 || lookTarget.dead || lookTarget.getSquaredDistance(this) > (double)(var2 * var2))
                {
                    lookTarget = null;
                }
            }
            else
            {
                if (random.nextFloat() < 0.05F)
                {
                    rotationSpeed = (random.nextFloat() - 0.5F) * 20.0F;
                }

                yaw += rotationSpeed;
                pitch = defaultPitch;
            }

            bool var3 = isInWater();
            bool var4 = isTouchingLava();
            if (var3 || var4)
            {
                jumping = random.nextFloat() < 0.8F;
            }

        }

        protected virtual int func_25026_x()
        {
            return 40;
        }

        public void faceEntity(Entity var1, float var2, float var3)
        {
            double var4 = var1.x - x;
            double var8 = var1.z - z;
            double var6;
            if (var1 is EntityLiving)
            {
                EntityLiving var10 = (EntityLiving)var1;
                var6 = y + (double)getEyeHeight() - (var10.y + (double)var10.getEyeHeight());
            }
            else
            {
                var6 = (var1.boundingBox.minY + var1.boundingBox.maxY) / 2.0D - (y + (double)getEyeHeight());
            }

            double var14 = (double)MathHelper.sqrt_double(var4 * var4 + var8 * var8);
            float var12 = (float)(java.lang.Math.atan2(var8, var4) * 180.0D / (double)((float)java.lang.Math.PI)) - 90.0F;
            float var13 = (float)(-(java.lang.Math.atan2(var6, var14) * 180.0D / (double)((float)java.lang.Math.PI)));
            pitch = -updateRotation(pitch, var13, var3);
            yaw = updateRotation(yaw, var12, var2);
        }

        public bool hasCurrentTarget()
        {
            return lookTarget != null;
        }

        public Entity getCurrentTarget()
        {
            return lookTarget;
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
            return world.canSpawnEntity(boundingBox) && world.getEntityCollisions(this, boundingBox).Count == 0 && !world.isBoxSubmergedInFluid(boundingBox);
        }

        protected override void tickInVoid()
        {
            damage(null, 4);
        }

        public float getSwingProgress(float var1)
        {
            float var2 = swingAnimationProgress - lastSwingAnimationProgress;
            if (var2 < 0.0F)
            {
                ++var2;
            }

            return lastSwingAnimationProgress + var2 * var1;
        }

        public Vec3D getPosition(float var1)
        {
            if (var1 == 1.0F)
            {
                return Vec3D.createVector(x, y, z);
            }
            else
            {
                double var2 = prevX + (x - prevX) * (double)var1;
                double var4 = prevY + (y - prevY) * (double)var1;
                double var6 = prevZ + (z - prevZ) * (double)var1;
                return Vec3D.createVector(var2, var4, var6);
            }
        }

        public override Vec3D getLookVector()
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
                var2 = MathHelper.cos(-yaw * ((float)java.lang.Math.PI / 180.0F) - (float)java.lang.Math.PI);
                var3 = MathHelper.sin(-yaw * ((float)java.lang.Math.PI / 180.0F) - (float)java.lang.Math.PI);
                var4 = -MathHelper.cos(-pitch * ((float)java.lang.Math.PI / 180.0F));
                var5 = MathHelper.sin(-pitch * ((float)java.lang.Math.PI / 180.0F));
                return Vec3D.createVector((double)(var3 * var4), (double)var5, (double)(var2 * var4));
            }
            else
            {
                var2 = prevPitch + (pitch - prevPitch) * var1;
                var3 = prevYaw + (yaw - prevYaw) * var1;
                var4 = MathHelper.cos(-var3 * ((float)java.lang.Math.PI / 180.0F) - (float)java.lang.Math.PI);
                var5 = MathHelper.sin(-var3 * ((float)java.lang.Math.PI / 180.0F) - (float)java.lang.Math.PI);
                float var6 = -MathHelper.cos(-var2 * ((float)java.lang.Math.PI / 180.0F));
                float var7 = MathHelper.sin(-var2 * ((float)java.lang.Math.PI / 180.0F));
                return Vec3D.createVector((double)(var5 * var6), (double)var7, (double)(var4 * var6));
            }
        }

        public HitResult rayTrace(double var1, float var3)
        {
            Vec3D var4 = getPosition(var3);
            Vec3D var5 = getLook(var3);
            Vec3D var6 = var4.addVector(var5.xCoord * var1, var5.yCoord * var1, var5.zCoord * var1);
            return world.raycast(var4, var6);
        }

        public virtual int getMaxSpawnedInChunk()
        {
            return 4;
        }

        public virtual ItemStack getHeldItem()
        {
            return null;
        }

        public override void processServerEntityStatus(sbyte var1)
        {
            if (var1 == 2)
            {
                walkAnimationSpeed = 1.5F;
                hearts = maxHealth;
                hurtTime = maxHurtTime = 10;
                attackedAtYaw = 0.0F;
                world.playSound(this, getHurtSound(), getSoundVolume(), (random.nextFloat() - random.nextFloat()) * 0.2F + 1.0F);
                damage(null, 0);
            }
            else if (var1 == 3)
            {
                world.playSound(this, getDeathSound(), getSoundVolume(), (random.nextFloat() - random.nextFloat()) * 0.2F + 1.0F);
                health = 0;
                onKilledBy(null);
            }
            else
            {
                base.processServerEntityStatus(var1);
            }

        }

        public virtual bool isSleeping()
        {
            return false;
        }

        public virtual int getItemStackTextureId(ItemStack var1)
        {
            return var1.getTextureId();
        }
    }
}