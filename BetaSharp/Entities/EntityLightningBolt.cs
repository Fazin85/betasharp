using BetaSharp.Blocks;
using BetaSharp.NBT;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds;

namespace BetaSharp.Entities;

public class EntityLightningBolt : EntityWeatherEffect
{
    public static readonly new java.lang.Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(EntityLightningBolt).TypeHandle);

    private int flashTimer;
    public long renderSeed;
    private int flashCount;

    public EntityLightningBolt(World world, double x, double y, double z) : base(world)
    {
        setPositionAndAnglesKeepPrevAngles(x, y, z, 0.0F, 0.0F);
        flashTimer = 2;
        renderSeed = random.NextLong();
        flashCount = random.NextInt(3) + 1;
        if (world.difficulty >= 2 && world.isRegionLoaded(MathHelper.floor_double(x), MathHelper.floor_double(y), MathHelper.floor_double(z), 10))
        {
            int strikeX = MathHelper.floor_double(x);
            int strikeY = MathHelper.floor_double(y);
            int strikeZ = MathHelper.floor_double(z);
            if (world.getBlockId(strikeX, strikeY, strikeZ) == 0 && Block.Fire.canPlaceAt(world, strikeX, strikeY, strikeZ))
            {
                world.setBlock(strikeX, strikeY, strikeZ, Block.Fire.id);
            }

            for (strikeX = 0; strikeX < 4; ++strikeX)
            {
                strikeY = MathHelper.floor_double(x) + random.NextInt(3) - 1;
                strikeZ = MathHelper.floor_double(y) + random.NextInt(3) - 1;
                int fireZ = MathHelper.floor_double(z) + random.NextInt(3) - 1;
                if (world.getBlockId(strikeY, strikeZ, fireZ) == 0 && Block.Fire.canPlaceAt(world, strikeY, strikeZ, fireZ))
                {
                    world.setBlock(strikeY, strikeZ, fireZ, Block.Fire.id);
                }
            }
        }

    }

    public override void tick()
    {
        base.tick();
        if (flashTimer == 2)
        {
            world.playSound(x, y, z, "ambient.weather.thunder", 10000.0F, 0.8F + random.NextFloat() * 0.2F);
            world.playSound(x, y, z, "random.explode", 2.0F, 0.5F + random.NextFloat() * 0.2F);
        }

        --flashTimer;
        if (flashTimer < 0)
        {
            if (flashCount == 0)
            {
                markDead();
            }
            else if (flashTimer < -random.NextInt(10))
            {
                --flashCount;
                flashTimer = 1;
                renderSeed = random.NextLong();
                if (world.isRegionLoaded(MathHelper.floor_double(x), MathHelper.floor_double(y), MathHelper.floor_double(z), 10))
                {
                    int floorX = MathHelper.floor_double(x);
                    int floorY = MathHelper.floor_double(y);
                    int floorZ = MathHelper.floor_double(z);
                    if (world.getBlockId(floorX, floorY, floorZ) == 0 && Block.Fire.canPlaceAt(world, floorX, floorY, floorZ))
                    {
                        world.setBlock(floorX, floorY, floorZ, Block.Fire.id);
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