using BetaSharp.Blocks;
using BetaSharp.Blocks.Entities;
using BetaSharp.Blocks.Materials;
using BetaSharp.Inventorys;
using BetaSharp.Items;
using BetaSharp.NBT;
using BetaSharp.Screens;
using BetaSharp.Stats;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds;
using BetaSharp.Worlds.Chunks;
using java.lang;

namespace BetaSharp.Entities;

public abstract class EntityPlayer : EntityLiving
{
    public static readonly new Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(EntityPlayer).TypeHandle);
    public InventoryPlayer inventory;
    public ScreenHandler playerScreenHandler;
    public ScreenHandler currentScreenHandler;
    public byte unused = 0;
    public int score;
    public float prevStepBobbingAmount;
    public float stepBobbingAmount;
    public bool handSwinging;
    public int handSwingTicks;
    public string name;
    public int dimensionId;
    public string playerCloakUrl;
    public double prevCapeX;
    public double prevCapeY;
    public double prevCapeZ;
    public double capeX;
    public double capeY;
    public double capeZ;
    protected bool sleeping;
    public Vec3i sleepingPos;
    private int sleepTimer;
    public float sleepOffsetX;
    public float sleepOffsetY;
    public float sleepOffsetZ;
    private Vec3i playerSpawnCoordinate;
    private Vec3i startMinecartRidingCoordinate;
    public int portalCooldown = 20;
    protected bool inTeleportationState;
    public float changeDimensionCooldown;
    public float lastScreenDistortion;
    private int damageSpill;
    public EntityFish fishHook = null;

    public EntityPlayer(World world) : base(world)
    {
        inventory = new InventoryPlayer(this);
        playerScreenHandler = new PlayerScreenHandler(inventory, !world.isRemote);
        currentScreenHandler = playerScreenHandler;
        standingEyeHeight = 1.62F;
        Vec3i var2 = world.getSpawnPos();
        setPositionAndAnglesKeepPrevAngles((double)var2.x + 0.5D, (double)(var2.y + 1), (double)var2.z + 0.5D, 0.0F, 0.0F);
        health = 20;
        modelName = "humanoid";
        rotationOffset = 180.0F;
        fireImmunityTicks = 20;
        texture = "/mob/char.png";
    }

    protected override void initDataTracker()
    {
        base.initDataTracker();
        dataWatcher.addObject(16, java.lang.Byte.valueOf((byte)0));
    }

    public override void tick()
    {
        if (isSleeping())
        {
            ++sleepTimer;
            if (sleepTimer > 100)
            {
                sleepTimer = 100;
            }

            if (!world.isRemote)
            {
                if (!isSleepingInBed())
                {
                    wakeUp(true, true, false);
                }
                else if (world.canMonsterSpawn())
                {
                    wakeUp(false, true, true);
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

        base.tick();
        if (!world.isRemote && currentScreenHandler != null && !currentScreenHandler.canUse(this))
        {
            closeHandledScreen();
            currentScreenHandler = playerScreenHandler;
        }

        prevCapeX = capeX;
        prevCapeY = capeY;
        prevCapeZ = capeZ;
        double var1 = x - capeX;
        double var3 = y - capeY;
        double var5 = z - capeZ;
        double var7 = 10.0D;
        if (var1 > var7)
        {
            prevCapeX = capeX = x;
        }

        if (var5 > var7)
        {
            prevCapeZ = capeZ = z;
        }

        if (var3 > var7)
        {
            prevCapeY = capeY = y;
        }

        if (var1 < -var7)
        {
            prevCapeX = capeX = x;
        }

        if (var5 < -var7)
        {
            prevCapeZ = capeZ = z;
        }

        if (var3 < -var7)
        {
            prevCapeY = capeY = y;
        }

        capeX += var1 * 0.25D;
        capeZ += var5 * 0.25D;
        capeY += var3 * 0.25D;
        increaseStat(Stats.Stats.minutesPlayedStat, 1);
        if (vehicle == null)
        {
            startMinecartRidingCoordinate = null;
        }

    }

    protected override bool isMovementBlocked()
    {
        return health <= 0 || isSleeping();
    }

    public virtual void closeHandledScreen()
    {
        currentScreenHandler = playerScreenHandler;
    }

    public override void updateCloak()
    {
        playerCloakUrl = "http://s3.amazonaws.com/MinecraftCloaks/" + name + ".png";
        cloakUrl = playerCloakUrl;
    }

    protected virtual bool isPvpEnabled()
    {
        return false;
    }

    public override void tickRiding()
    {
        double var1 = x;
        double var3 = y;
        double var5 = z;
        base.tickRiding();
        prevStepBobbingAmount = stepBobbingAmount;
        stepBobbingAmount = 0.0F;
        increaseRidingMotionStats(x - var1, y - var3, z - var5);
    }

    public override void teleportToTop()
    {
        standingEyeHeight = 1.62F;
        setBoundingBoxSpacing(0.6F, 1.8F);
        base.teleportToTop();
        health = 20;
        deathTime = 0;
    }

    public override void tickLiving()
    {
        if (handSwinging)
        {
            ++handSwingTicks;
            if (handSwingTicks >= 8)
            {
                handSwingTicks = 0;
                handSwinging = false;
            }
        }
        else
        {
            handSwingTicks = 0;
        }

        swingAnimationProgress = (float)handSwingTicks / 8.0F;
    }

    public override void tickMovement()
    {
        if (world.difficulty == 0 && health < 20 && age % 20 * 12 == 0)
        {
            heal(1);
        }

        inventory.inventoryTick();
        prevStepBobbingAmount = stepBobbingAmount;
        base.tickMovement();
        float var1 = MathHelper.sqrt_double(velocityX * velocityX + velocityZ * velocityZ);
        float var2 = (float)System.Math.Atan(-velocityY * (double)0.2F) * 15.0F;
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

        stepBobbingAmount += (var1 - stepBobbingAmount) * 0.4F;
        tilt += (var2 - tilt) * 0.8F;
        if (health > 0)
        {
            var var3 = world.getEntities(this, boundingBox.expand(1.0D, 0.0D, 1.0D));
            if (var3 != null)
            {
                for (int var4 = 0; var4 < var3.Count; ++var4)
                {
                    Entity var5 = var3[var4];
                    if (!var5.dead)
                    {
                        collideWithEntity(var5);
                    }
                }
            }
        }

    }

    private void collideWithEntity(Entity entity)
    {
        entity.onPlayerInteraction(this);
    }

    public int getScore()
    {
        return score;
    }

    public override void onKilledBy(Entity adversary)
    {
        base.onKilledBy(adversary);
        setBoundingBoxSpacing(0.2F, 0.2F);
        setPosition(x, y, z);
        velocityY = (double)0.1F;
        if (name.Equals("Notch"))
        {
            dropItem(new ItemStack(Item.Apple, 1), true);
        }

        inventory.dropInventory();
        if (adversary != null)
        {
            velocityX = (double)(-MathHelper.cos((attackedAtYaw + yaw) * (float)System.Math.PI / 180.0F) * 0.1F);
            velocityZ = (double)(-MathHelper.sin((attackedAtYaw + yaw) * (float)System.Math.PI / 180.0F) * 0.1F);
        }
        else
        {
            velocityX = velocityZ = 0.0D;
        }

        standingEyeHeight = 0.1F;
        increaseStat(Stats.Stats.deathsStat, 1);
    }

    public override void updateKilledAchievement(Entity entityKilled, int score)
    {
        this.score += score;
        if (entityKilled is EntityPlayer)
        {
            increaseStat(Stats.Stats.playerKillsStat, 1);
        }
        else
        {
            increaseStat(Stats.Stats.mobKillsStat, 1);
        }

    }

    public virtual void dropSelectedItem()
    {
        dropItem(inventory.removeStack(inventory.selectedSlot, 1), false);
    }

    public void dropItem(ItemStack stack)
    {
        dropItem(stack, false);
    }

    public void dropItem(ItemStack stack, bool throwRandomly)
    {
        if (stack != null)
        {
            EntityItem var3 = new EntityItem(world, x, y - (double)0.3F + (double)getEyeHeight(), z, stack);
            var3.delayBeforeCanPickup = 40;
            float var4 = 0.1F;
            float var5;
            if (throwRandomly)
            {
                var5 = random.NextFloat() * 0.5F;
                float var6 = random.NextFloat() * (float)System.Math.PI * 2.0F;
                var3.velocityX = (double)(-MathHelper.sin(var6) * var5);
                var3.velocityZ = (double)(MathHelper.cos(var6) * var5);
                var3.velocityY = (double)0.2F;
            }
            else
            {
                var4 = 0.3F;
                var3.velocityX = (double)(-MathHelper.sin(yaw / 180.0F * (float)System.Math.PI) * MathHelper.cos(pitch / 180.0F * (float)System.Math.PI) * var4);
                var3.velocityZ = (double)(MathHelper.cos(yaw / 180.0F * (float)System.Math.PI) * MathHelper.cos(pitch / 180.0F * (float)System.Math.PI) * var4);
                var3.velocityY = (double)(-MathHelper.sin(pitch / 180.0F * (float)System.Math.PI) * var4 + 0.1F);
                var4 = 0.02F;
                var5 = random.NextFloat() * (float)System.Math.PI * 2.0F;
                var4 *= random.NextFloat();
                var3.velocityX += java.lang.Math.cos((double)var5) * (double)var4;
                var3.velocityY += (double)((random.NextFloat() - random.NextFloat()) * 0.1F);
                var3.velocityZ += java.lang.Math.sin((double)var5) * (double)var4;
            }

            spawnItem(var3);
            increaseStat(Stats.Stats.dropStat, 1);
        }
    }

    protected virtual void spawnItem(EntityItem itemEntity)
    {
        world.SpawnEntity(itemEntity);
    }

    public float getBlockBreakingSpeed(Block block)
    {
        float var2 = inventory.getStrVsBlock(block);
        if (isInFluid(Material.Water))
        {
            var2 /= 5.0F;
        }

        if (!onGround)
        {
            var2 /= 5.0F;
        }

        return var2;
    }

    public bool canHarvest(Block block)
    {
        return inventory.canHarvestBlock(block);
    }

    public override void readNbt(NBTTagCompound nbt)
    {
        base.readNbt(nbt);
        NBTTagList var2 = nbt.GetTagList("Inventory");
        inventory.readFromNBT(var2);
        dimensionId = nbt.GetInteger("Dimension");
        sleeping = nbt.GetBoolean("Sleeping");
        sleepTimer = nbt.GetShort("SleepTimer");
        if (sleeping)
        {
            sleepingPos = new Vec3i(MathHelper.floor_double(x), MathHelper.floor_double(y), MathHelper.floor_double(z));
            wakeUp(true, true, false);
        }

        if (nbt.HasKey("SpawnX") && nbt.HasKey("SpawnY") && nbt.HasKey("SpawnZ"))
        {
            playerSpawnCoordinate = new Vec3i(nbt.GetInteger("SpawnX"), nbt.GetInteger("SpawnY"), nbt.GetInteger("SpawnZ"));
        }

    }

    public override void writeNbt(NBTTagCompound nbt)
    {
        base.writeNbt(nbt);
        nbt.SetTag("Inventory", inventory.writeToNBT(new NBTTagList()));
        nbt.SetInteger("Dimension", dimensionId);
        nbt.SetBoolean("Sleeping", sleeping);
        nbt.SetShort("SleepTimer", (short)sleepTimer);
        if (playerSpawnCoordinate != null)
        {
            nbt.SetInteger("SpawnX", playerSpawnCoordinate.x);
            nbt.SetInteger("SpawnY", playerSpawnCoordinate.y);
            nbt.SetInteger("SpawnZ", playerSpawnCoordinate.z);
        }

    }

    public virtual void openChestScreen(IInventory inventory)
    {
    }

    public virtual void openCraftingScreen(int x, int y, int z)
    {
    }

    public virtual void sendPickup(Entity item, int count)
    {
    }

    public override float getEyeHeight()
    {
        return 0.12F;
    }

    protected virtual void resetEyeHeight()
    {
        standingEyeHeight = 1.62F;
    }

    public override bool damage(Entity damageSource, int amount)
    {
        entityAge = 0;
        if (health <= 0)
        {
            return false;
        }
        else
        {
            if (isSleeping() && !world.isRemote)
            {
                wakeUp(true, true, false);
            }

            if (damageSource is EntityMonster || damageSource is EntityArrow)
            {
                switch(world.difficulty)
                {
                    case 0:
                        amount = 0;
                        break;
                    case 1:
                        amount = amount / 3 + 1;
                        break;
                    case 3:
                        amount = amount * 3 / 2;
                        break;
                }
            }

            if (amount == 0)
            {
                return false;
            }
            else
            {
                java.lang.Object var3 = damageSource;
                if (damageSource is EntityArrow && ((EntityArrow)damageSource).owner != null)
                {
                    var3 = ((EntityArrow)damageSource).owner;
                }

                if (var3 is EntityLiving)
                {
                    commandWolvesToAttack((EntityLiving)var3, false);
                }

                increaseStat(Stats.Stats.damageTakenStat, amount);
                return base.damage(damageSource, amount);
            }
        }
    }

    protected void commandWolvesToAttack(EntityLiving entity, bool sitting)
    {
        if (entity is not EntityCreeper && entity is not EntityGhast)
        {
            if (entity is EntityWolf)
            {
                EntityWolf var3 = (EntityWolf)entity;
                if (var3.isWolfTamed() && name.Equals(var3.getWolfOwner()))
                {
                    return;
                }
            }

            if (entity is not EntityPlayer || isPvpEnabled())
            {
                var var7 = world.collectEntitiesByClass(EntityWolf.Class, new Box(x, y, z, x + 1.0D, y + 1.0D, z + 1.0D).expand(16.0D, 4.0D, 16.0D));

                foreach (Entity var5 in var7)
                {
                    EntityWolf var6 = (EntityWolf)var5;

                    if (!var6.isWolfTamed()) continue;
                    if (var6.getTarget() != null) continue;
                    if (!name.Equals(var6.getWolfOwner())) continue;
                    if (sitting && var6.isWolfSitting()) continue;

                    var6.setWolfSitting(false);
                    var6.setTarget(entity);
                }
            }
        }
    }

    protected override void applyDamage(int amount)
    {
        int var2 = 25 - inventory.getTotalArmorValue();
        int var3 = amount * var2 + damageSpill;
        inventory.damageArmor(amount);
        amount = var3 / 25;
        damageSpill = var3 % 25;
        base.applyDamage(amount);
    }

    public virtual void openFurnaceScreen(BlockEntityFurnace furnace)
    {
    }

    public virtual void openDispenserScreen(BlockEntityDispenser dispenser)
    {
    }

    public virtual void openEditSignScreen(BlockEntitySign sign)
    {
    }

    public void interact(Entity entity)
    {
        if (!entity.interact(this))
        {
            ItemStack var2 = getHand();
            if (var2 != null && entity is EntityLiving)
            {
                var2.useOnEntity((EntityLiving)entity);
                if (var2.count <= 0)
                {
                    var2.onRemoved(this);
                    clearStackInHand();
                }
            }

        }
    }

    public ItemStack getHand()
    {
        return inventory.getSelectedItem();
    }

    public void clearStackInHand()
    {
        inventory.setStack(inventory.selectedSlot, (ItemStack)null);
    }

    public override double getStandingEyeHeight()
    {
        return (double)(standingEyeHeight - 0.5F);
    }

    public virtual void swingHand()
    {
        handSwingTicks = -1;
        handSwinging = true;
    }

    public void attack(Entity target)
    {
        int var2 = inventory.getDamageVsEntity(target);
        if (var2 > 0)
        {
            if (velocityY < 0.0D)
            {
                ++var2;
            }

            target.damage(this, var2);
            ItemStack var3 = getHand();
            if (var3 != null && target is EntityLiving)
            {
                var3.postHit((EntityLiving)target, this);
                if (var3.count <= 0)
                {
                    var3.onRemoved(this);
                    clearStackInHand();
                }
            }

            if (target is EntityLiving)
            {
                if (target.isAlive())
                {
                    commandWolvesToAttack((EntityLiving)target, true);
                }

                increaseStat(Stats.Stats.damageDealtStat, var2);
            }
        }

    }

    public virtual void respawn()
    {
    }

    public abstract void spawn();

    public virtual void onCursorStackChanged(ItemStack stack)
    {
    }

    public override void markDead()
    {
        base.markDead();
        playerScreenHandler.onClosed(this);
        if (currentScreenHandler != null)
        {
            currentScreenHandler.onClosed(this);
        }

    }

    public override bool isInsideWall()
    {
        return !sleeping && base.isInsideWall();
    }

    public virtual SleepAttemptResult trySleep(int x, int y, int z)
    {
        if (!world.isRemote)
        {
            if (isSleeping() || !isAlive())
            {
                return SleepAttemptResult.OTHER_PROBLEM;
            }

            if (world.dimension.isNether)
            {
                return SleepAttemptResult.NOT_POSSIBLE_HERE;
            }

            if (world.canMonsterSpawn())
            {
                return SleepAttemptResult.NOT_POSSIBLE_NOW;
            }

            if (java.lang.Math.abs(base.x - (double)x) > 3.0D || java.lang.Math.abs(base.y - (double)y) > 2.0D || java.lang.Math.abs(base.z - (double)z) > 3.0D)
            {
                return SleepAttemptResult.TOO_FAR_AWAY;
            }
        }

        setBoundingBoxSpacing(0.2F, 0.2F);
        standingEyeHeight = 0.2F;
        if (world.isPosLoaded(x, y, z))
        {
            int var4 = world.getBlockMeta(x, y, z);
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

            calculateSleepOffset(var5);
            setPosition((double)((float)x + var6), (double)((float)y + 15.0F / 16.0F), (double)((float)z + var7));
        }
        else
        {
            setPosition((double)((float)x + 0.5F), (double)((float)y + 15.0F / 16.0F), (double)((float)z + 0.5F));
        }

        sleeping = true;
        sleepTimer = 0;
        sleepingPos = new Vec3i(x, y, z);
        velocityX = velocityZ = velocityY = 0.0D;
        if (!world.isRemote)
        {
            world.updateSleepingPlayers();
        }

        return SleepAttemptResult.OK;
    }

    private void calculateSleepOffset(int bedDir)
    {
        sleepOffsetX = 0.0F;
        sleepOffsetZ = 0.0F;
        switch (bedDir)
        {
            case 0:
                sleepOffsetZ = -1.8F;
                break;
            case 1:
                sleepOffsetX = 1.8F;
                break;
            case 2:
                sleepOffsetZ = 1.8F;
                break;
            case 3:
                sleepOffsetX = -1.8F;
                break;
        }

    }

    public virtual void wakeUp(bool resetSleepTimer, bool updateSleepingPlayers, bool setSpawnPos)
    {
        setBoundingBoxSpacing(0.6F, 1.8F);
        resetEyeHeight();
        Vec3i var4 = sleepingPos;
        Vec3i var5 = sleepingPos;
        if (var4 != null && world.getBlockId(var4.x, var4.y, var4.z) == Block.Bed.id)
        {
            BlockBed.updateState(world, var4.x, var4.y, var4.z, false);
            var5 = BlockBed.findWakeUpPosition(world, var4.x, var4.y, var4.z, 0);
            if (var5 == null)
            {
                var5 = new Vec3i(var4.x, var4.y + 1, var4.z);
            }

            setPosition((double)((float)var5.x + 0.5F), (double)((float)var5.y + standingEyeHeight + 0.1F), (double)((float)var5.z + 0.5F));
        }

        sleeping = false;
        if (!world.isRemote && updateSleepingPlayers)
        {
            world.updateSleepingPlayers();
        }

        if (resetSleepTimer)
        {
            sleepTimer = 0;
        }
        else
        {
            sleepTimer = 100;
        }

        if (setSpawnPos)
        {
            this.setSpawnPos(sleepingPos);
        }

    }

    private bool isSleepingInBed()
    {
        return world.getBlockId(sleepingPos.x, sleepingPos.y, sleepingPos.z) == Block.Bed.id;
    }

    public static Vec3i findRespawnPosition(World world, Vec3i spawnPos)
    {
        ChunkSource var2 = world.getChunkSource();
        var2.loadChunk(spawnPos.x - 3 >> 4, spawnPos.z - 3 >> 4);
        var2.loadChunk(spawnPos.x + 3 >> 4, spawnPos.z - 3 >> 4);
        var2.loadChunk(spawnPos.x - 3 >> 4, spawnPos.z + 3 >> 4);
        var2.loadChunk(spawnPos.x + 3 >> 4, spawnPos.z + 3 >> 4);
        if (world.getBlockId(spawnPos.x, spawnPos.y, spawnPos.z) != Block.Bed.id)
        {
            return null;
        }
        else
        {
            Vec3i var3 = BlockBed.findWakeUpPosition(world, spawnPos.x, spawnPos.y, spawnPos.z, 0);
            return var3;
        }
    }

    public float getSleepingRotation()
    {
        if (sleepingPos != null)
        {
            int var1 = world.getBlockMeta(sleepingPos.x, sleepingPos.y, sleepingPos.z);
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

    public override bool isSleeping()
    {
        return sleeping;
    }

    public bool isPlayerFullyAsleep()
    {
        return sleeping && sleepTimer >= 100;
    }

    public int getSleepTimer()
    {
        return sleepTimer;
    }

    public virtual void sendMessage(string msg)
    {
    }

    public Vec3i getSpawnPos()
    {
        return playerSpawnCoordinate;
    }

    public void setSpawnPos(Vec3i spawnPos)
    {
        if (spawnPos != null)
        {
            playerSpawnCoordinate = new Vec3i(spawnPos);
        }
        else
        {
            playerSpawnCoordinate = null;
        }

    }

    public void incrementStat(StatBase stat)
    {
        increaseStat(stat, 1);
    }

    public virtual void increaseStat(StatBase stat, int amount)
    {
    }

    protected override void jump()
    {
        base.jump();
        increaseStat(Stats.Stats.jumpStat, 1);
    }

    public override void travel(float x, float z)
    {
        double var3 = base.x;
        double var5 = y;
        double var7 = base.z;
        base.travel(x, z);
        updateMovementStat(base.x - var3, y - var5, base.z - var7);
    }

    private void updateMovementStat(double x, double y, double z)
    {
        if (vehicle == null)
        {
            int var7;
            if (isInFluid(Material.Water))
            {
                var7 = java.lang.Math.round(MathHelper.sqrt_double(x * x + y * y + z * z) * 100.0F);
                if (var7 > 0)
                {
                    increaseStat(Stats.Stats.distanceDoveStat, var7);
                }
            }
            else if (isInWater())
            {
                var7 = java.lang.Math.round(MathHelper.sqrt_double(x * x + z * z) * 100.0F);
                if (var7 > 0)
                {
                    increaseStat(Stats.Stats.distanceSwumStat, var7);
                }
            }
            else if (isOnLadder())
            {
                if (y > 0.0D)
                {
                    increaseStat(Stats.Stats.distanceClimbedStat, (int)java.lang.Math.round(y * 100.0D));
                }
            }
            else if (onGround)
            {
                var7 = java.lang.Math.round(MathHelper.sqrt_double(x * x + z * z) * 100.0F);
                if (var7 > 0)
                {
                    increaseStat(Stats.Stats.distanceWalkedStat, var7);
                }
            }
            else
            {
                var7 = java.lang.Math.round(MathHelper.sqrt_double(x * x + z * z) * 100.0F);
                if (var7 > 25)
                {
                    increaseStat(Stats.Stats.distanceFlownStat, var7);
                }
            }

        }
    }

    private void increaseRidingMotionStats(double x, double y, double z)
    {
        if (vehicle != null)
        {
            int var7 = java.lang.Math.round(MathHelper.sqrt_double(x * x + y * y + z * z) * 100.0F);
            if (var7 > 0)
            {
                if (vehicle is EntityMinecart)
                {
                    increaseStat(Stats.Stats.distanceByMinecartStat, var7);
                    if (startMinecartRidingCoordinate == null)
                    {
                        startMinecartRidingCoordinate = new Vec3i(MathHelper.floor_double(base.x), MathHelper.floor_double(base.y), MathHelper.floor_double(base.z));
                    }
                    else if (startMinecartRidingCoordinate.getSqDistanceTo(MathHelper.floor_double(base.x), MathHelper.floor_double(base.y), MathHelper.floor_double(base.z)) >= 1000.0D)
                    {
                        increaseStat(Achievements.CraftRail, 1);
                    }
                }
                else if (vehicle is EntityBoat)
                {
                    increaseStat(Stats.Stats.distanceByBoatStat, var7);
                }
                else if (vehicle is EntityPig)
                {
                    increaseStat(Stats.Stats.distanceByPigStat, var7);
                }
            }
        }

    }

    protected override void onLanding(float fallDistance)
    {
        if (fallDistance >= 2.0F)
        {
            increaseStat(Stats.Stats.distanceFallenStat, (int)java.lang.Math.round((double)fallDistance * 100.0D));
        }

        base.onLanding(fallDistance);
    }

    public override void onKillOther(EntityLiving other)
    {
        if (other is EntityMonster)
        {
            incrementStat(Achievements.KillEnemy);
        }

    }

    public override int getItemStackTextureId(ItemStack stack)
    {
        int var2 = base.getItemStackTextureId(stack);
        if (stack.itemId == Item.FishingRod.id && fishHook != null)
        {
            var2 = stack.getTextureId() + 16;
        }

        return var2;
    }

    public override void tickPortalCooldown()
    {
        if (portalCooldown > 0)
        {
            portalCooldown = 10;
        }
        else
        {
            inTeleportationState = true;
        }
    }
}