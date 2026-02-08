using betareborn.Blocks;
using betareborn.Chunks;
using betareborn.Containers;
using betareborn.Items;
using betareborn.NBT;
using betareborn.Stats;
using betareborn.Worlds;
using betareborn.Worlds.Chunks;
using java.lang;
using betareborn.Blocks.BlockEntities;
using betareborn.Blocks.Materials;
using betareborn.Inventorys;

namespace betareborn.Entities
{
    public abstract class EntityPlayer : EntityLiving
    {
        public static readonly new Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(EntityPlayer).TypeHandle);
        public InventoryPlayer inventory;
        public Container inventorySlots;
        public Container craftingInventory;
        public byte field_9371_f = 0;
        public int score = 0;
        public float field_775_e;
        public float field_774_f;
        public bool isSwinging = false;
        public int swingProgressInt = 0;
        public string username;
        public int dimension;
        public string playerCloakUrl;
        public double field_20066_r;
        public double field_20065_s;
        public double field_20064_t;
        public double field_20063_u;
        public double field_20062_v;
        public double field_20061_w;
        protected bool sleeping;
        public Vec3i bedChunkCoordinates;
        private int sleepTimer;
        public float field_22063_x;
        public float field_22062_y;
        public float field_22061_z;
        private Vec3i playerSpawnCoordinate;
        private Vec3i startMinecartRidingCoordinate;
        public int timeUntilPortal = 20;
        protected bool inPortal = false;
        public float timeInPortal;
        public float prevTimeInPortal;
        private int damageRemainder = 0;
        public EntityFish fishEntity = null;

        public EntityPlayer(World var1) : base(var1)
        {
            inventory = new InventoryPlayer(this);
            inventorySlots = new ContainerPlayer(inventory, !var1.isRemote);
            craftingInventory = inventorySlots;
            yOffset = 1.62F;
            Vec3i var2 = var1.getSpawnPoint();
            setPositionAndAnglesKeepPrevAngles((double)var2.x + 0.5D, (double)(var2.y + 1), (double)var2.z + 0.5D, 0.0F, 0.0F);
            health = 20;
            field_9351_C = "humanoid";
            field_9353_B = 180.0F;
            fireResistance = 20;
            texture = "/mob/char.png";
        }

        protected override void entityInit()
        {
            base.entityInit();
            dataWatcher.addObject(16, java.lang.Byte.valueOf((byte)0));
        }

        public override void onUpdate()
        {
            if (isPlayerSleeping())
            {
                ++sleepTimer;
                if (sleepTimer > 100)
                {
                    sleepTimer = 100;
                }

                if (!worldObj.isRemote)
                {
                    if (!isInBed())
                    {
                        wakeUpPlayer(true, true, false);
                    }
                    else if (worldObj.isDaytime())
                    {
                        wakeUpPlayer(false, true, true);
                    }
                }
            }
            else if (sleepTimer > 0)
            {
                ++sleepTimer;
                if (sleepTimer >= 110)
                {
                    sleepTimer = 0;
                }
            }

            base.onUpdate();
            if (!worldObj.isRemote && craftingInventory != null && !craftingInventory.isUsableByPlayer(this))
            {
                closeScreen();
                craftingInventory = inventorySlots;
            }

            field_20066_r = field_20063_u;
            field_20065_s = field_20062_v;
            field_20064_t = field_20061_w;
            double var1 = posX - field_20063_u;
            double var3 = posY - field_20062_v;
            double var5 = posZ - field_20061_w;
            double var7 = 10.0D;
            if (var1 > var7)
            {
                field_20066_r = field_20063_u = posX;
            }

            if (var5 > var7)
            {
                field_20064_t = field_20061_w = posZ;
            }

            if (var3 > var7)
            {
                field_20065_s = field_20062_v = posY;
            }

            if (var1 < -var7)
            {
                field_20066_r = field_20063_u = posX;
            }

            if (var5 < -var7)
            {
                field_20064_t = field_20061_w = posZ;
            }

            if (var3 < -var7)
            {
                field_20065_s = field_20062_v = posY;
            }

            field_20063_u += var1 * 0.25D;
            field_20061_w += var5 * 0.25D;
            field_20062_v += var3 * 0.25D;
            increaseStat(Stats.Stats.minutesPlayedStat, 1);
            if (ridingEntity == null)
            {
                startMinecartRidingCoordinate = null;
            }

        }

