using BetaSharp.Worlds.Core;
using BetaSharp.Worlds.Core.Systems;

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
        return _level.canSpawnEntity(boundingBox);
    }

    public override int getTalkInterval()
    {
        return 120;
    }
}
