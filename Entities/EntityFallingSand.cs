using betareborn.Blocks;
using betareborn.NBT;
using betareborn.Util.Maths;
using betareborn.Worlds;
using java.lang;

namespace betareborn.Entities
{
    public class EntityFallingSand : Entity
    {
        public static readonly new Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(EntityFallingSand).TypeHandle);

        public int blockId;
        public int fallTime = 0;

        public EntityFallingSand(World var1) : base(var1)
        {
        }

        public EntityFallingSand(World var1, double var2, double var4, double var6, int var8) : base(var1)
        {
            blockId = var8;
            preventEntitySpawning = true;
            setBoundingBoxSpacing(0.98F, 0.98F);
            standingEyeHeight = height / 2.0F;
            setPosition(var2, var4, var6);
            velocityX = 0.0D;
            velocityY = 0.0D;
            velocityZ = 0.0D;
            prevX = var2;
            prevY = var4;
            prevZ = var6;
        }

        protected override bool bypassesSteppingEffects()
        {
            return false;
        }

        protected override void initDataTracker()
        {
        }

        public override bool isCollidable()
        {
            return !dead;
        }

        public override void tick()
        {
            if (blockId == 0)
            {
                markDead();
            }
            else
            {
                prevX = x;
                prevY = y;
                prevZ = z;
                ++fallTime;
                velocityY -= (double)0.04F;
                move(velocityX, velocityY, velocityZ);
                velocityX *= (double)0.98F;
                velocityY *= (double)0.98F;
                velocityZ *= (double)0.98F;
                int var1 = MathHelper.floor_double(x);
                int var2 = MathHelper.floor_double(y);
                int var3 = MathHelper.floor_double(z);
                if (world.getBlockId(var1, var2, var3) == blockId)
                {
                    world.setBlock(var1, var2, var3, 0);
                }

                if (onGround)
                {
                    velocityX *= (double)0.7F;
                    velocityZ *= (double)0.7F;
                    velocityY *= -0.5D;
                    markDead();
                    if ((!world.canPlace(blockId, var1, var2, var3, true, 1) || BlockSand.canFallThrough(world, var1, var2 - 1, var3) || !world.setBlock(var1, var2, var3, blockId)) && !world.isRemote)
                    {
                        dropItem(blockId, 1);
                    }
                }
                else if (fallTime > 100 && !world.isRemote)
                {
                    dropItem(blockId, 1);
                    markDead();
                }

            }
        }

        public override void writeNbt(NBTTagCompound var1)
        {
            var1.SetByte("Tile", (sbyte)blockId);
        }

        public override void readNbt(NBTTagCompound var1)
        {
            blockId = var1.GetByte("Tile") & 255;
        }

        public override float getShadowRadius()
        {
            return 0.0F;
        }

        public World getWorld()
        {
            return world;
        }
    }

}