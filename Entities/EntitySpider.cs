using betareborn.Items;
using betareborn.NBT;
using betareborn.Util.Maths;
using betareborn.Worlds;

namespace betareborn.Entities
{
    public class EntitySpider : EntityMob
    {
        public static readonly new java.lang.Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(EntitySpider).TypeHandle);

        public EntitySpider(World var1) : base(var1)
        {
            texture = "/mob/spider.png";
            setBoundingBoxSpacing(1.4F, 0.9F);
            movementSpeed = 0.8F;
        }

        public override double getPassengerRidingHeight()
        {
            return (double)height * 0.75D - 0.5D;
        }

        protected override bool bypassesSteppingEffects()
        {
            return false;
        }

        protected override Entity findPlayerToAttack()
        {
            float var1 = getBrightnessAtEyes(1.0F);
            if (var1 < 0.5F)
            {
                double var2 = 16.0D;
                return world.getClosestPlayer(this, var2);
            }
            else
            {
                return null;
            }
        }

        protected override string getLivingSound()
        {
            return "mob.spider";
        }

        protected override string getHurtSound()
        {
            return "mob.spider";
        }

        protected override string getDeathSound()
        {
            return "mob.spiderdeath";
        }

        protected override void attackEntity(Entity var1, float var2)
        {
            float var3 = getBrightnessAtEyes(1.0F);
            if (var3 > 0.5F && random.nextInt(100) == 0)
            {
                playerToAttack = null;
            }
            else
            {
                if (var2 > 2.0F && var2 < 6.0F && random.nextInt(10) == 0)
                {
                    if (onGround)
                    {
                        double var4 = var1.x - x;
                        double var6 = var1.z - z;
                        float var8 = MathHelper.sqrt_double(var4 * var4 + var6 * var6);
                        velocityX = var4 / (double)var8 * 0.5D * (double)0.8F + velocityX * (double)0.2F;
                        velocityZ = var6 / (double)var8 * 0.5D * (double)0.8F + velocityZ * (double)0.2F;
                        velocityY = (double)0.4F;
                    }
                }
                else
                {
                    base.attackEntity(var1, var2);
                }

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
            return Item.STRING.id;
        }

        public override bool isOnLadder()
        {
            return horizontalCollison;
        }
    }

}