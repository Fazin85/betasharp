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

        public override void writeTagContents(DataOutput output)
        {
            output.writeInt(intValue);
        }

        public override void readTagContents(DataInput input)
        {
            intValue = input.readInt();
        }

        public override byte getType()
        {
            return 3;
        }

        public override string toString()
        {
            return intValue.ToString();
        }
    }
}