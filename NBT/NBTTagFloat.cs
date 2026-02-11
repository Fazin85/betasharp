using System.Globalization;
using java.io;

namespace betareborn.NBT
{
    public sealed class NBTTagFloat : NBTBase
    {
        public float floatValue;

        public NBTTagFloat()
        {
        }

        public NBTTagFloat(float value)
        {
            floatValue = value;
        }

        public override void writeTagContents(DataOutput output)
        {
            output.writeFloat(floatValue);
        }

        public override void readTagContents(DataInput input)
        {
            floatValue = input.readFloat();
        }

        public override byte getType()
        {
            return 5;
        }

        public override string toString()
        {
            return floatValue.ToString(CultureInfo.CurrentCulture);
        }
    }
}