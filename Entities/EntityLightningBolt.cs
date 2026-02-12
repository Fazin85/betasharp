using betareborn.Blocks;
using betareborn.NBT;
using betareborn.Util.Maths;
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
            field_27029_a = random.nextLong();
            field_27030_c = random.nextInt(3) + 1;
            if (var1.difficulty >= 2 && var1.isRegionLoaded(MathHelper.floor_double(var2), MathHelper.floor_double(var4), MathHelper.floor_double(var6), 10))
            {
                int var8 = MathHelper.floor_double(var2);
                int var9 = MathHelper.floor_double(var4);
                int var10 = MathHelper.floor_double(var6);
                if (var1.GetBlockId(var8, var9, var10) == 0 && Block.FIRE.canPlaceAt(var1, var8, var9, var10))
                {
                    var1.setBlock(var8, var9, var10, Block.FIRE.id);
                }

                for (var8 = 0; var8 < 4; ++var8)
                {
                    var9 = MathHelper.floor_double(var2) + random.nextInt(3) - 1;
                    var10 = MathHelper.floor_double(var4) + random.nextInt(3) - 1;
                    int var11 = MathHelper.floor_double(var6) + random.nextInt(3) - 1;
                    if (var1.GetBlockId(var9, var10, var11) == 0 && Block.FIRE.canPlaceAt(var1, var9, var10, var11))
                    {
                        var1.setBlock(var9, var10, var11, Block.FIRE.id);
                    }
                }
            }

        }

        public override void tick()
        {
            base.tick();
            if (field_27028_b == 2)
            {
                world.playSound(x, y, z, "ambient.weather.thunder", 10000.0F, 0.8F + random.nextFloat() * 0.2F);
                world.playSound(x, y, z, "random.explode", 2.0F, 0.5F + random.nextFloat() * 0.2F);
            }

            --field_27028_b;
            if (field_27028_b < 0)
            {
                if (field_27030_c == 0)
                {
                    markDead();
                }
                else if (field_27028_b < -random.nextInt(10))
                {
                    --field_27030_c;
                    field_27028_b = 1;
                    field_27029_a = random.nextLong();
                    if (world.isRegionLoaded(MathHelper.floor_double(x), MathHelper.floor_double(y), MathHelper.floor_double(z), 10))
                    {
                        int var1 = MathHelper.floor_double(x);
                        int var2 = MathHelper.floor_double(y);
                        int var3 = MathHelper.floor_double(z);
                        if (world.GetBlockId(var1, var2, var3) == 0 && Block.FIRE.canPlaceAt(world, var1, var2, var3))
                        {
                            world.setBlock(var1, var2, var3, Block.FIRE.id);
                        }
                    }
                }
            }

            if (field_27028_b >= 0)
            {
                double var6 = 3.0D;
                var var7 = world.getEntities(this, new Box(x - var6, y - var6, z - var6, x + var6, y + 6.0D + var6, z + var6));

                for (int var4 = 0; var4 < var7.Count; ++var4)
                {
                    Entity var5 = var7[var4];
                    var5.onStruckByLightning(this);
                }

                world.lightningTicksLeft = 2;
            }

        }

        protected override void initDataTracker()
        {
        }

        public override void readNbt(NBTTagCompound var1)
        {
        }

        public override void writeNbt(NBTTagCompound var1)
        {
        }

        public override bool shouldRender(Vec3D var1)
        {
            return field_27028_b >= 0;
        }
    }
}