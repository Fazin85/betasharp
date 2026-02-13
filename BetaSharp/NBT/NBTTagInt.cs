using java.io;

namespace BetaSharp.NBT;

public sealed class NBTTagInt : NBTBase
{
    public int Value { get; set; }

    public NBTTagInt()
    {
    }

    public NBTTagInt(int value)
    {
        Value = value;
    }

    public override void WriteTagContents(DataOutput output)
    {
        output.writeInt(Value);
    }

    public override void ReadTagContents(DataInput input)
    {
        Value = input.readInt();
    }

    public override byte GetTagType()
    {
        return 3;
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}