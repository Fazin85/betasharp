using betareborn.Items;
using betareborn.NBT;
using betareborn.Util.Maths;
using betareborn.Worlds;

namespace betareborn.Entities
{
    public class EntitySkeleton : EntityMob
    {
        public static readonly new java.lang.Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(EntitySkeleton).TypeHandle);

        private static readonly ItemStack defaultHeldItem = new ItemStack(Item.BOW, 1);

        public EntitySkeleton(World var1) : base(var1)
        {
            texture = "/mob/skeleton.png";
        }

        protected override String getLivingSound()
        {
            return "mob.skeleton";
        }

        protected override String getHurtSound()
        {
            return "mob.skeletonhurt";
        }

        protected override String getDeathSound()
        {
            return "mob.skeletonhurt";
        }

        public override void tickMovement()
        {
            if (world.canMonsterSpawn())
            {
                float var1 = getBrightnessAtEyes(1.0F);
                if (var1 > 0.5F && world.hasSkyLight(MathHelper.floor_double(x), MathHelper.floor_double(y), MathHelper.floor_double(z)) && random.nextFloat() * 30.0F < (var1 - 0.4F) * 2.0F)
                {
                    fireTicks = 300;
                }
            }

            base.tickMovement();
        }

        protected override void attackEntity(Entity var1, float var2)
        {
            if (var2 < 10.0F)
            {
                double var3 = var1.x - x;
                double var5 = var1.z - z;
                if (attackTime == 0)
                {
                    EntityArrow var7 = new EntityArrow(world, this);
                    var7.y += (double)1.4F;
                    double var8 = var1.y + (double)var1.getEyeHeight() - (double)0.2F - var7.y;
                    float var10 = MathHelper.sqrt_double(var3 * var3 + var5 * var5) * 0.2F;
                    world.playSound(this, "random.bow", 1.0F, 1.0F / (random.nextFloat() * 0.4F + 0.8F));
                    world.spawnEntity(var7);
                    var7.setArrowHeading(var3, var8 + (double)var10, var5, 0.6F, 12.0F);
                    attackTime = 30;
                }

                yaw = (float)(java.lang.Math.atan2(var5, var3) * 180.0D / (double)((float)Math.PI)) - 90.0F;
                hasAttacked = true;
            }

        }

        public override void writeNbt(NbtTagCompound var1)
        {
            base.writeNbt(var1);
        }

        public override void readNbt(NbtTagCompound var1)
        {
            base.readNbt(var1);
        }

        protected override int getDropItemId()
        {
            return Item.ARROW.id;
        }

        protected override void dropFewItems()
        {
            int var1 = random.nextInt(3);

            int var2;
            for (var2 = 0; var2 < var1; ++var2)
            {
                dropItem(Item.ARROW.id, 1);
            }

            var1 = random.nextInt(3);

            for (var2 = 0; var2 < var1; ++var2)
            {
                dropItem(Item.BONE.id, 1);
            }

        }

        public override ItemStack getHeldItem()
        {
            return defaultHeldItem;
        }
    }

}