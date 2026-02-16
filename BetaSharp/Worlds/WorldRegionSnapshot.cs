using BetaSharp.Blocks;
using BetaSharp.Blocks.Entities;
using BetaSharp.Blocks.Materials;
using BetaSharp.NBT;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds.Biomes.Source;
using BetaSharp.Worlds.Chunks;

namespace BetaSharp.Worlds;

public class WorldRegionSnapshot : BlockView, IDisposable
{
    private readonly int ChunkX;
    private readonly int ChunkZ;
    private readonly ChunkSnapshot[][] Chunks;
    private readonly float[] LightTable;
    private readonly int SkylightSubtracted;
    private readonly BiomeSource BiomeSource;
    private bool IsLit = false;
    private readonly Dictionary<BlockPos, BlockEntity> TileEntityCache = [];

    public WorldRegionSnapshot(World World, int MinX, int var3, int MinZ, int MaxX, int var6, int MaxZ)
    {
        BiomeSource = new(World);

        ChunkX = MinX >> 4;
        ChunkZ = MinZ >> 4;
        int MaxChunkX = MaxX >> 4;
        int MaxChunkZ = MaxZ >> 4;

        int Width = MaxChunkX - ChunkX + 1;
        int Depth = MaxChunkZ - ChunkZ + 1;

        Chunks = new ChunkSnapshot[Width][];
        for (int i = 0; i < Width; i++)
        {
            ChunkSnapshot[] row = new ChunkSnapshot[Depth];
            for (int j = 0; j < Depth; j++)
            {
                int cx = ChunkX + i;
                int cz = ChunkZ + j;
                Chunk originalChunk = World.getChunk(cx, cz);
                row[j] = new(originalChunk);
            }
            Chunks[i] = row;
        }

        LightTable = (float[])World.Dimension.lightLevelToLuminance.Clone();
        SkylightSubtracted = World.ambientDarkness;
    }

    public int getBlockId(int x, int y, int z)
    {
        if (y is < 0 or >= 128) return 0;

        int ChunkIdX = (x >> 4) - ChunkX;
        int ChunkIdZ = (z >> 4) - ChunkZ;

        if (ChunkIdX >= 0 && ChunkIdX < Chunks.Length &&
            ChunkIdZ >= 0 && ChunkIdZ < Chunks[ChunkIdX].Length)
        {
            ChunkSnapshot Chunk = Chunks[ChunkIdX][ChunkIdZ];
            return Chunk == null ? 0 : Chunk.getBlockID(x & 15, y, z & 15);
        }

        return 0;
    }

    public Material getMaterial(int X, int Y, int Z)
    {
        int BlockId = getBlockId(X, Y, Z);
        return BlockId == 0 ? Material.Air : Block.Blocks[BlockId].material;
    }

    public int getBlockMeta(int X, int Y, int Z)
    {
        if (Y is < 0 or >= 128) return 0;

        int chunkIdxX = (X >> 4) - ChunkX;
        int chunkIdxZ = (Z >> 4) - ChunkZ;
        
        if (chunkIdxX >= 0 && chunkIdxX < Chunks.Length &&
            chunkIdxZ >= 0 && chunkIdxZ < Chunks[chunkIdxX].Length)
        {
            ChunkSnapshot chunk = Chunks[chunkIdxX][chunkIdxZ];
            return chunk == null ? 0 : chunk.getBlockMetadata(X & 15, Y, Z & 15);
        }
        
        return 0;
    }

