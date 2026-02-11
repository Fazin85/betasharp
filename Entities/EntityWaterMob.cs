using betareborn.NBT;
using betareborn.Worlds;

namespace betareborn.Entities
{
    public class EntityWaterMob : EntityCreature, SpawnableEntity
    {
        public static readonly new java.lang.Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(EntityWaterMob).TypeHandle);

        public EntityWaterMob(World var1) : base(var1)
        {
        }

        public override bool canBreatheUnderwater()
        {
            return true;
        }

        public override void writeNbt(NbtTagCompound var1)
        {
            base.writeNbt(var1);
        }

        public override void readNbt(NbtTagCompound var1)
        {
            base.readNbt(var1);
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

}