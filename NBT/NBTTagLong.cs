using java.io;

namespace betareborn.NBT
{
    public sealed class NBTTagLong : NBTBase
    {
        public long longValue;

        public NBTTagLong()
        {
        }

        public NBTTagLong(long value)
        {
            longValue = value;
        }

        public override void writeTagContents(DataOutput output)
        {
            output.writeLong(longValue);
        }

        public override void readTagContents(DataInput input)
        {
            longValue = input.readLong();
        }

        public override byte getType()
        {
            return 4;
        }

        public override string toString()
        {
            return longValue.ToString();
        }
    }
}