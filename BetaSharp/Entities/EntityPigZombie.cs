using BetaSharp.Items;
using BetaSharp.NBT;
using BetaSharp.Worlds.Core;
using BetaSharp.Worlds.Core.Systems;

namespace BetaSharp.Entities;

internal class EntityPigZombie : EntityZombie
{
    private static readonly ItemStack defaultHeldItem = new(Item.GoldenSword, 1);
    private int angerLevel;
    private int randomSoundDelay;

    public EntityPigZombie(IWorldContext world) : base(world)
    {
        texture = "/mob/pigzombie.png";
        movementSpeed = 0.5F;
        attackStrength = 5;
        isImmuneToFire = true;
    }

    public override void tick()
    {
        movementSpeed = playerToAttack != null ? 0.95F : 0.5F;
        if (randomSoundDelay > 0 && --randomSoundDelay == 0)
        {
            world.Broadcaster.PlaySoundAtEntity(this, "mob.zombiepig.zpigangry", getSoundVolume() * 2.0F, ((random.NextFloat() - random.NextFloat()) * 0.2F + 1.0F) * 1.8F);
        }

        base.tick();
    }

    public override bool canSpawn() => world.Difficulty > 0 && world.Entities.GetEntityCollisionsScratch(this, boundingBox).Count == 0 && !world.Reader.IsBoxSubmergedInFluid(boundingBox);

    public override void writeNbt(NBTTagCompound nbt)
    {
        base.writeNbt(nbt);
        nbt.SetShort("Anger", (short)angerLevel);
    }

    public override void readNbt(NBTTagCompound nbt)
    {
        base.readNbt(nbt);
        angerLevel = nbt.GetShort("Anger");
    }

    protected override Entity? findPlayerToAttack() => angerLevel == 0 ? null : base.findPlayerToAttack();

    public override void tickMovement() => base.tickMovement();

    public override bool damage(Entity entity, int amount)
    {
        if (entity is EntityPlayer)
        {
            List<Entity> entities = world.Entities.GetEntities(this, boundingBox.Expand(32.0D, 32.0D, 32.0D));

            for (int i = 0; i < entities.Count; ++i)
            {
                Entity inBoundEntity = entities[i];
                if (inBoundEntity is EntityPigZombie)
                {
                    EntityPigZombie pigZombie = (EntityPigZombie)inBoundEntity;
                    pigZombie.becomeAngryAt(entity);
                }
            }

            becomeAngryAt(entity);
        }

        return base.damage(entity, amount);
    }

    private void becomeAngryAt(Entity entity)
    {
        playerToAttack = entity;
        angerLevel = 400 + random.NextInt(400);
        randomSoundDelay = random.NextInt(40);
    }

    protected override string getLivingSound() => "mob.zombiepig.zpig";

    protected override string getHurtSound() => "mob.zombiepig.zpighurt";

    protected override string getDeathSound() => "mob.zombiepig.zpigdeath";

    protected override int getDropItemId() => Item.CookedPorkchop.id;

    public override ItemStack getHeldItem() => defaultHeldItem;
}
