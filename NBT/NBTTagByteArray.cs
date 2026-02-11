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

        public override void WriteTagContents(DataOutput output)
        {
            output.writeInt(byteArray.Length);
            output.write(byteArray);
        }

        public override void ReadTagContents(DataInput input)
        {
            var length = input.readInt();
            byteArray = new byte[length];
            input.readFully(byteArray);
        }

        public override byte GetTagType()
        {
            return 7;
        }

        public override string ToString()
        {
            return $"[{byteArray.Length} bytes]";
        }
    }
}