        protected override bool isMovementBlocked()
        {
            return health <= 0 || isPlayerSleeping();
        }

        public virtual void closeScreen()
        {
            craftingInventory = inventorySlots;
        }

        public override void updateCloak()
        {
            playerCloakUrl = "http://s3.amazonaws.com/MinecraftCloaks/" + username + ".png";
            cloakUrl = playerCloakUrl;
        }

        public override void updateRidden()
        {
            double var1 = posX;
            double var3 = posY;
            double var5 = posZ;
            base.updateRidden();
            field_775_e = field_774_f;
            field_774_f = 0.0F;
            addMountedMovementStat(posX - var1, posY - var3, posZ - var5);
        }

        public override void preparePlayerToSpawn()
        {
            yOffset = 1.62F;
            setSize(0.6F, 1.8F);
            base.preparePlayerToSpawn();
            health = 20;
            deathTime = 0;
        }

        public override void updatePlayerActionState()
        {
            if (isSwinging)
            {
                ++swingProgressInt;
                if (swingProgressInt >= 8)
                {
                    swingProgressInt = 0;
                    isSwinging = false;
                }
            }
            else
            {
                swingProgressInt = 0;
            }

            swingProgress = (float)swingProgressInt / 8.0F;
        }

        public override void onLivingUpdate()
        {
            if (worldObj.difficultySetting == 0 && health < 20 && ticksExisted % 20 * 12 == 0)
            {
                heal(1);
            }

            inventory.decrementAnimations();
            field_775_e = field_774_f;
            base.onLivingUpdate();
            float var1 = MathHelper.sqrt_double(motionX * motionX + motionZ * motionZ);
            float var2 = (float)java.lang.Math.atan(-motionY * (double)0.2F) * 15.0F;
            if (var1 > 0.1F)
            {
                var1 = 0.1F;
            }

            if (!onGround || health <= 0)
            {
                var1 = 0.0F;
            }

            if (onGround || health <= 0)
            {
                var2 = 0.0F;
            }

            field_774_f += (var1 - field_774_f) * 0.4F;
            field_9328_R += (var2 - field_9328_R) * 0.8F;
            if (health > 0)
            {
                var var3 = worldObj.getEntitiesWithinAABBExcludingEntity(this, boundingBox.expand(1.0D, 0.0D, 1.0D));
                if (var3 != null)
                {
                    for (int var4 = 0; var4 < var3.Count; ++var4)
                    {
                        Entity var5 = var3[var4];
                        if (!var5.isDead)
                        {
                            collideWithPlayer(var5);
                        }
                    }
                }
            }

        }

        private void collideWithPlayer(Entity var1)
        {
            var1.onCollideWithPlayer(this);
        }

        public int getScore()
        {
            return score;
        }

        public override void onDeath(Entity var1)
        {
            base.onDeath(var1);
            setSize(0.2F, 0.2F);
            setPosition(posX, posY, posZ);
            motionY = (double)0.1F;
            if (username.Equals("Notch"))
            {
                dropPlayerItemWithRandomChoice(new ItemStack(Item.appleRed, 1), true);
            }

            inventory.dropAllItems();
            if (var1 != null)
            {
                motionX = (double)(-MathHelper.cos((attackedAtYaw + rotationYaw) * (float)java.lang.Math.PI / 180.0F) * 0.1F);
                motionZ = (double)(-MathHelper.sin((attackedAtYaw + rotationYaw) * (float)java.lang.Math.PI / 180.0F) * 0.1F);
            }
            else
            {
                motionX = motionZ = 0.0D;
            }

            yOffset = 0.1F;
            increaseStat(Stats.Stats.deathsStat, 1);
        }

        public override void addToPlayerScore(Entity var1, int var2)
        {
            score += var2;
            if (var1 is EntityPlayer)
            {
                increaseStat(Stats.Stats.playerKillsStat, 1);
            }
            else
            {
                increaseStat(Stats.Stats.mobKillsStat, 1);
            }

        }

