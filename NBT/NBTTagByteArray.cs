using java.io;

namespace betareborn.NBT
{
    public sealed class NBTTagByteArray : NBTBase
    {
        public byte[] Value { get; set; } = [];

        public NBTTagByteArray()
        {
        }

        public NBTTagByteArray(byte[] value)
        {
            Value = value;
        }

        public override void WriteTagContents(DataOutput output)
        {
            output.writeInt(Value.Length);
            output.write(Value);
        }

        public override void ReadTagContents(DataInput input)
        {
            var length = input.readInt();
            Value = new byte[length];
            input.readFully(Value);
        }

        public override byte GetTagType()
        {
            return 7;
        }

        public override string ToString()
        {
            return $"[{Value.Length} bytes]";
        }
    }
}