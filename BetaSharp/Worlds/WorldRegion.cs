using BetaSharp.Blocks;
using BetaSharp.Blocks.Entities;
using BetaSharp.Blocks.Materials;
using BetaSharp.Worlds.Biomes.Source;
using BetaSharp.Worlds.Chunks;

namespace BetaSharp.Worlds;

public class WorldRegion : java.lang.Object, BlockView
{
    private readonly int chunkX;
    private readonly int chunkZ;
    private readonly Chunk[][] chunks;
    private readonly World world;

    public WorldRegion(World world, int minX, int minY, int minZ, int maxX, int maxY, int maxZ)
    {
        this.world = world;
        chunkX = minX >> 4;
        chunkZ = minZ >> 4;
        int var8 = maxX >> 4;
        int var9 = maxZ >> 4;
        chunks = new Chunk[var8 - chunkX + 1][];
        for (int i = 0; i < chunks.Length; i++)
        {
            chunks[i] = new Chunk[var9 - chunkZ + 1];
        }

        for (int var10 = chunkX; var10 <= var8; ++var10)
        {
            for (int var11 = chunkZ; var11 <= var9; ++var11)
            {
                chunks[var10 - chunkX][var11 - chunkZ] = world.getChunk(var10, var11);
            }
        }

    }

    public int getBlockId(int x, int y, int z)
    {
        if (y < 0)
        {
            return 0;
        }
        else if (y >= 128)
        {
            return 0;
        }
        else
        {
            int var4 = (x >> 4) - chunkX;
            int var5 = (z >> 4) - chunkZ;
            if (var4 >= 0 && var4 < chunks.Length && var5 >= 0 && var5 < chunks[var4].Length)
            {
                Chunk var6 = chunks[var4][var5];
                return var6 == null ? 0 : var6.getBlockId(x & 15, y, z & 15);
            }
            else
            {
                return 0;
            }
        }
    }

    public BlockEntity getBlockEntity(int x, int y, int z)
    {
        int var4 = (x >> 4) - chunkX;
        int var5 = (z >> 4) - chunkZ;
        return chunks[var4][var5].getBlockEntity(x & 15, y, z & 15);
    }

    public float getNaturalBrightness(int x, int y, int z, int blockLight)
    {
        int var5 = getRawBrightness(x, y, z);
        if (var5 < blockLight)
        {
            var5 = blockLight;
        }

        return world.dimension.lightLevelToLuminance[var5];
    }

    public float getLuminance(int x, int y, int z)
    {
        return world.dimension.lightLevelToLuminance[getRawBrightness(x, y, z)];
    }

    public int getRawBrightness(int x, int y, int z)
    {
        return getRawBrightness(x, y, z, true);
    }

    public int getRawBrightness(int x, int y, int z, bool useNeighborLight)
    {
        if (x >= -32000000 && z >= -32000000 && x < 32000000 && z <= 32000000)
        {
            int var5;
            int var6;
            if (useNeighborLight)
            {
                var5 = getBlockId(x, y, z);
                if (var5 == Block.SLAB.id || var5 == Block.FARMLAND.id || var5 == Block.WOODEN_STAIRS.id || var5 == Block.COBBLESTONE_STAIRS.id)
                {
                    var6 = getRawBrightness(x, y + 1, z, false);
                    int var7 = getRawBrightness(x + 1, y, z, false);
                    int var8 = getRawBrightness(x - 1, y, z, false);
                    int var9 = getRawBrightness(x, y, z + 1, false);
                    int var10 = getRawBrightness(x, y, z - 1, false);
                    if (var7 > var6)
                    {
                        var6 = var7;
                    }

                    if (var8 > var6)
                    {
                        var6 = var8;
                    }

                    if (var9 > var6)
                    {
                        var6 = var9;
                    }

                    if (var10 > var6)
                    {
                        var6 = var10;
                    }

                    return var6;
                }
            }

            if (y < 0)
            {
                return 0;
            }
            else if (y >= 128)
            {
                var5 = 15 - world.ambientDarkness;
                if (var5 < 0)
                {
                    var5 = 0;
                }

                return var5;
            }
            else
            {
                var5 = (x >> 4) - chunkX;
                var6 = (z >> 4) - chunkZ;
                return chunks[var5][var6].getLight(x & 15, y, z & 15, world.ambientDarkness);
            }
        }
        else
        {
            return 15;
        }
    }

    public int getBlockMeta(int x, int y, int z)
    {
        if (y < 0)
        {
            return 0;
        }
        else if (y >= 128)
        {
            return 0;
        }
        else
        {
            int var4 = (x >> 4) - chunkX;
            int var5 = (z >> 4) - chunkZ;
            return chunks[var4][var5].getBlockMeta(x & 15, y, z & 15);
        }
    }

    public Material getMaterial(int x, int y, int z)
    {
        int var4 = getBlockId(x, y, z);
        return var4 == 0 ? Material.Air : Block.BLOCKS[var4].material;
    }

    public BiomeSource getBiomeSource()
    {
        return world.getBiomeSource();
    }

    public bool isOpaque(int x, int y, int z)
    {
        Block var4 = Block.BLOCKS[getBlockId(x, y, z)];
        return var4 == null ? false : var4.isOpaque();
    }

    public bool shouldSuffocate(int x, int y, int z)
    {
        Block var4 = Block.BLOCKS[getBlockId(x, y, z)];
        return var4 == null ? false : var4.material.BlocksMovement && var4.isFullCube();
    }
}