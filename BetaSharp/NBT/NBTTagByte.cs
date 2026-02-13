using java.io;

namespace BetaSharp.NBT;

public sealed class NBTTagByte : NBTBase
{
    public sbyte Value { get; set; }

    public NBTTagByte()
    {
    }

    public NBTTagByte(sbyte value)
    {
        Value = value;
    }

    public override void WriteTagContents(DataOutput output)
    {
        output.writeByte(Value);
    }

    public override void ReadTagContents(DataInput input)
    {
        Value = (sbyte) input.readByte();
    }

    public override byte GetTagType()
    {
        return 1;
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}