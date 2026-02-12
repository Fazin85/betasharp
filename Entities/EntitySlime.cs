using betareborn.Items;
using betareborn.NBT;
using betareborn.Util.Maths;
using betareborn.Worlds;
using betareborn.Worlds.Chunks;

namespace betareborn.Entities
{
    public class EntitySlime : EntityLiving, IMob
    {
        public static readonly new java.lang.Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(EntitySlime).TypeHandle);

        public float field_768_a;
        public float field_767_b;
        private int slimeJumpDelay = 0;

        public EntitySlime(World var1) : base(var1)
        {
            texture = "/mob/slime.png";
            int var2 = 1 << random.nextInt(3);
            standingEyeHeight = 0.0F;
            slimeJumpDelay = random.nextInt(20) + 10;
            setSlimeSize(var2);
        }

        protected override void initDataTracker()
        {
            base.initDataTracker();
            dataWatcher.addObject(16, new java.lang.Byte((byte)1));
        }

        public void setSlimeSize(int var1)
        {
            dataWatcher.updateObject(16, new java.lang.Byte((byte)var1));
            setBoundingBoxSpacing(0.6F * (float)var1, 0.6F * (float)var1);
            health = var1 * var1;
            setPosition(x, y, z);
        }

        public int getSlimeSize()
        {
            return dataWatcher.getWatchableObjectByte(16);
        }

        public override void writeNbt(NBTTagCompound var1)
        {
            base.writeNbt(var1);
            var1.SetInteger("Size", getSlimeSize() - 1);
        }

        public override void readNbt(NBTTagCompound var1)
        {
            base.readNbt(var1);
            setSlimeSize(var1.GetInteger("Size") + 1);
        }

        public override void tick()
        {
            field_767_b = field_768_a;
            bool var1 = onGround;
            base.tick();
            if (onGround && !var1)
            {
                int var2 = getSlimeSize();

                for (int var3 = 0; var3 < var2 * 8; ++var3)
                {
                    float var4 = random.nextFloat() * (float)Math.PI * 2.0F;
                    float var5 = random.nextFloat() * 0.5F + 0.5F;
                    float var6 = MathHelper.sin(var4) * (float)var2 * 0.5F * var5;
                    float var7 = MathHelper.cos(var4) * (float)var2 * 0.5F * var5;
                    world.addParticle("slime", x + (double)var6, boundingBox.minY, z + (double)var7, 0.0D, 0.0D, 0.0D);
                }

                if (var2 > 2)
                {
                    world.playSound(this, "mob.slime", getSoundVolume(), ((random.nextFloat() - random.nextFloat()) * 0.2F + 1.0F) / 0.8F);
                }

                field_768_a = -0.5F;
            }

            field_768_a *= 0.6F;
        }

        public override void tickLiving()
        {
            func_27021_X();
            EntityPlayer var1 = world.getClosestPlayer(this, 16.0D);
            if (var1 != null)
            {
                faceEntity(var1, 10.0F, 20.0F);
            }

            if (onGround && slimeJumpDelay-- <= 0)
            {
                slimeJumpDelay = random.nextInt(20) + 10;
                if (var1 != null)
                {
                    slimeJumpDelay /= 3;
                }

                jumping = true;
                if (getSlimeSize() > 1)
                {
                    world.playSound(this, "mob.slime", getSoundVolume(), ((random.nextFloat() - random.nextFloat()) * 0.2F + 1.0F) * 0.8F);
                }

                field_768_a = 1.0F;
                sidewaysSpeed = 1.0F - random.nextFloat() * 2.0F;
                forwardSpeed = (float)(1 * getSlimeSize());
            }
            else
            {
                jumping = false;
                if (onGround)
                {
                    sidewaysSpeed = forwardSpeed = 0.0F;
                }
            }

        }

        public override void markDead()
        {
            int var1 = getSlimeSize();
            if (!world.isRemote && var1 > 1 && health == 0)
            {
                for (int var2 = 0; var2 < 4; ++var2)
                {
                    float var3 = ((float)(var2 % 2) - 0.5F) * (float)var1 / 4.0F;
                    float var4 = ((float)(var2 / 2) - 0.5F) * (float)var1 / 4.0F;
                    EntitySlime var5 = new EntitySlime(world);
                    var5.setSlimeSize(var1 / 2);
                    var5.setPositionAndAnglesKeepPrevAngles(x + (double)var3, y + 0.5D, z + (double)var4, random.nextFloat() * 360.0F, 0.0F);
                    world.spawnEntity(var5);
                }
            }

            base.markDead();
        }

        public override void onPlayerInteraction(EntityPlayer var1)
        {
            int var2 = getSlimeSize();
            if (var2 > 1 && canSee(var1) && (double)getDistance(var1) < 0.6D * (double)var2 && var1.damage(this, var2))
            {
                world.playSound(this, "mob.slimeattack", 1.0F, (random.nextFloat() - random.nextFloat()) * 0.2F + 1.0F);
            }

        }

        protected override String getHurtSound()
        {
            return "mob.slime";
        }

        protected override String getDeathSound()
        {
            return "mob.slime";
        }

        protected override int getDropItemId()
        {
            return getSlimeSize() == 1 ? Item.SLIMEBALL.id : 0;
        }

        public override bool canSpawn()
        {
            Chunk var1 = world.getChunkFromPos(MathHelper.floor_double(x), MathHelper.floor_double(z));
            return (getSlimeSize() == 1 || world.difficulty > 0) && random.nextInt(10) == 0 && var1.getSlimeRandom(987234911L).nextInt(10) == 0 && y < 16.0D;
        }

        protected override float getSoundVolume()
        {
            return 0.6F;
        }
    }

}