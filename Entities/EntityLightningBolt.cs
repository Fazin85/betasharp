using betareborn.Blocks;
using betareborn.NBT;
using betareborn.Worlds;

namespace betareborn.Entities
{
    public class EntityLightningBolt : EntityWeatherEffect
    {
        public static readonly new java.lang.Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(EntityLightningBolt).TypeHandle);

        private int field_27028_b;
        public long field_27029_a = 0L;
        private int field_27030_c;

        public EntityLightningBolt(World var1, double var2, double var4, double var6) : base(var1)
        {
            setPositionAndAnglesKeepPrevAngles(var2, var4, var6, 0.0F, 0.0F);
            field_27028_b = 2;
            field_27029_a = rand.nextLong();
            field_27030_c = rand.nextInt(3) + 1;
            if (var1.difficultySetting >= 2 && var1.doChunksNearChunkExist(MathHelper.floor_double(var2), MathHelper.floor_double(var4), MathHelper.floor_double(var6), 10))
            {
                int var8 = MathHelper.floor_double(var2);
                int var9 = MathHelper.floor_double(var4);
                int var10 = MathHelper.floor_double(var6);
                if (var1.getBlockId(var8, var9, var10) == 0 && Block.FIRE.canPlaceBlockAt(var1, var8, var9, var10))
                {
                    var1.setBlockWithNotify(var8, var9, var10, Block.FIRE.id);
                }

                for (var8 = 0; var8 < 4; ++var8)
                {
                    var9 = MathHelper.floor_double(var2) + rand.nextInt(3) - 1;
                    var10 = MathHelper.floor_double(var4) + rand.nextInt(3) - 1;
                    int var11 = MathHelper.floor_double(var6) + rand.nextInt(3) - 1;
                    if (var1.getBlockId(var9, var10, var11) == 0 && Block.FIRE.canPlaceBlockAt(var1, var9, var10, var11))
                    {
                        var1.setBlockWithNotify(var9, var10, var11, Block.FIRE.id);
                    }
                }
            }

        }

        public override void onUpdate()
        {
            base.onUpdate();
            if (field_27028_b == 2)
            {
                worldObj.playSoundEffect(posX, posY, posZ, "ambient.weather.thunder", 10000.0F, 0.8F + rand.nextFloat() * 0.2F);
                worldObj.playSoundEffect(posX, posY, posZ, "random.explode", 2.0F, 0.5F + rand.nextFloat() * 0.2F);
            }

            --field_27028_b;
            if (field_27028_b < 0)
            {
                if (field_27030_c == 0)
                {
                    setEntityDead();
                }
                else if (field_27028_b < -rand.nextInt(10))
                {
                    --field_27030_c;
                    field_27028_b = 1;
                    field_27029_a = rand.nextLong();
                    if (worldObj.doChunksNearChunkExist(MathHelper.floor_double(posX), MathHelper.floor_double(posY), MathHelper.floor_double(posZ), 10))
                    {
                        int var1 = MathHelper.floor_double(posX);
                        int var2 = MathHelper.floor_double(posY);
                        int var3 = MathHelper.floor_double(posZ);
                        if (worldObj.getBlockId(var1, var2, var3) == 0 && Block.FIRE.canPlaceBlockAt(worldObj, var1, var2, var3))
                        {
                            worldObj.setBlockWithNotify(var1, var2, var3, Block.FIRE.id);
                        }
                    }
                }
            }

            if (field_27028_b >= 0)
            {
                double var6 = 3.0D;
                var var7 = worldObj.getEntitiesWithinAABBExcludingEntity(this, Box.createCached(posX - var6, posY - var6, posZ - var6, posX + var6, posY + 6.0D + var6, posZ + var6));

                for (int var4 = 0; var4 < var7.Count; ++var4)
                {
                    Entity var5 = var7[var4];
                    var5.onStruckByLightning(this);
                }

                worldObj.field_27172_i = 2;
            }

        }

        protected override void entityInit()
        {
        }

        public override void readEntityFromNBT(NBTTagCompound var1)
        {
        }

        public override void writeEntityToNBT(NBTTagCompound var1)
        {
        }

        public override bool isInRangeToRenderVec3D(Vec3D var1)
        {
            return field_27028_b >= 0;
        }
    }
}