using betareborn.Items;
using betareborn.NBT;
using betareborn.Util.Maths;
using betareborn.Worlds;
using java.lang;

namespace betareborn.Entities
{
    public class EntityWolf : EntityAnimal
    {
        public static readonly new Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(EntityWolf).TypeHandle);
        private bool looksWithInterest = false;
        private float field_25048_b;
        private float field_25054_c;
        private bool isWolfShaking;
        private bool field_25052_g;
        private float timeWolfIsShaking;
        private float prevTimeWolfIsShaking;

        public EntityWolf(World var1) : base(var1)
        {
            texture = "/mob/wolf.png";
            setBoundingBoxSpacing(0.8F, 0.8F);
            movementSpeed = 1.1F;
            health = 8;
        }

        protected override void initDataTracker()
        {
            base.initDataTracker();
            dataWatcher.addObject(16, java.lang.Byte.valueOf((byte)0));
            dataWatcher.addObject(17, new JString(""));
            dataWatcher.addObject(18, new java.lang.Integer(health));
        }

        protected override bool bypassesSteppingEffects()
        {
            return false;
        }

        public override string getTexture()
        {
            return isWolfTamed() ? "/mob/wolf_tame.png" : (isWolfAngry() ? "/mob/wolf_angry.png" : base.getTexture());
        }

        public override void writeNbt(NbtTagCompound var1)
        {
            base.writeNbt(var1);
            var1.SetBoolean("Angry", isWolfAngry());
            var1.SetBoolean("Sitting", isWolfSitting());
            if (getWolfOwner() == null)
            {
                var1.SetString("Owner", "");
            }
            else
            {
                var1.SetString("Owner", getWolfOwner());
            }

        }

        public override void readNbt(NbtTagCompound var1)
        {
            base.readNbt(var1);
            setWolfAngry(var1.GetBoolean("Angry"));
            setWolfSitting(var1.GetBoolean("Sitting"));
            string var2 = var1.GetString("Owner");
            if (var2.Length > 0)
            {
                setWolfOwner(var2);
                setWolfTamed(true);
            }

        }

        protected override bool canDespawn()
        {
            return !isWolfTamed();
        }

        protected override string getLivingSound()
        {
            return isWolfAngry() ? "mob.wolf.growl" : (random.nextInt(3) == 0 ? (isWolfTamed() && dataWatcher.getWatchableObjectInt(18) < 10 ? "mob.wolf.whine" : "mob.wolf.panting") : "mob.wolf.bark");
        }

        protected override string getHurtSound()
        {
            return "mob.wolf.hurt";
        }

        protected override string getDeathSound()
        {
            return "mob.wolf.death";
        }

        protected override float getSoundVolume()
        {
            return 0.4F;
        }

        protected override int getDropItemId()
        {
            return -1;
        }

        public override void tickLiving()
        {
            base.tickLiving();
            if (!hasAttacked && !hasPath() && isWolfTamed() && vehicle == null)
            {
                EntityPlayer var3 = world.getPlayer(getWolfOwner());
                if (var3 != null)
                {
                    float var2 = var3.getDistance(this);
                    if (var2 > 5.0F)
                    {
                        getPathOrWalkableBlock(var3, var2);
                    }
                }
                else if (!isInWater())
                {
                    setWolfSitting(true);
                }
            }
            else if (playerToAttack == null && !hasPath() && !isWolfTamed() && world.random.nextInt(100) == 0)
            {
                var var1 = world.collectEntitiesByClass(EntitySheep.Class, new Box(x, y, z, x + 1.0D, y + 1.0D, z + 1.0D).expand(16.0D, 4.0D, 16.0D));
                if (var1.Count > 0)
                {
                    setTarget(var1[world.random.nextInt(var1.Count)]);
                }
            }

            if (isInWater())
            {
                setWolfSitting(false);
            }

            if (!world.isRemote)
            {
                dataWatcher.updateObject(18, java.lang.Integer.valueOf(health));
            }

        }