        public virtual void dropCurrentItem()
        {
            dropPlayerItemWithRandomChoice(inventory.removeStack(inventory.currentItem, 1), false);
        }

        public void dropPlayerItem(ItemStack var1)
        {
            dropPlayerItemWithRandomChoice(var1, false);
        }

        public void dropPlayerItemWithRandomChoice(ItemStack var1, bool var2)
        {
            if (var1 != null)
            {
                EntityItem var3 = new EntityItem(worldObj, posX, posY - (double)0.3F + (double)getEyeHeight(), posZ, var1);
                var3.delayBeforeCanPickup = 40;
                float var4 = 0.1F;
                float var5;
                if (var2)
                {
                    var5 = rand.nextFloat() * 0.5F;
                    float var6 = rand.nextFloat() * (float)java.lang.Math.PI * 2.0F;
                    var3.motionX = (double)(-MathHelper.sin(var6) * var5);
                    var3.motionZ = (double)(MathHelper.cos(var6) * var5);
                    var3.motionY = (double)0.2F;
                }
                else
                {
                    var4 = 0.3F;
                    var3.motionX = (double)(-MathHelper.sin(rotationYaw / 180.0F * (float)java.lang.Math.PI) * MathHelper.cos(rotationPitch / 180.0F * (float)java.lang.Math.PI) * var4);
                    var3.motionZ = (double)(MathHelper.cos(rotationYaw / 180.0F * (float)java.lang.Math.PI) * MathHelper.cos(rotationPitch / 180.0F * (float)java.lang.Math.PI) * var4);
                    var3.motionY = (double)(-MathHelper.sin(rotationPitch / 180.0F * (float)java.lang.Math.PI) * var4 + 0.1F);
                    var4 = 0.02F;
                    var5 = rand.nextFloat() * (float)java.lang.Math.PI * 2.0F;
                    var4 *= rand.nextFloat();
                    var3.motionX += java.lang.Math.cos((double)var5) * (double)var4;
                    var3.motionY += (double)((rand.nextFloat() - rand.nextFloat()) * 0.1F);
                    var3.motionZ += java.lang.Math.sin((double)var5) * (double)var4;
                }

                joinEntityItemWithWorld(var3);
                increaseStat(Stats.Stats.dropStat, 1);
            }
        }

        protected virtual void joinEntityItemWithWorld(EntityItem var1)
        {
            worldObj.spawnEntity(var1);
        }

        public float getCurrentPlayerStrVsBlock(Block var1)
        {
            float var2 = inventory.getStrVsBlock(var1);
            if (isInsideOfMaterial(Material.WATER))
            {
                var2 /= 5.0F;
            }

            if (!onGround)
            {
                var2 /= 5.0F;
            }

            return var2;
        }

        public bool canHarvest(Block var1)
        {
            return inventory.canHarvestBlock(var1);
        }

        public override void readEntityFromNBT(NBTTagCompound var1)
        {
            base.readEntityFromNBT(var1);
            NBTTagList var2 = var1.getTagList("Inventory");
            inventory.readFromNBT(var2);
            dimension = var1.getInteger("Dimension");
            sleeping = var1.getBoolean("Sleeping");
            sleepTimer = var1.getShort("SleepTimer");
            if (sleeping)
            {
                bedChunkCoordinates = new Vec3i(MathHelper.floor_double(posX), MathHelper.floor_double(posY), MathHelper.floor_double(posZ));
                wakeUpPlayer(true, true, false);
            }

            if (var1.hasKey("SpawnX") && var1.hasKey("SpawnY") && var1.hasKey("SpawnZ"))
            {
                playerSpawnCoordinate = new Vec3i(var1.getInteger("SpawnX"), var1.getInteger("SpawnY"), var1.getInteger("SpawnZ"));
            }

        }

