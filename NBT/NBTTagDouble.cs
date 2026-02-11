using java.io;

namespace betareborn.NBT
{
    public class NBTTagDouble : NBTBase
    {
        public double doubleValue;

        public NBTTagDouble()
        {
        }

        public NBTTagDouble(double var1)
        {
            doubleValue = var1;
        }

        public override void writeTagContents(DataOutput var1)
        {
            var1.writeDouble(doubleValue);
        }

        public override void readTagContents(DataInput var1)
        {
            doubleValue = var1.readDouble();
        }

        public override byte getType()
        {
            return 6;
        }

        public override string toString()
        {
            return "" + doubleValue;
        }
    }
}