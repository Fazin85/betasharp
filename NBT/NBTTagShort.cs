using java.io;

namespace betareborn.NBT
{
    public sealed class NBTTagShort : NBTBase
    {
        public short shortValue;

        public NBTTagShort()
        {
        }

        public NBTTagShort(short value)
        {
            shortValue = value;
        }

        public override void writeTagContents(DataOutput output)
        {
            output.writeShort(shortValue);
        }

        public override void readTagContents(DataInput input)
        {
            shortValue = input.readShort();
        }

        public override byte getType()
        {
            return 2;
        }

        public override string toString()
        {
            return shortValue.ToString();
        }
    }
}