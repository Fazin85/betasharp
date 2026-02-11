using java.io;

namespace betareborn.NBT
{
    public class NBTTagLong : NBTBase
    {
        public long longValue;

        public NBTTagLong()
        {
        }

        public NBTTagLong(long var1)
        {
            longValue = var1;
        }

        public override void writeTagContents(DataOutput var1)
        {
            var1.writeLong(longValue);
        }

        public override void readTagContents(DataInput var1)
        {
            longValue = var1.readLong();
        }

        public override byte getType()
        {
            return 4;
        }

        public override string toString()
        {
            return "" + longValue;
        }
    }
}