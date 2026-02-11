using java.io;
using java.lang;

namespace betareborn.NBT
{
    public class NBTTagString : NBTBase
    {
        public string stringValue;

        public NBTTagString()
        {
        }

        public NBTTagString(string var1)
        {
            stringValue = var1;
            if (var1 == null)
            {
                throw new IllegalArgumentException("Empty string not allowed");
            }
        }

        public override void writeTagContents(DataOutput var1)
        {
            var1.writeUTF(stringValue);
        }

        public override void readTagContents(DataInput var1)
        {
            stringValue = var1.readUTF();
        }

        public override byte getType()
        {
            return 8;
        }

        public override string toString()
        {
            return "" + stringValue;
        }
    }
}