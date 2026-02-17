using System.Globalization;

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

    public override void WriteTagContents(Stream output)
    {
        output.WriteFloat(Value);
    }

    public override void ReadTagContents(Stream input)
    {
        Value = input.ReadFloat();
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