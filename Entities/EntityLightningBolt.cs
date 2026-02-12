using betareborn.Blocks;
using betareborn.NBT;
using betareborn.Util.Maths;
using betareborn.Worlds;

namespace betareborn.Entities
{
    public class EntityLightningBolt : EntityWeatherEffect
    {
        public static readonly new java.lang.Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(EntityLightningBolt).TypeHandle);

        private int flashTimer;
        public long renderSeed = 0L;
        private int flashCount;

        public EntityLightningBolt(World world, double x, double y, double z) : base(world)
        {
            setPositionAndAnglesKeepPrevAngles(x, y, z, 0.0F, 0.0F);
            flashTimer = 2;
            renderSeed = random.nextLong();
            flashCount = random.nextInt(3) + 1;
            if (world.difficulty >= 2 && world.isRegionLoaded(MathHelper.floor_double(x), MathHelper.floor_double(y), MathHelper.floor_double(z), 10))
            {
                int var8 = MathHelper.floor_double(var2);
                int var9 = MathHelper.floor_double(var4);
                int var10 = MathHelper.floor_double(var6);
                if (var1.getBlockId(var8, var9, var10) == 0 && Block.FIRE.canPlaceAt(var1, var8, var9, var10))
                {
                    world.setBlock(strikeX, strikeY, strikeZ, Block.FIRE.id);
                }

                for (strikeX = 0; strikeX < 4; ++strikeX)
                {
                    var9 = MathHelper.floor_double(var2) + random.nextInt(3) - 1;
                    var10 = MathHelper.floor_double(var4) + random.nextInt(3) - 1;
                    int var11 = MathHelper.floor_double(var6) + random.nextInt(3) - 1;
                    if (var1.getBlockId(var9, var10, var11) == 0 && Block.FIRE.canPlaceAt(var1, var9, var10, var11))
                    {
                        world.setBlock(strikeY, strikeZ, fireZ, Block.FIRE.id);
                    }
                }
            }

        }

        public override void tick()
        {
            base.tick();
            if (flashTimer == 2)
            {
                world.playSound(x, y, z, "ambient.weather.thunder", 10000.0F, 0.8F + random.nextFloat() * 0.2F);
                world.playSound(x, y, z, "random.explode", 2.0F, 0.5F + random.nextFloat() * 0.2F);
            }

            --flashTimer;
            if (flashTimer < 0)
            {
                if (flashCount == 0)
                {
                    markDead();
                }
                else if (flashTimer < -random.nextInt(10))
                {
                    --flashCount;
                    flashTimer = 1;
                    renderSeed = random.nextLong();
                    if (world.isRegionLoaded(MathHelper.floor_double(x), MathHelper.floor_double(y), MathHelper.floor_double(z), 10))
                    {
                        int var1 = MathHelper.floor_double(x);
                        int var2 = MathHelper.floor_double(y);
                        int var3 = MathHelper.floor_double(z);
                        if (world.getBlockId(var1, var2, var3) == 0 && Block.FIRE.canPlaceAt(world, var1, var2, var3))
                        {
                            world.setBlock(floorX, floorY, floorZ, Block.FIRE.id);
                        }
                    }
                }
            }

            if (flashTimer >= 0)
            {
                double searchRadius = 3.0D;
                var entities = world.getEntities(this, new Box(x - searchRadius, y - searchRadius, z - searchRadius, x + searchRadius, y + 6.0D + searchRadius, z + searchRadius));

                for (int i = 0; i < entities.Count; ++i)
                {
                    Entity entity = entities[i];
                    entity.onStruckByLightning(this);
                }

                world.lightningTicksLeft = 2;
            }

        }

        protected override void initDataTracker()
        {
        }

        public override void readNbt(NBTTagCompound nbt)
        {
        }

        public override void writeNbt(NBTTagCompound nbt)
        {
        }

        public override bool shouldRender(Vec3D cameraPos)
        {
            return flashTimer >= 0;
        }
    }
}