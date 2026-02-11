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

        public override void WriteTagContents(DataOutput output)
        {
            output.writeUTF(stringValue);
        }

        public override void ReadTagContents(DataInput input)
        {
            stringValue = input.readUTF();
        }

        public override byte GetTagType()
        {
            return 8;
        }

        public override string ToString()
        {
            return stringValue;
        }
    }
}