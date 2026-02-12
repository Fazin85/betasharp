using betareborn.Blocks;
using betareborn.NBT;
using betareborn.Util.Maths;
using betareborn.Worlds;
using java.lang;

namespace betareborn.Entities
{
    public abstract class EntityAnimal : EntityCreature, SpawnableEntity
    {
        public static readonly new Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(EntityAnimal).TypeHandle);

        public EntityAnimal(World var1) : base(var1)
        {
        }

        protected override float getBlockPathWeight(int var1, int var2, int var3)
        {
            return world.getBlockId(var1, var2 - 1, var3) == Block.GRASS_BLOCK.id ? 10.0F : world.getLuminance(var1, var2, var3) - 0.5F;
        }

        public override void writeNbt(NBTTagCompound var1)
        {
            base.writeNbt(var1);
        }

        public override void readNbt(NBTTagCompound var1)
        {
            base.readNbt(var1);
        }

        public override bool canSpawn()
        {
            int var1 = MathHelper.floor_double(x);
            int var2 = MathHelper.floor_double(boundingBox.minY);
            int var3 = MathHelper.floor_double(z);
            return world.getBlockId(var1, var2 - 1, var3) == Block.GRASS_BLOCK.id && world.getBrightness(var1, var2, var3) > 8 && base.canSpawn();
        }

        public override int getTalkInterval()
        {
            return 120;
        }
    }

}