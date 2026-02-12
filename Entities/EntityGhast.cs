using betareborn.Items;
using betareborn.Util.Maths;
using betareborn.Worlds;

namespace betareborn.Entities
{
    public class EntityGhast : EntityFlying, IMob
    {
        public static readonly new java.lang.Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(EntityGhast).TypeHandle);

        public int courseChangeCooldown = 0;
        public double waypointX;
        public double waypointY;
        public double waypointZ;
        private Entity targetedEntity = null;
        private int aggroCooldown = 0;
        public int prevAttackCounter = 0;
        public int attackCounter = 0;

        public EntityGhast(World var1) : base(var1)
        {
            texture = "/mob/ghast.png";
            setBoundingBoxSpacing(4.0F, 4.0F);
            isImmuneToFire = true;
        }

        protected override void initDataTracker()
        {
            base.initDataTracker();
            dataWatcher.addObject(16, java.lang.Byte.valueOf((byte)0));
        }

        public override void tick()
        {
            base.tick();
            sbyte var1 = dataWatcher.getWatchableObjectByte(16);
            texture = var1 == 1 ? "/mob/ghast_fire.png" : "/mob/ghast.png";
        }

        public override void tickLiving()
        {
            if (!world.isRemote && world.difficulty == 0)
            {
                markDead();
            }

            func_27021_X();
            prevAttackCounter = attackCounter;
            double var1 = waypointX - x;
            double var3 = waypointY - y;
            double var5 = waypointZ - z;
            double var7 = (double)MathHelper.sqrt_double(var1 * var1 + var3 * var3 + var5 * var5);
            if (var7 < 1.0D || var7 > 60.0D)
            {
                waypointX = x + (double)((random.nextFloat() * 2.0F - 1.0F) * 16.0F);
                waypointY = y + (double)((random.nextFloat() * 2.0F - 1.0F) * 16.0F);
                waypointZ = z + (double)((random.nextFloat() * 2.0F - 1.0F) * 16.0F);
            }

            if (courseChangeCooldown-- <= 0)
            {
                courseChangeCooldown += random.nextInt(5) + 2;
                if (isCourseTraversable(waypointX, waypointY, waypointZ, var7))
                {
                    velocityX += var1 / var7 * 0.1D;
                    velocityY += var3 / var7 * 0.1D;
                    velocityZ += var5 / var7 * 0.1D;
                }
                else
                {
                    waypointX = x;
                    waypointY = y;
                    waypointZ = z;
                }
            }

            if (targetedEntity != null && targetedEntity.dead)
            {
                targetedEntity = null;
            }

            if (targetedEntity == null || aggroCooldown-- <= 0)
            {
                targetedEntity = world.getClosestPlayer(this, 100.0D);
                if (targetedEntity != null)
                {
                    aggroCooldown = 20;
                }
            }

            double var9 = 64.0D;
            if (targetedEntity != null && targetedEntity.getSquaredDistance(this) < var9 * var9)
            {
                double var11 = targetedEntity.x - x;
                double var13 = targetedEntity.boundingBox.minY + (double)(targetedEntity.height / 2.0F) - (y + (double)(height / 2.0F));
                double var15 = targetedEntity.z - z;
                bodyYaw = yaw = -((float)java.lang.Math.atan2(var11, var15)) * 180.0F / (float)java.lang.Math.PI;
                if (canSee(targetedEntity))
                {
                    if (attackCounter == 10)
                    {
                        world.playSound(this, "mob.ghast.charge", getSoundVolume(), (random.nextFloat() - random.nextFloat()) * 0.2F + 1.0F);
                    }

                    ++attackCounter;
                    if (attackCounter == 20)
                    {
                        world.playSound(this, "mob.ghast.fireball", getSoundVolume(), (random.nextFloat() - random.nextFloat()) * 0.2F + 1.0F);
                        EntityFireball var17 = new EntityFireball(world, this, var11, var13, var15);
                        double var18 = 4.0D;
                        Vec3D var20 = getLook(1.0F);
                        var17.x = x + var20.x * var18;
                        var17.y = y + (double)(height / 2.0F) + 0.5D;
                        var17.z = z + var20.z * var18;
                        world.spawnEntity(var17);
                        attackCounter = -40;
                    }
                }
                else if (attackCounter > 0)
                {
                    --attackCounter;
                }
            }
            else
            {
                bodyYaw = yaw = -((float)java.lang.Math.atan2(velocityX, velocityZ)) * 180.0F / (float)java.lang.Math.PI;
                if (attackCounter > 0)
                {
                    --attackCounter;
                }
            }

            if (!world.isRemote)
            {
                sbyte var21 = dataWatcher.getWatchableObjectByte(16);
                byte var12 = (byte)(attackCounter > 10 ? 1 : 0);
                if (var21 != var12)
                {
                    dataWatcher.updateObject(16, java.lang.Byte.valueOf(var12));
                }
            }

        }

        private bool isCourseTraversable(double var1, double var3, double var5, double var7)
        {
            double var9 = (waypointX - x) / var7;
            double var11 = (waypointY - y) / var7;
            double var13 = (waypointZ - z) / var7;
            Box var15 = boundingBox;

            for (int var16 = 1; (double)var16 < var7; ++var16)
            {
                var15.translate(var9, var11, var13);
                if (world.getEntityCollisions(this, var15).Count > 0)
                {
                    return false;
                }
            }

            return true;
        }

        protected override String getLivingSound()
        {
            return "mob.ghast.moan";
        }

        protected override String getHurtSound()
        {
            return "mob.ghast.scream";
        }

        protected override String getDeathSound()
        {
            return "mob.ghast.death";
        }

        protected override int getDropItemId()
        {
            return Item.GUNPOWDER.id;
        }

        protected override float getSoundVolume()
        {
            return 10.0F;
        }

        public override bool canSpawn()
        {
            return random.nextInt(20) == 0 && base.canSpawn() && world.difficulty > 0;
        }

        public override int getMaxSpawnedInChunk()
        {
            return 1;
        }
    }

}