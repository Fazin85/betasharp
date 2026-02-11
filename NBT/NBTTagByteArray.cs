using java.io;

namespace betareborn.NBT
{
    public sealed class NBTTagByteArray : NBTBase
    {
        public byte[] byteArray = [];

        public NBTTagByteArray()
        {
        }

        public NBTTagByteArray(byte[] value)
        {
            byteArray = value;
        }

        public override void writeTagContents(DataOutput output)
        {
            output.writeInt(byteArray.Length);
            output.write(byteArray);
        }

        public override void readTagContents(DataInput input)
        {
            var length = input.readInt();
            byteArray = new byte[length];
            input.readFully(byteArray);
        }

        public override byte getType()
        {
            return 7;
        }

        public override string toString()
        {
            return $"[{byteArray.Length} bytes]";
        }
    }
}