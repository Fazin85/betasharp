using betareborn.Blocks;
using betareborn.Blocks.Materials;
using betareborn.Items;
using betareborn.NBT;
using betareborn.Util.Maths;
using betareborn.Worlds;

namespace betareborn.Entities
{
    public class EntityItem : Entity
    {

        public static readonly new java.lang.Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(EntityItem).TypeHandle);
        public ItemStack stack;
        private int field_803_e;
        public int age = 0;
        public int delayBeforeCanPickup;
        private int health = 5;
        public float field_804_d = (float)(java.lang.Math.random() * java.lang.Math.PI * 2.0D);

        public EntityItem(World var1, double var2, double var4, double var6, ItemStack var8) : base(var1)
        {
            setBoundingBoxSpacing(0.25F, 0.25F);
            standingEyeHeight = height / 2.0F;
            setPosition(var2, var4, var6);
            stack = var8;
            yaw = (float)(java.lang.Math.random() * 360.0D);
            velocityX = (double)((float)(java.lang.Math.random() * (double)0.2F - (double)0.1F));
            velocityY = (double)0.2F;
            velocityZ = (double)((float)(java.lang.Math.random() * (double)0.2F - (double)0.1F));
        }

        protected override bool bypassesSteppingEffects()
        {
            return false;
        }

        public EntityItem(World var1) : base(var1)
        {
            setBoundingBoxSpacing(0.25F, 0.25F);
            standingEyeHeight = height / 2.0F;
        }

        protected override void initDataTracker()
        {
        }

        public override void tick()
        {
            base.tick();
            if (delayBeforeCanPickup > 0)
            {
                --delayBeforeCanPickup;
            }

            prevX = x;
            prevY = y;
            prevZ = z;
            velocityY -= (double)0.04F;
            if (world.getMaterial(MathHelper.floor_double(x), MathHelper.floor_double(y), MathHelper.floor_double(z)) == Material.LAVA)
            {
                velocityY = (double)0.2F;
                velocityX = (double)((random.nextFloat() - random.nextFloat()) * 0.2F);
                velocityZ = (double)((random.nextFloat() - random.nextFloat()) * 0.2F);
                world.playSound(this, "random.fizz", 0.4F, 2.0F + random.nextFloat() * 0.4F);
            }

            pushOutOfBlocks(x, (boundingBox.minY + boundingBox.maxY) / 2.0D, z);
            move(velocityX, velocityY, velocityZ);
            float var1 = 0.98F;
            if (onGround)
            {
                var1 = 0.1F * 0.1F * 58.8F;
                int var2 = world.getBlockId(MathHelper.floor_double(x), MathHelper.floor_double(boundingBox.minY) - 1, MathHelper.floor_double(z));
                if (var2 > 0)
                {
                    var1 = Block.BLOCKS[var2].slipperiness * 0.98F;
                }
            }

            velocityX *= (double)var1;
            velocityY *= (double)0.98F;
            velocityZ *= (double)var1;
            if (onGround)
            {
                velocityY *= -0.5D;
            }

            ++field_803_e;
            ++age;
            if (age >= 6000)
            {
                markDead();
            }

        }

        public override bool checkWaterCollisions()
        {
            return world.updateMovementInFluid(boundingBox, Material.WATER, this);
        }

        protected override void damage(int var1)
        {
            damage((Entity)null, var1);
        }

        public override bool damage(Entity var1, int var2)
        {
            scheduleVelocityUpdate();
            health -= var2;
            if (health <= 0)
            {
                markDead();
            }

            return false;
        }

        public override void writeNbt(NbtTagCompound var1)
        {
            var1.SetShort("Health", (short)((byte)health));
            var1.SetShort("Age", (short)age);
            var1.SetCompoundTag("Item", stack.writeToNBT(new NbtTagCompound()));
        }

        public override void readNbt(NbtTagCompound var1)
        {
            health = var1.GetShort("Health") & 255;
            age = var1.GetShort("Age");
            NbtTagCompound var2 = var1.GetCompoundTag("Item");
            stack = new ItemStack(var2);
        }

        public override void onPlayerInteraction(EntityPlayer var1)
        {
            if (!world.isRemote)
            {
                int var2 = stack.count;
                if (delayBeforeCanPickup == 0 && var1.inventory.addItemStackToInventory(stack))
                {
                    if (stack.itemId == Block.LOG.id)
                    {
                        var1.incrementStat(Achievements.MINE_WOOD);
                    }

                    if (stack.itemId == Item.LEATHER.id)
                    {
                        var1.incrementStat(Achievements.KILL_COW);
                    }

                    world.playSound(this, "random.pop", 0.2F, ((random.nextFloat() - random.nextFloat()) * 0.7F + 1.0F) * 2.0F);
                    var1.sendPickup(this, var2);
                    if (stack.count <= 0)
                    {
                        markDead();
                    }
                }

            }
        }
    }

}