        public override void writeEntityToNBT(NBTTagCompound var1)
        {
            base.writeEntityToNBT(var1);
            var1.setTag("Inventory", inventory.writeToNBT(new NBTTagList()));
            var1.setInteger("Dimension", dimension);
            var1.setBoolean("Sleeping", sleeping);
            var1.setShort("SleepTimer", (short)sleepTimer);
            if (playerSpawnCoordinate != null)
            {
                var1.setInteger("SpawnX", playerSpawnCoordinate.x);
                var1.setInteger("SpawnY", playerSpawnCoordinate.y);
                var1.setInteger("SpawnZ", playerSpawnCoordinate.z);
            }

        }

        public virtual void displayGUIChest(IInventory var1)
        {
        }

        public virtual void displayWorkbenchGUI(int var1, int var2, int var3)
        {
        }

        public virtual void onItemPickup(Entity var1, int var2)
        {
        }

        public override float getEyeHeight()
        {
            return 0.12F;
        }

        protected virtual void resetHeight()
        {
            yOffset = 1.62F;
        }

        public override bool attackEntityFrom(Entity var1, int var2)
        {
            entityAge = 0;
            if (health <= 0)
            {
                return false;
            }
            else
            {
                if (isPlayerSleeping() && !worldObj.isRemote)
                {
                    wakeUpPlayer(true, true, false);
                }

                if (var1 is EntityMob || var1 is EntityArrow)
                {
                    if (worldObj.difficultySetting == 0)
                    {
                        var2 = 0;
                    }

                    if (worldObj.difficultySetting == 1)
                    {
                        var2 = var2 / 3 + 1;
                    }

                    if (worldObj.difficultySetting == 3)
                    {
                        var2 = var2 * 3 / 2;
                    }
                }

                if (var2 == 0)
                {
                    return false;
                }
                else
                {
                    java.lang.Object var3 = var1;
                    if (var1 is EntityArrow && ((EntityArrow)var1).owner != null)
                    {
                        var3 = ((EntityArrow)var1).owner;
                    }

                    if (var3 is EntityLiving)
                    {
                        alertWolves((EntityLiving)var3, false);
                    }

                    increaseStat(Stats.Stats.damageTakenStat, var2);
                    return base.attackEntityFrom(var1, var2);
                }
            }
        }

        protected bool func_27025_G()
        {
            return false;
        }

        protected void alertWolves(EntityLiving var1, bool var2)
        {
            if (!(var1 is EntityCreeper) && !(var1 is EntityGhast))
            {
                if (var1 is EntityWolf)
                {
                    EntityWolf var3 = (EntityWolf)var1;
                    if (var3.isWolfTamed() && username.Equals(var3.getWolfOwner()))
                    {
                        return;
                    }
                }

                if (!(var1 is EntityPlayer) || func_27025_G())
                {
                    var var7 = worldObj.getEntitiesWithinAABB(EntityWolf.Class, new Box(posX, posY, posZ, posX + 1.0D, posY + 1.0D, posZ + 1.0D).expand(16.0D, 4.0D, 16.0D));

                    foreach (Entity var5 in var7)
                    {
                        EntityWolf var6 = (EntityWolf)var5;

                        if (!var6.isWolfTamed()) continue;
                        if (var6.getTarget() != null) continue;
                        if (!username.Equals(var6.getWolfOwner())) continue;
                        if (var2 && var6.isWolfSitting()) continue;

                        var6.setWolfSitting(false);
                        var6.setTarget(var1);
                    }
                }
            }
        }

        protected override void damageEntity(int var1)
        {
            int var2 = 25 - inventory.getTotalArmorValue();
            int var3 = var1 * var2 + damageRemainder;
            inventory.damageArmor(var1);
            var1 = var3 / 25;
            damageRemainder = var3 % 25;
            base.damageEntity(var1);
        }

        public virtual void displayGUIFurnace(BlockEntityFurnace var1)
        {
        }

        public virtual void displayGUIDispenser(BlockEntityDispenser var1)
        {
        }

        public virtual void displayGUIEditSign(BlockEntitySign var1)
        {
        }

        public void useCurrentItemOnEntity(Entity var1)
        {
            if (!var1.interact(this))
            {
                ItemStack var2 = getCurrentEquippedItem();
                if (var2 != null && var1 is EntityLiving)
                {
                    var2.useItemOnEntity((EntityLiving)var1);
                    if (var2.count <= 0)
                    {
                        var2.func_1097_a(this);
                        destroyCurrentEquippedItem();
                    }
                }

            }
        }

