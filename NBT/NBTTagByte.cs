using java.io;

namespace betareborn.NBT
{
    public class NBTTagByte : NBTBase
    {
        public sbyte byteValue;

        public NBTTagByte()
        {
        }

        public NBTTagByte(sbyte var1)
        {
            byteValue = var1;
        }

        public override void writeTagContents(DataOutput var1)
        {
            var1.writeByte(byteValue);
        }

        public override void readTagContents(DataInput var1)
        {
            byteValue = (sbyte) var1.readByte();
        }

        public override byte getType()
        {
            return 1;
        }

        public override string toString()
        {
            return "" + byteValue;
        }
    }
}