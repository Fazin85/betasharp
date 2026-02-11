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

        public override void WriteTagContents(DataOutput output)
        {
            output.writeFloat(floatValue);
        }

        public override void ReadTagContents(DataInput input)
        {
            floatValue = input.readFloat();
        }

        public override byte GetTagType()
        {
            return 5;
        }

        public override string ToString()
        {
            return floatValue.ToString(CultureInfo.CurrentCulture);
        }
    }
}