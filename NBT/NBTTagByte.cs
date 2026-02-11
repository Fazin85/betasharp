using java.io;

namespace betareborn.NBT
{
    public sealed class NBTTagByte : NBTBase
    {
        public sbyte byteValue;

        public NBTTagByte()
        {
        }

        public NBTTagByte(sbyte value)
        {
            byteValue = value;
        }

        public override void WriteTagContents(DataOutput output)
        {
            output.writeByte(byteValue);
        }

        public override void ReadTagContents(DataInput input)
        {
            byteValue = (sbyte) input.readByte();
        }

        public override byte GetTagType()
        {
            return 1;
        }

        public override string ToString()
        {
            return byteValue.ToString();
        }
    }
}