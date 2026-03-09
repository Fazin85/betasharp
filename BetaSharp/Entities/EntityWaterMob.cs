using BetaSharp.Worlds.Core;

namespace BetaSharp.Entities;

public class EntityWaterMob : EntityCreature, SpawnableEntity
{
    public EntityWaterMob(IBlockWorldContext world) : base(world)
    {
    }

    public override bool canBreatheUnderwater() => true;

    public override bool canSpawn() => _level.Entities.CanSpawnEntity(boundingBox);

    public override int getTalkInterval() => 120;
}
