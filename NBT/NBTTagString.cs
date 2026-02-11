using java.io;
using java.lang;

namespace betareborn.NBT
{
    public sealed class NBTTagString : NBTBase
    {
        public string stringValue = string.Empty;

        public NBTTagString()
        {
        }

        public NBTTagString(string value)
        {
            stringValue = value;
            
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new IllegalArgumentException("Empty string not allowed");
            }
        }

        public override void writeTagContents(DataOutput output)
        {
            output.writeUTF(stringValue);
        }

        public override void readTagContents(DataInput input)
        {
            stringValue = input.readUTF();
        }

        public override byte getType()
        {
            return 8;
        }

        public override string toString()
        {
            return stringValue;
        }
    }
}