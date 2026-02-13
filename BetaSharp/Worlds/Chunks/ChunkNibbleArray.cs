namespace BetaSharp.Worlds.Chunks;

public class ChunkNibbleArray : java.lang.Object
{
    public readonly byte[] bytes;

    public ChunkNibbleArray(int size)
    {
        bytes = new byte[size >> 1];
    }

    public ChunkNibbleArray(byte[] bytes)
    {
        this.bytes = bytes;
    }

    public int getNibble(int x, int y, int z)
    {
        int var4 = x << 11 | z << 7 | y;
        int var5 = var4 >> 1;
        int var6 = var4 & 1;
        return var6 == 0 ? bytes[var5] & 15 : bytes[var5] >> 4 & 15;
    }

    public void setNibble(int x, int y, int z, int value)
    {
        int var5 = x << 11 | z << 7 | y;
        int var6 = var5 >> 1;
        int var7 = var5 & 1;
        if (var7 == 0)
        {
            bytes[var6] = (byte)(bytes[var6] & 240 | value & 15);
        }
        else
        {
            bytes[var6] = (byte)(bytes[var6] & 15 | (value & 15) << 4);
        }

    }

    public bool isArrayInitialized()
    {
        return bytes != null;
    }
}