using java.io;

namespace BetaSharp.NBT;

public sealed class NBTTagString : NBTBase
{
    public string Value { get; set; } = string.Empty;

    public NBTTagString()
    {
    }

    public NBTTagString(string value)
    {
        Value = value;
    }

    public override void WriteTagContents(DataOutput output)
    {
        output.writeUTF(Value);
    }

    public override void ReadTagContents(DataInput input)
    {
        Value = input.readUTF();
    }

    public override byte GetTagType()
    {
        return 8;
    }

    public override string ToString()
    {
        return Value;
    }
}