using betareborn.Blocks;
using betareborn.Items;
using betareborn.Materials;
using betareborn.NBT;
using betareborn.Worlds;

namespace betareborn.Entities
{
    public class EntityItem : Entity
    {

        public static readonly new java.lang.Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(EntityItem).TypeHandle);
        public ItemStack item;
        private int field_803_e;
        public int age = 0;
        public int delayBeforeCanPickup;
        private int health = 5;
        public float field_804_d = (float)(java.lang.Math.random() * java.lang.Math.PI * 2.0D);

        public EntityItem(World var1, double var2, double var4, double var6, ItemStack var8) : base(var1)
        {
            setSize(0.25F, 0.25F);
            yOffset = height / 2.0F;
            setPosition(var2, var4, var6);
            item = var8;
            rotationYaw = (float)(java.lang.Math.random() * 360.0D);
            motionX = (double)((float)(java.lang.Math.random() * (double)0.2F - (double)0.1F));
            motionY = (double)0.2F;
            motionZ = (double)((float)(java.lang.Math.random() * (double)0.2F - (double)0.1F));
        }

        protected override bool canTriggerWalking()
        {
            return false;
        }

        public EntityItem(World var1) : base(var1)
        {
            setSize(0.25F, 0.25F);
            yOffset = height / 2.0F;
        }

        protected override void entityInit()
        {
        }

        public override void onUpdate()
        {
            base.onUpdate();
            if (delayBeforeCanPickup > 0)
            {
                --delayBeforeCanPickup;
            }

            prevPosX = posX;
            prevPosY = posY;
            prevPosZ = posZ;
            motionY -= (double)0.04F;
            if (worldObj.getBlockMaterial(MathHelper.floor_double(posX), MathHelper.floor_double(posY), MathHelper.floor_double(posZ)) == Material.lava)
            {
                motionY = (double)0.2F;
                motionX = (double)((rand.nextFloat() - rand.nextFloat()) * 0.2F);
                motionZ = (double)((rand.nextFloat() - rand.nextFloat()) * 0.2F);
                worldObj.playSoundAtEntity(this, "random.fizz", 0.4F, 2.0F + rand.nextFloat() * 0.4F);
            }

            pushOutOfBlocks(posX, (boundingBox.minY + boundingBox.maxY) / 2.0D, posZ);
            moveEntity(motionX, motionY, motionZ);
            float var1 = 0.98F;
            if (onGround)
            {
                var1 = 0.1F * 0.1F * 58.8F;
                int var2 = worldObj.getBlockId(MathHelper.floor_double(posX), MathHelper.floor_double(boundingBox.minY) - 1, MathHelper.floor_double(posZ));
                if (var2 > 0)
                {
                    var1 = Block.blocksList[var2].slipperiness * 0.98F;
                }
            }

            motionX *= (double)var1;
            motionY *= (double)0.98F;
            motionZ *= (double)var1;
            if (onGround)
            {
                motionY *= -0.5D;
            }

            ++field_803_e;
            ++age;
            if (age >= 6000)
            {
                setEntityDead();
            }

        }

        public override bool handleWaterMovement()
        {
            return worldObj.handleMaterialAcceleration(boundingBox, Material.water, this);
        }

        protected override void dealFireDamage(int var1)
        {
            attackEntityFrom((Entity)null, var1);
        }

        public override bool attackEntityFrom(Entity var1, int var2)
        {
            setBeenAttacked();
            health -= var2;
            if (health <= 0)
            {
                setEntityDead();
            }

            return false;
        }

        public override void writeEntityToNBT(NBTTagCompound var1)
        {
            var1.setShort("Health", (short)((byte)health));
            var1.setShort("Age", (short)age);
            var1.setCompoundTag("Item", item.writeToNBT(new NBTTagCompound()));
        }

        public override void readEntityFromNBT(NBTTagCompound var1)
        {
            health = var1.getShort("Health") & 255;
            age = var1.getShort("Age");
            NBTTagCompound var2 = var1.getCompoundTag("Item");
            item = new ItemStack(var2);
        }

        public override void onCollideWithPlayer(EntityPlayer var1)
        {
            if (!worldObj.multiplayerWorld)
            {
                int var2 = item.stackSize;
                if (delayBeforeCanPickup == 0 && var1.inventory.addItemStackToInventory(item))
                {
                    if (item.itemID == Block.wood.blockID)
                    {
                        var1.triggerAchievement(Achievements.MINE_WOOD);
                    }

                    if (item.itemID == Item.leather.shiftedIndex)
                    {
                        var1.triggerAchievement(Achievements.KILL_COW);
                    }

                    worldObj.playSoundAtEntity(this, "random.pop", 0.2F, ((rand.nextFloat() - rand.nextFloat()) * 0.7F + 1.0F) * 2.0F);
                    var1.onItemPickup(this, var2);
                    if (item.stackSize <= 0)
                    {
                        setEntityDead();
                    }
                }

            }
        }
    }

}