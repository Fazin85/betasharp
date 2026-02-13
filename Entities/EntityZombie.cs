using betareborn.Items;
using betareborn.Util.Maths;
using betareborn.Worlds;

namespace betareborn.Entities
{
    public class EntityZombie : EntityMonster
    {
        public static readonly new java.lang.Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(EntityZombie).TypeHandle);

        public EntityZombie(World world) : base(world)
        {
            texture = "/mob/zombie.png";
            movementSpeed = 0.5F;
            attackStrength = 5;
        }

        public override void tickMovement()
        {
            if (world.canMonsterSpawn())
            {
                float brightness = getBrightnessAtEyes(1.0F);
                if (brightness > 0.5F && world.hasSkyLight(MathHelper.floor_double(x), MathHelper.floor_double(y), MathHelper.floor_double(z)) && random.nextFloat() * 30.0F < (brightness - 0.4F) * 2.0F)
                {
                    fireTicks = 300;
                }
            }

            base.tickMovement();
        }

        protected override String getLivingSound()
        {
            return "mob.zombie";
        }

        protected override String getHurtSound()
        {
            return "mob.zombiehurt";
        }

        protected override String getDeathSound()
        {
            return "mob.zombiedeath";
        }

        protected override int getDropItemId()
        {
            return Item.FEATHER.id;
        }
    }

}