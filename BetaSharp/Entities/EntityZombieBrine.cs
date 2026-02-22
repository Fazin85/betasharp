using BetaSharp.Blocks;
using BetaSharp.Items;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds;

namespace BetaSharp.Entities;

public class EntityZombieBrine : EntityZombie
{
    public EntityZombieBrine(World world) : base(world)
    {
        texture = "/mob/zombiebrine.png";
        movementSpeed = 1.3F;
        attackStrength = 10;
        attackTime = 5;
        health = 100;
        isImmuneToFire = true;
    }

    protected override void dropFewItems() {
        dropSomeItems(Item.Totem.id, 1);
        dropSomeItems(Block.Sponge.id, 4);
    }
}