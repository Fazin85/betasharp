using BetaSharp.Worlds.Core;
using BetaSharp.Worlds.Core.Systems;

namespace BetaSharp.Entities;

public class EntityWaterMob : EntityCreature, SpawnableEntity
{
    public EntityWaterMob(IWorldContext world) : base(world)
    {
    }

    public override bool canBreatheUnderwater() => true;

    public override bool canSpawn() => world.Entities.CanSpawnEntity(boundingBox);

    public override int getTalkInterval() => 120;
}
