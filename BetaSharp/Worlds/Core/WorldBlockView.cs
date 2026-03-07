using BetaSharp.Blocks;
using BetaSharp.Blocks.Entities;
using BetaSharp.Blocks.Materials;
using BetaSharp.Entities;
using BetaSharp.Items;
using BetaSharp.Rules;
using BetaSharp.Worlds.Chunks;
using BetaSharp.Worlds.Dimensions;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds.Generation.Biomes.Source;

namespace BetaSharp.Worlds.Core;

public class WorldBlockView : IBlockReader, IBlockWorldContext
{
    public int AmbientDarkness;
    public bool PauseTicking = false;
    private readonly IChunkSource _chunkSource;
    private readonly Dimension _dimension;
    private readonly bool _isRemote;
    private readonly World? _worldContext;

    public event Action<int, int, int, int>? OnBlockChanged;
    public event Action<int, int, int, int>? OnNeighborsShouldUpdate;

    public bool isRemote => _worldContext?.isRemote ?? _isRemote;
    public RuleSet Rules => _worldContext?.Rules ?? throw new InvalidOperationException("WorldBlockView has no world context for Rules.");
    public JavaRandom random => _worldContext?.random ?? throw new InvalidOperationException("WorldBlockView has no world context for random.");
    void IBlockWorldContext.SpawnEntity(Entity entity)
    {
        if (_worldContext != null)
            _worldContext.Entities.SpawnEntity(entity);
    }

    void IBlockWorldContext.SpawnItemDrop(double x, double y, double z, ItemStack itemStack)
    {
        if (_worldContext == null) return;
        var droppedItem = new EntityItem(_worldContext, x, y, z, itemStack);
        droppedItem.delayBeforeCanPickup = 10;
        _worldContext.Entities.SpawnEntity(droppedItem);
    }

    public WorldBlockView(IChunkSource chunkSource, Dimension dimension, bool isRemote, World? worldContext = null)
    {
        _chunkSource = chunkSource;
        _dimension = dimension;
        _isRemote = isRemote;
        _worldContext = worldContext;
    }

    public IChunkSource ChunkSource => _chunkSource;

    public bool HasChunk(int x, int z) => _chunkSource.IsChunkLoaded(x, z);

    public Chunk GetChunkFromPos(int x, int z) => GetChunk(x >> 4, z >> 4);

    public Chunk GetChunk(int chunkX, int chunkZ) => _chunkSource.GetChunk(chunkX, chunkZ);

    public bool IsPosLoaded(int x, int y, int z) => y is >= 0 and < 128 && HasChunk(x >> 4, z >> 4);

    public bool IsRegionLoaded(int x, int y, int z, int range) => IsRegionLoaded(x - range, y - range, z - range, x + range, y + range, z + range);

    public bool IsRegionLoaded(int minX, int minY, int minZ, int maxX, int maxY, int maxZ)
    {
        if (maxY >= 0 && minY < 128)
        {
            minX >>= 4;
            minZ >>= 4;
            maxX >>= 4;
            maxZ >>= 4;

            for (int x = minX; x <= maxX; ++x)
            {
                for (int z = minZ; z <= maxZ; ++z)
                {
                    if (!HasChunk(x, z)) return false;
                }
            }

            return true;
        }

        return false;
    }

    // --- Block Reading (IBlockReader) ---

    public int getBlockId(int x, int y, int z)
    {
        if (x < -32000000 || z < -32000000 || x >= 32000000 || z > 32000000 || y < 0 || y >= 128) return 0;
        return GetChunk(x >> 4, z >> 4).GetBlockId(x & 15, y, z & 15);
    }

    public int getBlockMeta(int x, int y, int z)
    {
        if (x < -32000000 || z < -32000000 || x >= 32000000 || z > 32000000 || y < 0 || y >= 128) return 0;
        return GetChunk(x >> 4, z >> 4).GetBlockMeta(x & 15, y, z & 15);
    }

    public Material getMaterial(int x, int y, int z)
    {
        int blockId = getBlockId(x, y, z);
        return blockId == 0 ? Material.Air : Block.Blocks[blockId].material;
    }

    public BlockEntity? getBlockEntity(int x, int y, int z)
    {
        Chunk? chunk = GetChunk(x >> 4, z >> 4);
        return chunk?.GetBlockEntity(x & 15, y, z & 15);
    }

    public bool isOpaque(int x, int y, int z)
    {
        Block? block = Block.Blocks[getBlockId(x, y, z)];
        return block != null && block.isOpaque();
    }

    public bool shouldSuffocate(int x, int y, int z)
    {
        Block? block = Block.Blocks[getBlockId(x, y, z)];
        return block != null && block.material.Suffocates && block.isFullCube();
    }

    public BiomeSource getBiomeSource() => _dimension.BiomeSource;

    public bool isRaining() => _worldContext?.Environment.IsRaining ?? false;

    public bool isRaining(int x, int y, int z) => _worldContext?.Environment.IsRainingAt(x, y, z) ?? false;

    public int getLightLevel(int x, int y, int z) => _worldContext?.Lighting.GetLightLevel(x, y, z) ?? GetBrightness(x, y, z);

    public void ScheduleBlockUpdate(int x, int y, int z, int blockId, int tickRate) => _worldContext?.TickScheduler.ScheduleBlockUpdate(x, y, z, blockId, tickRate);

    // --- Block Writing / Mutation ---

    public bool SetBlockWithoutNotifyingNeighbors(int x, int y, int z, int blockId, int meta)
    {
        if (x < -32000000 || z < -32000000 || x >= 32000000 || z > 32000000 || y < 0 || y >= 128) return false;
        return GetChunk(x >> 4, z >> 4).SetBlock(x & 15, y, z & 15, blockId, meta);
    }