        public override void tickMovement()
        {
            base.tickMovement();
            looksWithInterest = false;
            if (hasCurrentTarget() && !hasPath() && !isWolfAngry())
            {
                Entity var1 = getCurrentTarget();
                if (var1 is EntityPlayer)
                {
                    EntityPlayer var2 = (EntityPlayer)var1;
                    ItemStack var3 = var2.inventory.getSelectedItem();
                    if (var3 != null)
                    {
                        if (!isWolfTamed() && var3.itemId == Item.BONE.id)
                        {
                            looksWithInterest = true;
                        }
                        else if (isWolfTamed() && Item.ITEMS[var3.itemId] is ItemFood)
                        {
                            looksWithInterest = ((ItemFood)Item.ITEMS[var3.itemId]).getIsWolfsFavoriteMeat();
                        }
                    }
                }
            }

            if (!interpolateOnly && isWolfShaking && !field_25052_g && !hasPath() && onGround)
            {
                field_25052_g = true;
                timeWolfIsShaking = 0.0F;
                prevTimeWolfIsShaking = 0.0F;
                world.broadcastEntityEvent(this, (byte)8);
            }

        }

        public override void tick()
        {
            base.tick();
            field_25054_c = field_25048_b;
            if (looksWithInterest)
            {
                field_25048_b += (1.0F - field_25048_b) * 0.4F;
            }
            else
            {
                field_25048_b += (0.0F - field_25048_b) * 0.4F;
            }

            if (looksWithInterest)
            {
                lookTimer = 10;
            }

            if (isWet())
            {
                isWolfShaking = true;
                field_25052_g = false;
                timeWolfIsShaking = 0.0F;
                prevTimeWolfIsShaking = 0.0F;
            }
            else if ((isWolfShaking || field_25052_g) && field_25052_g)
            {
                if (timeWolfIsShaking == 0.0F)
                {
                    world.playSound(this, "mob.wolf.shake", getSoundVolume(), (random.nextFloat() - random.nextFloat()) * 0.2F + 1.0F);
                }

                prevTimeWolfIsShaking = timeWolfIsShaking;
                timeWolfIsShaking += 0.05F;
                if (prevTimeWolfIsShaking >= 2.0F)
                {
                    isWolfShaking = false;
                    field_25052_g = false;
                    prevTimeWolfIsShaking = 0.0F;
                    timeWolfIsShaking = 0.0F;
                }

                if (timeWolfIsShaking > 0.4F)
                {
                    float var1 = (float)boundingBox.minY;
                    int var2 = (int)(MathHelper.sin((timeWolfIsShaking - 0.4F) * (float)java.lang.Math.PI) * 7.0F);

                    for (int var3 = 0; var3 < var2; ++var3)
                    {
                        float var4 = (random.nextFloat() * 2.0F - 1.0F) * width * 0.5F;
                        float var5 = (random.nextFloat() * 2.0F - 1.0F) * width * 0.5F;
                        world.addParticle("splash", x + (double)var4, (double)(var1 + 0.8F), z + (double)var5, velocityX, velocityY, velocityZ);
                    }
                }
            }

        }

        public bool getWolfShaking()
        {
            return isWolfShaking;
        }

        public float getShadingWhileShaking(float var1)
        {
            return 12.0F / 16.0F + (prevTimeWolfIsShaking + (timeWolfIsShaking - prevTimeWolfIsShaking) * var1) / 2.0F * 0.25F;
        }

        public float getShakeAngle(float var1, float var2)
        {
            float var3 = (prevTimeWolfIsShaking + (timeWolfIsShaking - prevTimeWolfIsShaking) * var1 + var2) / 1.8F;
            if (var3 < 0.0F)
            {
                var3 = 0.0F;
            }
            else if (var3 > 1.0F)
            {
                var3 = 1.0F;
            }

            return MathHelper.sin(var3 * (float)java.lang.Math.PI) * MathHelper.sin(var3 * (float)java.lang.Math.PI * 11.0F) * 0.15F * (float)java.lang.Math.PI;
        }

        public float getInterestedAngle(float var1)
        {
            return (field_25054_c + (field_25048_b - field_25054_c) * var1) * 0.15F * (float)java.lang.Math.PI;
        }

        public override float getEyeHeight()
        {
            return height * 0.8F;
        }

        protected override int func_25026_x()
        {
            return isWolfSitting() ? 20 : base.func_25026_x();
        }

