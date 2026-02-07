using betareborn.Blocks;
using betareborn.Items;
using betareborn.Materials;
using betareborn.NBT;
using betareborn.Worlds;
using java.lang;

namespace betareborn.Entities
{
    public abstract class Entity : java.lang.Object
    {
        public static readonly Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(Entity).TypeHandle);
        private static int nextEntityID = 0;
        public int entityId = nextEntityID++;
        public double renderDistanceWeight = 1.0D;
        public bool preventEntitySpawning = false;
        public Entity riddenByEntity;
        public Entity ridingEntity;
        public World worldObj;
        public double prevPosX;
        public double prevPosY;
        public double prevPosZ;
        public double posX;
        public double posY;
        public double posZ;
        public double motionX;
        public double motionY;
        public double motionZ;
        public float rotationYaw;
        public float rotationPitch;
        public float prevRotationYaw;
        public float prevRotationPitch;
        public readonly Box boundingBox = Box.create(0.0D, 0.0D, 0.0D, 0.0D, 0.0D, 0.0D);
        public bool onGround = false;
        public bool isCollidedHorizontally;
        public bool isCollidedVertically;
        public bool isCollided = false;
        public bool beenAttacked = false;
        public bool isInWeb;
        public bool field_9293_aM = true;
        public bool isDead = false;
        public float yOffset = 0.0F;
        public float width = 0.6F;
        public float height = 1.8F;
        public float prevDistanceWalkedModified = 0.0F;
        public float distanceWalkedModified = 0.0F;
        protected float fallDistance = 0.0F;
        private int nextStepDistance = 1;
        public double lastTickPosX;
        public double lastTickPosY;
        public double lastTickPosZ;
        public float ySize = 0.0F;
        public float stepHeight = 0.0F;
        public bool noClip = false;
        public float entityCollisionReduction = 0.0F;
        protected java.util.Random rand = new();
        public int ticksExisted = 0;
        public int fireResistance = 1;
        public int fire = 0;
        protected int maxAir = 300;
        protected bool inWater = false;
        public int heartsLife = 0;
        public int air = 300;
        private bool isFirstUpdate = true;
        public string skinUrl;
        public string cloakUrl;
        protected bool isImmuneToFire = false;
        protected DataWatcher dataWatcher = new();
        public float entityBrightness = 0.0F;
        private double entityRiderPitchDelta;
        private double entityRiderYawDelta;
        public bool addedToChunk = false;
        public int chunkCoordX;
        public int chunkCoordY;
        public int chunkCoordZ;
        public int serverPosX;
        public int serverPosY;
        public int serverPosZ;
        public bool ignoreFrustumCheck;

        public Entity(World var1)
        {
            worldObj = var1;
            setPosition(0.0D, 0.0D, 0.0D);
            dataWatcher.addObject(0, java.lang.Byte.valueOf(0));
            entityInit();
        }

        protected abstract void entityInit();

        public DataWatcher getDataWatcher()
        {
            return dataWatcher;
        }

        public override bool equals(object var1)
        {
            return var1 is Entity ? ((Entity)var1).entityId == entityId : false;
        }

        public override int hashCode()
        {
            return entityId;
        }

        public virtual void preparePlayerToSpawn()
        {
            if (worldObj != null)
            {
                while (posY > 0.0D)
                {
                    setPosition(posX, posY, posZ);
                    if (worldObj.getCollidingBoundingBoxes(this, boundingBox).Count == 0)
                    {
                        break;
                    }

                    ++posY;
                }

                motionX = motionY = motionZ = 0.0D;
                rotationPitch = 0.0F;
            }
        }

        public virtual void setEntityDead()
        {
            isDead = true;
        }

        protected virtual void setSize(float var1, float var2)
        {
            width = var1;
            height = var2;
        }

        protected void setRotation(float var1, float var2)
        {
            rotationYaw = var1 % 360.0F;
            rotationPitch = var2 % 360.0F;
        }

        public void setPosition(double var1, double var3, double var5)
        {
            posX = var1;
            posY = var3;
            posZ = var5;
            float var7 = width / 2.0F;
            float var8 = height;
            boundingBox.set(var1 - (double)var7, var3 - (double)yOffset + (double)ySize, var5 - (double)var7, var1 + (double)var7, var3 - (double)yOffset + (double)ySize + (double)var8, var5 + (double)var7);
        }

        public void func_346_d(float var1, float var2)
        {
            float var3 = rotationPitch;
            float var4 = rotationYaw;
            rotationYaw = (float)((double)rotationYaw + (double)var1 * 0.15D);
            rotationPitch = (float)((double)rotationPitch - (double)var2 * 0.15D);
            if (rotationPitch < -90.0F)
            {
                rotationPitch = -90.0F;
            }

            if (rotationPitch > 90.0F)
            {
                rotationPitch = 90.0F;
            }

            prevRotationPitch += rotationPitch - var3;
            prevRotationYaw += rotationYaw - var4;
        }

        public virtual void onUpdate()
        {
            onEntityUpdate();
        }

