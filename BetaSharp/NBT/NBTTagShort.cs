using java.io;

namespace BetaSharp.NBT;

public sealed class NBTTagShort : NBTBase
{
    public short Value { get; set; }

    public NBTTagShort()
    {
    }

    public NBTTagShort(short value)
    {
        Value = value;
    }

    public override void WriteTagContents(DataOutput output)
    {
        output.writeShort(Value);
    }

    public override void ReadTagContents(DataInput input)
    {
        Value = input.readShort();
    }

    public override byte GetTagType()
    {
        return 2;
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}