        private void getPathOrWalkableBlock(Entity var1, float var2)
        {
            PathEntity var3 = world.findPath(this, var1, 16.0F);
            if (var3 == null && var2 > 12.0F)
            {
                int var4 = MathHelper.floor_double(var1.x) - 2;
                int var5 = MathHelper.floor_double(var1.z) - 2;
                int var6 = MathHelper.floor_double(var1.boundingBox.minY);

                for (int var7 = 0; var7 <= 4; ++var7)
                {
                    for (int var8 = 0; var8 <= 4; ++var8)
                    {
                        if ((var7 < 1 || var8 < 1 || var7 > 3 || var8 > 3) && world.shouldSuffocate(var4 + var7, var6 - 1, var5 + var8) && !world.shouldSuffocate(var4 + var7, var6, var5 + var8) && !world.shouldSuffocate(var4 + var7, var6 + 1, var5 + var8))
                        {
                            setPositionAndAnglesKeepPrevAngles((double)((float)(var4 + var7) + 0.5F), (double)var6, (double)((float)(var5 + var8) + 0.5F), yaw, pitch);
                            return;
                        }
                    }
                }
            }
            else
            {
                setPathToEntity(var3);
            }

        }

        protected override bool isMovementCeased()
        {
            return isWolfSitting() || field_25052_g;
        }