        public virtual void onEntityUpdate()
        {
            if (ridingEntity != null && ridingEntity.isDead)
            {
                ridingEntity = null;
            }

            ++ticksExisted;
            prevDistanceWalkedModified = distanceWalkedModified;
            prevPosX = posX;
            prevPosY = posY;
            prevPosZ = posZ;
            prevRotationPitch = rotationPitch;
            prevRotationYaw = rotationYaw;
            if (handleWaterMovement())
            {
                if (!inWater && !isFirstUpdate)
                {
                    float var1 = MathHelper.sqrt_double(motionX * motionX * (double)0.2F + motionY * motionY + motionZ * motionZ * (double)0.2F) * 0.2F;
                    if (var1 > 1.0F)
                    {
                        var1 = 1.0F;
                    }

                    worldObj.playSoundAtEntity(this, "random.splash", var1, 1.0F + (rand.nextFloat() - rand.nextFloat()) * 0.4F);
                    float var2 = (float)MathHelper.floor_double(boundingBox.minY);

                    int var3;
                    float var4;
                    float var5;
                    for (var3 = 0; (float)var3 < 1.0F + width * 20.0F; ++var3)
                    {
                        var4 = (rand.nextFloat() * 2.0F - 1.0F) * width;
                        var5 = (rand.nextFloat() * 2.0F - 1.0F) * width;
                        worldObj.addParticle("bubble", posX + (double)var4, (double)(var2 + 1.0F), posZ + (double)var5, motionX, motionY - (double)(rand.nextFloat() * 0.2F), motionZ);
                    }

                    for (var3 = 0; (float)var3 < 1.0F + width * 20.0F; ++var3)
                    {
                        var4 = (rand.nextFloat() * 2.0F - 1.0F) * width;
                        var5 = (rand.nextFloat() * 2.0F - 1.0F) * width;
                        worldObj.addParticle("splash", posX + (double)var4, (double)(var2 + 1.0F), posZ + (double)var5, motionX, motionY, motionZ);
                    }
                }

                fallDistance = 0.0F;
                inWater = true;
                fire = 0;
            }
            else
            {
                inWater = false;
            }

            if (worldObj.multiplayerWorld)
            {
                fire = 0;
            }
            else if (fire > 0)
            {
                if (isImmuneToFire)
                {
                    fire -= 4;
                    if (fire < 0)
                    {
                        fire = 0;
                    }
                }
                else
                {
                    if (fire % 20 == 0)
                    {
                        attackEntityFrom((Entity)null, 1);
                    }

                    --fire;
                }
            }

            if (handleLavaMovement())
            {
                setOnFireFromLava();
            }

            if (posY < -64.0D)
            {
                kill();
            }

            if (!worldObj.multiplayerWorld)
            {
                setEntityFlag(0, fire > 0);
                setEntityFlag(2, ridingEntity != null);
            }

            isFirstUpdate = false;
        }

        protected void setOnFireFromLava()
        {
            if (!isImmuneToFire)
            {
                attackEntityFrom((Entity)null, 4);
                fire = 600;
            }

        }

        protected virtual void kill()
        {
            setEntityDead();
        }

        public bool isOffsetPositionInLiquid(double var1, double var3, double var5)
        {
            Box var7 = boundingBox.translate(var1, var3, var5);
            var var8 = worldObj.getCollidingBoundingBoxes(this, var7);
            return var8.Count > 0 ? false : !worldObj.getIsAnyLiquid(var7);
        }

