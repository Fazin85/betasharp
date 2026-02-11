using betareborn.NBT;
using betareborn.Util.Maths;
using betareborn.Worlds;

namespace betareborn.Entities
{
    public class EntityMob : EntityCreature, IMob
    {
        public static readonly new java.lang.Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(EntityMob).TypeHandle);

        protected int attackStrength = 2;

        public EntityMob(World var1) : base(var1)
        {
            health = 20;
        }

        public override void tickMovement()
        {
            float var1 = getBrightnessAtEyes(1.0F);
            if (var1 > 0.5F)
            {
                entityAge += 2;
            }

            base.tickMovement();
        }

        public override void tick()
        {
            base.tick();
            if (!world.isRemote && world.difficulty == 0)
            {
                markDead();
            }

        }

        protected override Entity findPlayerToAttack()
        {
            EntityPlayer var1 = world.getClosestPlayer(this, 16.0D);
            return var1 != null && canSee(var1) ? var1 : null;
        }

        public override bool damage(Entity var1, int var2)
        {
            if (base.damage(var1, var2))
            {
                if (passenger != var1 && vehicle != var1)
                {
                    if (var1 != this)
                    {
                        playerToAttack = var1;
                    }

                    return true;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        protected override void attackEntity(Entity var1, float var2)
        {
            if (attackTime <= 0 && var2 < 2.0F && var1.boundingBox.maxY > boundingBox.minY && var1.boundingBox.minY < boundingBox.maxY)
            {
                attackTime = 20;
                var1.damage(this, attackStrength);
            }

        }

        protected override float getBlockPathWeight(int var1, int var2, int var3)
        {
            return 0.5F - world.getLuminance(var1, var2, var3);
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
            int var1 = MathHelper.floor_double(x);
            int var2 = MathHelper.floor_double(boundingBox.minY);
            int var3 = MathHelper.floor_double(z);
            if (world.getBrightness(LightType.Sky, var1, var2, var3) > random.nextInt(32))
            {
                return false;
            }
            else
            {
                int var4 = world.getLightLevel(var1, var2, var3);
                if (world.isThundering())
                {
                    int var5 = world.ambientDarkness;
                    world.ambientDarkness = 10;
                    var4 = world.getLightLevel(var1, var2, var3);
                    world.ambientDarkness = var5;
                }

                return var4 <= random.nextInt(8) && base.canSpawn();
            }
        }
    }

}