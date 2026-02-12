using betareborn.Util.Maths;
using betareborn.Worlds;
using java.lang;

namespace betareborn.Entities
{
    public class EntityCreature : EntityLiving
    {
        public static readonly new Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(EntityCreature).TypeHandle);
        private PathEntity pathToEntity;
        protected Entity playerToAttack;
        protected bool hasAttacked = false;

        public EntityCreature(World var1) : base(var1)
        {
        }

        protected virtual bool isMovementCeased()
        {
            return false;
        }

        public override void tickLiving()
        {
            hasAttacked = isMovementCeased();
            float var1 = 16.0F;
            if (playerToAttack == null)
            {
                playerToAttack = findPlayerToAttack();
                if (playerToAttack != null)
                {
                    pathToEntity = world.findPath(this, playerToAttack, var1);
                }
            }
            else if (!playerToAttack.isAlive())
            {
                playerToAttack = null;
            }
            else
            {
                float var2 = playerToAttack.getDistance(this);
                if (canSee(playerToAttack))
                {
                    attackEntity(playerToAttack, var2);
                }
                else
                {
                    attackBlockedEntity(playerToAttack, var2);
                }
            }

            if (hasAttacked || playerToAttack == null || pathToEntity != null && random.nextInt(20) != 0)
            {
                if (!hasAttacked && (pathToEntity == null && random.nextInt(80) == 0 || random.nextInt(80) == 0))
                {
                    func_31026_E();
                }
            }
            else
            {
                pathToEntity = world.findPath(this, playerToAttack, var1);
            }

            int var21 = MathHelper.floor_double(boundingBox.minY + 0.5D);
            bool var3 = isInWater();
            bool var4 = isTouchingLava();
            pitch = 0.0F;
            if (pathToEntity != null && random.nextInt(100) != 0)
            {
                Vec3D? var5 = pathToEntity.getPosition(this);
                double var6 = (double)(width * 2.0F);

                while (var5 != null && var5.Value.squareDistanceTo(new Vec3D(x, var5.Value.y, z)) < var6 * var6)
                {
                    pathToEntity.incrementPathIndex();
                    if (pathToEntity.isFinished())
                    {
                        var5 = null;
                        pathToEntity = null;
                    }
                    else
                    {
                        var5 = pathToEntity.getPosition(this);
                    }
                }

                jumping = false;
                if (var5 != null)
                {
                    double var8 = var5.Value.x - x;
                    double var10 = var5.Value.z - z;
                    double var12 = var5.Value.y - (double)var21;
                    float var14 = (float)(java.lang.Math.atan2(var10, var8) * 180.0D / (double)((float)java.lang.Math.PI)) - 90.0F;
                    float var15 = var14 - yaw;

                    for (forwardSpeed = movementSpeed; var15 < -180.0F; var15 += 360.0F)
                    {
                    }

                    while (var15 >= 180.0F)
                    {
                        var15 -= 360.0F;
                    }

                    if (var15 > 30.0F)
                    {
                        var15 = 30.0F;
                    }

                    if (var15 < -30.0F)
                    {
                        var15 = -30.0F;
                    }

                    yaw += var15;
                    if (hasAttacked && playerToAttack != null)
                    {
                        double var16 = playerToAttack.x - x;
                        double var18 = playerToAttack.z - z;
                        float var20 = yaw;
                        yaw = (float)(java.lang.Math.atan2(var18, var16) * 180.0D / (double)((float)java.lang.Math.PI)) - 90.0F;
                        var15 = (var20 - yaw + 90.0F) * (float)java.lang.Math.PI / 180.0F;
                        sidewaysSpeed = -MathHelper.sin(var15) * forwardSpeed * 1.0F;
                        forwardSpeed = MathHelper.cos(var15) * forwardSpeed * 1.0F;
                    }

                    if (var12 > 0.0D)
                    {
                        jumping = true;
                    }
                }

                if (playerToAttack != null)
                {
                    faceEntity(playerToAttack, 30.0F, 30.0F);
                }

                if (horizontalCollison && !hasPath())
                {
                    jumping = true;
                }

                if (random.nextFloat() < 0.8F && (var3 || var4))
                {
                    jumping = true;
                }

            }
            else
            {
                base.tickLiving();
                pathToEntity = null;
            }
        }

        protected void func_31026_E()
        {
            bool var1 = false;
            int var2 = -1;
            int var3 = -1;
            int var4 = -1;
            float var5 = -99999.0F;

            for (int var6 = 0; var6 < 10; ++var6)
            {
                int var7 = MathHelper.floor_double(x + (double)random.nextInt(13) - 6.0D);
                int var8 = MathHelper.floor_double(y + (double)random.nextInt(7) - 3.0D);
                int var9 = MathHelper.floor_double(z + (double)random.nextInt(13) - 6.0D);
                float var10 = getBlockPathWeight(var7, var8, var9);
                if (var10 > var5)
                {
                    var5 = var10;
                    var2 = var7;
                    var3 = var8;
                    var4 = var9;
                    var1 = true;
                }
            }

            if (var1)
            {
                pathToEntity = world.findPath(this, var2, var3, var4, 10.0F);
            }

        }

        protected virtual void attackEntity(Entity var1, float var2)
        {
        }

        protected virtual void attackBlockedEntity(Entity var1, float var2)
        {
        }

        protected virtual float getBlockPathWeight(int var1, int var2, int var3)
        {
            return 0.0F;
        }

        protected virtual Entity findPlayerToAttack()
        {
            return null;
        }

        public override bool canSpawn()
        {
            int var1 = MathHelper.floor_double(x);
            int var2 = MathHelper.floor_double(boundingBox.minY);
            int var3 = MathHelper.floor_double(z);
            return base.canSpawn() && getBlockPathWeight(var1, var2, var3) >= 0.0F;
        }

        public bool hasPath()
        {
            return pathToEntity != null;
        }

        public void setPathToEntity(PathEntity var1)
        {
            pathToEntity = var1;
        }

        public Entity getTarget()
        {
            return playerToAttack;
        }

        public void setTarget(Entity var1)
        {
            playerToAttack = var1;
        }
    }

}