        public virtual void moveEntity(double var1, double var3, double var5)
        {
            if (noClip)
            {
                boundingBox.translate(var1, var3, var5);
                posX = (boundingBox.minX + boundingBox.maxX) / 2.0D;
                posY = boundingBox.minY + (double)yOffset - (double)ySize;
                posZ = (boundingBox.minZ + boundingBox.maxZ) / 2.0D;
            }
            else
            {
                ySize *= 0.4F;
                double var7 = posX;
                double var9 = posZ;
                if (isInWeb)
                {
                    isInWeb = false;
                    var1 *= 0.25D;
                    var3 *= (double)0.05F;
                    var5 *= 0.25D;
                    motionX = 0.0D;
                    motionY = 0.0D;
                    motionZ = 0.0D;
                }

                double var11 = var1;
                double var13 = var3;
                double var15 = var5;
                Box var17 = boundingBox.copy();
                bool var18 = onGround && isSneaking();
                if (var18)
                {
                    double var19;
                    for (var19 = 0.05D; var1 != 0.0D && worldObj.getCollidingBoundingBoxes(this, boundingBox.translate(var1, -1.0D, 0.0D)).Count == 0; var11 = var1)
                    {
                        if (var1 < var19 && var1 >= -var19)
                        {
                            var1 = 0.0D;
                        }
                        else if (var1 > 0.0D)
                        {
                            var1 -= var19;
                        }
                        else
                        {
                            var1 += var19;
                        }
                    }

                    for (; var5 != 0.0D && worldObj.getCollidingBoundingBoxes(this, boundingBox.translate(0.0D, -1.0D, var5)).Count == 0; var15 = var5)
                    {
                        if (var5 < var19 && var5 >= -var19)
                        {
                            var5 = 0.0D;
                        }
                        else if (var5 > 0.0D)
                        {
                            var5 -= var19;
                        }
                        else
                        {
                            var5 += var19;
                        }
                    }
                }

                var var35 = worldObj.getCollidingBoundingBoxes(this, boundingBox.stretch(var1, var3, var5));

                for (int var20 = 0; var20 < var35.Count; ++var20)
                {
                    var3 = var35[var20].getYOffset(boundingBox, var3);
                }

                boundingBox.translate(0.0D, var3, 0.0D);
                if (!field_9293_aM && var13 != var3)
                {
                    var5 = 0.0D;
                    var3 = var5;
                    var1 = var5;
                }

                bool var36 = onGround || var13 != var3 && var13 < 0.0D;

                int var21;
                for (var21 = 0; var21 < var35.Count; ++var21)
                {
                    var1 = var35[var21].getXOffset(boundingBox, var1);
                }

                boundingBox.translate(var1, 0.0D, 0.0D);
                if (!field_9293_aM && var11 != var1)
                {
                    var5 = 0.0D;
                    var3 = var5;
                    var1 = var5;
                }

                for (var21 = 0; var21 < var35.Count; ++var21)
                {
                    var5 = var35[var21].getZOffset(boundingBox, var5);
                }

                boundingBox.translate(0.0D, 0.0D, var5);
                if (!field_9293_aM && var15 != var5)
                {
                    var5 = 0.0D;
                    var3 = var5;
                    var1 = var5;
                }

                double var23;
                int var28;
                double var37;
                if (stepHeight > 0.0F && var36 && (var18 || ySize < 0.05F) && (var11 != var1 || var15 != var5))
                {
                    var37 = var1;
                    var23 = var3;
                    double var25 = var5;
                    var1 = var11;
                    var3 = (double)stepHeight;
                    var5 = var15;
                    Box var27 = boundingBox.copy();
                    boundingBox.clone(var17);
                    var35 = worldObj.getCollidingBoundingBoxes(this, boundingBox.stretch(var11, var3, var15));

                    for (var28 = 0; var28 < var35.Count; ++var28)
                    {
                        var3 = var35[var28].getYOffset(boundingBox, var3);
                    }

                    boundingBox.translate(0.0D, var3, 0.0D);
                    if (!field_9293_aM && var13 != var3)
                    {
                        var5 = 0.0D;
                        var3 = var5;
                        var1 = var5;
                    }

                    for (var28 = 0; var28 < var35.Count; ++var28)
                    {
                        var1 = var35[var28].getXOffset(boundingBox, var1);
                    }

                    boundingBox.translate(var1, 0.0D, 0.0D);
                    if (!field_9293_aM && var11 != var1)
                    {
                        var5 = 0.0D;
                        var3 = var5;
                        var1 = var5;
                    }

                    for (var28 = 0; var28 < var35.Count; ++var28)
                    {
                        var5 = var35[var28].getZOffset(boundingBox, var5);
                    }

                    boundingBox.translate(0.0D, 0.0D, var5);
                    if (!field_9293_aM && var15 != var5)
                    {
                        var5 = 0.0D;
                        var3 = var5;
                        var1 = var5;
                    }

                    if (!field_9293_aM && var13 != var3)
                    {
                        var5 = 0.0D;
                        var3 = var5;
                        var1 = var5;
                    }
                    else
                    {
                        var3 = (double)(-stepHeight);

                        for (var28 = 0; var28 < var35.Count; ++var28)
                        {
                            var3 = var35[var28].getYOffset(boundingBox, var3);
                        }

                        boundingBox.translate(0.0D, var3, 0.0D);
                    }

                    if (var37 * var37 + var25 * var25 >= var1 * var1 + var5 * var5)
                    {
                        var1 = var37;
                        var3 = var23;
                        var5 = var25;
                        boundingBox.clone(var27);
                    }
                    else
                    {
                        double var41 = boundingBox.minY - (double)((int)boundingBox.minY);
                        if (var41 > 0.0D)
                        {
                            ySize = (float)((double)ySize + var41 + 0.01D);
                        }
                    }
                }

                posX = (boundingBox.minX + boundingBox.maxX) / 2.0D;
                posY = boundingBox.minY + (double)yOffset - (double)ySize;
                posZ = (boundingBox.minZ + boundingBox.maxZ) / 2.0D;
                isCollidedHorizontally = var11 != var1 || var15 != var5;
                isCollidedVertically = var13 != var3;
                onGround = var13 != var3 && var13 < 0.0D;
                isCollided = isCollidedHorizontally || isCollidedVertically;
                updateFallState(var3, onGround);
                if (var11 != var1)
                {
                    motionX = 0.0D;
                }

                if (var13 != var3)
                {
                    motionY = 0.0D;
                }

                if (var15 != var5)
                {
                    motionZ = 0.0D;
                }

                var37 = posX - var7;
                var23 = posZ - var9;
                int var26;
                int var38;
                int var39;
                if (canTriggerWalking() && !var18 && ridingEntity == null)
                {
                    distanceWalkedModified = (float)((double)distanceWalkedModified + (double)MathHelper.sqrt_double(var37 * var37 + var23 * var23) * 0.6D);
                    var38 = MathHelper.floor_double(posX);
                    var26 = MathHelper.floor_double(posY - (double)0.2F - (double)yOffset);
                    var39 = MathHelper.floor_double(posZ);
                    var28 = worldObj.getBlockId(var38, var26, var39);
                    if (worldObj.getBlockId(var38, var26 - 1, var39) == Block.FENCE.id)
                    {
                        var28 = worldObj.getBlockId(var38, var26 - 1, var39);
                    }

                    if (distanceWalkedModified > (float)nextStepDistance && var28 > 0)
                    {
                        ++nextStepDistance;
                        BlockSoundGroup var29 = Block.BLOCKS[var28].soundGroup;
                        if (worldObj.getBlockId(var38, var26 + 1, var39) == Block.SNOW.id)
                        {
                            var29 = Block.SNOW.soundGroup;
                            worldObj.playSoundAtEntity(this, var29.func_1145_d(), var29.getVolume() * 0.15F, var29.getPitch());
                        }
                        else if (!Block.BLOCKS[var28].material.isFluid())
                        {
                            worldObj.playSoundAtEntity(this, var29.func_1145_d(), var29.getVolume() * 0.15F, var29.getPitch());
                        }

                        Block.BLOCKS[var28].onEntityWalking(worldObj, var38, var26, var39, this);
                    }
                }

                var38 = MathHelper.floor_double(boundingBox.minX + 0.001D);
                var26 = MathHelper.floor_double(boundingBox.minY + 0.001D);
                var39 = MathHelper.floor_double(boundingBox.minZ + 0.001D);
                var28 = MathHelper.floor_double(boundingBox.maxX - 0.001D);
                int var40 = MathHelper.floor_double(boundingBox.maxY - 0.001D);
                int var30 = MathHelper.floor_double(boundingBox.maxZ - 0.001D);
                if (worldObj.checkChunksExist(var38, var26, var39, var28, var40, var30))
                {
                    for (int var31 = var38; var31 <= var28; ++var31)
                    {
                        for (int var32 = var26; var32 <= var40; ++var32)
                        {
                            for (int var33 = var39; var33 <= var30; ++var33)
                            {
                                int var34 = worldObj.getBlockId(var31, var32, var33);
                                if (var34 > 0)
                                {
                                    Block.BLOCKS[var34].onEntityCollidedWithBlock(worldObj, var31, var32, var33, this);
                                }
                            }
                        }
                    }
                }

                bool var42 = isWet();
                if (worldObj.isBoundingBoxBurning(boundingBox.contract(0.001D, 0.001D, 0.001D)))
                {
                    dealFireDamage(1);
                    if (!var42)
                    {
                        ++fire;
                        if (fire == 0)
                        {
                            fire = 300;
                        }
                    }
                }
                else if (fire <= 0)
                {
                    fire = -fireResistance;
                }

                if (var42 && fire > 0)
                {
                    worldObj.playSoundAtEntity(this, "random.fizz", 0.7F, 1.6F + (rand.nextFloat() - rand.nextFloat()) * 0.4F);
                    fire = -fireResistance;
                }

            }
        }

