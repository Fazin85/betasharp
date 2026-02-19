using System.Buffers;
using BetaSharp.Blocks.Entities;
using BetaSharp.NBT;
using BetaSharp.Util.Maths;

namespace BetaSharp.Worlds.Chunks;

public class ChunkSnapshot : IDisposable
{
    public ChunkNibbleArray SkylightMap { get; private set; }
    public ChunkNibbleArray BlocklightMap { get; private set; }

    private readonly byte[] _blocks;
    private readonly ChunkNibbleArray _data;
    private readonly Dictionary<int, NBTTagCompound> _tileEntities = [];
    private bool _disposed;
    private bool _isLit;

    public ChunkSnapshot(Chunk toSnapshot)
    {
        _blocks = ArrayPool<byte>.Shared.Rent(32768);
        Buffer.BlockCopy(toSnapshot.blocks, 0, _blocks, 0, toSnapshot.blocks.Length);

        _data = createNibbleArray(toSnapshot.meta.bytes);
        SkylightMap = createNibbleArray(toSnapshot.skyLight.bytes);
        _data = createNibbleArray(toSnapshot.meta.bytes);
        SkylightMap = createNibbleArray(toSnapshot.skyLight.bytes);
        BlocklightMap = createNibbleArray(toSnapshot.blockLight.bytes);

        foreach (KeyValuePair<BlockPos, BlockEntity> entry in toSnapshot.blockEntities)
        {
            BlockEntity entity = entry.Value;
            BlockPos pos = entry.Key;

            if (pos.y < 0 || pos.y >= 128) continue;

            NBTTagCompound nbt = new();
            entity.writeNbt(nbt);

            int localX = pos.x & 15;
            int localZ = pos.z & 15;
            int localY = pos.y;

            int index = localX << 11 | localZ << 7 | localY;
            _tileEntities[index] = nbt;
        }
    }

    private static ChunkNibbleArray createNibbleArray(byte[] toCopy)
    {
        byte[] bytes = ArrayPool<byte>.Shared.Rent(toCopy.Length);
        Buffer.BlockCopy(toCopy, 0, bytes, 0, toCopy.Length);
        return new(bytes);
    }

    public int getBlockID(int x, int y, int z)
    {
        return _blocks[x << 11 | z << 7 | y] & 255;
    }

    public int getBlockMetadata(int x, int y, int z)
    {
        return _data.getNibble(x, y, z);
    }

    public int getBlockLightValue(int x, int y, int z, int var4)
    {
        int var5 = SkylightMap.getNibble(x, y, z);
        if (var5 > 0)
        {
            _isLit = true;
        }

        var5 -= var4;
        int var6 = BlocklightMap.getNibble(x, y, z);
        if (var6 > var5)
        {
            var5 = var6;
        }

        return var5;
    }

    public bool getIsLit()
    {
        return _isLit;
    }

    public NBTTagCompound? GetTileEntityNbt(int x, int y, int z)
    {
        int index = x << 11 | z << 7 | y;
        return _tileEntities.TryGetValue(index, out NBTTagCompound? nbt) ? nbt : null;
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        GC.SuppressFinalize(this);
        ArrayPool<byte>.Shared.Return(_blocks);
        ArrayPool<byte>.Shared.Return(_data.bytes);
        ArrayPool<byte>.Shared.Return(SkylightMap.bytes);
        ArrayPool<byte>.Shared.Return(BlocklightMap.bytes);
        _disposed = true;
    }
}
