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

    public override void WriteTagContents(Stream output)
    {
        output.WriteInt(Value);
    }

    public override void ReadTagContents(Stream input)
    {
        Value = input.ReadInt();
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