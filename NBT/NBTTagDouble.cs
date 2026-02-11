using System.Globalization;
using java.io;

namespace betareborn.NBT
{
    public sealed class NBTTagDouble : NBTBase
    {
        public double doubleValue;

        public NBTTagDouble()
        {
        }

        public NBTTagDouble(double value)
        {
            doubleValue = value;
        }

        public override void WriteTagContents(DataOutput output)
        {
            output.writeDouble(doubleValue);
        }

        public override void ReadTagContents(DataInput input)
        {
            doubleValue = input.readDouble();
        }

        public override byte GetTagType()
        {
            return 6;
        }

        public override string ToString()
        {
            return doubleValue.ToString(CultureInfo.CurrentCulture);
        }
    }
}