    public BlockEntity? getBlockEntity(int X, int Y, int Z)
    {
        if (Y is < 0 or >= 128) return null;

        var pos = new BlockPos(X, Y, Z);
        if (TileEntityCache.TryGetValue(pos, out BlockEntity? entity))
        {
            return entity;
        }

        int ChunkIdX = (X >> 4) - ChunkX;
        int ChunkIdZ = (Z >> 4) - ChunkZ;

        if (ChunkIdX >= 0 && ChunkIdX < Chunks.Length &&
            ChunkIdZ >= 0 && ChunkIdZ < Chunks[ChunkIdX].Length)
        {
            ChunkSnapshot chunk = Chunks[ChunkIdX][ChunkIdZ];
            if (chunk == null) return null;

            NBTTagCompound? NBT = chunk.GetTileEntityNbt(X & 15, Y, Z & 15);
            if (NBT != null)
            {
                var NewEntity = BlockEntity.createFromNbt(NBT);
                if (NewEntity != null)
                {
                    TileEntityCache[pos] = NewEntity;
                    return NewEntity;
                }
            }
        }

        return null;
    }

    public float getNaturalBrightness(int X, int Y, int Z, int MinLight)
    {
        int light = getLightValue(X, Y, Z);
        return LightTable[Math.Max(light, MinLight)];
    }

    public float getLuminance(int X, int Y, int Z)
    {
        return LightTable[getLightValue(X, Y, Z)];
    }

    public int getLightValue(int X, int Y, int Z) => GetLightValueExt(X, Y, Z, true);

    private bool IsOutsideWorldLimits(int x, int z)
    {
        return x < -32000000 || z < -32000000 || x >= 32000000 || z > 32000000;
    }

    public int GetLightValueExt(int X, int y, int Z, bool CheckStairs)
    {
        if (IsOutsideWorldLimits(X, Z)) return 15;
        
        if (y < 0) return 0;
        if (y >= 128)
        {
            return Math.Max(0, 15 - SkylightSubtracted);
        }

        if (CheckStairs)
        {
            int BlockId = getBlockId(X, y, Z);
            if (BlockId == Block.Slab.id || BlockId == Block.Farmland.id || BlockId == Block.WoodenStairs.id || BlockId == Block.CobblestoneStairs.id)
            {
                int MaxLight = GetLightValueExt(X, y + 1, Z, false);
                MaxLight = Math.Max(MaxLight, GetLightValueExt(X + 1, y, Z, false)); // East
                MaxLight = Math.Max(MaxLight, GetLightValueExt(X - 1, y, Z, false)); // West
                MaxLight = Math.Max(MaxLight, GetLightValueExt(X, y, Z + 1, false)); // South
                MaxLight = Math.Max(MaxLight, GetLightValueExt(X, y, Z - 1, false)); // North
                return MaxLight;
            }
        }

        int ChunkIdX = (X >> 4) - ChunkX;
        int ChunkIdZ = (Z >> 4) - ChunkZ;

        if (ChunkIdX >= 0 && ChunkIdX < Chunks.Length &&
            ChunkIdZ >= 0 && ChunkIdZ < Chunks[ChunkIdX].Length)
        {
            ChunkSnapshot chunk = Chunks[ChunkIdX][ChunkIdZ];
            if (chunk == null) return 0;

            int lightValue = chunk.getBlockLightValue(X & 15, y, Z & 15, SkylightSubtracted);

            if (chunk.getIsLit())
            {
                IsLit = true;
            }

            return lightValue;
        }

        return 0;
    }

    public BiomeSource getBiomeSource()
    {
        return BiomeSource;
    }

    public bool shouldSuffocate(int X, int Y, int Z)
    {
        Block block = Block.Blocks[getBlockId(X, Y, Z)];
        return block != null && block.material.BlocksMovement && block.isFullCube();
    }

    public bool isOpaque(int X, int Y, int Z)
    {
        Block block = Block.Blocks[getBlockId(X, Y, Z)];
        return block != null && block.isOpaque();
    }

    public bool getIsLit()
    {
        return IsLit;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);

        foreach (ChunkSnapshot[] column in Chunks)
        {
            if (column == null) continue;

            foreach (ChunkSnapshot snapshot in column)
            {
                snapshot?.Dispose();
            }
        }
    }
}