        protected virtual bool canTriggerWalking()
        {
            return true;
        }

        protected void updateFallState(double var1, bool var3)
        {
            if (var3)
            {
                if (fallDistance > 0.0F)
                {
                    fall(fallDistance);
                    fallDistance = 0.0F;
                }
            }
            else if (var1 < 0.0D)
            {
                fallDistance = (float)((double)fallDistance - var1);
            }

        }

        public virtual Box getBoundingBox()
        {
            return null;
        }

        protected virtual void dealFireDamage(int var1)
        {
            if (!isImmuneToFire)
            {
                attackEntityFrom((Entity)null, var1);
            }

        }

        protected virtual void fall(float var1)
        {
            if (riddenByEntity != null)
            {
                riddenByEntity.fall(var1);
            }

        }

        public bool isWet()
        {
            return inWater || worldObj.canBlockBeRainedOn(MathHelper.floor_double(posX), MathHelper.floor_double(posY), MathHelper.floor_double(posZ));
        }

        public virtual bool isInWater()
        {
            return inWater;
        }

        public virtual bool handleWaterMovement()
        {
            return worldObj.handleMaterialAcceleration(boundingBox.expand(0.0D, (double)-0.4F, 0.0D).contract(0.001D, 0.001D, 0.001D), Material.WATER, this);
        }

        public bool isInsideOfMaterial(Material var1)
        {
            double var2 = posY + (double)getEyeHeight();
            int var4 = MathHelper.floor_double(posX);
            int var5 = MathHelper.floor_float((float)MathHelper.floor_double(var2));
            int var6 = MathHelper.floor_double(posZ);
            int var7 = worldObj.getBlockId(var4, var5, var6);
            if (var7 != 0 && Block.BLOCKS[var7].material == var1)
            {
                float var8 = BlockFluid.getPercentAir(worldObj.getBlockMeta(var4, var5, var6)) - 1.0F / 9.0F;
                float var9 = (float)(var5 + 1) - var8;
                return var2 < (double)var9;
            }
            else
            {
                return false;
            }
        }

