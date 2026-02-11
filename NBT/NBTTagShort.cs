using java.io;

namespace betareborn.NBT
{
    public class NBTTagShort : NBTBase
    {
        public short shortValue;

        public NBTTagShort()
        {
        }

        public NBTTagShort(short var1)
        {
            shortValue = var1;
        }

        public override void writeTagContents(DataOutput var1)
        {
            var1.writeShort(shortValue);
        }

        public override void readTagContents(DataInput var1)
        {
            shortValue = var1.readShort();
        }

        public override byte getType()
        {
            return 2;
        }

        public override string toString()
        {
            return "" + shortValue;
        }
    }
}