        public ItemStack getCurrentEquippedItem()
        {
            return inventory.getCurrentItem();
        }

        public void destroyCurrentEquippedItem()
        {
            inventory.setStack(inventory.currentItem, (ItemStack)null);
        }

        public override double getYOffset()
        {
            return (double)(yOffset - 0.5F);
        }

        public virtual void swingItem()
        {
            swingProgressInt = -1;
            isSwinging = true;
        }

        public void attackTargetEntityWithCurrentItem(Entity var1)
        {
            int var2 = inventory.getDamageVsEntity(var1);
            if (var2 > 0)
            {
                if (motionY < 0.0D)
                {
                    ++var2;
                }

                var1.attackEntityFrom(this, var2);
                ItemStack var3 = getCurrentEquippedItem();
                if (var3 != null && var1 is EntityLiving)
                {
                    var3.hitEntity((EntityLiving)var1, this);
                    if (var3.count <= 0)
                    {
                        var3.func_1097_a(this);
                        destroyCurrentEquippedItem();
                    }
                }

                if (var1 is EntityLiving)
                {
                    if (var1.isEntityAlive())
                    {
                        alertWolves((EntityLiving)var1, true);
                    }

                    increaseStat(Stats.Stats.damageDealtStat, var2);
                }
            }

        }

        public virtual void respawnPlayer()
        {
        }

        public abstract void func_6420_o();

        public void onItemStackChanged(ItemStack var1)
        {
        }

        public override void setEntityDead()
        {
            base.setEntityDead();
            inventorySlots.onCraftGuiClosed(this);
            if (craftingInventory != null)
            {
                craftingInventory.onCraftGuiClosed(this);
            }

        }

        public override bool isEntityInsideOpaqueBlock()
        {
            return !sleeping && base.isEntityInsideOpaqueBlock();
        }

        public EnumStatus sleepInBedAt(int var1, int var2, int var3)
        {
            if (!worldObj.isRemote)
            {
                if (isPlayerSleeping() || !isEntityAlive())
                {
                    return EnumStatus.OTHER_PROBLEM;
                }

                if (worldObj.dimension.isNether)
                {
                    return EnumStatus.NOT_POSSIBLE_HERE;
                }

                if (worldObj.isDaytime())
                {
                    return EnumStatus.NOT_POSSIBLE_NOW;
                }

                if (java.lang.Math.abs(posX - (double)var1) > 3.0D || java.lang.Math.abs(posY - (double)var2) > 2.0D || java.lang.Math.abs(posZ - (double)var3) > 3.0D)
                {
                    return EnumStatus.TOO_FAR_AWAY;
                }
            }

            setSize(0.2F, 0.2F);
            yOffset = 0.2F;
            if (worldObj.blockExists(var1, var2, var3))
            {
                int var4 = worldObj.getBlockMeta(var1, var2, var3);
                int var5 = BlockBed.getDirection(var4);
                float var6 = 0.5F;
                float var7 = 0.5F;
                switch (var5)
                {
                    case 0:
                        var7 = 0.9F;
                        break;
                    case 1:
                        var6 = 0.1F;
                        break;
                    case 2:
                        var7 = 0.1F;
                        break;
                    case 3:
                        var6 = 0.9F;
                        break;
                }

                func_22052_e(var5);
                setPosition((double)((float)var1 + var6), (double)((float)var2 + 15.0F / 16.0F), (double)((float)var3 + var7));
            }
            else
            {
                setPosition((double)((float)var1 + 0.5F), (double)((float)var2 + 15.0F / 16.0F), (double)((float)var3 + 0.5F));
            }

            sleeping = true;
            sleepTimer = 0;
            bedChunkCoordinates = new Vec3i(var1, var2, var3);
            motionX = motionZ = motionY = 0.0D;
            if (!worldObj.isRemote)
            {
                worldObj.updateAllPlayersSleepingFlag();
            }

            return EnumStatus.OK;
        }

