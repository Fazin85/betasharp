using BetaSharp.Items;
using BetaSharp.NBT;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds.Core;
using BetaSharp.Worlds.Core.Systems;

namespace BetaSharp.Entities;

public class EntitySkeleton : EntityMonster
{
    private static readonly ItemStack defaultHeldItem = new(Item.BOW, 1);

    public EntitySkeleton(IWorldContext world) : base(world) => texture = "/mob/skeleton.png";

    protected override string getLivingSound() => "mob.skeleton";

    protected override string getHurtSound() => "mob.skeletonhurt";

    protected override string getDeathSound() => "mob.skeletonhurt";

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

    protected override void attackEntity(Entity entity, float distance)
    {
        if (distance < 10.0F)
        {
            double dx = entity.x - x;
            double dy = entity.z - z;
            if (attackTime == 0)
            {
                EntityArrow arrow = new(world, this);
                arrow.y += 1.4F;
                double targetHeightOffset = entity.y + entity.getEyeHeight() - 0.2F - arrow.y;
                float distanceFactor = MathHelper.Sqrt(dx * dx + dy * dy) * 0.2F;
                world.Broadcaster.PlaySoundAtEntity(this, "random.bow", 1.0F, 1.0F / (random.NextFloat() * 0.4F + 0.8F));
                world.SpawnEntity(arrow);
                arrow.setArrowHeading(dx, targetHeightOffset + distanceFactor, dy, 0.6F, 12.0F);
                attackTime = 30;
            }

            yaw = (float)(Math.Atan2(dy, dx) * 180.0D / (float)Math.PI) - 90.0F;
            hasAttacked = true;
        }
    }

    public override void writeNbt(NBTTagCompound nbt) => base.writeNbt(nbt);

    public override void readNbt(NBTTagCompound nbt) => base.readNbt(nbt);

    protected override int getDropItemId() => Item.ARROW.id;

    protected override void dropFewItems()
    {
        int amount = random.NextInt(3);

        int i;
        for (i = 0; i < amount; ++i)
        {
            dropItem(Item.ARROW.id, 1);
        }

        amount = random.NextInt(3);

        for (i = 0; i < amount; ++i)
        {
            dropItem(Item.Bone.id, 1);
        }
    }

    public override ItemStack getHeldItem() => defaultHeldItem;
}