        public virtual float getEyeHeight()
        {
            return 0.0F;
        }

        public bool handleLavaMovement()
        {
            return worldObj.isMaterialInBB(boundingBox.expand((double)-0.1F, (double)-0.4F, (double)-0.1F), Material.LAVA);
        }

        public void moveFlying(float var1, float var2, float var3)
        {
            float var4 = MathHelper.sqrt_float(var1 * var1 + var2 * var2);
            if (var4 >= 0.01F)
            {
                if (var4 < 1.0F)
                {
                    var4 = 1.0F;
                }

                var4 = var3 / var4;
                var1 *= var4;
                var2 *= var4;
                float var5 = MathHelper.sin(rotationYaw * (float)java.lang.Math.PI / 180.0F);
                float var6 = MathHelper.cos(rotationYaw * (float)java.lang.Math.PI / 180.0F);
                motionX += (double)(var1 * var6 - var2 * var5);
                motionZ += (double)(var2 * var6 + var1 * var5);
            }
        }

        public virtual float getEntityBrightness(float var1)
        {
            int var2 = MathHelper.floor_double(posX);
            double var3 = (boundingBox.maxY - boundingBox.minY) * 0.66D;
            int var5 = MathHelper.floor_double(posY - (double)yOffset + var3);
            int var6 = MathHelper.floor_double(posZ);
            if (worldObj.checkChunksExist(MathHelper.floor_double(boundingBox.minX), MathHelper.floor_double(boundingBox.minY), MathHelper.floor_double(boundingBox.minZ), MathHelper.floor_double(boundingBox.maxX), MathHelper.floor_double(boundingBox.maxY), MathHelper.floor_double(boundingBox.maxZ)))
            {
                float var7 = worldObj.getLuminance(var2, var5, var6);
                if (var7 < entityBrightness)
                {
                    var7 = entityBrightness;
                }

                return var7;
            }
            else
            {
                return entityBrightness;
            }
        }

        public void setWorld(World var1)
        {
            worldObj = var1;
        }

        public void setPositionAndRotation(double x, double y, double z, float yaw, float pitch)
        {
            prevPosX = posX = x;
            prevPosY = posY = y;
            prevPosZ = posZ = z;
            prevRotationYaw = rotationYaw = yaw;
            prevRotationPitch = rotationPitch = pitch;
            ySize = 0.0F;
            double var9 = (double)(prevRotationYaw - yaw);
            if (var9 < -180.0D)
            {
                prevRotationYaw += 360.0F;
            }

            if (var9 >= 180.0D)
            {
                prevRotationYaw -= 360.0F;
            }

            setPosition(posX, posY, posZ);
            setRotation(yaw, pitch);
        }

        public void setPositionAndAnglesKeepPrevAngles(double var1, double var3, double var5, float var7, float var8)
        {
            lastTickPosX = prevPosX = posX = var1;
            lastTickPosY = prevPosY = posY = var3 + (double)yOffset;
            lastTickPosZ = prevPosZ = posZ = var5;
            rotationYaw = var7;
            rotationPitch = var8;
            setPosition(posX, posY, posZ);
        }

        public float getDistanceToEntity(Entity var1)
        {
            float var2 = (float)(posX - var1.posX);
            float var3 = (float)(posY - var1.posY);
            float var4 = (float)(posZ - var1.posZ);
            return MathHelper.sqrt_float(var2 * var2 + var3 * var3 + var4 * var4);
        }

        public double getSquaredDistance(double var1, double var3, double var5)
        {
            double var7 = posX - var1;
            double var9 = posY - var3;
            double var11 = posZ - var5;
            return var7 * var7 + var9 * var9 + var11 * var11;
        }

        public double getDistance(double var1, double var3, double var5)
        {
            double var7 = posX - var1;
            double var9 = posY - var3;
            double var11 = posZ - var5;
            return (double)MathHelper.sqrt_double(var7 * var7 + var9 * var9 + var11 * var11);
        }

        public double getDistanceSqToEntity(Entity var1)
        {
            double var2 = posX - var1.posX;
            double var4 = posY - var1.posY;
            double var6 = posZ - var1.posZ;
            return var2 * var2 + var4 * var4 + var6 * var6;
        }

        public virtual void onCollideWithPlayer(EntityPlayer var1)
        {
        }

