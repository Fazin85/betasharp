using System.Buffers;

namespace BetaSharp.Worlds.Chunks;

public class ChunkSnapshot : IDisposable
{
    private readonly byte[] blocks;
    private readonly ChunkNibbleArray data;
    public readonly ChunkNibbleArray skylightMap;
    public readonly ChunkNibbleArray blocklightMap;
    private bool disposed = false;
    private bool isLit = false;

    public ChunkSnapshot(Chunk toSnapshot)
    {
        blocks = ArrayPool<byte>.Shared.Rent(32768);
        Buffer.BlockCopy(toSnapshot.blocks, 0, blocks, 0, toSnapshot.blocks.Length);

        data = createNibbleArray(toSnapshot.meta.bytes);
        skylightMap = createNibbleArray(toSnapshot.skyLight.bytes);
        blocklightMap = createNibbleArray(toSnapshot.blockLight.bytes);
    }

    private static ChunkNibbleArray createNibbleArray(byte[] toCopy)
    {
        byte[] bytes = ArrayPool<byte>.Shared.Rent(toCopy.Length);
        Buffer.BlockCopy(toCopy, 0, bytes, 0, toCopy.Length);
        return new(bytes);
    }

    public int getBlockID(int x, int y, int z)
    {
        return blocks[x << 11 | z << 7 | y] & 255;
    }

    public int getBlockMetadata(int x, int y, int z)
    {
        return data.getNibble(x, y, z);
    }

    public int getBlockLightValue(int x, int y, int z, int var4)
    {
        int var5 = skylightMap.getNibble(x, y, z);
        if (var5 > 0)
        {
            isLit = true;
        }

        var5 -= var4;
        int var6 = blocklightMap.getNibble(x, y, z);
        if (var6 > var5)
        {
            var5 = var6;
        }

        return var5;
    }

    public bool getIsLit()
    {
        return isLit;
    }

    public void Dispose()
    {
        if (disposed)
        {
            return;
        }

        GC.SuppressFinalize(this);
        ArrayPool<byte>.Shared.Return(blocks);
        ArrayPool<byte>.Shared.Return(data.bytes);
        ArrayPool<byte>.Shared.Return(skylightMap.bytes);
        ArrayPool<byte>.Shared.Return(blocklightMap.bytes);
        disposed = true;
    }
}