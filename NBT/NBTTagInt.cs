using java.io;

namespace betareborn.NBT
{
    public class NBTTagInt : NBTBase
    {
        public int intValue;

        public NBTTagInt()
        {
        }

        public NBTTagInt(int var1)
        {
            intValue = var1;
        }

        public override void writeTagContents(DataOutput var1)
        {
            var1.writeInt(intValue);
        }

        public override void readTagContents(DataInput var1)
        {
            intValue = var1.readInt();
        }

        public override byte getType()
        {
            return 3;
        }

        public override string toString()
        {
            return "" + intValue;
        }
    }
}