using betareborn.Items;
using betareborn.NBT;
using betareborn.Worlds;

namespace betareborn.Entities
{
    public class EntityPig : EntityAnimal
    {
        public static readonly new java.lang.Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(EntityPig).TypeHandle);

        public EntityPig(World var1) : base(var1)
        {
            texture = "/mob/pig.png";
            setBoundingBoxSpacing(0.9F, 0.9F);
        }

        protected override void initDataTracker()
        {
            dataWatcher.addObject(16, java.lang.Byte.valueOf((byte)0));
        }

        public override void writeNbt(NBTTagCompound var1)
        {
            base.writeNbt(var1);
            var1.SetBoolean("Saddle", getSaddled());
        }

        public override void readNbt(NBTTagCompound var1)
        {
            base.readNbt(var1);
            setSaddled(var1.GetBoolean("Saddle"));
        }

        protected override string getLivingSound()
        {
            return "mob.pig";
        }

        protected override string getHurtSound()
        {
            return "mob.pig";
        }

        protected override string getDeathSound()
        {
            return "mob.pigdeath";
        }

        public override bool interact(EntityPlayer var1)
        {
            if (!getSaddled() || world.isRemote || passenger != null && passenger != var1)
            {
                return false;
            }
            else
            {
                var1.setVehicle(this);
                return true;
            }
        }

        protected override int getDropItemId()
        {
            return fireTicks > 0 ? Item.COOKED_PORKCHOP.id : Item.RAW_PORKCHOP.id;
        }

        public bool getSaddled()
        {
            return (dataWatcher.getWatchableObjectByte(16) & 1) != 0;
        }

        public void setSaddled(bool var1)
        {
            if (var1)
            {
                dataWatcher.updateObject(16, java.lang.Byte.valueOf((byte)1));
            }
            else
            {
                dataWatcher.updateObject(16, java.lang.Byte.valueOf((byte)0));
            }

        }

        public override void onStruckByLightning(EntityLightningBolt var1)
        {
            if (!world.isRemote)
            {
                EntityPigZombie var2 = new EntityPigZombie(world);
                var2.setPositionAndAnglesKeepPrevAngles(x, y, z, yaw, pitch);
                world.spawnEntity(var2);
                markDead();
            }
        }

        protected override void onLanding(float var1)
        {
            base.onLanding(var1);
            if (var1 > 5.0F && passenger is EntityPlayer)
            {
                ((EntityPlayer)passenger).incrementStat(Achievements.KILL_PIG);
            }

        }
    }

}