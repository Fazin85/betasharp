using BetaSharp.Items;
using BetaSharp.NBT;
using BetaSharp.Util;
using BetaSharp.Worlds.Core;

namespace BetaSharp.Entities;

public class EntityPig : EntityAnimal
{
    public readonly SyncedProperty<bool> Saddled;

    public EntityPig(IBlockWorldContext world) : base(world)
    {
        texture = "/mob/pig.png";
        setBoundingBoxSpacing(0.9F, 0.9F);
        Saddled = DataSynchronizer.MakeProperty(16, false);
    }

    public override void writeNbt(NBTTagCompound nbt)
    {
        base.writeNbt(nbt);
        nbt.SetBoolean("Saddle", Saddled.Value);
    }

    public override void readNbt(NBTTagCompound nbt)
    {
        base.readNbt(nbt);
        Saddled.Value = nbt.GetBoolean("Saddle");
    }

    protected override string getLivingSound() => "mob.pig";

    protected override string getHurtSound() => "mob.pig";

    protected override string getDeathSound() => "mob.pigdeath";

    public override bool interact(EntityPlayer player)
    {
        if (!Saddled.Value || _level.IsRemote || (passenger != null && passenger != player))
        {
            return false;
        }

        player.setVehicle(this);
        return true;
    }

    protected override int getDropItemId() => fireTicks > 0 ? Item.CookedPorkchop.id : Item.RawPorkchop.id;
    public override void onStruckByLightning(EntityLightningBolt bolt)
    {
        if (!_level.IsRemote)
        {
            EntityPigZombie pigZombie = new(_level);
            pigZombie.setPositionAndAnglesKeepPrevAngles(x, y, z, yaw, pitch);
            _level.SpawnEntity(pigZombie);
            markDead();
        }
    }

    protected override void onLanding(float fallDistance)
    {
        base.onLanding(fallDistance);
        if (fallDistance > 5.0F && passenger is EntityPlayer)
        {
            ((EntityPlayer)passenger).incrementStat(Achievements.KillPig);
        }
    }
}
