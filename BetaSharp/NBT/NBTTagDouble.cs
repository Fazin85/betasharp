using System.Globalization;

namespace BetaSharp.NBT;

public sealed class NBTTagDouble : NBTBase
{
    public double Value { get; set; }

    public NBTTagDouble()
    {
    }

    public NBTTagDouble(double value)
    {
        Value = value;
    }

    public override void WriteTagContents(Stream output)
    {
        output.WriteDouble(Value);
    }

    public override void ReadTagContents(Stream input)
    {
        Value = input.ReadDouble();
    }

    public override byte GetTagType()
    {
        return 6;
    }

    public override string ToString()
    {
        return Value.ToString(CultureInfo.CurrentCulture);
    }
}