        public virtual void applyEntityCollision(Entity var1)
        {
            if (var1.riddenByEntity != this && var1.ridingEntity != this)
            {
                double var2 = var1.posX - posX;
                double var4 = var1.posZ - posZ;
                double var6 = MathHelper.abs_max(var2, var4);
                if (var6 >= (double)0.01F)
                {
                    var6 = (double)MathHelper.sqrt_double(var6);
                    var2 /= var6;
                    var4 /= var6;
                    double var8 = 1.0D / var6;
                    if (var8 > 1.0D)
                    {
                        var8 = 1.0D;
                    }

                    var2 *= var8;
                    var4 *= var8;
                    var2 *= (double)0.05F;
                    var4 *= (double)0.05F;
                    var2 *= (double)(1.0F - entityCollisionReduction);
                    var4 *= (double)(1.0F - entityCollisionReduction);
                    addVelocity(-var2, 0.0D, -var4);
                    var1.addVelocity(var2, 0.0D, var4);
                }

            }
        }

        public virtual void addVelocity(double var1, double var3, double var5)
        {
            motionX += var1;
            motionY += var3;
            motionZ += var5;
        }

        protected void setBeenAttacked()
        {
            beenAttacked = true;
        }

        public virtual bool attackEntityFrom(Entity var1, int var2)
        {
            setBeenAttacked();
            return false;
        }

        public virtual bool canBeCollidedWith()
        {
            return false;
        }

        public virtual bool canBePushed()
        {
            return false;
        }

        public virtual void addToPlayerScore(Entity var1, int var2)
        {
        }

        public virtual bool isInRangeToRenderVec3D(Vec3D var1)
        {
            double var2 = posX - var1.xCoord;
            double var4 = posY - var1.yCoord;
            double var6 = posZ - var1.zCoord;
            double var8 = var2 * var2 + var4 * var4 + var6 * var6;
            return isInRangeToRenderDist(var8);
        }

        public virtual bool isInRangeToRenderDist(double var1)
        {
            double var3 = boundingBox.getAverageSizeLength();
            var3 *= 64.0D * renderDistanceWeight;
            return var1 < var3 * var3;
        }

        public virtual string getEntityTexture()
        {
            return null;
        }

        public bool addEntityID(NBTTagCompound var1)
        {
            string var2 = getEntityString();
            if (!isDead && var2 != null)
            {
                var1.setString("id", var2);
                writeToNBT(var1);
                return true;
            }
            else
            {
                return false;
            }
        }

        public void writeToNBT(NBTTagCompound var1)
        {
            var1.setTag("Pos", newDoubleNBTList([posX, posY + (double)ySize, posZ]));
            var1.setTag("Motion", newDoubleNBTList([motionX, motionY, motionZ]));
            var1.setTag("Rotation", newFloatNBTList([rotationYaw, rotationPitch]));
            var1.setFloat("FallDistance", fallDistance);
            var1.setShort("Fire", (short)fire);
            var1.setShort("Air", (short)air);
            var1.setBoolean("OnGround", onGround);
            writeEntityToNBT(var1);
        }

        public void readFromNBT(NBTTagCompound var1)
        {
            NBTTagList var2 = var1.getTagList("Pos");
            NBTTagList var3 = var1.getTagList("Motion");
            NBTTagList var4 = var1.getTagList("Rotation");
            motionX = ((NBTTagDouble)var3.tagAt(0)).doubleValue;
            motionY = ((NBTTagDouble)var3.tagAt(1)).doubleValue;
            motionZ = ((NBTTagDouble)var3.tagAt(2)).doubleValue;
            if (java.lang.Math.abs(motionX) > 10.0D)
            {
                motionX = 0.0D;
            }

            if (java.lang.Math.abs(motionY) > 10.0D)
            {
                motionY = 0.0D;
            }

            if (java.lang.Math.abs(motionZ) > 10.0D)
            {
                motionZ = 0.0D;
            }

            prevPosX = lastTickPosX = posX = ((NBTTagDouble)var2.tagAt(0)).doubleValue;
            prevPosY = lastTickPosY = posY = ((NBTTagDouble)var2.tagAt(1)).doubleValue;
            prevPosZ = lastTickPosZ = posZ = ((NBTTagDouble)var2.tagAt(2)).doubleValue;
            prevRotationYaw = rotationYaw = ((NBTTagFloat)var4.tagAt(0)).floatValue;
            prevRotationPitch = rotationPitch = ((NBTTagFloat)var4.tagAt(1)).floatValue;
            fallDistance = var1.getFloat("FallDistance");
            fire = var1.getShort("Fire");
            air = var1.getShort("Air");
            onGround = var1.getBoolean("OnGround");
            setPosition(posX, posY, posZ);
            setRotation(rotationYaw, rotationPitch);
            readEntityFromNBT(var1);
        }

        protected string getEntityString()
        {
            return EntityRegistry.getId(this);
        }

        public abstract void readEntityFromNBT(NBTTagCompound var1);

        public abstract void writeEntityToNBT(NBTTagCompound var1);

        protected NBTTagList newDoubleNBTList(params double[] var1)
        {
            NBTTagList var2 = new();
            double[] var3 = var1;
            int var4 = var1.Length;

            for (int var5 = 0; var5 < var4; ++var5)
            {
                double var6 = var3[var5];
                var2.setTag(new NBTTagDouble(var6));
            }

            return var2;
        }

