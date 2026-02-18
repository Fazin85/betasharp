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

    public override void WriteTagContents(Stream output)
    {
        output.WriteString(Value);
    }

    public override void ReadTagContents(Stream input)
    {
        Value = input.ReadString();
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