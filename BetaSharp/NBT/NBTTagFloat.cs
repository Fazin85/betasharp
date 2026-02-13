using System.Globalization;
using java.io;

namespace BetaSharp.NBT;

public sealed class NBTTagFloat : NBTBase
{
    public float Value { get; set; }

    public NBTTagFloat()
    {
    }

    public NBTTagFloat(float value)
    {
        Value = value;
    }

    public override void WriteTagContents(DataOutput output)
    {
        output.writeFloat(Value);
    }

    public override void ReadTagContents(DataInput input)
    {
        Value = input.readFloat();
    }

    public override byte GetTagType()
    {
        return 5;
    }

    public override string ToString()
    {
        return Value.ToString(CultureInfo.CurrentCulture);
    }
}