        protected NBTTagList newFloatNBTList(params float[] var1)
        {
            NBTTagList var2 = new();
            float[] var3 = var1;
            int var4 = var1.Length;

            for (int var5 = 0; var5 < var4; ++var5)
            {
                float var6 = var3[var5];
                var2.setTag(new NBTTagFloat(var6));
            }

            return var2;
        }

        public virtual float getShadowSize()
        {
            return height / 2.0F;
        }

        public EntityItem dropItem(int var1, int var2)
        {
            return dropItemWithOffset(var1, var2, 0.0F);
        }

        public EntityItem dropItemWithOffset(int var1, int var2, float var3)
        {
            return entityDropItem(new ItemStack(var1, var2, 0), var3);
        }

        public EntityItem entityDropItem(ItemStack var1, float var2)
        {
            EntityItem var3 = new EntityItem(worldObj, posX, posY + (double)var2, posZ, var1);
            var3.delayBeforeCanPickup = 10;
            worldObj.spawnEntity(var3);
            return var3;
        }

        public virtual bool isEntityAlive()
        {
            return !isDead;
        }

        public virtual bool isEntityInsideOpaqueBlock()
        {
            for (int var1 = 0; var1 < 8; ++var1)
            {
                float var2 = ((float)((var1 >> 0) % 2) - 0.5F) * width * 0.9F;
                float var3 = ((float)((var1 >> 1) % 2) - 0.5F) * 0.1F;
                float var4 = ((float)((var1 >> 2) % 2) - 0.5F) * width * 0.9F;
                int var5 = MathHelper.floor_double(posX + (double)var2);
                int var6 = MathHelper.floor_double(posY + (double)getEyeHeight() + (double)var3);
                int var7 = MathHelper.floor_double(posZ + (double)var4);
                if (worldObj.shouldSuffocate(var5, var6, var7))
                {
                    return true;
                }
            }

            return false;
        }

        public virtual bool interact(EntityPlayer var1)
        {
            return false;
        }

        public virtual Box getCollisionBox(Entity var1)
        {
            return null;
        }

        public virtual void updateRidden()
        {
            if (ridingEntity.isDead)
            {
                ridingEntity = null;
            }
            else
            {
                motionX = 0.0D;
                motionY = 0.0D;
                motionZ = 0.0D;
                onUpdate();
                if (ridingEntity != null)
                {
                    ridingEntity.updateRiderPosition();
                    entityRiderYawDelta += (double)(ridingEntity.rotationYaw - ridingEntity.prevRotationYaw);

                    for (entityRiderPitchDelta += (double)(ridingEntity.rotationPitch - ridingEntity.prevRotationPitch); entityRiderYawDelta >= 180.0D; entityRiderYawDelta -= 360.0D)
                    {
                    }

                    while (entityRiderYawDelta < -180.0D)
                    {
                        entityRiderYawDelta += 360.0D;
                    }

                    while (entityRiderPitchDelta >= 180.0D)
                    {
                        entityRiderPitchDelta -= 360.0D;
                    }

                    while (entityRiderPitchDelta < -180.0D)
                    {
                        entityRiderPitchDelta += 360.0D;
                    }

                    double var1 = entityRiderYawDelta * 0.5D;
                    double var3 = entityRiderPitchDelta * 0.5D;
                    float var5 = 10.0F;
                    if (var1 > (double)var5)
                    {
                        var1 = (double)var5;
                    }

                    if (var1 < (double)(-var5))
                    {
                        var1 = (double)(-var5);
                    }

                    if (var3 > (double)var5)
                    {
                        var3 = (double)var5;
                    }

                    if (var3 < (double)(-var5))
                    {
                        var3 = (double)(-var5);
                    }

                    entityRiderYawDelta -= var1;
                    entityRiderPitchDelta -= var3;
                    rotationYaw = (float)((double)rotationYaw + var1);
                    rotationPitch = (float)((double)rotationPitch + var3);
                }
            }
        }

        public virtual void updateRiderPosition()
        {
            riddenByEntity.setPosition(posX, posY + getMountedYOffset() + riddenByEntity.getYOffset(), posZ);
        }

        public virtual double getYOffset()
        {
            return (double)yOffset;
        }

        public virtual double getMountedYOffset()
        {
            return (double)height * 0.75D;
        }

        public void mountEntity(Entity var1)
        {
            entityRiderPitchDelta = 0.0D;
            entityRiderYawDelta = 0.0D;
            if (var1 == null)
            {
                if (ridingEntity != null)
                {
                    setPositionAndAnglesKeepPrevAngles(ridingEntity.posX, ridingEntity.boundingBox.minY + (double)ridingEntity.height, ridingEntity.posZ, rotationYaw, rotationPitch);
                    ridingEntity.riddenByEntity = null;
                }

                ridingEntity = null;
            }
            else if (ridingEntity == var1)
            {
                ridingEntity.riddenByEntity = null;
                ridingEntity = null;
                setPositionAndAnglesKeepPrevAngles(var1.posX, var1.boundingBox.minY + (double)var1.height, var1.posZ, rotationYaw, rotationPitch);
            }
            else
            {
                if (ridingEntity != null)
                {
                    ridingEntity.riddenByEntity = null;
                }

                if (var1.riddenByEntity != null)
                {
                    var1.riddenByEntity.ridingEntity = null;
                }

                ridingEntity = var1;
                var1.riddenByEntity = this;
            }
        }

