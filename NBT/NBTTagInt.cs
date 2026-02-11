using java.io;

namespace betareborn.NBT
{
    public sealed class NBTTagInt : NBTBase
    {
        public int intValue;

        public NBTTagInt()
        {
        }

        public NBTTagInt(int value)
        {
            intValue = value;
        }

        public override void WriteTagContents(DataOutput output)
        {
            output.writeInt(intValue);
        }

        public override void ReadTagContents(DataInput input)
        {
            intValue = input.readInt();
        }

        public override byte GetTagType()
        {
            return 3;
        }

        public override string ToString()
        {
            return intValue.ToString();
        }
    }
}