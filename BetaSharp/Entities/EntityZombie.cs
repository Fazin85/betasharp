using BetaSharp.Items;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds.Core;
using BetaSharp.Worlds.Core.Systems;

namespace BetaSharp.Entities;

public class EntityZombie : EntityMonster
{
    public EntityZombie(IWorldContext world) : base(world)
    {
        texture = "/mob/zombie.png";
        movementSpeed = 0.5F;
        attackStrength = 5;
    }

    public override void tickMovement()
    {
        if (world.Environment.CanMonsterSpawn())
        {
            float brightness = getBrightnessAtEyes(1.0F);
            if (brightness > 0.5F && world.Lighting.HasSkyLight(MathHelper.Floor(x), MathHelper.Floor(y), MathHelper.Floor(z)) && random.NextFloat() * 30.0F < (brightness - 0.4F) * 2.0F)
            {
                fireTicks = 300;
            }
        }

        base.tickMovement();
    }

    protected override string getLivingSound() => "mob.zombie";

    protected override string getHurtSound() => "mob.zombiehurt";

    protected override string getDeathSound() => "mob.zombiedeath";

    protected override int getDropItemId() => Item.Feather.id;
}
