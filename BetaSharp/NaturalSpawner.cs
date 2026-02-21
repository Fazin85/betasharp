using BetaSharp.Blocks;
using BetaSharp.Entities;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds;
using BetaSharp.Worlds.Biomes;

namespace BetaSharp;

public static class NaturalSpawner
{
    private const int SpawnMaxRadius = 8; // Expressed in chunks
    private const float SpawnMinRadius = 24.0F; // Expressed in blocks
    private const int SpawnCloseness = 6;

    private static readonly HashSet<ChunkPos> ChunksForSpawning = [];
    private static readonly Func<World, EntityLiving>[] nightSpawnEntities =
    [
        w => new EntitySpider(w),
        w => new EntityZombie(w),
        w => new EntitySkeleton(w),
    ];

    private static BlockPos GetRandomSpawningPointInChunk(World world, int centerX, int centerZ)
    {
        int x = centerX + world.random.NextInt(16);
        int y = world.random.NextInt(128);
        int z = centerZ + world.random.NextInt(16);
        return new BlockPos(x, y, z);
    }

    public static void PerformSpawning(World world, bool spawnHostile, bool spawnPeaceful)
    {
        if (!spawnHostile && !spawnPeaceful) return;

        ChunksForSpawning.Clear();

        foreach (var p in world.players)
        {
            int chunkX = MathHelper.Floor(p.x / 16.0D);
            int chunkZ = MathHelper.Floor(p.z / 16.0D);

            for (int x = -SpawnMaxRadius; x <= SpawnMaxRadius; ++x)
            {
                for (int z = -SpawnMaxRadius; z <= SpawnMaxRadius; ++z)
                {
                    ChunksForSpawning.Add(new ChunkPos(chunkX + x, chunkZ + z));
                }
            }
        }

        Vec3i worldSpawn = world.getSpawnPos();
        foreach (var creatureKind in CreatureKind.Values)
        {
            if (((!creatureKind.Peaceful && spawnHostile) || (creatureKind.Peaceful && spawnPeaceful)) && world.countEntities(creatureKind.EntityType) <= creatureKind.MobCap * ChunksForSpawning.Count / 256)
            {
                foreach (var chunk in ChunksForSpawning)
                {
                    Biome biome = world.getBiomeSource().GetBiome(chunk);
                    var spawnables = biome.GetSpawnableList(creatureKind);

                    if (spawnables == null || spawnables.Count == 0)
                    {
                        continue;
                    }

                    int totalWeight = 0;
                    foreach (var entry in spawnables)
                    {
                        totalWeight += entry.SpawnWeight;
                    }

                    int r = world.random.NextInt(totalWeight);
                    SpawnListEntry toSpawn = null;

                    foreach (var entry in spawnables)
                    {
                        r -= entry.SpawnWeight;

                        if (r < 0)
                        {
                            toSpawn = entry;
                            break;
                        }
                    }

                    BlockPos spawnPos = GetRandomSpawningPointInChunk(world, chunk.x * 16, chunk.z * 16);
                    if (world.shouldSuffocate(spawnPos.x, spawnPos.y, spawnPos.z)) continue;
                    if (world.getMaterial(spawnPos.x, spawnPos.y, spawnPos.z) != creatureKind.SpawnMaterial) continue;

                    int spawnedCount = 0;
                    bool breakToNextChunk = false;

                    for (int i = 0; i < 3 && !breakToNextChunk; ++i)
                    {
                        int x = spawnPos.x;
                        int y = spawnPos.y;
                        int z = spawnPos.z;

                        for (int j = 0; j < 4 && !breakToNextChunk; ++j)
                        {
                            x += world.random.NextInt(SpawnCloseness) - world.random.NextInt(SpawnCloseness);
                            y += world.random.NextInt(1) - world.random.NextInt(1);
                            z += world.random.NextInt(SpawnCloseness) - world.random.NextInt(SpawnCloseness);
                            if (creatureKind.CanSpawnAtLocation(world, x, y, z))
                            {
                                Vec3D entityPos = new Vec3D(x + 0.5D, y, z + 0.5D);
                                if (world.getClosestPlayer(entityPos.x, entityPos.y, entityPos.z, SpawnMinRadius) != null) continue;
                                if (entityPos.squareDistanceTo((Vec3D)worldSpawn) < SpawnMinRadius * SpawnMinRadius) continue;
                                EntityLiving spawnedEntity = toSpawn.Factory(world);

                                spawnedEntity.setPositionAndAnglesKeepPrevAngles(entityPos.x, entityPos.y, entityPos.z, world.random.NextFloat() * 360.0F, 0.0F);
                                if (spawnedEntity.canSpawn())
                                {
                                    spawnedCount++;
                                    world.SpawnEntity(spawnedEntity);
                                    EntitySpecificInit(spawnedEntity, world, entityPos.x, entityPos.y, entityPos.z);
                                    if (spawnedCount >= spawnedEntity.getMaxSpawnedInChunk())
                                    {
                                        breakToNextChunk = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    private static void EntitySpecificInit(EntityLiving entity, World world, double x, double y, double z)
    {
        if (entity is EntitySpider && world.random.NextInt(100) == 0)
        {
            EntitySkeleton var5 = new EntitySkeleton(world);
            var5.setPositionAndAnglesKeepPrevAngles((double)x, (double)y, (double)z, entity.yaw, 0.0F);
            world.SpawnEntity(var5);
            var5.setVehicle(entity);
        }
        else if (entity is EntitySheep)
        {
            ((EntitySheep)entity).setFleeceColor(EntitySheep.getRandomFleeceColor(world.random));
        }
    }

    public static bool SpawnMonstersAndWakePlayers(World world, List<EntityPlayer> players)
    {
        bool monstersSpawned = false;
        var pathfinder = new Pathfinder(world);
        foreach (var player in players)
        {
            bool breakFromLoop = false;

            for (int i = 0; i < 20 && !breakFromLoop; ++i)
            {
                int var9 = MathHelper.Floor(player.x) + world.random.NextInt(32) - world.random.NextInt(32);
                int var10 = MathHelper.Floor(player.z) + world.random.NextInt(32) - world.random.NextInt(32);
                int var11 = MathHelper.Floor(player.y) + world.random.NextInt(16) - world.random.NextInt(16);
                if (var11 < 1)
                {
                    var11 = 1;
                }
                else if (var11 > 128)
                {
                    var11 = 128;
                }

                int var12 = world.random.NextInt(nightSpawnEntities.Length);

                int var13;
                for (var13 = var11; var13 > 2 && !world.shouldSuffocate(var9, var13 - 1, var10); --var13)
                {
                }

                while (!CreatureKind.Monster.CanSpawnAtLocation(world, var9, var13, var10) && var13 < var11 + 16 && var13 < 128)
                {
                    ++var13;
                }

                if (var13 < var11 + 16 && var13 < 128)
                {
                    float var14 = (float)var9 + 0.5F;
                    float var15 = (float)var13;
                    float var16 = (float)var10 + 0.5F;

                    EntityLiving entity = nightSpawnEntities[var12](world);

                    entity.setPositionAndAnglesKeepPrevAngles((double)var14, (double)var15, (double)var16, world.random.NextFloat() * 360.0F, 0.0F);
                    if (entity.canSpawn())
                    {
                        PathEntity var18 = pathfinder.createEntityPathTo(entity, player, 32.0F);
                        if (var18 != null && var18.pathLength > 1)
                        {
                            PathPoint var19 = var18.func_22328_c();
                            if (java.lang.Math.abs((double)var19.xCoord - player.x) < 1.5D && java.lang.Math.abs((double)var19.zCoord - player.z) < 1.5D && java.lang.Math.abs((double)var19.yCoord - player.y) < 1.5D)
                            {
                                Vec3i var20 = BlockBed.findWakeUpPosition(world, MathHelper.Floor(player.x), MathHelper.Floor(player.y), MathHelper.Floor(player.z), 1);
                                if (var20 == null)
                                {
                                    var20 = new Vec3i(var9, var13 + 1, var10);
                                }

                                entity.setPositionAndAnglesKeepPrevAngles((double)((float)var20.x + 0.5F), (double)var20.y, (double)((float)var20.z + 0.5F), 0.0F, 0.0F);
                                world.SpawnEntity(entity);
                                EntitySpecificInit(entity, world, (float)var20.x + 0.5F, (float)var20.y, (float)var20.z + 0.5F);
                                player.wakeUp(true, false, false);
                                entity.playLivingSound();
                                monstersSpawned = true;
                                breakFromLoop = true;
                            }
                        }
                    }
                }
            }
        }

        return monstersSpawned;
    }
}
