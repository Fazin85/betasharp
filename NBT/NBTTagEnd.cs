using java.io;

namespace betareborn.NBT
{
    public class NBTTagEnd : NBTBase
    {
        public override void readTagContents(DataInput var1)
        {
        }

        public override void writeTagContents(DataOutput var1)
        {
        }


        public override byte getType()
        {
            return 0;
        }

        public override string toString()
        {
            return "END";
        }
    }
}