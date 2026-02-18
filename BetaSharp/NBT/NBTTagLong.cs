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

    public override void WriteTagContents(Stream output)
    {
        output.WriteLong(Value);
    }

    public override void ReadTagContents(Stream input)
    {
        Value = input.ReadLong();
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