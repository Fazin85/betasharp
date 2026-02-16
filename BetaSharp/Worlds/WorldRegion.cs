using BetaSharp.Blocks;
using BetaSharp.Blocks.Entities;
using BetaSharp.Blocks.Materials;
using BetaSharp.Worlds.Biomes.Source;
using BetaSharp.Worlds.Chunks;

namespace BetaSharp.Worlds;

public class WorldRegion : BlockView
{
    private readonly int ChunkX;
    private readonly int ChunkZ;
    private readonly Chunk[][] Chunks;
    private readonly World World;

    public WorldRegion(World World, int MinX, int MinY, int MinZ, int MaxX, int MaxY, int MaxZ)
    {
        this.World = World;
        ChunkX = MinX >> 4;
        ChunkZ = MinZ >> 4;
        int EndX = MaxX >> 4;
        int EndZ = MaxZ >> 4;

        int Width = EndX - ChunkX + 1;
        int Depth = EndZ - ChunkZ + 1;

        Chunks = new Chunk[Width][];
        for (int i = 0; i < Chunks.Length; i++)
        {
            Chunks[i] = new Chunk[Depth];
        }

        for (int cx = ChunkX; cx <= EndX; ++cx)
        {
            for (int cz = ChunkZ; cz <= EndZ; ++cz)
            {
                Chunks[cx - ChunkX][cz - ChunkZ] = World.getChunk(cx, cz);
            }
        }

    }

    public int getBlockId(int X, int Y, int Z)
    {
        if (Y is < 0 or >= 128) return 0;

        int cx = (X >> 4) - ChunkX;
        int cz = (Z >> 4) - ChunkZ;

        if (cx >= 0 && cx < Chunks.Length && cz >= 0 && cz < Chunks[cx].Length)
        {
            Chunk Chunk = Chunks[cx][cz];
            return Chunk?.getBlockId(X & 15, Y, Z & 15) ?? 0;
        }

        return 0;
    }

    public BlockEntity? getBlockEntity(int x, int y, int z)
    {
        int cx = (x >> 4) - ChunkX;
        int cz = (z >> 4) - ChunkZ;

        if (cx < 0 || cx >= Chunks.Length || cz < 0 || cz < 0 || cz >= Chunks[cx].Length)
            return null;

        return Chunks[cx][cz]?.getBlockEntity(x & 15, y, z & 15);
    }

    public float getNaturalBrightness(int X, int Y, int Z, int BlockLight)
    {
        int FinalLight = Math.Max(getRawBrightness(X, Y, Z), BlockLight);
        return World.Dimension.lightLevelToLuminance[FinalLight];
    }

    public float getLuminance(int X, int Y, int Z)
    {
        return World.Dimension.lightLevelToLuminance[getRawBrightness(X, Y, Z)];
    }

    public int getRawBrightness(int x, int y, int z)
    {
        return getRawBrightness(x, y, z, true);
    }


    public int getRawBrightness(int x, int y, int z, bool useNeighborLight)
    {
        if (IsOutsideWorldLimits(x, z)) return 15;
        if (useNeighborLight)
        {
            int id = getBlockId(x, y, z);
            if (id == Block.Slab.id || id == Block.Farmland.id || id == Block.WoodenStairs.id || id == Block.CobblestoneStairs.id)
            {
                int max = getRawBrightness(x, y + 1, z, false);
                max = Math.Max(max, getRawBrightness(x + 1, y, z, false));
                max = Math.Max(max, getRawBrightness(x - 1, y, z, false));
                max = Math.Max(max, getRawBrightness(x, y, z + 1, false));
                max = Math.Max(max, getRawBrightness(x, y, z - 1, false));
                return max;
            }
        }

        if (y < 0) return 0;
        if (y >= 128) return Math.Max(0, 15 - World.ambientDarkness);

        int ChunkIdX = (x >> 4) - ChunkX;
        int chunkIdZ = (z >> 4) - ChunkZ;

        return Chunks[ChunkIdX][chunkIdZ].getLight(x & 15, y, z & 15, World.ambientDarkness);
    }

    public int getBlockMeta(int x, int y, int z)
    {
        if (y is < 0 or >= 128) return 0;

        int cx = (x >> 4) - ChunkX;
        int cz = (z >> 4) - ChunkZ;
        return Chunks[cx][cz].getBlockMeta(x & 15, y, z & 15);
    }

    public Material getMaterial(int x, int y, int z)
    {
        int var4 = getBlockId(x, y, z);
        return var4 == 0 ? Material.Air : Block.Blocks[var4].material;
    }

    public BiomeSource getBiomeSource() => World.getBiomeSource();

    public bool isOpaque(int x, int y, int z)
    {
        Block block = Block.Blocks[getBlockId(x, y, z)];
        return block != null && block.isOpaque();
    }

    public bool shouldSuffocate(int x, int y, int z)
    {
        Block block = Block.Blocks[getBlockId(x, y, z)];
        return block != null && block.material.BlocksMovement && block.isFullCube();
    }
    
    private bool IsOutsideWorldLimits(int x, int z)
    {
        return x < -32000000 || z < -32000000 || x >= 32000000 || z > 32000000;
    }
}