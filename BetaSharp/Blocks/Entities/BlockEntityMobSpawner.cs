using BetaSharp.Entities;
using BetaSharp.NBT;
using BetaSharp.Util.Maths;

namespace BetaSharp.Blocks.Entities;

public class BlockEntityMobSpawner : BlockEntity
{
    public static readonly new java.lang.Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(BlockEntityMobSpawner).TypeHandle);

    public int spawnDelay = -1;
    private string spawnedEntityId = "Pig";
    public double rotation;
    public double lastRotation = 0.0D;

    public BlockEntityMobSpawner()
    {
        spawnDelay = 20;
    }

    public string getSpawnedEntityId()
    {
        return spawnedEntityId;
    }

    public void setSpawnedEntityId(string spawnedEntityId)
    {
        this.spawnedEntityId = spawnedEntityId;
    }

    public bool isPlayerInRange()
    {
        return world.getClosestPlayer(x + 0.5D, y + 0.5D, z + 0.5D, 16.0D) != null;
    }

    public override void tick()
    {
        lastRotation = rotation;
        if (isPlayerInRange())
        {
            double particleX = (double)(x + world.random.nextFloat());
            double particleY = (double)(y + world.random.nextFloat());
            double particleZ = (double)(z + world.random.nextFloat());
            world.addParticle("smoke", particleX, particleY, particleZ, 0.0D, 0.0D, 0.0D);
            world.addParticle("flame", particleX, particleY, particleZ, 0.0D, 0.0D, 0.0D);

            for (rotation += 1000.0F / (spawnDelay + 200.0F); rotation > 360.0D; lastRotation -= 360.0D)
            {
                rotation -= 360.0D;
            }

            if (!world.isRemote)
            {
                if (spawnDelay == -1)
                {
                    resetDelay();
                }

                if (spawnDelay > 0)
                {
                    --spawnDelay;
                    return;
                }

                byte max = 4;

                for (int spawnAttempt = 0; spawnAttempt < max; ++spawnAttempt)
                {
                    EntityLiving entityLiving = (EntityLiving)EntityRegistry.create(spawnedEntityId, world);
                    if (entityLiving == null)
                    {
                        return;
                    }

                    int count = world.collectEntitiesByClass(entityLiving.getClass(), new Box(x, y, z, x + 1, y + 1, z + 1).expand(8.0D, 4.0D, 8.0D)).Count;
                    if (count >= 6)
                    {
                        resetDelay();
                        return;
                    }

                    if (entityLiving != null)
                    {
                        double posX = x + (world.random.nextDouble() - world.random.nextDouble()) * 4.0D;
                        double posY = y + world.random.nextInt(3) - 1;
                        double posZ = z + (world.random.nextDouble() - world.random.nextDouble()) * 4.0D;
                        entityLiving.setPositionAndAnglesKeepPrevAngles(posX, posY, posZ, world.random.nextFloat() * 360.0F, 0.0F);
                        if (entityLiving.canSpawn())
                        {
                            world.SpawnEntity(entityLiving);

                            for (int particleIndex = 0; particleIndex < 20; ++particleIndex)
                            {
                                particleX = x + 0.5D + ((double)world.random.nextFloat() - 0.5D) * 2.0D;
                                particleY = y + 0.5D + ((double)world.random.nextFloat() - 0.5D) * 2.0D;
                                particleZ = z + 0.5D + ((double)world.random.nextFloat() - 0.5D) * 2.0D;
                                world.addParticle("smoke", particleX, particleY, particleZ, 0.0D, 0.0D, 0.0D);
                                world.addParticle("flame", particleX, particleY, particleZ, 0.0D, 0.0D, 0.0D);
                            }

                            entityLiving.animateSpawn();
                            resetDelay();
                        }
                    }
                }
            }

            base.tick();
        }
    }

    private void resetDelay()
    {
        spawnDelay = 200 + world.random.nextInt(600);
    }

    public override void readNbt(NBTTagCompound nbt)
    {
        base.readNbt(nbt);
        spawnedEntityId = nbt.GetString("EntityId");
        spawnDelay = nbt.GetShort("Delay");
    }

    public override void writeNbt(NBTTagCompound nbt)
    {
        base.writeNbt(nbt);
        nbt.SetString("EntityId", spawnedEntityId);
        nbt.SetShort("Delay", (short)spawnDelay);
    }
}