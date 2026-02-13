using System.Globalization;
using java.io;

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

    public override void WriteTagContents(DataOutput output)
    {
        output.writeDouble(Value);
    }

    public override void ReadTagContents(DataInput input)
    {
        Value = input.readDouble();
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