using BetaSharp.Items;
using BetaSharp.NBT;
using BetaSharp.Util;
using BetaSharp.Worlds;

namespace BetaSharp.Entities;

public class EntityCreeper : EntityMonster
{
    public readonly SyncedProperty<bool> Igniting;
    public readonly SyncedProperty<bool> Powered;
    private int timeSinceIgnited;
    private int lastActiveTime;

    public EntityCreeper(World world) : base(world)
    {
        texture = "/mob/creeper.png";
        Igniting = DataSynchronizer.MakeProperty<bool>(16, false);
        Powered = DataSynchronizer.MakeProperty<bool>(17, false);
    }

    public override void writeNbt(NBTTagCompound nbt)
    {
        base.writeNbt(nbt);
        if (Powered.Value)
        {
            nbt.SetBoolean("powered", true);
        }
    }

    public override void readNbt(NBTTagCompound nbt)
    {
        base.readNbt(nbt);
        Powered.Value = nbt.GetBoolean("powered");
    }

    protected override void attackBlockedEntity(Entity entity, float distance)
    {
        if (!world.isRemote)
        {
            if (timeSinceIgnited > 0)
            {
                Igniting.Value = false;
                --timeSinceIgnited;
                if (timeSinceIgnited < 0)
                {
                    timeSinceIgnited = 0;
                }
            }
        }
    }

    public override void tick()
    {
        lastActiveTime = timeSinceIgnited;
        if (world.isRemote)
        {
            if (Igniting.Value && timeSinceIgnited == 0)
            {
                world.playSound(this, "random.fuse", 1.0F, 0.5F);
            }

            timeSinceIgnited += Igniting.Value ? 1 : -1;
            if (timeSinceIgnited < 0)
            {
                timeSinceIgnited = 0;
            }

            if (timeSinceIgnited >= 30)
            {
                timeSinceIgnited = 30;
            }
        }

        base.tick();
        if (!world.isRemote && playerToAttack == null && timeSinceIgnited > 0)
        {
            Igniting.Value = false;
            --timeSinceIgnited;
            if (timeSinceIgnited < 0)
            {
                timeSinceIgnited = 0;
            }
        }

    }

    protected override string getHurtSound()
    {
        return "mob.creeper";
    }

    protected override string getDeathSound()
    {
        return "mob.creeperdeath";
    }

    public override void onKilledBy(Entity entity)
    {
        base.onKilledBy(entity);
        if (entity is EntitySkeleton)
        {
            dropItem(Item.RecordThirteen.id + random.NextInt(2), 1);
        }

    }

    protected override void attackEntity(Entity entity, float distance)
    {
        if (!world.isRemote)
        {
            if (!Igniting.Value && distance < 3.0F || Igniting.Value && distance < 7.0F)
            {
                if (timeSinceIgnited == 0)
                {
                    world.playSound(this, "random.fuse", 1.0F, 0.5F);
                }

                Igniting.Value = true;
                ++timeSinceIgnited;
                if (timeSinceIgnited >= 30)
                {
                    if (Powered.Value)
                    {
                        world.createExplosion(this, x, y, z, 6.0F);
                    }
                    else
                    {
                        world.createExplosion(this, x, y, z, 3.0F);
                    }

                    markDead();
                }

                hasAttacked = true;
            }
            else
            {
                Igniting.Value = false;
                --timeSinceIgnited;
                if (timeSinceIgnited < 0)
                {
                    timeSinceIgnited = 0;
                }
            }

        }
    }

    public float setCreeperFlashTime(float partialTick)
    {
        return ((float)lastActiveTime + (float)(timeSinceIgnited - lastActiveTime) * partialTick) / 28.0F;
    }

    protected override int getDropItemId()
    {
        return Item.Gunpowder.id;
    }

    public override void onStruckByLightning(EntityLightningBolt bolt)
    {
        base.onStruckByLightning(bolt);
        Powered.Value = true;
    }
}
