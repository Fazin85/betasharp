using betareborn.Items;
using betareborn.NBT;
using betareborn.Worlds;

namespace betareborn.Entities
{
    public class EntityPigZombie : EntityZombie
    {
        public static readonly new java.lang.Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(EntityPigZombie).TypeHandle);

        private int angerLevel = 0;
        private int randomSoundDelay = 0;
        private static readonly ItemStack defaultHeldItem = new ItemStack(Item.GOLDEN_SWORD, 1);

        public EntityPigZombie(World var1) : base(var1)
        {
            texture = "/mob/pigzombie.png";
            movementSpeed = 0.5F;
            attackStrength = 5;
            isImmuneToFire = true;
        }

        public override void tick()
        {
            movementSpeed = playerToAttack != null ? 0.95F : 0.5F;
            if (randomSoundDelay > 0 && --randomSoundDelay == 0)
            {
                world.playSound(this, "mob.zombiepig.zpigangry", getSoundVolume() * 2.0F, ((random.nextFloat() - random.nextFloat()) * 0.2F + 1.0F) * 1.8F);
            }

            base.tick();
        }

        public override bool canSpawn()
        {
            return world.difficulty > 0 && world.canSpawnEntity(boundingBox) && world.getEntityCollisions(this, boundingBox).Count == 0 && !world.isBoxSubmergedInFluid(boundingBox);
        }

        public override void writeNbt(NBTTagCompound var1)
        {
            base.writeNbt(var1);
            var1.SetShort("Anger", (short)angerLevel);
        }

        public override void readNbt(NBTTagCompound var1)
        {
            base.readNbt(var1);
            angerLevel = var1.GetShort("Anger");
        }

        protected override Entity findPlayerToAttack()
        {
            return angerLevel == 0 ? null : base.findPlayerToAttack();
        }

        public override void tickMovement()
        {
            base.tickMovement();
        }

        public override bool damage(Entity var1, int var2)
        {
            if (var1 is EntityPlayer)
            {
                var var3 = world.getEntities(this, boundingBox.expand(32.0D, 32.0D, 32.0D));

                for (int var4 = 0; var4 < var3.Count; ++var4)
                {
                    Entity var5 = var3[var4];
                    if (var5 is EntityPigZombie)
                    {
                        EntityPigZombie var6 = (EntityPigZombie)var5;
                        var6.becomeAngryAt(var1);
                    }
                }

                becomeAngryAt(var1);
            }

            return base.damage(var1, var2);
        }

        private void becomeAngryAt(Entity var1)
        {
            playerToAttack = var1;
            angerLevel = 400 + random.nextInt(400);
            randomSoundDelay = random.nextInt(40);
        }

        protected override String getLivingSound()
        {
            return "mob.zombiepig.zpig";
        }

        protected override String getHurtSound()
        {
            return "mob.zombiepig.zpighurt";
        }

        protected override String getDeathSound()
        {
            return "mob.zombiepig.zpigdeath";
        }

        protected override int getDropItemId()
        {
            return Item.COOKED_PORKCHOP.id;
        }

        public override ItemStack getHeldItem()
        {
            return defaultHeldItem;
        }
    }

}