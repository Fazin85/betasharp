using BetaSharp.NBT;
using BetaSharp.Worlds;

namespace BetaSharp.Entities;

public class EntityWaterMob : EntityCreature, SpawnableEntity
{
    public static readonly new java.lang.Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(EntityWaterMob).TypeHandle);

    public EntityWaterMob(World world) : base(world)
    {
    }

    public override bool canBreatheUnderwater()
    {
        return true;
    }

    public override void writeNbt(NBTTagCompound nbt)
    {
        base.writeNbt(nbt);
    }

    public override void readNbt(NBTTagCompound nbt)
    {
        base.readNbt(nbt);
    }

    public override bool canSpawn()
    {
        return world.canSpawnEntity(boundingBox);
    }

    public override int getTalkInterval()
    {
        return 120;
    }
}