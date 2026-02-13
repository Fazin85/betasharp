using betareborn.Worlds;

namespace betareborn.Entities
{
    public class EntityGiantZombie : EntityMonster
    {
        public static readonly new java.lang.Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(EntityGiantZombie).TypeHandle);

        public EntityGiantZombie(World world) : base(world)
        {
            texture = "/mob/zombie.png";
            movementSpeed = 0.5F;
            attackStrength = 50;
            health *= 10;
            standingEyeHeight *= 6.0F;
            setBoundingBoxSpacing(width * 6.0F, height * 6.0F);
        }

        protected override float getBlockPathWeight(int x, int y, int z)
        {
            return world.getLuminance(x, y, z) - 0.5F;
        }
    }

}