    public bool SetBlockWithoutNotifyingNeighbors(int x, int y, int z, int blockId)
    {
        if (x >= -32000000 && z >= -32000000 && x < 32000000 && z <= 32000000 && y is >= 0 and < 128)
        {
            return GetChunk(x >> 4, z >> 4).SetBlock(x & 15, y, z & 15, blockId);
        }

        return false;
    }

    public bool SetBlockMetaWithoutNotifyingNeighbors(int x, int y, int z, int meta)
    {
        if (x >= -32000000 && z >= -32000000 && x < 32000000 && z <= 32000000 && y is >= 0 and < 128)
        {
            GetChunk(x >> 4, z >> 4).SetBlockMeta(x & 15, y, z & 15, meta);
            return true;
        }

        return false;
    }

    public bool setBlock(int x, int y, int z, int blockId)
    {
        if (SetBlockWithoutNotifyingNeighbors(x, y, z, blockId))
        {
            OnBlockChanged?.Invoke(x, y, z, blockId);
            return true;
        }

        return false;
    }

    public bool setBlock(int x, int y, int z, int blockId, int meta)
    {
        if (SetBlockWithoutNotifyingNeighbors(x, y, z, blockId, meta))
        {
            OnBlockChanged?.Invoke(x, y, z, blockId);
            return true;
        }

        return false;
    }

    public void setBlockMeta(int x, int y, int z, int meta)
    {
        if (SetBlockMetaWithoutNotifyingNeighbors(x, y, z, meta))
        {
            int blockId = getBlockId(x, y, z);
            if (Block.BlocksIngoreMetaUpdate[blockId & 255])
            {
                OnBlockChanged?.Invoke(x, y, z, blockId); // Replaced _world
            }
            else
            {
                OnNeighborsShouldUpdate?.Invoke(x, y, z, blockId); // Replaced _world
            }
        }
    }

    public void SetBlocksDirty(int x, int z, int minY, int maxY)
    {
        if (minY > maxY)
        {
            (maxY, minY) = (minY, maxY);
        }

        SetBlocksDirty(x, minY, z, x, maxY, z);
    }

    public void SetBlocksDirty(int x, int y, int z)
    {
        for (int i = 0; i < EventListeners.Count; ++i)
        {
            EventListeners[i].setBlocksDirty(x, y, z, x, y, z);
        }
    }

    public void SetBlocksDirty(int minX, int minY, int minZ, int maxX, int maxY, int maxZ)
    {
        for (int i = 0; i < EventListeners.Count; ++i)
        {
            EventListeners[i].setBlocksDirty(minX, minY, minZ, maxX, maxY, maxZ);
        }
    }

    // --- Extras

    public bool IsAir(int x, int y, int z) => getBlockId(x, y, z) == 0;

    public int GetBrightness(int x, int y, int z)
    {
        if (y < 0) return 0;
        if (y >= 128) return !_dimension.HasCeiling ? 15 : 0;

        return GetChunk(x >> 4, z >> 4).GetLight(x & 15, y, z & 15, 0);
    }

    public bool IsTopY(int x, int y, int z)
    {
        if (x >= -32000000 && z >= -32000000 && x < 32000000 && z <= 32000000)
        {
            if (y < 0) return false;
            if (y >= 128) return true;
            if (!HasChunk(x >> 4, z >> 4)) return false;

            Chunk chunk = GetChunk(x >> 4, z >> 4);
            return chunk.IsAboveMaxHeight(x & 15, y, z & 15);
        }

        return false;
    }

    public int GetTopY(int x, int z)
    {
        if (x >= -32000000 && z >= -32000000 && x < 32000000 && z <= 32000000)
        {
            int chunkX = x >> 4;
            int chunkZ = z >> 4;

            if (!HasChunk(chunkX, chunkZ)) return 0;

            Chunk chunk = GetChunk(chunkX, chunkZ);
            return chunk.GetHeight(x & 15, z & 15);
        }

        return 0;
    }

    public int GetTopSolidBlockY(int x, int z)
    {
        Chunk chunk = GetChunkFromPos(x, z);
        int currentY = 127;
        int localX = x & 15;
        int localZ = z & 15;

        for (; currentY > 0; --currentY)
        {
            int blockId = chunk.GetBlockId(localX, currentY, localZ);
            Material material = blockId == 0 ? Material.Air : Block.Blocks[blockId].material;

            if (material.BlocksMovement || material.IsFluid)
            {
                return currentY + 1;
            }
        }

        return -1;
    }

    public int GetSpawnPositionValidityY(int x, int z)
    {
        Chunk chunk = GetChunkFromPos(x, z);
        int currentY = 127;
        int localX = x & 15;
        int localZ = z & 15;

        for (; currentY > 0; currentY--)
        {
            int blockId = chunk.GetBlockId(localX, currentY, localZ);
            if (blockId != 0 && Block.Blocks[blockId].material.BlocksMovement)
            {
                return currentY + 1;
            }
        }

        return -1;
    }

    //

    public void NotifyNeighbors(int x, int y, int z, int blockId)
    {
        NotifyUpdate(x - 1, y, z, blockId);
        NotifyUpdate(x + 1, y, z, blockId);
        NotifyUpdate(x, y - 1, z, blockId);
        NotifyUpdate(x, y + 1, z, blockId);
        NotifyUpdate(x, y, z - 1, blockId);
        NotifyUpdate(x, y, z + 1, blockId);
    }

    private void NotifyUpdate(int x, int y, int z, int blockId)
    {
        if (!PauseTicking && !_isRemote)
        {
            Block? block = Block.Blocks[getBlockId(x, y, z)];
            if (block != null)
            {
                block.neighborUpdate(this, x, y, z, blockId);
            }
        }
    }
}
