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

        public override void writeTagContents(DataOutput output)
        {
            output.writeDouble(doubleValue);
        }

        public override void readTagContents(DataInput input)
        {
            doubleValue = input.readDouble();
        }

        public override byte getType()
        {
            return 6;
        }

        public override string toString()
        {
            return doubleValue.ToString(CultureInfo.CurrentCulture);
        }
    }
}