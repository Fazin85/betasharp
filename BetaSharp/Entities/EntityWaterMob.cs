using BetaSharp.Worlds.Core;

namespace BetaSharp.Entities;

public class EntityWaterMob : EntityCreature, SpawnableEntity
{
    public EntityWaterMob(World world) : base(world)
    {
    }

    public override bool canBreatheUnderwater()
    {
        return true;
    }

    public override bool canSpawn()
    {
        return world.Entities.CanSpawnEntity(boundingBox);
    }

    public override int getTalkInterval()
    {
        return 120;
    }
}
