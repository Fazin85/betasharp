using betareborn.Blocks;
using betareborn.Items;
using betareborn.NBT;
using betareborn.Worlds;
using java.lang;

namespace betareborn.Entities
{
    public class EntitySheep : EntityAnimal
    {
        public static readonly new Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(EntitySheep).TypeHandle);
        public static readonly float[][] fleeceColorTable = [[1.0F, 1.0F, 1.0F], [0.95F, 0.7F, 0.2F], [0.9F, 0.5F, 0.85F], [0.6F, 0.7F, 0.95F], [0.9F, 0.9F, 0.2F], [0.5F, 0.8F, 0.1F], [0.95F, 0.7F, 0.8F], [0.3F, 0.3F, 0.3F], [0.6F, 0.6F, 0.6F], [0.3F, 0.6F, 0.7F], [0.7F, 0.4F, 0.9F], [0.2F, 0.4F, 0.8F], [0.5F, 0.4F, 0.3F], [0.4F, 0.5F, 0.2F], [0.8F, 0.3F, 0.3F], [0.1F, 0.1F, 0.1F]];

        public EntitySheep(World var1) : base(var1)
        {
            texture = "/mob/sheep.png";
            setBoundingBoxSpacing(0.9F, 1.3F);
        }

        protected override void initDataTracker()
        {
            base.initDataTracker();
            dataWatcher.addObject(16, new java.lang.Byte((byte)0));
        }

        public override bool damage(Entity var1, int var2)
        {
            return base.damage(var1, var2);
        }

        protected override void dropFewItems()
        {
            if (!getSheared())
            {
                dropItem(new ItemStack(Block.WOOL.id, 1, getFleeceColor()), 0.0F);
            }

        }

        protected override int getDropItemId()
        {
            return Block.WOOL.id;
        }

        public override bool interact(EntityPlayer var1)
        {
            ItemStack var2 = var1.inventory.getSelectedItem();
            if (var2 != null && var2.itemId == Item.SHEARS.id && !getSheared())
            {
                if (!world.isRemote)
                {
                    setSheared(true);
                    int var3 = 2 + random.nextInt(3);

                    for (int var4 = 0; var4 < var3; ++var4)
                    {
                        EntityItem var5 = dropItem(new ItemStack(Block.WOOL.id, 1, getFleeceColor()), 1.0F);
                        var5.velocityY += (double)(random.nextFloat() * 0.05F);
                        var5.velocityX += (double)((random.nextFloat() - random.nextFloat()) * 0.1F);
                        var5.velocityZ += (double)((random.nextFloat() - random.nextFloat()) * 0.1F);
                    }
                }

                var2.damageItem(1, var1);
            }

            return false;
        }

        public override void writeNbt(NbtTagCompound var1)
        {
            base.writeNbt(var1);
            var1.SetBoolean("Sheared", getSheared());
            var1.SetByte("Color", (sbyte)getFleeceColor());
        }

        public override void readNbt(NbtTagCompound var1)
        {
            base.readNbt(var1);
            setSheared(var1.GetBoolean("Sheared"));
            setFleeceColor(var1.GetByte("Color"));
        }

        protected override string getLivingSound()
        {
            return "mob.sheep";
        }

        protected override string getHurtSound()
        {
            return "mob.sheep";
        }

        protected override string getDeathSound()
        {
            return "mob.sheep";
        }

        public int getFleeceColor()
        {
            return dataWatcher.getWatchableObjectByte(16) & 15;
        }

        public void setFleeceColor(int var1)
        {
            sbyte var2 = dataWatcher.getWatchableObjectByte(16);
            dataWatcher.updateObject(16, java.lang.Byte.valueOf((byte)(var2 & 240 | var1 & 15)));
        }

        public bool getSheared()
        {
            return (dataWatcher.getWatchableObjectByte(16) & 16) != 0;
        }

        public void setSheared(bool var1)
        {
            sbyte var2 = dataWatcher.getWatchableObjectByte(16);
            if (var1)
            {
                dataWatcher.updateObject(16, java.lang.Byte.valueOf((byte)(var2 | 16)));
            }
            else
            {
                dataWatcher.updateObject(16, java.lang.Byte.valueOf((byte)(var2 & -17)));
            }

        }

        public static int getRandomFleeceColor(java.util.Random var0)
        {
            int var1 = var0.nextInt(100);
            return var1 < 5 ? 15 : (var1 < 10 ? 7 : (var1 < 15 ? 8 : (var1 < 18 ? 12 : (var0.nextInt(500) == 0 ? 6 : 0))));
        }
    }

}