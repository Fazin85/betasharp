using java.io;

namespace betareborn.NBT
{
    public class NBTTagByteArray : NBTBase
    {
        public byte[] byteArray;

        public NBTTagByteArray()
        {
        }

        public NBTTagByteArray(byte[] var1)
        {
            byteArray = var1;
        }

        public override void writeTagContents(DataOutput var1)
        {
            var1.writeInt(byteArray.Length);
            var1.write(byteArray);
        }

        public override void readTagContents(DataInput var1)
        {
            int var2 = var1.readInt();
            byteArray = new byte[var2];
            var1.readFully(byteArray);
        }

        public override byte getType()
        {
            return 7;
        }

        public override string toString()
        {
            return "[" + byteArray.Length + " bytes]";
        }
    }
}