        public override bool damage(Entity var1, int var2)
        {
            setWolfSitting(false);
            if (var1 != null && !(var1 is EntityPlayer) && !(var1 is EntityArrow))
            {
                var2 = (var2 + 1) / 2;
            }

            if (!base.damage((Entity)var1, var2))
            {
                return false;
            }
            else
            {
                if (!isWolfTamed() && !isWolfAngry())
                {
                    if (var1 is EntityPlayer)
                    {
                        setWolfAngry(true);
                        playerToAttack = var1;
                    }

                    if (var1 is EntityArrow && ((EntityArrow)var1).owner != null)
                    {
                        var1 = ((EntityArrow)var1).owner;
                    }

                    if (var1 is EntityLiving)
                    {
                        var var3 = world.collectEntitiesByClass(typeof(EntityWolf), new Box(x, y, z, x + 1.0D, y + 1.0D, z + 1.0D).expand(16.0D, 4.0D, 16.0D));

                        foreach (var var5 in var3)
                        {
                            EntityWolf var6 = (EntityWolf)var5;
                            if (!var6.isWolfTamed() && var6.playerToAttack == null)
                            {
                                var6.playerToAttack = var1;
                                if (var1 is EntityPlayer)
                                {

                                    var6.setWolfAngry(true);
                                }
                            }
                        }
                    }
                }
                else if (var1 != this && var1 != null)
                {
                    if (isWolfTamed() && var1 is EntityPlayer && ((EntityPlayer)var1).name.Equals(getWolfOwner(), StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }

                    playerToAttack = (Entity)var1;
                }

                return true;
            }
        }

        protected override Entity findPlayerToAttack()
        {
            return isWolfAngry() ? world.getClosestPlayer(this, 16.0D) : null;
        }

        protected override void attackEntity(Entity var1, float var2)
        {
            if (var2 > 2.0F && var2 < 6.0F && random.nextInt(10) == 0)
            {
                if (onGround)
                {
                    double var8 = var1.x - x;
                    double var5 = var1.z - z;
                    float var7 = MathHelper.sqrt_double(var8 * var8 + var5 * var5);
                    velocityX = var8 / (double)var7 * 0.5D * (double)0.8F + velocityX * (double)0.2F;
                    velocityZ = var5 / (double)var7 * 0.5D * (double)0.8F + velocityZ * (double)0.2F;
                    velocityY = (double)0.4F;
                }
            }
            else if ((double)var2 < 1.5D && var1.boundingBox.maxY > boundingBox.minY && var1.boundingBox.minY < boundingBox.maxY)
            {
                attackTime = 20;
                byte var3 = 2;
                if (isWolfTamed())
                {
                    var3 = 4;
                }

                var1.damage(this, var3);
            }

        }

        public override bool interact(EntityPlayer var1)
        {
            ItemStack var2 = var1.inventory.getSelectedItem();
            if (!isWolfTamed())
            {
                if (var2 != null && var2.itemId == Item.BONE.id && !isWolfAngry())
                {
                    --var2.count;
                    if (var2.count <= 0)
                    {
                        var1.inventory.setStack(var1.inventory.selectedSlot, (ItemStack)null);
                    }

                    if (!world.isRemote)
                    {
                        if (random.nextInt(3) == 0)
                        {
                            setWolfTamed(true);
                            setPathToEntity((PathEntity)null);
                            setWolfSitting(true);
                            health = 20;
                            setWolfOwner(var1.name);
                            showHeartsOrSmokeFX(true);
                            world.broadcastEntityEvent(this, (byte)7);
                        }
                        else
                        {
                            showHeartsOrSmokeFX(false);
                            world.broadcastEntityEvent(this, (byte)6);
                        }
                    }

                    return true;
                }
            }
            else
            {
                if (var2 != null && Item.ITEMS[var2.itemId] is ItemFood)
                {
                    ItemFood var3 = (ItemFood)Item.ITEMS[var2.itemId];
                    if (var3.getIsWolfsFavoriteMeat() && dataWatcher.getWatchableObjectInt(18) < 20)
                    {
                        --var2.count;
                        if (var2.count <= 0)
                        {
                            var1.inventory.setStack(var1.inventory.selectedSlot, (ItemStack)null);
                        }

                        heal(((ItemFood)Item.RAW_PORKCHOP).getHealAmount());
                        return true;
                    }
                }

                if (var1.name.Equals(getWolfOwner(), StringComparison.OrdinalIgnoreCase))
                {
                    if (!world.isRemote)
                    {
                        setWolfSitting(!isWolfSitting());
                        jumping = false;
                        setPathToEntity((PathEntity)null);
                    }

                    return true;
                }
            }

            return false;
        }

        void showHeartsOrSmokeFX(bool var1)
        {
            string var2 = "heart";
            if (!var1)
            {
                var2 = "smoke";
            }

            for (int var3 = 0; var3 < 7; ++var3)
            {
                double var4 = random.nextGaussian() * 0.02D;
                double var6 = random.nextGaussian() * 0.02D;
                double var8 = random.nextGaussian() * 0.02D;
                world.addParticle(var2, x + (double)(random.nextFloat() * width * 2.0F) - (double)width, y + 0.5D + (double)(random.nextFloat() * height), z + (double)(random.nextFloat() * width * 2.0F) - (double)width, var4, var6, var8);
            }

        }

        public override void processServerEntityStatus(sbyte var1)
        {
            if (var1 == 7)
            {
                showHeartsOrSmokeFX(true);
            }
            else if (var1 == 6)
            {
                showHeartsOrSmokeFX(false);
            }
            else if (var1 == 8)
            {
                field_25052_g = true;
                timeWolfIsShaking = 0.0F;
                prevTimeWolfIsShaking = 0.0F;
            }
            else
            {
                base.processServerEntityStatus(var1);
            }

        }

        public float setTailRotation()
        {
            return isWolfAngry() ? (float)java.lang.Math.PI * 0.49F : (isWolfTamed() ? (0.55F - (float)(20 - dataWatcher.getWatchableObjectInt(18)) * 0.02F) * (float)java.lang.Math.PI : (float)java.lang.Math.PI * 0.2F);
        }

        public override int getMaxSpawnedInChunk()
        {
            return 8;
        }

        public string getWolfOwner()
        {
            return dataWatcher.getWatchableObjectString(17);
        }

        public void setWolfOwner(string var1)
        {
            dataWatcher.updateObject(17, new JString(var1));
        }

        public bool isWolfSitting()
        {
            return (dataWatcher.getWatchableObjectByte(16) & 1) != 0;
        }

        public void setWolfSitting(bool var1)
        {
            sbyte var2 = dataWatcher.getWatchableObjectByte(16);
            if (var1)
            {
                dataWatcher.updateObject(16, java.lang.Byte.valueOf((byte)(var2 | 1)));
            }
            else
            {
                dataWatcher.updateObject(16, java.lang.Byte.valueOf((byte)(var2 & -2)));
            }

        }

        public bool isWolfAngry()
        {
            return (dataWatcher.getWatchableObjectByte(16) & 2) != 0;
        }

        public void setWolfAngry(bool var1)
        {
            sbyte var2 = dataWatcher.getWatchableObjectByte(16);
            if (var1)
            {
                dataWatcher.updateObject(16, java.lang.Byte.valueOf((byte)(var2 | 2)));
            }
            else
            {
                dataWatcher.updateObject(16, java.lang.Byte.valueOf((byte)(var2 & -3)));
            }

        }

        public bool isWolfTamed()
        {
            return (dataWatcher.getWatchableObjectByte(16) & 4) != 0;
        }

        public void setWolfTamed(bool var1)
        {
            sbyte var2 = dataWatcher.getWatchableObjectByte(16);
            if (var1)
            {
                dataWatcher.updateObject(16, java.lang.Byte.valueOf((byte)(var2 | 4)));
            }
            else
            {
                dataWatcher.updateObject(16, java.lang.Byte.valueOf((byte)(var2 & -5)));
            }

        }
    }

}