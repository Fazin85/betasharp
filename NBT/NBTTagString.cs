using java.io;

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