        private void func_22052_e(int var1)
        {
            field_22063_x = 0.0F;
            field_22061_z = 0.0F;
            switch (var1)
            {
                case 0:
                    field_22061_z = -1.8F;
                    break;
                case 1:
                    field_22063_x = 1.8F;
                    break;
                case 2:
                    field_22061_z = 1.8F;
                    break;
                case 3:
                    field_22063_x = -1.8F;
                    break;
            }

        }

        public void wakeUpPlayer(bool var1, bool var2, bool var3)
        {
            setSize(0.6F, 1.8F);
            resetHeight();
            Vec3i var4 = bedChunkCoordinates;
            Vec3i var5 = bedChunkCoordinates;
            if (var4 != null && worldObj.getBlockId(var4.x, var4.y, var4.z) == Block.BED.id)
            {
                BlockBed.updateState(worldObj, var4.x, var4.y, var4.z, false);
                var5 = BlockBed.findWakeUpPosition(worldObj, var4.x, var4.y, var4.z, 0);
                if (var5 == null)
                {
                    var5 = new Vec3i(var4.x, var4.y + 1, var4.z);
                }

                setPosition((double)((float)var5.x + 0.5F), (double)((float)var5.y + yOffset + 0.1F), (double)((float)var5.z + 0.5F));
            }

            sleeping = false;
            if (!worldObj.isRemote && var2)
            {
                worldObj.updateAllPlayersSleepingFlag();
            }

            if (var1)
            {
                sleepTimer = 0;
            }
            else
            {
                sleepTimer = 100;
            }

            if (var3)
            {
                setPlayerSpawnCoordinate(bedChunkCoordinates);
            }

        }

        private bool isInBed()
        {
            return worldObj.getBlockId(bedChunkCoordinates.x, bedChunkCoordinates.y, bedChunkCoordinates.z) == Block.BED.id;
        }

        public static Vec3i func_25060_a(World var0, Vec3i var1)
        {
            ChunkSource var2 = var0.getIChunkProvider();
            var2.loadChunk(var1.x - 3 >> 4, var1.z - 3 >> 4);
            var2.loadChunk(var1.x + 3 >> 4, var1.z - 3 >> 4);
            var2.loadChunk(var1.x - 3 >> 4, var1.z + 3 >> 4);
            var2.loadChunk(var1.x + 3 >> 4, var1.z + 3 >> 4);
            if (var0.getBlockId(var1.x, var1.y, var1.z) != Block.BED.id)
            {
                return null;
            }
            else
            {
                Vec3i var3 = BlockBed.findWakeUpPosition(var0, var1.x, var1.y, var1.z, 0);
                return var3;
            }
        }

        public float getBedOrientationInDegrees()
        {
            if (bedChunkCoordinates != null)
            {
                int var1 = worldObj.getBlockMeta(bedChunkCoordinates.x, bedChunkCoordinates.y, bedChunkCoordinates.z);
                int var2 = BlockBed.getDirection(var1);
                switch (var2)
                {
                    case 0:
                        return 90.0F;
                    case 1:
                        return 0.0F;
                    case 2:
                        return 270.0F;
                    case 3:
                        return 180.0F;
                }
            }

            return 0.0F;
        }

        public override bool isPlayerSleeping()
        {
            return sleeping;
        }

        public bool isPlayerFullyAsleep()
        {
            return sleeping && sleepTimer >= 100;
        }

        public int func_22060_M()
        {
            return sleepTimer;
        }

        public virtual void addChatMessage(string var1)
        {
        }

        public Vec3i getPlayerSpawnCoordinate()
        {
            return playerSpawnCoordinate;
        }

        public void setPlayerSpawnCoordinate(Vec3i var1)
        {
            if (var1 != null)
            {
                playerSpawnCoordinate = new Vec3i(var1);
            }
            else
            {
                playerSpawnCoordinate = null;
            }

        }

        public void triggerAchievement(StatBase var1)
        {
            increaseStat(var1, 1);
        }

        public virtual void increaseStat(StatBase var1, int var2)
        {
        }

        protected override void jump()
        {
            base.jump();
            // motionY = (double)0.42F*5; // fun
            increaseStat(Stats.Stats.jumpStat, 1);
        }

