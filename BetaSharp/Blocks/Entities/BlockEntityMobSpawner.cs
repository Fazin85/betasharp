using BetaSharp.Entities;
using BetaSharp.NBT;
using BetaSharp.Util.Maths;

namespace BetaSharp.Blocks.Entities;

public class BlockEntityMobSpawner : BlockEntity
{
    public int SpawnDelay { get; set; } = -1;
    private string _spawnedEntityId = "Pig";
    public double Rotation { get; set; }
    public double LastRotation { get; set; } = 0.0D;

    public BlockEntityMobSpawner()
    {
        SpawnDelay = 20;
    }

    public string GetSpawnedEntityId()
    {
        return _spawnedEntityId;
    }

    public void SetSpawnedEntityId(string spawnedEntityId)
    {
        _spawnedEntityId = spawnedEntityId;
    }

    public bool IsPlayerInRange()
    {
        return world.getClosestPlayer(x + 0.5D, y + 0.5D, z + 0.5D, 16.0D) != null;
    }

    public override void tick()
    {
        LastRotation = Rotation;
        if (IsPlayerInRange())
        {
            double particleX = (double)(x + world.random.NextFloat());
            double particleY = (double)(y + world.random.NextFloat());
            double particleZ = (double)(z + world.random.NextFloat());
            world.addParticle("smoke", particleX, particleY, particleZ, 0.0D, 0.0D, 0.0D);
            world.addParticle("flame", particleX, particleY, particleZ, 0.0D, 0.0D, 0.0D);

            for (Rotation += 1000.0F / (SpawnDelay + 200.0F); Rotation > 360.0D; LastRotation -= 360.0D)
            {
                Rotation -= 360.0D;
            }

            if (!world.isRemote)
            {
                if (SpawnDelay == -1)
                {
                    ResetDelay();
                }

                if (SpawnDelay > 0)
                {
                    --SpawnDelay;
                    return;
                }

                byte max = 4;

                for (int spawnAttempt = 0; spawnAttempt < max; ++spawnAttempt)
                {
                    EntityLiving entityLiving = (EntityLiving)EntityRegistry.create(_spawnedEntityId, world);
                    if (entityLiving == null)
                    {
                        return;
                    }

                    int count = world.collectEntitiesByClass(entityLiving.getClass(), new Box(x, y, z, x + 1, y + 1, z + 1).expand(8.0D, 4.0D, 8.0D)).Count;
                    if (count >= 6)
                    {
                        ResetDelay();
                        return;
                    }

                    if (entityLiving != null)
                    {
                        double posX = x + (world.random.NextDouble() - world.random.NextDouble()) * 4.0D;
                        double posY = y + world.random.NextInt(3) - 1;
                        double posZ = z + (world.random.NextDouble() - world.random.NextDouble()) * 4.0D;
                        entityLiving.setPositionAndAnglesKeepPrevAngles(posX, posY, posZ, world.random.NextFloat() * 360.0F, 0.0F);
                        if (entityLiving.canSpawn())
                        {
                            world.SpawnEntity(entityLiving);

                            for (int particleIndex = 0; particleIndex < 20; ++particleIndex)
                            {
                                particleX = x + 0.5D + ((double)world.random.NextFloat() - 0.5D) * 2.0D;
                                particleY = y + 0.5D + ((double)world.random.NextFloat() - 0.5D) * 2.0D;
                                particleZ = z + 0.5D + ((double)world.random.NextFloat() - 0.5D) * 2.0D;
                                world.addParticle("smoke", particleX, particleY, particleZ, 0.0D, 0.0D, 0.0D);
                                world.addParticle("flame", particleX, particleY, particleZ, 0.0D, 0.0D, 0.0D);
                            }

                            entityLiving.animateSpawn();
                            ResetDelay();
                        }
                    }
                }
            }

            base.tick();
        }
    }

    private void ResetDelay()
    {
        SpawnDelay = 200 + world.random.NextInt(600);
        Log.Info("Spawn Delay: " + SpawnDelay);
    }

    public override void readNbt(NBTTagCompound nbt)
    {
        base.readNbt(nbt);
        _spawnedEntityId = nbt.GetString("EntityId");
        SpawnDelay = nbt.GetShort("Delay");
    }

    public override void writeNbt(NBTTagCompound nbt)
    {
        base.writeNbt(nbt);
        nbt.SetString("EntityId", _spawnedEntityId);
        nbt.SetShort("Delay", (short)SpawnDelay);
    }
}
