namespace BetaSharp.Worlds.Chunks;

public class ChunkNibbleArray
{
    public readonly byte[] Bytes;

    public ChunkNibbleArray(int size)
    {
        Bytes = new byte[size >> 1];
    }

    public ChunkNibbleArray(byte[] bytes)
    {
        Bytes = bytes;
    }

    public int GetNibble(int x, int y, int z)
    {
        int index = (x << 11 | z << 7 | y) >> 1;
        int nibble = (x << 11 | z << 7 | y) & 1;
        return nibble == 0 ? Bytes[index] & 15 : Bytes[index] >> 4 & 15;
    }

    public void SetNibble(int x, int y, int z, int value)
    {
        int index = (x << 11 | z << 7 | y) >> 1;
        int nibble = (x << 11 | z << 7 | y) & 1;
        if (nibble == 0)
        {
            Bytes[index] = (byte)(Bytes[index] & 240 | value & 15);
        }
        else
        {
            Bytes[index] = (byte)(Bytes[index] & 15 | (value & 15) << 4);
        }
    }

    public bool IsArrayInitialized()
    {
        return Bytes != null;
    }
}