        public override void moveEntityWithHeading(float var1, float var2)
        {
            double var3 = posX;
            double var5 = posY;
            double var7 = posZ;
            base.moveEntityWithHeading(var1, var2);
            addMovementStat(posX - var3, posY - var5, posZ - var7);
        }

        private void addMovementStat(double var1, double var3, double var5)
        {
            if (ridingEntity == null)
            {
                int var7;
                if (isInsideOfMaterial(Material.WATER))
                {
                    var7 = java.lang.Math.round(MathHelper.sqrt_double(var1 * var1 + var3 * var3 + var5 * var5) * 100.0F);
                    if (var7 > 0)
                    {
                        increaseStat(Stats.Stats.distanceDoveStat, var7);
                    }
                }
                else if (isInWater())
                {
                    var7 = java.lang.Math.round(MathHelper.sqrt_double(var1 * var1 + var5 * var5) * 100.0F);
                    if (var7 > 0)
                    {
                        increaseStat(Stats.Stats.distanceSwumStat, var7);
                    }
                }
                else if (isOnLadder())
                {
                    if (var3 > 0.0D)
                    {
                        increaseStat(Stats.Stats.distanceClimbedStat, (int)java.lang.Math.round(var3 * 100.0D));
                    }
                }
                else if (onGround)
                {
                    var7 = java.lang.Math.round(MathHelper.sqrt_double(var1 * var1 + var5 * var5) * 100.0F);
                    if (var7 > 0)
                    {
                        increaseStat(Stats.Stats.distanceWalkedStat, var7);
                    }
                }
                else
                {
                    var7 = java.lang.Math.round(MathHelper.sqrt_double(var1 * var1 + var5 * var5) * 100.0F);
                    if (var7 > 25)
                    {
                        increaseStat(Stats.Stats.distanceFlownStat, var7);
                    }
                }

            }
        }

        private void addMountedMovementStat(double var1, double var3, double var5)
        {
            if (ridingEntity != null)
            {
                int var7 = java.lang.Math.round(MathHelper.sqrt_double(var1 * var1 + var3 * var3 + var5 * var5) * 100.0F);
                if (var7 > 0)
                {
                    if (ridingEntity is EntityMinecart)
                    {
                        increaseStat(Stats.Stats.distanceByMinecartStat, var7);
                        if (startMinecartRidingCoordinate == null)
                        {
                            startMinecartRidingCoordinate = new Vec3i(MathHelper.floor_double(posX), MathHelper.floor_double(posY), MathHelper.floor_double(posZ));
                        }
                        else if (startMinecartRidingCoordinate.getSqDistanceTo(MathHelper.floor_double(posX), MathHelper.floor_double(posY), MathHelper.floor_double(posZ)) >= 1000.0D)
                        {
                            increaseStat(Achievements.CRAFT_RAIL, 1);
                        }
                    }
                    else if (ridingEntity is EntityBoat)
                    {
                        increaseStat(Stats.Stats.distanceByBoatStat, var7);
                    }
                    else if (ridingEntity is EntityPig)
                    {
                        increaseStat(Stats.Stats.distanceByPigStat, var7);
                    }
                }
            }

        }

        protected override void fall(float var1)
        {
            if (var1 >= 2.0F)
            {
                increaseStat(Stats.Stats.distanceFallenStat, (int)java.lang.Math.round((double)var1 * 100.0D));
            }

            base.fall(var1);
        }

        public override void onKillEntity(EntityLiving var1)
        {
            if (var1 is EntityMob)
            {
                triggerAchievement(Achievements.KILL_ENEMY);
            }

        }

        public override int getItemIcon(ItemStack var1)
        {
            int var2 = base.getItemIcon(var1);
            if (var1.itemID == Item.fishingRod.id && fishEntity != null)
            {
                var2 = var1.getIconIndex() + 16;
            }

            return var2;
        }

        public override void setInPortal()
        {
            if (timeUntilPortal > 0)
            {
                timeUntilPortal = 10;
            }
            else
            {
                inPortal = true;
            }
        }
    }

}