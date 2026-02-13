using java.io;

namespace BetaSharp.NBT;

public sealed class NBTTagLong : NBTBase
{
    public long Value { get; set; }

    public NBTTagLong()
    {
    }

    public NBTTagLong(long value)
    {
        Value = value;
    }

    public override void WriteTagContents(DataOutput output)
    {
        output.writeLong(Value);
    }

    public override void ReadTagContents(DataInput input)
    {
        Value = input.readLong();
    }

    public override byte GetTagType()
    {
        return 4;
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}