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

    public override void WriteTagContents(Stream output)
    {
        output.WriteShort(Value);
    }

    public override void ReadTagContents(Stream input)
    {
        Value = input.ReadShort();
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