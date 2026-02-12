using java.io;

namespace betareborn.NBT
{
    public sealed class NBTTagString : NBTBase
    {
        public string Value { get; set; } = string.Empty;

        public NBTTagString()
        {
        }

        public NBTTagString(string value)
        {
            stringValue = value;
        }

        public override void WriteTagContents(DataOutput output)
        {
            output.writeUTF(Value);
        }

        public override void ReadTagContents(DataInput input)
        {
            Value = input.readUTF();
        }

        public override byte GetTagType()
        {
            return 8;
        }

        public override string ToString()
        {
            return Value;
        }
    }
}