using java.io;

namespace betareborn.NBT
{
    public sealed class NBTTagShort : NBTBase
    {
        public short shortValue;

        public NBTTagShort()
        {
        }

        public NBTTagShort(short value)
        {
            shortValue = value;
        }

        public override void WriteTagContents(DataOutput output)
        {
            output.writeShort(shortValue);
        }

        public override void ReadTagContents(DataInput input)
        {
            shortValue = input.readShort();
        }

        public override byte GetTagType()
        {
            return 2;
        }

        public override string ToString()
        {
            return shortValue.ToString();
        }
    }
}