using java.io;

namespace BetaSharp.NBT;

public sealed class NBTTagByteArray : NBTBase
{
    public byte[] Values { get; set; } = [];

    public NBTTagByteArray()
    {
    }

    public NBTTagByteArray(byte[] value)
    {
        Values = value;
    }

    public override void WriteTagContents(DataOutput output)
    {
        output.writeInt(Values.Length);
        output.write(Values);
    }

    public override void ReadTagContents(DataInput input)
    {
        var length = input.readInt();
        Values = new byte[length];
        input.readFully(Values);
    }

    public override byte GetTagType()
    {
        return 7;
    }

    public override string ToString()
    {
        return $"[{Values.Length} bytes]";
    }
}