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
            setSize(0.9F, 0.9F);
        }

        protected override void entityInit()
        {
            dataWatcher.addObject(16, java.lang.Byte.valueOf((byte)0));
        }

        public override void writeEntityToNBT(NBTTagCompound var1)
        {
            base.writeEntityToNBT(var1);
            var1.setBoolean("Saddle", getSaddled());
        }

        public override void readEntityFromNBT(NBTTagCompound var1)
        {
            base.readEntityFromNBT(var1);
            setSaddled(var1.getBoolean("Saddle"));
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
            if (!getSaddled() || worldObj.multiplayerWorld || riddenByEntity != null && riddenByEntity != var1)
            {
                return false;
            }
            else
            {
                var1.mountEntity(this);
                return true;
            }
        }

        protected override int getDropItemId()
        {
            return fire > 0 ? Item.porkCooked.shiftedIndex : Item.porkRaw.shiftedIndex;
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
            if (!worldObj.multiplayerWorld)
            {
                EntityPigZombie var2 = new EntityPigZombie(worldObj);
                var2.setLocationAndAngles(posX, posY, posZ, rotationYaw, rotationPitch);
                worldObj.entityJoinedWorld(var2);
                setEntityDead();
            }
        }

        protected override void fall(float var1)
        {
            base.fall(var1);
            if (var1 > 5.0F && riddenByEntity is EntityPlayer)
            {
                ((EntityPlayer)riddenByEntity).triggerAchievement(Achievements.KILL_PIG);
            }

        }
    }

}