        public virtual void setPositionAndRotation2(double var1, double var3, double var5, float var7, float var8, int var9)
        {
            setPosition(var1, var3, var5);
            setRotation(var7, var8);
            var var10 = worldObj.getCollidingBoundingBoxes(this, boundingBox.contract(1.0D / 32.0D, 0.0D, 1.0D / 32.0D));
            if (var10.Count > 0)
            {
                double var11 = 0.0D;

                for (int var13 = 0; var13 < var10.Count; ++var13)
                {
                    Box var14 = var10[var13];
                    if (var14.maxY > var11)
                    {
                        var11 = var14.maxY;
                    }
                }

                var3 += var11 - boundingBox.minY;
                setPosition(var1, var3, var5);
            }

        }

        public virtual float getCollisionBorderSize()
        {
            return 0.1F;
        }

        public virtual Vec3D getLookVec()
        {
            return null;
        }

        public virtual void setInPortal()
        {
        }

        public virtual void setVelocity(double var1, double var3, double var5)
        {
            motionX = var1;
            motionY = var3;
            motionZ = var5;
        }

        public virtual void handleHealthUpdate(sbyte var1)
        {
        }

        public virtual void performHurtAnimation()
        {
        }

        public virtual void updateCloak()
        {
        }

        public virtual void outfitWithItem(int var1, int var2, int var3)
        {
        }

        public bool isBurning()
        {
            return fire > 0 || getEntityFlag(0);
        }

        public bool isRiding()
        {
            return ridingEntity != null || getEntityFlag(2);
        }

        public virtual bool isSneaking()
        {
            return getEntityFlag(1);
        }

        protected bool getEntityFlag(int var1)
        {
            return (dataWatcher.getWatchableObjectByte(0) & 1 << var1) != 0;
        }

        protected void setEntityFlag(int var1, bool var2)
        {
            sbyte var3 = dataWatcher.getWatchableObjectByte(0);
            byte newValue;
            if (var2)
            {
                newValue = (byte)((byte)var3 | (1 << var1));
            }
            else
            {
                newValue = (byte)((byte)var3 & ~(1 << var1));
            }
            dataWatcher.updateObject(0, java.lang.Byte.valueOf(newValue));

        }

        public virtual void onStruckByLightning(EntityLightningBolt var1)
        {
            dealFireDamage(5);
            ++fire;
            if (fire == 0)
            {
                fire = 300;
            }

        }

        public virtual void onKillEntity(EntityLiving var1)
        {
        }

        protected virtual bool pushOutOfBlocks(double var1, double var3, double var5)
        {
            int var7 = MathHelper.floor_double(var1);
            int var8 = MathHelper.floor_double(var3);
            int var9 = MathHelper.floor_double(var5);
            double var10 = var1 - (double)var7;
            double var12 = var3 - (double)var8;
            double var14 = var5 - (double)var9;
            if (worldObj.shouldSuffocate(var7, var8, var9))
            {
                bool var16 = !worldObj.shouldSuffocate(var7 - 1, var8, var9);
                bool var17 = !worldObj.shouldSuffocate(var7 + 1, var8, var9);
                bool var18 = !worldObj.shouldSuffocate(var7, var8 - 1, var9);
                bool var19 = !worldObj.shouldSuffocate(var7, var8 + 1, var9);
                bool var20 = !worldObj.shouldSuffocate(var7, var8, var9 - 1);
                bool var21 = !worldObj.shouldSuffocate(var7, var8, var9 + 1);
                int var22 = -1;
                double var23 = 9999.0D;
                if (var16 && var10 < var23)
                {
                    var23 = var10;
                    var22 = 0;
                }

                if (var17 && 1.0D - var10 < var23)
                {
                    var23 = 1.0D - var10;
                    var22 = 1;
                }

                if (var18 && var12 < var23)
                {
                    var23 = var12;
                    var22 = 2;
                }

                if (var19 && 1.0D - var12 < var23)
                {
                    var23 = 1.0D - var12;
                    var22 = 3;
                }

                if (var20 && var14 < var23)
                {
                    var23 = var14;
                    var22 = 4;
                }

                if (var21 && 1.0D - var14 < var23)
                {
                    var23 = 1.0D - var14;
                    var22 = 5;
                }

                float var25 = rand.nextFloat() * 0.2F + 0.1F;
                if (var22 == 0)
                {
                    motionX = (double)(-var25);
                }

                if (var22 == 1)
                {
                    motionX = (double)var25;
                }

                if (var22 == 2)
                {
                    motionY = (double)(-var25);
                }

                if (var22 == 3)
                {
                    motionY = (double)var25;
                }

                if (var22 == 4)
                {
                    motionZ = (double)(-var25);
                }

                if (var22 == 5)
                {
                    motionZ = (double)var25;
                }
            }

            return false;
        }
    }

}