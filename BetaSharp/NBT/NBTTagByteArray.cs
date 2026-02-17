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

    public override void WriteTagContents(Stream output)
    {
        output.WriteInt(Values.Length);
        output.Write(Values);
    }

    public override void ReadTagContents(Stream input)
    {
        var length = input.ReadInt();
        Values = new byte[length];
        input.ReadExactly(Values);
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