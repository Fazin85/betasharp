using java.io;

namespace betareborn.NBT
{
    public class NBTTagFloat : NBTBase
    {
        public float floatValue;

        public NBTTagFloat()
        {
        }

        public NBTTagFloat(float var1)
        {
            floatValue = var1;
        }

        public override void writeTagContents(DataOutput var1)
        {
            var1.writeFloat(floatValue);
        }

        public override void readTagContents(DataInput var1)
        {
            floatValue = var1.readFloat();
        }

        public override byte getType()
        {
            return 5;
        }

        public override string toString()
        {
            return "" + floatValue;
        }
    }
}