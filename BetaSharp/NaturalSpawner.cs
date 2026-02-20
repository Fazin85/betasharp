using BetaSharp.Blocks;
using BetaSharp.Blocks.Materials;
using BetaSharp.Entities;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds;
using BetaSharp.Worlds.Biomes;
using java.lang;

namespace BetaSharp;

public class NaturalSpawner
{
    private static HashSet<ChunkPos> eligibleChunksForSpawning = [];
    protected static readonly Class[] nightSpawnEntities =
    [
        EntitySpider.Class,
        EntityZombie.Class,
        EntitySkeleton.Class,
    ];

    protected static BlockPos getRandomSpawningPointInChunk(World var0, int var1, int var2)
    {
        int var3 = var1 + var0.random.NextInt(16);
        int var4 = var0.random.NextInt(128);
        int var5 = var2 + var0.random.NextInt(16);
        return new BlockPos(var3, var4, var5);
    }

    public static int performSpawning(World var0, bool var1, bool var2)
    {
        if (!var1 && !var2)
        {
            return 0;
        }
        else
        {
            eligibleChunksForSpawning.Clear();

            int var3;
            int var6;
            for (var3 = 0; var3 < var0.players.Count; ++var3)
            {
                EntityPlayer var4 = var0.players[var3];
                int var5 = MathHelper.Floor(var4.x / 16.0D);
                var6 = MathHelper.Floor(var4.z / 16.0D);
                byte var7 = 8;

                for (int var8 = -var7; var8 <= var7; ++var8)
                {
                    for (int var9 = -var7; var9 <= var7; ++var9)
                    {
                        eligibleChunksForSpawning.Add(new ChunkPos(var8 + var5, var9 + var6));
                    }
                }
            }

            var3 = 0;
            Vec3i var35 = var0.getSpawnPos();
            EnumCreatureType[] var36 = EnumCreatureType.values;
            var6 = var36.Length;

            for (int var37 = 0; var37 < var6; ++var37)
            {
                EnumCreatureType var38 = var36[var37];
                if ((!var38.isPeaceful() || var2) && (var38.isPeaceful() || var1) && var0.countEntities(var38.getCreatureClass()) <= var38.getMaxAllowed() * eligibleChunksForSpawning.Count / 256)
                {
                    foreach (var chunk in eligibleChunksForSpawning)
                    {
                        Biome var11 = var0.getBiomeSource().GetBiome(chunk);
                        var var12 = var11.GetSpawnableList(var38);

                        if (var12 == null || var12.Count == 0)
                        {
                            continue;
                        }

                        int var13 = 0;
                        foreach (var entry in var12)
                        {
                            var13 += entry.spawnRarityRate;
                        }

                        int var40 = var0.random.NextInt(var13);
                        SpawnListEntry? e = null;

                        foreach (var entry in var12)
                        {
                            var40 -= entry.spawnRarityRate;

                            if (var40 < 0)
                            {
                                e = entry;
                                break;
                            }
                        }

                        BlockPos var41 = getRandomSpawningPointInChunk(var0, chunk.x * 16, chunk.z * 16);
                        int var42 = var41.x;
                        int var18 = var41.y;
                        int var19 = var41.z;

                        if (var0.shouldSuffocate(var42, var18, var19))
                        {
                            continue;
                        }

                        if (var0.getMaterial(var42, var18, var19) != var38.getMaterial())
                        {
                            continue;
                        }

                        int var20 = 0;
                        bool breakToNextChunk = false;

                        for (int var21 = 0; var21 < 3 && !breakToNextChunk; ++var21)
                        {
                            int var22 = var42;
                            int var23 = var18;
                            int var24 = var19;
                            byte var25 = 6;

                            for (int var26 = 0; var26 < 4 && !breakToNextChunk; ++var26)
                            {
                                var22 += var0.random.NextInt(var25) - var0.random.NextInt(var25);
                                var23 += var0.random.NextInt(1) - var0.random.NextInt(1);
                                var24 += var0.random.NextInt(var25) - var0.random.NextInt(var25);
                                if (canCreatureTypeSpawnAtLocation(var38, var0, var22, var23, var24))
                                {
                                    float var27 = (float)var22 + 0.5F;
                                    float var28 = (float)var23;
                                    float var29 = (float)var24 + 0.5F;
                                    if (var0.getClosestPlayer((double)var27, (double)var28, (double)var29, 24.0D) == null)
                                    {
                                        float var30 = var27 - (float)var35.x;
                                        float var31 = var28 - (float)var35.y;
                                        float var32 = var29 - (float)var35.z;
                                        float var33 = var30 * var30 + var31 * var31 + var32 * var32;
                                        if (var33 >= 576.0F)
                                        {
                                            EntityLiving var43;
                                            try
                                            {
                                                var43 = (EntityLiving)e!.entityClass.getConstructor(World.Class).newInstance(var0);
                                            }
                                            catch (java.lang.Exception ex)
                                            {
                                                ex.printStackTrace();
                                                return var3;
                                            }

                                            var43.setPositionAndAnglesKeepPrevAngles((double)var27, (double)var28, (double)var29, var0.random.NextFloat() * 360.0F, 0.0F);
                                            if (var43.canSpawn())
                                            {
                                                ++var20;
                                                var0.SpawnEntity(var43);
                                                creatureSpecificInit(var43, var0, var27, var28, var29);
                                                if (var20 >= var43.getMaxSpawnedInChunk())
                                                {
                                                    breakToNextChunk = true;
                                                }
                                            }

                                            var3 += var20;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            //IS VAR3 CORRECT? I THINK?
            return var3;
        }
    }

    private static bool canCreatureTypeSpawnAtLocation(EnumCreatureType var0, World var1, int var2, int var3, int var4)
    {
        return var0.getMaterial() == Material.Water ? var1.getMaterial(var2, var3, var4).IsFluid && !var1.shouldSuffocate(var2, var3 + 1, var4) : var1.shouldSuffocate(var2, var3 - 1, var4) && !var1.shouldSuffocate(var2, var3, var4) && !var1.getMaterial(var2, var3, var4).IsFluid && !var1.shouldSuffocate(var2, var3 + 1, var4);
    }

    private static void creatureSpecificInit(EntityLiving var0, World var1, float var2, float var3, float var4)
    {
        if (var0 is EntitySpider && var1.random.NextInt(100) == 0)
        {
            EntitySkeleton var5 = new EntitySkeleton(var1);
            var5.setPositionAndAnglesKeepPrevAngles((double)var2, (double)var3, (double)var4, var0.yaw, 0.0F);
            var1.SpawnEntity(var5);
            var5.setVehicle(var0);
        }
        else if (var0 is EntitySheep)
        {
            ((EntitySheep)var0).setFleeceColor(EntitySheep.getRandomFleeceColor(var1.random));
        }

    }

    public static bool spawnMonstersAndWakePlayers(World var0, List<EntityPlayer> players)
    {
        bool monstersSpawned = false;
        var pathfinder = new Pathfinder(var0);
        using var var4 = players.GetEnumerator();
        while (true)
        {
            EntityPlayer var5;
            Class[] var6;
            do
            {
                do
                {
                    if (!var4.MoveNext())
                    {
                        return monstersSpawned;
                    }

                    var5 = var4.Current;
                    var6 = nightSpawnEntities;
                } while (var6 == null);
            } while (var6.Length == 0);

            bool var7 = false;

            for (int var8 = 0; var8 < 20 && !var7; ++var8)
            {
                int var9 = MathHelper.Floor(var5.x) + var0.random.NextInt(32) - var0.random.NextInt(32);
                int var10 = MathHelper.Floor(var5.z) + var0.random.NextInt(32) - var0.random.NextInt(32);
                int var11 = MathHelper.Floor(var5.y) + var0.random.NextInt(16) - var0.random.NextInt(16);
                if (var11 < 1)
                {
                    var11 = 1;
                }
                else if (var11 > 128)
                {
                    var11 = 128;
                }

                int var12 = var0.random.NextInt(var6.Length);

                int var13;
                for (var13 = var11; var13 > 2 && !var0.shouldSuffocate(var9, var13 - 1, var10); --var13)
                {
                }

                while (!canCreatureTypeSpawnAtLocation(EnumCreatureType.monster, var0, var9, var13, var10) && var13 < var11 + 16 && var13 < 128)
                {
                    ++var13;
                }

                if (var13 < var11 + 16 && var13 < 128)
                {
                    float var14 = (float)var9 + 0.5F;
                    float var15 = (float)var13;
                    float var16 = (float)var10 + 0.5F;

                    EntityLiving var17;
                    try
                    {
                        var17 = (EntityLiving)var6[var12].getConstructor(typeof(World)).newInstance(var0);
                    }
                    catch (java.lang.Exception ex)
                    {
                        ex.printStackTrace();
                        return monstersSpawned;
                    }

                    var17.setPositionAndAnglesKeepPrevAngles((double)var14, (double)var15, (double)var16, var0.random.NextFloat() * 360.0F, 0.0F);
                    if (var17.canSpawn())
                    {
                        PathEntity var18 = pathfinder.createEntityPathTo(var17, var5, 32.0F);
                        if (var18 != null && var18.pathLength > 1)
                        {
                            PathPoint var19 = var18.func_22328_c();
                            if (java.lang.Math.abs((double)var19.xCoord - var5.x) < 1.5D && java.lang.Math.abs((double)var19.zCoord - var5.z) < 1.5D && java.lang.Math.abs((double)var19.yCoord - var5.y) < 1.5D)
                            {
                                Vec3i var20 = BlockBed.findWakeUpPosition(var0, MathHelper.Floor(var5.x), MathHelper.Floor(var5.y), MathHelper.Floor(var5.z), 1);
                                if (var20 == null)
                                {
                                    var20 = new Vec3i(var9, var13 + 1, var10);
                                }

                                var17.setPositionAndAnglesKeepPrevAngles((double)((float)var20.x + 0.5F), (double)var20.y, (double)((float)var20.z + 0.5F), 0.0F, 0.0F);
                                var0.SpawnEntity(var17);
                                creatureSpecificInit(var17, var0, (float)var20.x + 0.5F, (float)var20.y, (float)var20.z + 0.5F);
                                var5.wakeUp(true, false, false);
                                var17.playLivingSound();
                                monstersSpawned = true;
                                var7 = true;
                            }
                        }
                    }
                }
            }
        }
    }
}
